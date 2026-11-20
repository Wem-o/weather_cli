using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace weather_cli {
    class State {
        private static readonly HttpClient http_geo = new() {
            BaseAddress = new Uri("https://geocoding-api.open-meteo.com"),
        };

        private static readonly HttpClient http_weather = new() {
            BaseAddress = new Uri("https://api.open-meteo.com"),
        };

        Location? location = null;

        List<WeatherData>? hourly = null;
        List<WeatherData>? daily = null;
        List<WeatherData>? minutely = null;
        WeatherData? current = null;

        public State() { }

        public void Execute(in Arguments args) {
            // Console.WriteLine($"Executing: {args.command_type}");
            if (args.ShouldQuit()) throw new Exception("The code is ass");

            switch (args.command_type) {
                case CommandType.Help:
                    this.Help();
                    break;
                case CommandType.Location:
                    this.Location(args.arguments);
                    break;
                case CommandType.Report:
                    this.Report();
                    break;
                default:
                    break;
            }
        }

        void Help() {
            Console.WriteLine("Help:\n");
            Console.WriteLine("\thelp\t\t\tPrint this message");
            Console.WriteLine("\tlocation [set <CITY>|get]\t\tSet or print the tracked location");
            Console.WriteLine("\treport\t\t\tPrints a general weather report for today");
        }

        void Location(in List<String> args) {
            // If location has no arguments assume
            // the user wants to get the current location
            if (!args.Any() || args.ElementAtOrDefault(0)?.ToLower() == "get") {
                if (this.location is null) {
                    Console.WriteLine("No location set yet! Try `location set <CITY>`");
                } else {
                    Console.WriteLine(this.location.Format());
                }
                return;
            }

            // Try to fetch the specified location

            // Check for validity
            if (args.Count() < 2
                || args.ElementAtOrDefault(0)?.ToLower() != "set") {
                throw new InvalidCommand(
                    "Location should be set as: `location set <CITY>`"
                );
            }

            Location? location_maybe = GetLocation(string.Join(' ', args[1..]));
            if (!(this.location is not null && location_maybe is null)) {
                this.location = location_maybe;
                Console.WriteLine($"Location set to: {(
                    this.location is not null ? this.location.Format() : "Unset"
                )}");
            }
        }

        void Report() {
            JsonNode? json = MakeMeteoCall(current: [
                "temperature_2m",
                "apparent_temperature",
                "relative_humidity_2m",
                "weather_code",
                "is_day",
                "precipitation",
                "wind_direction_10m",
                "wind_speed_10m"
                ],
                forecast_minutely_count: 3,
                minutely: [
                "temperature_2m",
                "apparent_temperature",
                "relative_humidity_2m",
                "weather_code",
                "is_day",
                "precipitation",
                "precipitation_probability",
                "wind_direction_10m",
                "wind_speed_10m",
                "visibility"
            ]);

            if (json is null) return;

            if (!ParseCall(json)) {
                Console.WriteLine("Failed to get report!");
                return;
            }

            Console.WriteLine(this.current!.Format());
        }

        bool ParseCall(in JsonNode node) {
            if (node["error"] is not null && (bool)node["error"]! == true) {
                Console.WriteLine($"API returned error: {node["reason"]}");
                return false;
            }

            var offset = (int)node["utc_offset_seconds"]!;

            var current = node["current"];

            if (current is not null) try {
                this.current = new WeatherData(
                    WeatherKind.Current,
                    (long)current["time"]!,
                    offset,
                    (float)current["temperature_2m"]!,
                    (float)current["apparent_temperature"]!,
                    (ushort)current["relative_humidity_2m"]!,
                    (ushort)current["weather_code"]!,
                    (int)-1,
                    (int)current["is_day"]! != 0,
                    (float)current["precipitation"]!,
                    (ushort)100,
                    (float)current["wind_speed_10m"]!,
                    (float)current["wind_direction_10m"]!,
                    ((string)node["current_units"]!["temperature_2m"]!)[1],
                    (string)node["current_units"]!["precipitation"]!,
                    (string)node["current_units"]!["wind_speed_10m"]!
                );
            } catch (NullReferenceException) {
                Console.WriteLine($"Failed to parse api the current weather section of the api response!");
            }


            JsonNode minutely = node["minutely_15"]!;
            if (minutely is not null) try {
                    int count = minutely["time"]!.AsArray().Count();

                    var data = new List<WeatherData>(capacity: count);
                    for (int i = 0; i < count; i++) {
                        data.Add(new WeatherData(
                            WeatherKind.Minutely,
                            (long)minutely["time"]![i]!,
                            offset,
                            (float)minutely["temperature_2m"]![i]!,
                            (float)minutely["apparent_temperature"]![i]!,
                            (ushort)minutely["relative_humidity_2m"]![i]!,
                            (ushort)minutely["weather_code"]![i]!,
                            (int)minutely["visibility"]![i]!,
                            (int)minutely["is_day"]![i]! != 0,
                            (float)minutely["precipitation"]![i]!,
                            (ushort)minutely["precipitation_probability"]![i]!,
                            (float)minutely["wind_speed_10m"]!,
                            (float)minutely["wind_direction_10m"]!,
                            ((string)node["minutely_15_units"]!["temperature_2m"]!)[1],
                            (string)node["minutely_15_units"]!["precipitation"]!,
                            (string)node["minutely_15_units"]!["wind_speed_10m"]!
                        ));
                    }

                    this.minutely = data;
                } catch (NullReferenceException) {
                    Console.WriteLine($"Failed to parse api the minutely weather section of the api response!");
                }


            return true;
        }

        JsonNode? MakeMeteoCall(
            List<String>? current = null,
            List<String>? daily = null,
            ushort forecast_days = 0,
            List<String>? hourly = null,
            List<String>? minutely = null,
            uint forecast_minutely_count = 0,
            bool should_minutely_start_from_current_time = true
        ) {
            if (this.location is null) {
                throw new Exception("Tried to GetReport with no location!");
            }

            // https://www.open-meteo.com/en/docs
            string request_url =
            $"v1/forecast?latitude={this.location.lat}&longitude={this.location.lng}&timezone=auto&timeformat=unixtime";
            request_url += $"&forecast_minutely_15={forecast_minutely_count}&forecast_days={forecast_days}";

            if (current is not null) {
                request_url += $"&current={String.Join(",", current)}";
            }

            if (daily is not null) {
                request_url += $"&daily={String.Join(",", daily)}";
            }

            if (hourly is not null) {
                request_url += $"&hourly={String.Join(",", hourly)}";
            }

            if (minutely is not null) {
                if (should_minutely_start_from_current_time) {
                    var current_time = DateTimeOffset.Now;
                    // Round to the nearest 15 min
                    current_time = current_time.AddMinutes(-(current_time.Minute % 15));

                    request_url += $"&start_minutely_15=${Utils.FormatIso8601(current_time)}";
                }

                request_url += $"&minutely_15={String.Join(",", minutely)}";
            }
            // Console.WriteLine(request_url);

            HttpResponseMessage response = http_weather.Send(
                new HttpRequestMessage(HttpMethod.Get, request_url)
            );

            // Completely real branch of code that could happen
            if ((int)response.StatusCode == 418) {
                Console.WriteLine("The server was unable to brew coffe as it's a teapot. Try again later");
            } else if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Console.WriteLine($"Failed to make api request: api.open-meteo.com returned {(int)response.StatusCode}: {response.StatusCode.ToString()}");
                return null;
            }

            using Stream body = response.Content.ReadAsStream();
            return JsonNode.Parse(body);
        }

        Location? GetLocation(string location) {
            // Docs at:
            // open-meteo.com/en/docs/geocoding-apithis
            string request_url = $"v1/search?name={location}&count=1&";
            HttpResponseMessage response = http_geo.Send(
                new HttpRequestMessage(HttpMethod.Get, request_url)
            );

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Console.WriteLine("Failed to get location: non 200 http code");
                return null;
            }

            // Console.WriteLine(response);

            using Stream stream = response.Content.ReadAsStream();
            JsonNode? node = JsonNode.Parse(stream);
            var results = node?["results"]?[0];

            string? name = results?["name"]?.ToString();
            string? country = results?["country"]?.ToString();
            string? country_code = results?["country_code"]?.ToString();
            string? admin = results?["admin1"]?.ToString();
            float? lat = (float?)results?["latitude"];
            float? lng = (float?)results?["longitude"];
            string? tz = results?["timezone"]?.ToString();

            if (name is null || country is null || tz is null
                || country_code is null || lat is null || lng is null
            ) {
                Console.WriteLine("Not a valid location!");
                return null;
            }

            return new Location(
                name,
                country,
                country_code,
                admin,
                lat ?? 0,
                lng ?? 0,
                tz
            );
        }
    }
}

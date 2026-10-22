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
            JsonNode? json = GetReport(["sunrise", "sunset"], 1, ["temperature_2m"]);

            if (json is null) {
                Console.WriteLine("Failed to parse response");
                return;
            }

            // Console.WriteLine(json);

            int offset = (int)json["utc_offset_seconds"]!;
            Console.WriteLine(offset);

            JsonNode daily = json["daily"]!;
            Console.WriteLine(daily);
            int sunrise = (int)daily["sunrise"]![0]!;
            int sunset = (int)daily["sunset"]![0]!;

            TimeSpan utc_offset = new(0, 0, offset);

            // sunrise
            DateTime sunrise_utc = DateTime.UnixEpoch;
            sunrise_utc.AddSeconds(sunrise);
            DateTimeOffset sunrise_mewo = new(sunrise_utc, utc_offset);
            Console.WriteLine(sunrise_mewo);
        }

        JsonNode? GetReport(
            List<String>? daily = null,
            ushort forecast_days = 0,
            List<String>? hourly = null,
            List<String>? minutely = null,
            uint forecast_minutely_count = 0
        ) {
            if (this.location is null) {
                throw new Exception("Tried to GetReport with no location!");
            }

            // https://www.open-meteo.com/en/docs
            string request_url =
            $"v1/forecast?latitude={this.location.lat}&longitude={this.location.lng}&timezone=auto&timeformat=unixtime";
            request_url += $"&forecast_minutely_15={forecast_minutely_count}&forecast_days={forecast_days}";

            if (daily is not null) {
                request_url += $"&daily={String.Join(",", daily)}";
            }

            if (hourly is not null) {
                request_url += $"&hourly={String.Join(",", hourly)}";
            }

            if (minutely is not null) {
                request_url += $"&minutely_15={String.Join(",", minutely)}";
            }
            // Console.WriteLine(request_url);

            HttpResponseMessage response = http_weather.Send(
                new HttpRequestMessage(HttpMethod.Get, request_url)
            );

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Console.WriteLine("Failed to make api call to api.open-meteo.com");
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

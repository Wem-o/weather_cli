using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace weather_cli {
    class State {
        private static readonly HttpClient http_geo = new() {
            BaseAddress = new Uri("https://geocoding-api.open-meteo.com"),
        };

        Location? location = null;

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
                default:
                    break;
            }
        }

        void Help() {
            Console.WriteLine("Help:\n");
            Console.WriteLine("\thelp\t\t\tPrint this message");
            Console.WriteLine("\tlocation [set|get]\t\tSet or get the location");
            Console.WriteLine("\treport\t\t\tPrints a generic weather report");
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

        Location? GetLocation(string location) {
            // Docs at:
            // open-meteo.com/en/docs/geocoding-api
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

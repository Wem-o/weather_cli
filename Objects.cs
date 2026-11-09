namespace weather_cli {
    /// Represents what kind of weather a WeatherData object represents
    enum WeatherKind {
        Current,
        Daily,
        Hourly
    }

    /// Represents weather at a given time
    class WeatherData {
        public readonly WeatherKind kind;

        public readonly DateTimeOffset time;
        public readonly float temp;
        public readonly float atemp;
        public readonly char temp_unit;

        public readonly ushort humidity;
        public readonly ushort wmo_code;
        public readonly int visibility;
        public readonly bool is_day;

        public readonly ushort precipitation_prob;
        public readonly float precipitation;
        public readonly string precip_unit;

        public readonly float wind_speed;
        public readonly float wind_direction;
        public readonly string speed_unit;

        public readonly DateTimeOffset? sunrise = null;
        public readonly DateTimeOffset? sunset = null;

        public string Format() {
            switch (this.kind) {
                case WeatherKind.Current:
                    return String.Join('\n', [
                        $"Current weather ({time.Hour}:{time.Minute})",
                        $"\tTemperature: {temp}°{temp_unit} (feels like {atemp}°{temp_unit})",
                        $"\t{humidity}% humidity, {precipitation}{precip_unit} precipitation",
                        $"\tWind {wind_speed}{speed_unit} from ?North? ({wind_direction}°)"
                    ]);
                default:
                    return "NOT IMPLEMENTED!";
            }
        }

        public WeatherData(
            WeatherKind kind,
            long unix_timestamp,
            int utc_second_offset,
            float temp,
            float atemp,
            ushort humidity,
            ushort wmo_code,
            int visibility,
            bool is_day,
            float precipitation,
            ushort precipitation_prob,
            float wind_speed,
            float wind_direction,
            char temp_unit,
            string precip_unit,
            string speed_unit
        ) {
            this.kind = kind;

            TimeSpan utc_offset = new(0, 0, utc_second_offset);
            this.time = DateTimeOffset
                .FromUnixTimeSeconds(unix_timestamp)
                .ToOffset(utc_offset);

            this.temp = temp;
            this.atemp = atemp;

            this.humidity = humidity;
            this.wmo_code = wmo_code;
            this.visibility = visibility;
            this.is_day = is_day;

            this.wind_speed = wind_speed;
            this.wind_direction = wind_direction;

            this.temp_unit = temp_unit;
            this.precip_unit = precip_unit;
            this.speed_unit = speed_unit;

            this.precipitation = precipitation;
            this.precipitation_prob = precipitation_prob;
        }

        public WeatherData(
            WeatherKind kind,
            long unix_timestamp,
            int utc_second_offset,
            float temp,
            float atemp,
            ushort humidity,
            ushort wmo_code,
            int visibility,
            bool is_day,
            float precipitation,
            ushort precipitation_prob,
            long sunrise_unix_timestamp,
            long sunset_unix_timestamp,
            float wind_speed,
            float wind_direction,
            char temp_unit,
            string precip_unit,
            string speed_unit
        ) {
            this.kind = kind;

            TimeSpan utc_offset = new(0, 0, utc_second_offset);

            this.time = DateTimeOffset
                .FromUnixTimeSeconds(unix_timestamp)
                .ToOffset(utc_offset);

            this.sunrise = DateTimeOffset
                .FromUnixTimeSeconds(sunrise_unix_timestamp)
                .ToOffset(utc_offset);
            this.sunset = DateTimeOffset
                .FromUnixTimeSeconds(sunset_unix_timestamp)
                .ToOffset(utc_offset);

            this.temp = temp;
            this.atemp = atemp;

            this.humidity = humidity;
            this.wmo_code = wmo_code;
            this.visibility = visibility;
            this.is_day = is_day;

            this.precipitation = precipitation;
            this.precipitation_prob = precipitation_prob;

            this.wind_speed = wind_speed;
            this.wind_direction = wind_direction;

            this.temp_unit = temp_unit;
            this.precip_unit = precip_unit;
            this.speed_unit = speed_unit;

            this.precipitation = precipitation;
            this.precipitation_prob = precipitation_prob;
        }

        List<String> GetWmoSymbol() {
            switch (wmo_code) {
                default:
                    return ["NOT IMPLEMENTED"];
            }
        }
    }

    class Location(
        in string name,
        in string country,
        in string country_code,
        in string? admin,
        float lat,
        float lng,
        string tz
        ) {
        readonly string name = name;
        readonly string country = country;
        readonly string country_code = country_code;
        readonly string? admin = admin;
        public readonly float lat = lat;
        public readonly float lng = lng;
        readonly string tz = tz;

        public string Format() {
            return $"{name}, {(admin is not null ? admin + "," : "")} {country} ({country_code}), Timezone: {tz}";
        }
    }
}

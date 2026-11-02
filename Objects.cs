namespace weather_cli {
    /// Represents weather at a given time
    class WeatherData {
        public readonly DateTimeOffset time;
        public readonly float temp;
        public readonly float atemp;

        public readonly ushort humidity;
        public readonly ushort wmo_code;
        public readonly int visibility;
        public readonly bool is_day;

        public readonly float precipitation;
        public readonly ushort precipitation_prob;

        public readonly DateTimeOffset? sunrise = null;
        public readonly DateTimeOffset? sunset = null;

        public WeatherData(
            long unix_timestamp,
            int utc_second_offset,
            float temp,
            float atemp,
            ushort humidity,
            ushort wmo_code,
            int visibility,
            bool is_day,
            float precipitation,
            ushort precipitation_prob
        ) {
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

            this.precipitation = precipitation;
            this.precipitation_prob = precipitation_prob;
        }

        public WeatherData(
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
            long sunset_unix_timestamp
        ) {
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

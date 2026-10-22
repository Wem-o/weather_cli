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
            // You're telling me datetimeoffset has a fromunix
            // constructor but you CANNOT SET THE OFFSET THERE!!
            // This language is a work of art...
            DateTime utc_time = DateTime.UnixEpoch;
            utc_time.AddSeconds(unix_timestamp);

            TimeSpan utc_offset = new(0, 0, utc_second_offset);
            this.time = new DateTimeOffset(utc_time, utc_offset);

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
            // You're telling me datetimeoffset has a fromunix
            // constructor but you CANNOT SET THE OFFSET THERE!!
            // This language is a work of art...
            DateTime utc_time = DateTime.UnixEpoch;
            utc_time.AddSeconds(unix_timestamp);

            TimeSpan utc_offset = new(0, 0, utc_second_offset);

            this.time = new DateTimeOffset(utc_time, utc_offset);

            this.sunrise = new DateTimeOffset(sunrise_unix_timestamp, utc_offset);
            this.sunset = new DateTimeOffset(sunset_unix_timestamp, utc_offset);

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

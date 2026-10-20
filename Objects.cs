namespace weather_cli {
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
        readonly float lat = lat;
        readonly float lng = lng;
        readonly string tz = tz;

        public string Format() {
            return $"{name}, {(admin is not null ? admin + "," : "")} {country} ({country_code}), Timezone: {tz}";
        }
    }
}

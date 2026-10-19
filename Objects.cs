namespace weather_cli {
    class Location(
        in string name,
        in string country,
        in string country_code,
        in string? admin,
        float lat,
        float lng
        ) {
        readonly string name = name;
        readonly string country = country;
        readonly string country_code = country_code;
        readonly string? admin = admin;
        readonly float lat = lat;
        readonly float lng = lng;

        public string Format() {
            return $"{name}, {country} ({country_code}) {(admin is not null ? admin : "")}";
        }
    }
}

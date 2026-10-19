namespace weather_cli {
    class Location {
        readonly string name;
        readonly string country;
        readonly string country_code;
        readonly string? admin;

        public Location(
            in string name,
            in string country,
            in string country_code,
            in string? admin
        ) {
            this.name = name;
            this.country = country;
            this.country_code = country_code;
            this.admin = admin;
        }

        public string Format() {
            return $"{name}, {country} ({country_code}) {(admin is not null ? admin : "")}";
        }
    }
}

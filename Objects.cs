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
                    String[] icon = GetWmoSymbol();
                    return String.Join('\n', [
                        $"Current weather ({time.Hour}:{time.Minute})",
                        $"{icon[0]}\t{GetWmoString()},",
                        $"{icon[1]}\tTemperature: {temp}°{temp_unit} (feels like {atemp}°{temp_unit}),",
                        $"{icon[2]}\t{humidity}% humidity, {precipitation}{precip_unit} precipitation,",
                        $"{icon[3]}\tWind {wind_speed}{speed_unit} from ?North? ({wind_direction}°)",
                        $"{icon[4]}"
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

        String[] GetWmoSymbol() {
            switch (wmo_code) {
                case 0: // Clear sky
                    return [
                        "\x1b[33m  \\  |  / \x1b[0m",
                        "\x1b[33m    .-.   \x1b[0m",
                        "\x1b[33m - {   } -\x1b[0m",
                        "\x1b[33m    ˙-˙   \x1b[0m",
                        "\x1b[33m  /  |  \\ \x1b[0m",
                    ];
                case 1: // Mainly clear
                    return [
                        "\x1b[33m  \\  |  / \x1b[0m",
                        "\x1b[33m    .-.   \x1b[0m",
                        "\x1b[33m - {   } -\x1b[0m",
                        "\x1b[33m    ˙-\x1b[97m(_) \x1b[0m",
                        "\x1b[33m  /  \x1b[97m(___)\x1b[0m",
                    ];
                case 2: // Partly cloudy
                    return [
                        "\x1b[33m  \\  |  / \x1b[0m",
                        "\x1b[33m    \'\"\'   \x1b[0m",
                        "\x1b[33m - { \x1b[97m.-.\x1b[33m -\x1b[0m",
                        "\x1b[33m    \x1b[97m(___) \x1b[0m",
                        "\x1b[33m  /\x1b[97m(_____)\x1b[0m",
                    ];
                case 3: // Overcast
                    return [
                        "    .-.   ",
                        "  _(   ), ",
                        " (      ).",
                        "(________)",
                        "          "
                    ];
                case 51: // Light drizzle
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m .      . \x1b[0m",
                        "\x1b[94m   .   .  \x1b[0m"
                    ];
                case 53: // Moderate drizzle
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m .   .  . \x1b[0m",
                        "\x1b[94m  . .  .  \x1b[0m"
                    ];
                case 55: // Dense drizzle
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m .. .. .. \x1b[0m",
                        "\x1b[94m  . . .   \x1b[0m"
                    ];
                case 61: // Slight rain
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m  |  | |  \x1b[0m",
                        "\x1b[94m  . .  .  \x1b[0m"
                    ];
                case 63: // Moderate rain
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m  | |  |  \x1b[0m",
                        "\x1b[94m   ˙  ˙   \x1b[0m"
                    ];
                case 65: // Heavy rain
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[34m  | || |  \x1b[0m",
                        "\x1b[34m   |  |   \x1b[0m"
                    ];
                case 56: // Light freezing drizzle
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m *     *  \x1b[0m",
                        "\x1b[94m  .  .    \x1b[0m"
                    ];
                case 57: // Heavy freezing drizzle
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[34m *  *  *  \x1b[0m",
                        "\x1b[34m  .  .    \x1b[0m"
                    ];
                case 66: // Light freezing rain
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m |  *  |  \x1b[0m",
                        "\x1b[94m *     *  \x1b[0m"
                    ];
                case 67: // Heavy freezing rain
                    return [
                        "   .-.    ",
                        "  (   ).  ",
                        "(________)",
                        "\x1b[94m |  |  |  \x1b[0m",
                        "\x1b[94m *  *  *  \x1b[0m"
                    ];
                case 45: // Fog
                case 48: // Depositing rime fog
                    return [
                        "          ",
                        "     ⣠⣴⣶⣿⣶⣤⣀",
                        "⢀⣀⣠⣤⣤⣬⣽⣿⣿⣟⣉⡀",
                        "  ⢩⣭⣿⣿⣿⡉⠉⠉",
                        "    ⠈⠁"
                    ];
                case 71: // Slight snowfall
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "  *   *  ",
                        "    *    "
                    ];
                case 73: // Moderate snowfall
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "  *   *  ",
                        "   **    "
                    ];
                case 75: // Heavy snowfall
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "  * * *  ",
                        "   * *   "
                    ];
                case 77: // Snow grains
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "  * • *  ",
                        "   • •   "
                    ];
                case 80: // Slight rain showers
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[94m   \\  \\  \x1b[0m",
                        "         "
                    ];
                case 81: // Moderate rain showers
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m   \\  \\   \x1b[0m",
                        "\x1b[94m    \\      \x1b[0m"
                    ];
                case 82: // Violent rain showers
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m   \\  \\   \x1b[0m",
                        "\x1b[34m    \\  \\  \x1b[0m"
                    ];
                case 85: // Slight snow showers
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[94m   \\\x1b[0m  *  ",
                        "    *  * "
                    ];
                case 86: // Heavy snow showers
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[94m   \\  \\  \x1b[0m",
                        "\x1b[0m    *  * \x1b[0m"
                    ];
                case 95: // Thunderstorm
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m   \x1b[33m\\\x1b[34m\\\x1b[33m\\\x1b[34m   \x1b[0m",
                        "\x1b[94m   \x1b[33m\\\x1b[94m\\\x1b[33m\\\x1b[94m   \x1b[0m",
                    ];
                case 97: // Heavy thunderstorm
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m  \\\x1b[33m\\\x1b[34m\\\x1b[33m\\\x1b[34m\\  \x1b[0m",
                        "\x1b[94m  \\\x1b[33m\\\x1b[94m\\\x1b[33m\\\x1b[94m\\  \x1b[0m",
                    ];
                case 96: // Thunderstorm with slight hail
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m  *\x1b[33m\\\x1b[34m\\\x1b[33m\\\x1b[34m   \x1b[0m",
                        "\x1b[94m   \x1b[33m\\\x1b[94m\\\x1b[33m\\\x1b[94m*  \x1b[0m",
                    ];
                case 99: // Thunderstorm with heavy hail
                    return [
                        "   ,._   ",
                        "  (   )  ",
                        ".(_____) ",
                        "\x1b[34m  *\x1b[33m\\\x1b[34m*\x1b[33m\\\x1b[34m*  \x1b[0m",
                        "\x1b[94m  *\x1b[33m\\\x1b[94m*\x1b[33m\\\x1b[94m*  \x1b[0m",
                    ];
                default:
                    return [
                        "??????????",
                        "??????????",
                        "??????????",
                        "??????????",
                        "??????????"
                    ];
            }
        }

        String GetWmoString() {
            switch (wmo_code) {
                case 0:
                    return "Clear sky";
                case 1:
                    return "Mainly clear";
                case 2:
                    return "Partly cloudy";
                case 3:
                    return "Overcast";
                case 51:
                    return "Light drizzle";
                case 53:
                    return "Moderate drizzle";
                case 55:
                    return "Dense drizzle";
                case 61:
                    return "Slight rain";
                case 63:
                    return "Moderate rain";
                case 65:
                    return "Heavy rain";
                case 56:
                    return "Light freezing drizzle";
                case 57:
                    return "Dense freezing drizzle";
                case 66:
                    return "Light freezing rain";
                case 67:
                    return "Heavy freezing rain";
                case 45:
                    return "Fog";
                case 48:
                    return "Depositing rime fog";
                case 71:
                    return "Slight snowfall";
                case 73:
                    return "Moderate snowfall";
                case 75:
                    return "Heavy snowfall";
                case 77:
                    return "Snow grains";
                case 80:
                    return "Slight rain showers";
                case 81:
                    return "Moderate rain showers";
                case 82:
                    return "Violent rain showers";
                case 85:
                    return "Slight snow showers";
                case 86:
                    return "Heavy snow showers";
                case 95:
                    return "Thunderstorm";
                case 97:
                    return "Heavy thunderstorm";
                case 96:
                    return "Thunderstorm with slight hail";
                case 99:
                    return "Thunderstorm with heavy hail";
                default:
                    return "UNKNOWN";
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

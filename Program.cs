// WEATHER CLI: A CLI weather app

// Commands:
//  location set / get      Set or get the location
//  report                  Get a generic weather report

namespace weather_cli {
    /// Stores the type of command to executes
    enum CommandType {
        Location,
        Report
    }

    class InvalidCommand : Exception {
        public InvalidCommand() { }

        public InvalidCommand(string msg)
            :base(msg) {}
    }

    /// Stores the arguments passed in by the cli interface
    class Arguments {
        CommandType command_type;
        List<String>? arguments;

        public Arguments(ref String cmd) {
            string[] args = cmd.Trim().Split(' ');
            if (args.Count() < 0) {
                throw new InvalidCommand("No command is passed!");
            }

            CommandType? a = Program.TryParse(ref args[0]);

            if (a == null) throw new InvalidCommand(
                $"\"{args[0]}\" is not a valid command!"
            );
            else this.command_type = a.Value;

            this.arguments = args.Skip(1).Take(args.Count() - 2).ToList();
        }
    }

    static class Program {
        static void Main() {
            Console.Clear();
            Console.WriteLine("==== WEATHER CLI ====");

            // The loop:
            //  Take user input
            //  Parse it
            //  Execute it
            bool should_exit = false;
            while (!should_exit) {
                string user_in = Console.ReadLine();
                Arguments args = new(ref user_in);
                Console.WriteLine(args);
            }
        }

        public static CommandType? TryParse(ref string first_arg) {
            return first_arg switch {
                "location" => CommandType.Location,
                "report" => CommandType.Report,
                _ => null,
            };
        }
    }
}

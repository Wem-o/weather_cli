// WEATHER CLI: A CLI weather app

// Commands:
//  location set / get      Set or get (default) the location
//  report                  Get a generic weather report

namespace weather_cli {
    /// Stores the type of command to executes
    enum CommandType {
        Help,
        Quit,
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
        public readonly CommandType command_type;
        public readonly List<String> arguments = [];

        public bool ShouldQuit() {
            return this.command_type == CommandType.Quit;
        }

        public Arguments(ref String cmd) {
            string[] args = cmd.Trim().Split(' ');
            if (args.Count() <= 0 || args[0] == "") {
                throw new InvalidCommand("No command is passed!");
            }

            CommandType? a = Program.TryParse(ref args[0]);

            if (a == null) throw new InvalidCommand(
                $"\"{args[0]}\" is not a valid command!"
            );
            else this.command_type = a.Value;

            if (args.Count() > 1) {
                this.arguments = [.. args[1 .. args.Count()]];
            }
        }
    }

    static class Program {
        public static CommandType? TryParse(ref string first_arg) {
            return first_arg switch {
                "location" => CommandType.Location,
                "report" => CommandType.Report,
                "help" => CommandType.Help,
                "h" => CommandType.Help,
                "q" => CommandType.Quit,
                "quit" => CommandType.Quit,
                "exit" => CommandType.Quit,
                "bye" => CommandType.Quit,
                _ => null,
            };
        }

        static void Main() {
            Console.Clear();
            Console.WriteLine("==== WEATHER CLI ====");

            State state = new();

            // The loop:
            //  Take user input
            //  Parse it
            //  Execute it
            bool should_exit = false;
            while (!should_exit) {
                Console.Write(">");
                string user_in = Console.ReadLine() ?? "";
                Arguments args;
                try {
                    args = new(ref user_in);
                } catch (InvalidCommand e) {
                    Console.WriteLine(e.Message);
                    continue;
                }
                if (args.ShouldQuit()) {
                    should_exit = true;
                    continue;
                }

                state.Execute(args);
            }

            Console.WriteLine("Bye.....");
        }
    }
}

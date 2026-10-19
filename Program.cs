// WEATHER CLI: A CLI weather app

// Commands:
//  location set / get      Set or get the location
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
        public readonly List<String>? arguments;

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

            this.arguments = args.Skip(1).Take(args.Count() - 2).ToList();
        }
    }

    class State {
        public State() { }

        public void Execute(ref Arguments args) {
            // Console.WriteLine($"Executing: {args.command_type}");
            if (args.ShouldQuit()) throw new Exception("The code is ass");

            switch (args.command_type) {
                case CommandType.Help:
                    this.Help();
                    break;
                default:
                    break;
            }
        }

        void Help() {
            Console.WriteLine("Ugh, idk...");
        }
    }

    static class Program {
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
                if (args.ShouldQuit()) should_exit = true;

                state.Execute(ref args);
            }

            Console.WriteLine("Bye.....");
        }

        public static CommandType? TryParse(ref string first_arg) {
            return first_arg switch {
                "location" => CommandType.Location,
                "report" => CommandType.Report,
                "help" => CommandType.Help,
                "h" => CommandType.Help,
                "q" => CommandType.Quit,
                "quit" => CommandType.Quit,
                _ => null,
            };
        }
    }
}

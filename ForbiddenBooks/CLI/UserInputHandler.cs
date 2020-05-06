using System;
using System.Collections.Generic;
using ForbiddenBooks.DatabaseLogic;
using ForbiddenBooks.CLI.Commands.Base;
using ForbiddenBooks.CLI.Commands;

namespace ForbiddenBooks.CLI
{
    /// <summary>
    /// Main CLI module.
    /// </summary>
    public class UserInputHandler
    {
        public readonly DataController dc;

        private OutputHandler printer;
        private Parser parser;
        private Dictionary<string, Command> availableCommands;

        public UserInputHandler()
        {
            dc = new DataController();
            printer = new OutputHandler(dc);
            availableCommands = new Dictionary<string, Command>();

            GenerateAvailableCommands();
             
            parser = new Parser(' ', availableCommands);
        }
        public UserInputHandler(DataController dc)
        {
            this.dc = dc;
            printer = new OutputHandler(dc);
            availableCommands = new Dictionary<string, Command>();

            GenerateAvailableCommands();

            parser = new Parser(' ', availableCommands);
        }

        /// <summary>
        /// Parses user input into commands until asked to exit by the user.
        /// </summary>
        public void ReadInput()
        {
            while (true)
            {
                try
                {
                    Console.Write("$: ");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "exit") return;
                    Tuple<Command, string[]> parsedInput = parser.ParseCommand(input);
                    parsedInput.Item1.Invoke(parsedInput.Item2);
                
                }
                catch(KeyNotFoundException ke)
                {
                    string[] m = ke.Message.Substring(14).Split(' ');
                    Console.WriteLine(m[0] + "is not a valid flag for this command or too many flags were present.");
                    Console.WriteLine("Please use 'help' for more information");
                }
                catch(IndexOutOfRangeException)
                {
                    Console.WriteLine("This command requires more flags.");
                    Console.WriteLine("Please use 'help' for more information");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private void GenerateAvailableCommands()
        {
            // It will be more efficient if I generate
            // this after I know what command I want to call
            if (availableCommands != null)
            {
                availableCommands.Add("print", new PrintCommand(dc, printer));
                availableCommands.Add("add", new AddCommand(dc));
                availableCommands.Add("remove", new RemoveCommand(dc));
                availableCommands.Add("rm", new RemoveCommand(dc));
                availableCommands.Add("clear", new ClearConsoleCommand());
                availableCommands.Add("update", new UpdateCommand(dc));
                availableCommands.Add("user", new UserCommand(dc));
                availableCommands.Add("market", new MarketCommand(dc));
                availableCommands.Add("help", new HelpCommand(availableCommands));
            }
        }       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ForbiddenBooks.CLI.Commands.Base;
using ForbiddenBooks.Exceptions;

namespace ForbiddenBooks.CLI
{
    /// <summary>
    /// Parses user input into commands.
    /// </summary>
    public class Parser
    {
        private Dictionary<string, Command> commands;
        private char delimiter;

        public Parser(char delimiter, Dictionary<string, Command> commands)
        {
            this.delimiter = delimiter;
            this.commands = commands;
        }

        /// <summary>
        /// Parses input into a tuple that holds a command name as string literal and it's flags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns said tuple.</returns>
        public Tuple<string, string[]> ParseInput(string input)
        {
            input = input.ToLower();
            string[] flags = input.Split(delimiter).Where(x => x != "").ToArray();
            
            return new Tuple<string, string[]>(flags[0], flags[1..flags.Length]);
        }

        /// <summary>
        /// Parses input into a tuple that holds a command and flags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns said tuple.</returns>
        public Tuple<Command, string[]> ParseCommand(string input)
        {
            Tuple<string, string[]> parsed = ParseInput(input);
            Command command;

            if (!commands.TryGetValue(parsed.Item1, out command))
            {
                // TO DO: Make an exception and throw it here
                throw new InvalidCommand(parsed.Item1 + " is not a valid command");
            }
            else return new Tuple<Command, string[]>(command, parsed.Item2);
        }
    }
}

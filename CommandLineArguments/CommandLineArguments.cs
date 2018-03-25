using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Nwea.Web.Utilities
{
    
    public class CommandLineArguments
    {
        private CommandLineApplication app;
        private List<CommandOption> options;
        public Dictionary<string, string> Args;
        //private string[] args;

        public CommandLineArguments(string[] args, List<CLIOption> _options)
        {
            this.app = new CommandLineApplication();
            this.options = new List<CommandOption>();
            this.Args = new Dictionary<string, string>();

            _options.ForEach(option => AddOption(option));

            this.app.OnExecute(() => {
                

                this.options.ForEach((option) =>
                { 
                    string name = option.LongName;
                    string value = option.HasValue() ? option.Value() : "";
                    this.Args.Add(option.LongName, value);
                });
            });

            this.app.Execute(args);

        }

        private void AddOption(CLIOption co)
        {
            var option = ParseOption(co);
            this.options.Add(option);
        }
        private CommandOption ParseOption(CLIOption option)
        {
            return this.app.Option(option.Command, option.Description, (CommandOptionType)Enum.Parse(typeof(CommandOptionType), option.OptionType.ToString()));
        }
    }

    public class CLIOption
    {
        public string Command { get; set; }
        public string Description { get; set; }
        public CLIOptionTypes OptionType { get; set; }
    }


    public enum CLIOptionTypes
    {
        SingleValue = CommandOptionType.SingleValue,
        MultipleValue = CommandOptionType.MultipleValue,
        NoValue = CommandOptionType.NoValue
    }
    
}


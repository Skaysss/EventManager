using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using CommandSystem;
using RemoteAdmin;
using MEC;
using Newtonsoft.Json;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace EventManagerENG
{
    
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class VoteEventCommand : ICommand
    {
        public string Command => "voteevent";
        public string[] Aliases => new[] { "ve" };
        public string Description => "Start vote for event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Command only for players!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: voteevent [name of event]";
                return false;
            }

            string eventName = string.Join(" ", arguments);
            Player player = Player.Get(playerSender.ReferenceHub);

            Features.Instance.StartVote(eventName, player);
            response = $"Vote for event '{eventName}' is started!";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class VoteYesCommand : ICommand
    {
        public string Command => "yes";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Command only for players!";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            response = Features.Instance.Vote(player, true);
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class VotePassCommand : ICommand
    {
        public string Command => "pass";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Command only for players!";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            response = Features.Instance.Vote(player, false);
            return true;
        }
    }
}

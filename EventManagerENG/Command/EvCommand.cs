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
    public class EventCommand : ICommand
    {
        public string Command => "ev";
        public string[] Aliases => new[] { "event" };
        public string Description => "Start event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Command only for players!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: ev [name of event]";
                return false;
            }

            string eventName = string.Join(" ", arguments);
            Player player = Player.Get(playerSender.ReferenceHub);

            Features.Instance.StartEventImmediately(eventName, player);
            response = $"Event '{eventName}' is started!";
            return true;
        }
    }
}

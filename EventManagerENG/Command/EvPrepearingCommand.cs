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
    public class EventPreparingCommand : ICommand
    {
        public string Command => "evpreparing";
        public string[] Aliases => new[] { "evp" };
        public string Description => "Start prepearing for event";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Command only for players!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: evpreparing [nameofevent] (assist)";
                return false;
            }

            string eventName = arguments.At(0);
            string assistant = arguments.Count > 1 ? arguments.At(1) : null;
            Player player = Player.Get(playerSender.ReferenceHub);

            Features.Instance.StartEventPreparation(eventName, player, assistant);
            response = $"Prepering of event '{eventName}' is started" + (assistant != null ? $" with assist {assistant}" : "");
            return true;
        }
    }
}

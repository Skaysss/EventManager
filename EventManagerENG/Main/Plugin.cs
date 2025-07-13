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
    public class Features : Plugin<Plugin>
    {
        public override string Name => "EventManager";
        public override string Author => "Skay";
        public override Version Version => new Version(1, 0, 3);
        public override Version RequiredExiledVersion => new Version(9, 6, 1);

        public static Features Instance { get; private set; }
        public static ActiveEvent CurrentEvent { get; private set; }
        private CoroutineHandle eventCoroutine;
        private Dictionary<string, int> voteResults = new Dictionary<string, int>();
        private Dictionary<string, int> voteAgainst = new Dictionary<string, int>();
        private bool isVotingActive = false;
        private string currentVoteEvent;
        private Player voteInitiator;

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;

            if (eventCoroutine.IsRunning)
                Timing.KillCoroutines(eventCoroutine);
            base.OnDisabled();
        }

        private void OnRoundStart()
        {
            CurrentEvent = null;
            if (eventCoroutine.IsRunning)
                Timing.KillCoroutines(eventCoroutine);
        }
    
        public string Vote(Player player, bool voteFor)
        {
            if (!isVotingActive)
                return "Vote is not sterted!";

            if (voteFor)
            {
                voteResults[player.UserId] = 1;
                return "Your voice for YES send!";
            }
            else
            {
                voteAgainst[player.UserId] = 1;
                return "Your voice for PASS send!";
            }
        }
        public void StartEvent(string name, Player host, DateTime startTime)
        {
            CurrentEvent = new ActiveEvent
            {
                Name = name,
                Host = host,
                StartTime = startTime,
                AnnouncementTime = DateTime.Now,
                IsActive = false,

            };
            

            if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
            {
                Task.Run(() => SendEventToDiscordAsync(CurrentEvent));
            }



        }
        private async Task SendEventToDiscordAsync(ActiveEvent ev)
        {
            try
            {
                var embed = new
                {
                    title = $"{Config.EventTitle}",
                    description = $"**{ev.Name}**!",
                    color = 3447003,
                    fields = new[]
                    {
                        new { name = $"{Config.Eventolog}", value = ev.Host.Nickname, inline = true },
                        new { name = $"{Config.Time}", value = $"<t:{((DateTimeOffset)ev.StartTime).ToUnixTimeSeconds()}:F>", inline = true }
                    },
                    footer = new { text = "Event Manager System" },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                await SendDiscordMessage(embed, "Event Manager");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка отправки ивента в Discord: {ex}");
            }
        }
        private async Task SendEventStartToDiscordAsync(ActiveEvent ev)
        {
            try
            {
                var embed = new
                {
                    title = $"{Config.StartEvent}",
                    description = $"**{ev.Name}**!",
                    color = 65280,
                    fields = new[]
                    {
                        new { name = $"{Config.Eventolog}", value = ev.Host.Nickname, inline = true },
                        new { name = $"{Config.Time}", value = $"<t:{((DateTimeOffset)ev.StartTime).ToUnixTimeSeconds()}:F>", inline = true }
                    },
                    footer = new { text = $"{Config.Text}" },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                await SendDiscordMessage(embed, "Event Manager");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка отправки начала ивента в Discord: {ex}");
            }
        }

        private async Task SendEventPreparationToDiscordAsync(string eventName, Player initiator, string assistant)
        {
            try
            {
                var embed = new
                {
                    title = $"{Config.StartPrepearingDS}",
                    description = $"**{eventName}**!",
                    color = 16753920,
                    fields = new[]
                    {
                        new { name = $"{Config.OwnerEvent}", value = initiator.Nickname, inline = true },
                        new { name = $"{Config.AssistEvent}", value = assistant ?? "Не назначен", inline = true },
                    },
                    footer = new { text = $"{Config.Text}" },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                await SendDiscordMessage(embed, "Event Manager");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка отправки подготовки ивента в Discord: {ex}");
            }
        }

        private async Task SendVoteResultsToDiscordAsync(string eventName, int yes, int no)
        {
            try
            {
                var embed = new
                {
                    title = $"{Config.ResultVote}",
                    description = $"For event **{eventName}**",
                    color = 10181046,
                    fields = new[]
                    {
                        new { name = $"{Config.YesText}", value = yes.ToString(), inline = true },
                        new { name = $"{Config.NoText}", value = no.ToString(), inline = true },
                    },
                    footer = new { text = $"{Config.Text}" },
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                await SendDiscordMessage(embed, "Event Manager");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка отправки результатов голосования в Discord: {ex}");
            }
        }

        private async Task SendDiscordMessage(object embed, string username)
        {
            var payload = new
            {
                username = username,
                content = Config.DiscordMentionRoleId != 0 ? $"<@&{Config.DiscordMentionRoleId}>" : null,
                embeds = new[] { embed }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(Config.DiscordWebhookUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Log.Error($"Ошибка отправки в Discord: {response.StatusCode} - {errorContent}");
                }
            }
        }
       
        public void StartEventImmediately(string name, Player host)
        {
            CurrentEvent = new ActiveEvent
            {
                Name = name,
                Host = host,
                StartTime = DateTime.Now,
                AnnouncementTime = DateTime.Now,
                IsActive = true,

            };

            Map.Broadcast(10, $"Event '{name}' is starting!\nOwner: {host.Nickname}", Broadcast.BroadcastFlags.Normal);

            if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
            {
                Task.Run(() => SendEventStartToDiscordAsync(CurrentEvent));
            }
        }
        public void StartEventPreparation(string eventName, Player initiator, string assistant = null)
        {
            string message = $"{initiator.Nickname} {Config.StartPrepearing}: {eventName}";
            if (!string.IsNullOrEmpty(assistant))
            {
                message += $"\n{Config.AssistName}: {assistant}";
            }

            Map.Broadcast(10, message, Broadcast.BroadcastFlags.Normal);

            if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
            {
                Task.Run(() => SendEventPreparationToDiscordAsync(eventName, initiator, assistant));
            }
        }
        public void StartVote(string eventName, Player initiator)
        {
            if (isVotingActive) return;

            currentVoteEvent = eventName;
            voteInitiator = initiator;
            voteResults.Clear();
            voteAgainst.Clear();
            isVotingActive = true;

            Map.Broadcast(15, $"{initiator.Nickname} {Config.StartVoteText}: {eventName}\n.yes - +\n.pass - -", Broadcast.BroadcastFlags.Normal);



            Timing.CallDelayed(30f, () =>
            {
                isVotingActive = false;
                int yes = voteResults.Values.Sum();
                int no = voteAgainst.Values.Sum();

                voteInitiator.Broadcast(15, $"{Config.ResultText} {currentVoteEvent}:\nYES: {yes}\nNO: {no}", Broadcast.BroadcastFlags.Normal);

                if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
                {
                    Task.Run(() => SendVoteResultsToDiscordAsync(currentVoteEvent, yes, no));
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace EventManagerENG
{
    public class Plugin : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Discord Webhook URL")]
        public string DiscordWebhookUrl { get; set; } = "";

        [Description("ID for ping role (0 for disable)")]
        public ulong DiscordMentionRoleId { get; set; } = 0;

        [Description("Voice of yes")]
        public string YesText { get; set; } = "✅ Yes";

        [Description("Voice of no")]
        public string NoText { get; set; } = "❌ No";

        [Description("Text at the bottom of the Webhook message")]
        public string Text { get; set; } = "Event Manaher System";

        [Description("Text based on voting results")]
        public string ResultText { get; set; } = "Результаты голосования";

        [Description("Text when eventolog started vote of event")]
        public string StartVoteText { get; set; } = "Начал голосование за ивент";

        [Description("Text for name assist")]
        public string AssistName { get; set; } = "Помощник";

        [Description("Text when eventolog started prepearing for event")]
        public string StartPrepearing { get; set; } = "Начинает подготовку к ивенту";

        [Description("Text of result vote")]
        public string ResultVote { get; set; } = "📊 Результаты голосования";

        [Description("Text of title when eventolog started prepearing")]
        public string StartPrepearingDS { get; set; } = "🛠️ Начата подготовка к ивенту";

        [Description("Text of Owner event")]
        public string OwnerEvent { get; set; } = "👤 Организатор";

        [Description("Text of Assist event")]
        public string AssistEvent { get; set; } = "🛠️ Помощник";

        [Description("Text of title when event started")]
        public string StartEvent { get; set; } = "🚀 Ивент начался";

        [Description("Owner event")]
        public string Eventolog { get; set; } = "👤 Ведущий";

        [Description("Event time")]
        public string Time { get; set; } = "🕐 Начало";

        [Description("Title when event should start")]
        public string EventTitle { get; set; } = "🎉 Ивент";



    }
}

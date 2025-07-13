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
    public class ActiveEvent
    {
        public string Name { get; set; }
        public Player Host { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime AnnouncementTime { get; set; }
        public bool IsActive { get; set; }
    }
}

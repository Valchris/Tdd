﻿using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Tdd.Models
{
    [Serializable]
    public class Player
    {
        public Player(HubCallerContext context, int position)
        {
            this.Context = context;
            this.StartingLocation = Constants.StartingLocations[position];
            this.EndingLocation = Constants.EndingLocations[position];
            this.CurrentLife = Constants.StartingLife;
            this.Resources = JsonConvert.DeserializeObject<Resources>(JsonConvert.SerializeObject(Constants.StartingResources));
        }

        [JsonIgnore]
        public HubCallerContext Context {get; set;}

        public int CurrentLife { get; set; }

        public Point StartingLocation { get; set; }

        public Point EndingLocation { get; set; }

        public Resources Resources { get; set; }
    }
}
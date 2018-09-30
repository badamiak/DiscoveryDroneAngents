﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DiscoveryDroneAgents.API.Messages
{
    public class StopMovingMessage : Message
    {
        public StopMovingMessage(IActorRef sender, string name) : base(sender)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
﻿using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Messages
{
    public class AddDiscoveryDroneMessage: Message
    {
        public AddDiscoveryDroneMessage(IActorRef sender, DiscoveryDroneConfig droneConfig) : base(sender)
        {
            this.DroneConfig = droneConfig;
        }

        public string Name { get; }
        public int Range { get; }
        public int Vision { get; }
        public DiscoveryDroneConfig DroneConfig { get; }
    }
}

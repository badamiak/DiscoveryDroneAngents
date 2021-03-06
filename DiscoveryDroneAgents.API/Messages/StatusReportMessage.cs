﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DiscoveryDroneAgents.API.Model;

namespace DiscoveryDroneAgents.API.Messages
{
    public class StatusReportMessage : IMessage
    {
        public StatusReportMessage(DiscoveryDroneStatus status)
        {
            this.Status = status;
        }

        public DiscoveryDroneStatus Status { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DiscoveryDroneAgents.API.Messages
{
    public class StatusReportMessage : Message
    {
        public StatusReportMessage(IActorRef sender) : base(sender)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DiscoveryDroneAgents.API.Messages
{
    public class GetMapMessage : Message
    {
        public GetMapMessage(IActorRef sender, string whoseMap = "world") : base(sender)
        {
            this.WhoseMap = whoseMap;
        }

        public string WhoseMap { get; }
    }
}

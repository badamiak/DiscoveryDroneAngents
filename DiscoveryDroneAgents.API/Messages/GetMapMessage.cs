using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DiscoveryDroneAgents.API.Messages
{
    public class GetMapMessage : IMessage
    {
        public GetMapMessage(string whoseMap = "world")
        {
            this.WhoseMap = whoseMap;
        }

        public string WhoseMap { get; }
    }
}

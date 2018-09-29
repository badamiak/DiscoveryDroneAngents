using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Messages
{
    public class ReportStatusMessage: Message
    {
        public ReportStatusMessage(IActorRef sender) : base(sender)
        {
            this.Sender = sender;
        }

        public IActorRef Sender { get; }
    }
}

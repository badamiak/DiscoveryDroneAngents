using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Messages
{
    /// <summary>
    /// marker interface for all messages
    /// </summary>
    public abstract class Message
    {
        public IActorRef Sender { get; }

        public Message(IActorRef sender)
        {
            this.Sender = sender;
        }
    }
}

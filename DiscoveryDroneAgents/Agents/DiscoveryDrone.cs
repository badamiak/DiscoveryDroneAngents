using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.Agents
{
    public class DiscoveryDrone : UntypedActor
    {
        DiscoveryDroneConfig Config { get; }

        public DiscoveryDrone(DiscoveryDroneConfig config)
        {
            this.Config = config;
        }

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }
    }
}

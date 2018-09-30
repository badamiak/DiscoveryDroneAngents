using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Messages
{
    public class AddDiscoveryDroneMessage: IMessage
    {
        public AddDiscoveryDroneMessage(DiscoveryDroneConfig droneConfig)
        {
            this.DroneConfig = droneConfig;
        }

        public DiscoveryDroneConfig DroneConfig { get; }
    }
}

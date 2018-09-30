using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace DiscoveryDroneAgents.API.Messages
{
    public class UpdateMapMessage : Message
    {
        public UpdateMapMessage(IActorRef sender, int dronePositionX, int dronePositionY, int vision) : base(sender)
        {
            this.DronePositionX = dronePositionX;
            this.DronePositionY = dronePositionY;
            this.Vision = vision;
        }

        public int DronePositionX { get; }
        public int DronePositionY { get; }
        public int Vision { get; }
    }
}

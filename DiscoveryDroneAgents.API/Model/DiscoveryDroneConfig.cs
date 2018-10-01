using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Model
{
    public class DiscoveryDroneConfig
    {
        public DiscoveryDroneConfig(string name, int positionX, int positionY, float turnLikelines, int vision, int moveInterval)
        {
            this.Name = name;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.TurnLikelines = turnLikelines;
            this.Vision = vision;
            this.MoveInterval = moveInterval;
        }

        public string Name { get; }
        public int PositionX { get; }
        public int PositionY { get; }
        public float TurnLikelines { get; }
        public int Vision { get; }
        public int MoveInterval { get; }
    }
}

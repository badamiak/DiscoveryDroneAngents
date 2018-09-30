using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Model
{
    public class DiscoveryDroneStatus
    {
        public DiscoveryDroneStatus(string name, int positionX, int positionY, TileType[,] map)
        {
            this.Name = name;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.Map = map;
        }

        public string Name { get; }
        public int PositionX { get; }
        public int PositionY { get; }
        public TileType[,] Map { get; }
    }
}

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

        public DiscoveryDroneStatus Move(int positionX, int positionY)
        {
            return new DiscoveryDroneStatus(this.Name, positionX, positionY, this.Map);
        }

        public DiscoveryDroneStatus UpdateMap(TileType[,] map)
        {
            return new DiscoveryDroneStatus(this.Name, this.PositionX, this.PositionY, map);

        }
    }
}

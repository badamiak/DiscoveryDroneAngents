using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Model
{
    public class MapUpdate
    {
        TileType[,] Patch { get; }
        public int PositionX { get; }
        public int PositionY { get; }
        public int SizeX { get; }
        public int SizeY { get; }

        public MapUpdate(TileType[,] patch, int positionX, int positionY, int sizeX, int sizeY)
        {
            this.Patch = patch;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.SizeX = sizeX;
            this.SizeY = sizeY;
        }
    }
}

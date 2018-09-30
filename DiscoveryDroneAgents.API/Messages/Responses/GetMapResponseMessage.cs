using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DiscoveryDroneAgents.API.Model;

namespace DiscoveryDroneAgents.API.Messages.Responses
{
    public class GetMapResponseMessage : IMessage
    {
        public TileType[,] Map { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public List<DiscoveryDroneStatus> DronesPositions { get; }

        public GetMapResponseMessage(TileType[,] map, int sizeX, int sizeY, List<DiscoveryDroneStatus> dronesPositions)
        {
            this.Map = map;
            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.DronesPositions = dronesPositions;
        }
    }
}

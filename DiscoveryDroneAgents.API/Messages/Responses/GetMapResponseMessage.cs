using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DiscoveryDroneAgents.API.Model;

namespace DiscoveryDroneAgents.API.Messages.Responses
{
    public class GetMapResponseMessage : Message
    {
        public TileType[,] Map { get; }
        public int SizeX { get; }
        public int SizeY { get; }

        public GetMapResponseMessage(IActorRef sender, TileType[,] map, int sizeX, int sizeY) : base(sender)
        {
            this.Map = map;
            this.SizeX = sizeX;
            this.SizeY = sizeY;
        }
    }
}

using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Messages
{

    public class InitWorldMessage : Message
    {
        private readonly Dictionary<TileType, float> tileTypeProbabilities;

        public InitWorldMessage(IActorRef sender, int sizeX, int sizeY, Dictionary<TileType,float> tileTypeProbabilities) : base(sender)
        {
            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.tileTypeProbabilities = tileTypeProbabilities;
        }

        public int SizeX { get; }
        public int SizeY { get; }

        public float GetTileProbability(TileType tileType)
        {
            if(tileTypeProbabilities.ContainsKey(tileType))
            {
                return this.tileTypeProbabilities[tileType];
            }
            else
            {
                return 0f;
            }
        }
    }
}

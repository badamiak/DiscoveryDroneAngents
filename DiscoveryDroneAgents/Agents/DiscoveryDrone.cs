using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using DiscoveryDroneAgents.API.Messages;

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
        private DiscoveryDroneStatus status;
        Dictionary<Type, Action<Message>> handlers;
        public TileType[,] Map { get; }

        public DiscoveryDrone(DiscoveryDroneConfig config, TileType[,] map)
        {
            this.Config = config;
            this.Map = map;

            this.status = new DiscoveryDroneStatus(config.PositionX, config.PositionY, map);

            this.handlers = new Dictionary<Type, Action<Message>>();
        }

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }

        private void GetStatusMessageHandler(Message message)
        {
            Context.Sender.Tell(new StatusReportMessage(Context.Self, status));
        }
    }
}

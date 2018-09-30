using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using DiscoveryDroneAgents.API.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;

namespace DiscoveryDroneAgents.Agents
{
    public class DiscoveryDrone : UntypedActor
    {
        DiscoveryDroneConfig Config { get; }
        private DiscoveryDroneStatus status;
        Dictionary<Type, Action<Message>> handlers;
        public TileType[,] Map { get; }

        public ILoggingAdapter logger = Context.GetLogger();

        public DiscoveryDrone(DiscoveryDroneConfig config, TileType[,] map)
        {
            this.Config = config;
            this.Map = map;

            this.status = new DiscoveryDroneStatus(this.Config.Name, config.PositionX, config.PositionY, map);

            this.handlers = new Dictionary<Type, Action<Message>>();
            this.handlers.Add(typeof(ReportStatusMessage), ReportStatusMessageHandler);
        }

        protected override void PreStart()
        {
            base.PreStart();

            var getInitioalVisionTask = Context.Parent.Ask(new UpdateMapMessage(Context.Self, status.PositionX, status.PositionY, Config.Vision));
            getInitioalVisionTask.Wait();
        }

        protected override void OnReceive(object message)
        {
            if (handlers.ContainsKey(message.GetType()))
            {
                handlers[message.GetType()].Invoke(message as Message);
            }
            else
            {
                logger.Warning($"Unknown message type {message.GetType()} -> {message}");
            }
        }

        private void ReportStatusMessageHandler(Message message)
        {
            Context.Sender.Tell(new StatusReportMessage(Context.Self, status));
        }
    }
}

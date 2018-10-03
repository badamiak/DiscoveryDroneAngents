using Akka.Actor;
using Akka.Event;
using DiscoveryDroneAgents.API.Messages;
using DiscoveryDroneAgents.API.Messages.Responses;
using DiscoveryDroneAgents.API.Model;
using DiscoveryDroneAngents.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.Agents
{
    public class Relay : UntypedActor
    {
        TileType[,] map;

        private int worldSizeX = 0;
        private int worldSizeY = 0;

        public ILoggingAdapter logger = Context.GetLogger();


        private Dictionary<string, DiscoveryDroneStatus> dronesStatuses;
        private Dictionary<Type, Action<IMessage>> messageHandlers;

        public Relay(TileType[,] map, int sizeX, int sizeY)
        {
            this.map = map;
            this.worldSizeX = sizeX;
            this.worldSizeY = sizeY;
        }

        protected override void PreStart()
        {
            base.PreStart();

            dronesStatuses = new Dictionary<string, DiscoveryDroneStatus>();
            messageHandlers = new Dictionary<Type, Action<IMessage>>();

            messageHandlers.Add(typeof(StatusReportMessage), StatusReportHandler);
            messageHandlers.Add(typeof(AddDiscoveryDroneMessage), AddDroneHandler);
            messageHandlers.Add(typeof(GetMapMessage), GetMapHandler);
            messageHandlers.Add(typeof(StartMovingMessage), StartMovingHandler);
            messageHandlers.Add(typeof(StopMovingMessage), StopMovingHandler);



        }
        protected override void OnReceive(object message)
        {
            var messageType = message.GetType();

            logger.Debug($"Received message type {messageType}");

            if (messageHandlers.ContainsKey(message.GetType()))
            {
                messageHandlers[messageType].Invoke(message as IMessage);
            }
            else
            {
                logger.Info($"Message type unknown {messageType}");
            }
        }

        private void StatusReportHandler(IMessage message)
        {
            var parsed = message as StatusReportMessage;

            this.dronesStatuses[parsed.Status.Name] = parsed.Status;

            this.map = MapHelper.CorelateMaps(this.map, parsed.Status.Map, this.worldSizeX, this.worldSizeY);
        }

        private void AddDroneHandler(IMessage message)
        {
            var parsed = message as AddDiscoveryDroneMessage;

            var unchartedMap = MapHelper.GetUnchartedMap(this.worldSizeX, this.worldSizeY);

            var newDrone = Context.ActorOf(Props.Create<DiscoveryDrone>(parsed.DroneConfig, unchartedMap), parsed.DroneConfig.Name);

            var status = new DiscoveryDroneStatus(parsed.DroneConfig.Name, parsed.DroneConfig.PositionX, parsed.DroneConfig.PositionY, unchartedMap);
            this.dronesStatuses.Add(parsed.DroneConfig.Name, status);

            Context.ActorSelection($"akka://{Context.System.Name}/user/world").Tell( new StatusReportMessage(status));
        }

        private void GetMapHandler(IMessage message)
        {
            var parsed = message as GetMapMessage;

            if (parsed.WhoseMap == "relay")
            {
                Sender.Tell(new GetMapResponseMessage(this.map, this.worldSizeX, this.worldSizeY, this.dronesStatuses.Select(x => x.Value).ToList()));
            }
            else
            {
                Sender.Tell(new GetMapResponseMessage(dronesStatuses[parsed.WhoseMap].Map, this.worldSizeX, this.worldSizeY, new List<DiscoveryDroneStatus> { this.dronesStatuses[parsed.WhoseMap] }));
            }
        }

        private void StartMovingHandler(IMessage message)
        {
            var parsed = message as StartMovingMessage;

            Context.ActorSelection($"akka://{Context.System.Name}/user/world/relay/{parsed.Name}").Tell(message);
        }

        private void StopMovingHandler(IMessage message)
        {
            var parsed = message as StartMovingMessage;

            Context.ActorSelection($"akka://{Context.System.Name}/user/world/relay/{parsed.Name}").Tell(message);
        }

    }
}

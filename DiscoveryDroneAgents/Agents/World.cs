using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using DiscoveryDroneAgents.API.Messages;
using DiscoveryDroneAgents.API.Messages.Responses;
using DiscoveryDroneAgents.API.Model;
using DiscoveryDroneAngents.API;

namespace DiscoveryDroneAgents.Agents
{
    public class World : UntypedActor
    {
        public ILoggingAdapter logger = Context.GetLogger();
        private Dictionary<Type, Action<IMessage>> messageHandlers;
        private Dictionary<string, DiscoveryDroneStatus> dronesStatuses;
        private int worldSizeX = 0;
        private int worldSizeY = 0;


        private TileType[,] map;

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

        protected override void PreStart()
        {
            base.PreStart();
            logger.Info("Starting the system");
            logger.Debug("Initiating messageHandlers");

            dronesStatuses = new Dictionary<string, DiscoveryDroneStatus>();

            messageHandlers = new Dictionary<Type, Action<IMessage>>();

            messageHandlers.Add(typeof(InitWorldMessage), this.InitWorldHandler);
            messageHandlers.Add(typeof(GetMapMessage), this.GetMapHandler);
            messageHandlers.Add(typeof(UpdateMapMessage), this.UpdateMapHandler);
            messageHandlers.Add(typeof(AddDiscoveryDroneMessage), this.AddDroneHandler);
            messageHandlers.Add(typeof(StatusReportMessage), this.StatusReportHandler);
            messageHandlers.Add(typeof(StartMovingMessage), this.StartMovingHandler);
            messageHandlers.Add(typeof(StopMovingMessage), this.StopMovingHandler);


            logger.Debug("Initiated messageHandlers");
        }


        protected override void PostStop()
        {
            base.PostStop();
            logger.Info("Stopping the system");
        }

        private void StatusReportHandler(IMessage message)
        {
            var parsed = message as StatusReportMessage;

            this.dronesStatuses[parsed.Status.Name] = parsed.Status;
        }

        private void InitWorldHandler(IMessage message)
        {
            var parsed = message as InitWorldMessage;

            this.worldSizeX = parsed.SizeX;
            this.worldSizeY = parsed.SizeY;
            this.map = new TileType[parsed.SizeX, parsed.SizeY];

            var random  = new Random(0);
            var tileRandomizer = new List<TileType>();
            foreach(var type in Enum.GetValues(typeof(TileType)))
            {
                if ((TileType)type == TileType.Uncharted) continue;

                for(int i = 0; i < 100*parsed.GetTileProbability((TileType)type); i++)
                {
                    tileRandomizer.Add((TileType)type);
                }
            }


            for(int x = 0; x < this.worldSizeX; x++)
            {
                for( int y = 0; y < this.worldSizeY; y++)
                {
                    if(
                        x==0 
                        || x==1
                        || x == this.worldSizeX - 1 
                        || x == this.worldSizeX - 2
                        || y == 0
                        || y == 1
                        || y == worldSizeY - 1
                        || y == worldSizeY - 2
                        )
                    {
                        this.map[x, y] = TileType.HighObstacle;
                    }
                    else
                    {

                        this.map[x, y] = tileRandomizer[random.Next(tileRandomizer.Count)];
                    }
                }
            }
        }

        private void GetMapHandler(IMessage message)
        {
            var parsed = message as GetMapMessage;

            if(parsed.WhoseMap == "world")
            {
                Sender.Tell(new GetMapResponseMessage(this.map, this.worldSizeX, this.worldSizeY, this.dronesStatuses.Select(x => x.Value).ToList()));
            }
            else
            {
                Sender.Tell(new GetMapResponseMessage(dronesStatuses[parsed.WhoseMap].Map, this.worldSizeX, this.worldSizeY, new List<DiscoveryDroneStatus> { this.dronesStatuses[parsed.WhoseMap] }));
            }
        }

        private void AddDroneHandler(IMessage message)
        {
            var parsed = message as AddDiscoveryDroneMessage;

            var unchartedMap = MapHelper.GetUnchartedMap(this.worldSizeX, this.worldSizeY);
            var newDrone = Context.ActorOf(Props.Create<DiscoveryDrone>(parsed.DroneConfig, unchartedMap), parsed.DroneConfig.Name);

            var status = new DiscoveryDroneStatus(parsed.DroneConfig.Name, parsed.DroneConfig.PositionX, parsed.DroneConfig.PositionY, unchartedMap);
            this.dronesStatuses.Add(parsed.DroneConfig.Name, status);
        }

        private void UpdateMapHandler(IMessage message)
        {
            var parsed = message as UpdateMapMessage;

            var patchSize = 1 + parsed.Vision * 2;
            var patchStartPositionX = parsed.DronePositionX - parsed.Vision;
            var patchStartPositionY = parsed.DronePositionY - parsed.Vision;
            TileType[,] patch = new TileType[patchSize, patchSize];

            int patchX = 0;
            int patchY = 0;

            for (int x = patchStartPositionX ; x <= parsed.DronePositionX + parsed.Vision; x++)
            {
                patchY = 0;
                for(int y = patchStartPositionY; y <= parsed.DronePositionY + parsed.Vision; y++)
                {
                    patch[patchX, patchY] = this.map[x, y];
                    patchY++;
                }
                patchX++;
            }

            Sender.Tell(new MapUpdate(patch, patchStartPositionX, patchStartPositionY, patchSize, patchSize));
        }

        private void StartMovingHandler(IMessage message)
        {
            var parsed = message as StartMovingMessage;

            var drone = Context.ActorSelection($"akka://{Context.System.Name}/user/world/{parsed.Name}");
            drone.Tell(message);
        }

        private void StopMovingHandler(IMessage message)
        {
            var parsed = message as StopMovingMessage;

            var drone = Context.ActorSelection($"akka://{Context.System.Name}/user/world/{parsed.Name}");
            drone.Tell(message);
        }
    }
}

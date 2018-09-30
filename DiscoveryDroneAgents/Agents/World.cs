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
        private Dictionary<Type, Action<Message>> messageHandlers;
        private Dictionary<string, DiscoveryDroneStatus> DronesStatuses;
        private int worldSizeX = 0;
        private int worldSizeY = 0;


        private TileType[,] map;

        protected override void OnReceive(object message)
        {
            var messageType = message.GetType();

            logger.Info($"Received message type {messageType}");

            if (messageHandlers.ContainsKey(message.GetType()))
            {
                messageHandlers[messageType].Invoke(message as Message);
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

            DronesStatuses = new Dictionary<string, DiscoveryDroneStatus>();

            messageHandlers = new Dictionary<Type, Action<Message>>();

            messageHandlers.Add(typeof(InitWorldMessage), this.InitWorldHandler);
            messageHandlers.Add(typeof(GetMapMessage), this.GetMapHandler);
            messageHandlers.Add(typeof(UpdateMapMessage), this.UpdateMapHandler);
            messageHandlers.Add(typeof(AddDiscoveryDroneMessage), this.AddDroneHandler);
            messageHandlers.Add(typeof(StartMovingMessage), this.StartMovingHandler);
            messageHandlers.Add(typeof(StopMovingMessage), this.StopMovingHandler);


            logger.Debug("Initiated messageHandlers");
        }

        protected override void PostStop()
        {
            base.PostStop();
            logger.Info("Stopping the system");
        }

        private void InitWorldHandler(Message message)
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
                    if(x==0 | x == this.worldSizeX-1 | y == 0 | y == worldSizeY-1)
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

        private void GetMapHandler(Message message)
        {
            var parsed = message as GetMapMessage;

            if(parsed.WhoseMap == "world")
            {
                (message as GetMapMessage)?.Sender.Tell(new GetMapResponseMessage(Context.Self, this.map, this.worldSizeX, this.worldSizeY, this.DronesStatuses.Select(x => x.Value).ToList()));
            }
            else
            {
                (message as GetMapMessage)?.Sender.Tell(new GetMapResponseMessage(Context.Self, DronesStatuses[parsed.WhoseMap].Map, this.worldSizeX, this.worldSizeY, new List<DiscoveryDroneStatus> { this.DronesStatuses[parsed.WhoseMap] }));
            }
        }

        private void AddDroneHandler(Message message)
        {
            var parsed = message as AddDiscoveryDroneMessage;

            var unchartedMap = MapHelper.GetUnchartedMap(this.worldSizeX, this.worldSizeY);
            var newDrone = Context.ActorOf(Props.Create<DiscoveryDrone>(parsed.DroneConfig, unchartedMap), parsed.DroneConfig.Name);

            var status = new DiscoveryDroneStatus(parsed.DroneConfig.Name, parsed.DroneConfig.PositionX, parsed.DroneConfig.PositionY, unchartedMap);
            this.DronesStatuses.Add(parsed.DroneConfig.Name, status);
        }

        private void UpdateMapHandler(Message message)
        {
            var parsed = message as UpdateMapMessage;

            Sender.Tell(new MapUpdate(null, 0, 0, 0, 0));
        }

        private void StartMovingHandler(Message message)
        {
            var parsed = message as StartMovingMessage;

            var drone = Context.ActorSelection($"akka://{Context.System.Name}/user/world/{parsed.Name}");
            drone.Tell(message);
        }

        private void StopMovingHandler(Message message)
        {
            var parsed = message as StopMovingMessage;

            var drone = Context.ActorSelection($"akka://{Context.System.Name}/user/world/{parsed.Name}");
            drone.Tell(message);
        }
    }
}

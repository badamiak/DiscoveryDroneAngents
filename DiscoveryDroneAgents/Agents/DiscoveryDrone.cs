using Akka.Actor;
using DiscoveryDroneAgents.API.Model;
using DiscoveryDroneAgents.API.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;
using System.Diagnostics;
using System.Timers;

namespace DiscoveryDroneAgents.Agents
{
    public class DiscoveryDrone : UntypedActor
    {
        DiscoveryDroneConfig Config { get; }
        private DiscoveryDroneStatus status;
        Dictionary<Type, Action<IMessage>> handlers;
        private MoveDirection moveDirection = MoveDirection.Right;
        public ILoggingAdapter logger = Context.GetLogger();

        private Timer timer;
        private IActorRef parent;
        private readonly Random random;

        public DiscoveryDrone(DiscoveryDroneConfig config, TileType[,] map)
        {
            this.Config = config;

            this.random = new Random();
            this.timer = new Timer(5000);
            this.timer.Elapsed += (sender, evantArgs) => this.Move();
            this.timer.AutoReset = true;

            this.status = new DiscoveryDroneStatus(this.Config.Name, config.PositionX, config.PositionY, map);

            this.handlers = new Dictionary<Type, Action<IMessage>>();
            this.handlers.Add(typeof(ReportStatusMessage), this.ReportStatusMessageHandler);
            this.handlers.Add(typeof(StartMovingMessage), this.StartMovingHandler);
            this.handlers.Add(typeof(StopMovingMessage), this.ReportStatusMessageHandler);
        }

        protected override void PreStart()
        {
            base.PreStart();

            this.parent = Context.Parent;

            logger.Info($"Boot-up: {Self.Path}");
        }

        protected override void OnReceive(object message)
        {
            logger.Debug($"Received message {message}");

            if (this.handlers.ContainsKey(message.GetType()))
            {
                this.handlers[message.GetType()].Invoke(message as IMessage);
            }
            else
            {
                this.logger.Warning($"Unknown message type {message.GetType()} -> {message}");
            }
        }

        protected override void PostStop()
        {
            base.PostStop();

            logger.Warning("Shutting down");
        }

        private void ReportStatusMessageHandler(IMessage message)
        {
            Context.Sender.Tell(new StatusReportMessage(this.status));
        }

        private bool CanMoveTo(int positionX, int positionY)
        {
            var mapTile = this.status.Map[positionX, positionY];
            return mapTile == TileType.Passable;
        }

        private void ChangeDirection()
        {
            this.moveDirection = (MoveDirection)this.random.Next(1, 4);
        }

        private void StartMovingHandler(IMessage _) => this.timer.Start();
        private void StopMovingHandler(IMessage _) => this.timer.Stop();


        private void Move()
        {
            var perciveTask = parent.Ask<MapUpdate>(new UpdateMapMessage(this.status.PositionX, this.status.PositionY, this.Config.Vision));
            perciveTask.Wait(); //TODO: przyjmij mapę

            var newMap = perciveTask.Result;


            if (this.random.NextDouble() <= this.Config.TurnLikelines)
            {
                this.ChangeDirection();
            }
            bool moved = false;
            while (!moved)
            {
                switch (this.moveDirection)
                {
                    case MoveDirection.Up:
                        {
                            if (this.CanMoveTo(this.status.PositionX, this.status.PositionY + 1))
                            {
                                this.status = this.status.Move(this.status.PositionX, this.status.PositionY + 1);
                                moved = true;
                            }
                            break;
                        }
                    case MoveDirection.Right:
                        {
                            if (this.CanMoveTo(this.status.PositionX + 1, this.status.PositionY))
                            {
                                this.status = this.status.Move(this.status.PositionX + 1, this.status.PositionY);
                                moved = true;
                            }
                            break;
                        }
                    case MoveDirection.Down:
                        {
                            if (this.CanMoveTo(this.status.PositionX, this.status.PositionY - 1))
                            {
                                this.status = this.status.Move(this.status.PositionX, this.status.PositionY - 1);
                                moved = true;
                            }
                            break;
                        }
                    case MoveDirection.Left:
                        {
                            if (this.CanMoveTo(this.status.PositionX - 1, this.status.PositionY))
                            {
                                this.status = this.status.Move(this.status.PositionX - 1, this.status.PositionY);
                                moved = true;
                            }
                            break;
                        }
                }
                if (!moved) this.ChangeDirection();
            }
        }
    }
}

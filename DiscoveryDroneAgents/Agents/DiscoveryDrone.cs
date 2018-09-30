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
        Dictionary<Type, Action<Message>> handlers;
        private MoveDirection moveDirection = MoveDirection.Right;
        public ILoggingAdapter logger = Context.GetLogger();

        private Timer timer;
        private readonly Random random;

        public DiscoveryDrone(DiscoveryDroneConfig config, TileType[,] map)
        {
            this.Config = config;

            this.random = new Random();
            this.timer = new Timer(5000);
            this.timer.Elapsed += (sender, evantArgs) => this.Move();
            this.timer.AutoReset = true;

            this.status = new DiscoveryDroneStatus(this.Config.Name, config.PositionX, config.PositionY, map);

            this.handlers = new Dictionary<Type, Action<Message>>();
            this.handlers.Add(typeof(ReportStatusMessage), this.ReportStatusMessageHandler);
            this.handlers.Add(typeof(StartMovingMessage), this.StartMovingHandler);
            this.handlers.Add(typeof(StopMovingMessage), this.ReportStatusMessageHandler);
        }

        protected override void PreStart()
        {
            base.PreStart();

            var getInitioalVisionTask = Context.Parent.Ask(new UpdateMapMessage(Context.Self, this.status.PositionX, this.status.PositionY, this.Config.Vision));
            getInitioalVisionTask.Wait(); //TODO: przyjmij mapę

            var newMap = getInitioalVisionTask.Result as MapUpdate;

        }

        protected override void OnReceive(object message)
        {
            if (this.handlers.ContainsKey(message.GetType()))
            {
                this.handlers[message.GetType()].Invoke(message as Message);
            }
            else
            {
                this.logger.Warning($"Unknown message type {message.GetType()} -> {message}");
            }
        }

        private void ReportStatusMessageHandler(Message message)
        {
            Context.Sender.Tell(new StatusReportMessage(Context.Self, this.status));
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

        private void StartMovingHandler(Message _) => this.timer.Start();
        private void StopMovingHandler(Message _) => this.timer.Stop();


        private void Move()
        {
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

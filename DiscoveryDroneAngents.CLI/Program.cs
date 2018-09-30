using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;
using DiscoveryDroneAgents.Agents;
using DiscoveryDroneAgents.API.Messages;
using DiscoveryDroneAgents.API.Messages.Responses;
using DiscoveryDroneAgents.API.Model;

namespace DiscoveryDroneAngents.CLI
{
    

    class Program
    {
        static IActorRef world, console;
        static void Main(string[] args)
        {
            var tileProbabilities = new Dictionary<TileType, float>();
            tileProbabilities.Add(TileType.HighObstacle, 0.03f);
            tileProbabilities.Add(TileType.LowObstacle, 0.1f);
            tileProbabilities.Add(TileType.Passable, 0.86f);

            using (var system = ActorSystem.Create("Mars"))
            {
                console = system.ActorOf(Props.Create<ConsoleActor>());

                world = system.ActorOf(Props.Create<World>(), "world");

                world.Tell(new InitWorldMessage(70, 20, tileProbabilities), console);


                string userInput = string.Empty;
                while(userInput != UserCommands.Exit)
                {
                    userInput = Console.ReadLine();
                    UserInputHandler(userInput);
                }
            }

        }

        static void UserInputHandler(string input)
        {
            try
            {
                if (input.StartsWith(UserCommands.ShowMap))
                {
                    var split = input.Split(' ');

                    if (split.Count() == 1)
                    {
                        world.Tell(new GetMapMessage("world"), console);
                    }
                    else
                    {
                        world.Tell(new GetMapMessage(split[1]), console);
                    }
                }
                else if (input == UserCommands.Help)
                {
                    Console.WriteLine(GetHelp());
                }
                else if (input.StartsWith(UserCommands.StartDrone))
                {
                    world.Tell(new StartMovingMessage(input.Split(' ')[1]), console);
                }
                else if (input.StartsWith(UserCommands.StopDrone))
                {
                    world.Tell(new StopMovingMessage(input.Split(' ')[1]), console);
                }
                else if (input.StartsWith(UserCommands.AddDrone))
                {
                    var split = input.Split(' ');
                    var droneName = split[1];
                    var dronePositionX = int.Parse(split[2]);
                    var dronePositionY = int.Parse(split[3]);
                    var droneTurnLikeliness = float.Parse(split[4].Replace('.', ','));


                    var config = new DiscoveryDroneConfig(
                        droneName,
                        dronePositionX,
                        dronePositionY,
                        droneTurnLikeliness,
                        2,
                        1);

                    var message = new AddDiscoveryDroneMessage(config);

                    world.Tell(message,console);
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(GetHelp());
            }

            string GetHelp()
            {
                return "Ussage:" +
                    $"{Environment.NewLine}help - show this help message" +
                    $"{Environment.NewLine}{UserCommands.ShowMap} <whose> - show <whose> map" +
                    $"{Environment.NewLine}{UserCommands.AddDrone} <name> <int positionX> <int positionY> <float turnTikeliness>" +
                    $"{Environment.NewLine}{UserCommands.StartDrone} <name> - call next step" +
                    $"{Environment.NewLine}{UserCommands.StopDrone} <name> - call next step" +
                    $"{Environment.NewLine}{UserCommands.Exit} - exit the application";
            }


        }
    }
}

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

            using (var system = ActorSystem.Create("World"))
            {
                console = system.ActorOf(Props.Create<ConsoleActor>());

                world = system.ActorOf(Props.Create<World>());

                world.Tell(new InitWorldMessage(console, 70, 20, tileProbabilities));


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
            if (input == UserCommands.ShowMap)
                world.Tell(new GetMapMessage(console));
            else if(input == UserCommands.Help)
            {
                Console.WriteLine(GetHelp());
            }
            else if(input.StartsWith(UserCommands.AddAgent))
            {
                var split = input.Split(' ');
                var droneName = split[1];
                var dronePositionX = int.Parse(split[2]);
                var dronePositionY = int.Parse(split[3]);
                var droneTurnLikeliness = float.Parse(split[4]);
                var droneVision = int.Parse(split[5]);
                var droneSpeed = int.Parse(split[6]);

                var config = new DiscoveryDroneConfig(
                    droneName,
                    dronePositionX,
                    dronePositionY,
                    droneTurnLikeliness,
                    droneVision,
                    droneSpeed);

                var message = new AddDiscoveryDroneMessage(console, config);

                world.Tell(message);
            }
            string GetHelp()
            {
                return "Ussage:" +
                    $"{Environment.NewLine}help - show this help message" +
                    $"{Environment.NewLine}{UserCommands.ShowMap} - show world map" +
                    $"{Environment.NewLine}{UserCommands.AddAgent} <name> <int positionX> <int positionY> <float turnTikeliness>" +
                    $"{Environment.NewLine}{UserCommands.Tick} - call next step" +
                    $"{Environment.NewLine}{UserCommands.Exit} - exit the application";
            }


        }
    }
}

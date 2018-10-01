using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thread = System.Threading.Thread;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
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
        const int MONITOR_TICK_MS = 1000;
        static Timer monitorTimer = new Timer(MONITOR_TICK_MS);
        static string monitoredEntity = "world";
        static void Main(string[] args)
        {
            monitorTimer.AutoReset = true;
            monitorTimer.Elapsed += (sender, eventArgs) => Monitor();

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

        static void Monitor()
        {
            world.Tell(new GetMapMessage(monitoredEntity), console);
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
                else if (input == UserCommands.Demo)
                {
                    Demo();
                }
                else if (input.StartsWith(UserCommands.MonitorStart))
                {
                    var split = input.Split(' ');
                    if (split.Count() == 3)
                    {
                        monitoredEntity = split[2];
                    }
                    monitorTimer.Start();
                }
                else if (input.StartsWith(UserCommands.MonitorStop))
                {
                    monitorTimer.Stop();
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
                    var moveInterval = int.Parse(split[5]);



                    var config = new DiscoveryDroneConfig(
                        droneName,
                        dronePositionX,
                        dronePositionY,
                        droneTurnLikeliness,
                        2,
                        moveInterval);

                    var message = new AddDiscoveryDroneMessage(config);

                    world.Tell(message, console);
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
                    $"{Environment.NewLine}{UserCommands.Help} - show this help message" +
                    $"{Environment.NewLine}{UserCommands.Demo} - about 60 seconds demo of the system with commentary" +
                    $"{Environment.NewLine}{UserCommands.ShowMap} <whose> - show <whose> map" +
                    $"{Environment.NewLine}{UserCommands.AddDrone} <name> <int positionX> <int positionY> <float turnTikeliness> <moveInterval>" +
                    $"{Environment.NewLine}{UserCommands.StartDrone} <name> - call next step" +
                    $"{Environment.NewLine}{UserCommands.StopDrone} <name> - call next step" +
                    $"{Environment.NewLine}{UserCommands.MonitorStart} <name> - start monitoring <name> map" +
                    $"{Environment.NewLine}{UserCommands.MonitorStop} - call next step" +
                    $"{Environment.NewLine}{UserCommands.Exit} - exit the application";
            }
        }

        static void Demo()
        {
            void Wait() => Thread.Sleep(3000);

            Console.WriteLine("Let us conquer Mars, or room, or sandbox or some other space");
            Wait();

            Console.WriteLine("That's how the world looks like.");
            Console.WriteLine(">map");
            Wait();
            Wait();

            world.Tell(new GetMapMessage(), console);
            Wait();

            Console.WriteLine("Adding drone");
            Wait();
            Console.WriteLine(">add-drone Dave 23 12 0.2 2");
            Wait();
            Console.WriteLine("It means add drone named Dave on x=23 y=12 that will turn in 20% of moves and move each 2 seconds");
            Wait();
            Wait();
            Wait();
            Wait();

            world.Tell(new AddDiscoveryDroneMessage(new DiscoveryDroneConfig("Dave", 23, 12, 0.2f, 2, 2)));
            Wait();

            Console.WriteLine("Now look again at the map, the drone is visible as an 'A'");
            Wait();
            Wait();
            Wait();

            world.Tell(new GetMapMessage(), console);
            Wait();
            Wait();

            Console.WriteLine("Now we just add another drone");
            Wait();

            Console.WriteLine(">add-drone Hal 65 37 0.05 5");
            Wait();
            world.Tell(new AddDiscoveryDroneMessage(new DiscoveryDroneConfig("Hal", 65, 17, 0.05f, 2, 5)));

            world.Tell(new GetMapMessage(), console);

            Console.WriteLine("It's on the lower right of the map");
            Wait();
            Wait();

            Console.WriteLine("Now let's look what can they see, Dave...");
            Console.WriteLine(">map Dave");
            Wait();
            Wait();

            world.Tell(new GetMapMessage("Dave"), console);
            Wait();
            Wait();
            Wait();

            Console.WriteLine("and Hal...");
            Console.WriteLine(">map Hal");
            Wait();

            world.Tell(new GetMapMessage("Hal"), console);

            Wait();
            Wait();
            Wait();

            Console.WriteLine("For now they just stood in place and observed their calm surrounding");
            Wait();
            Wait();
            Wait();

            Console.WriteLine("Time to spin them for a run");
            Wait();

            Console.WriteLine(">start-drone Dave");
            Console.WriteLine(">start-drone Hal");
            Wait();
            world.Tell(new StartMovingMessage("Dave"));
            world.Tell(new StartMovingMessage("Hal"));

            Console.WriteLine("They should be moving right now, lets see how are they doing");
            Wait();
            Wait();
            Wait();

            Console.WriteLine(">monitor start world");
            monitorTimer.Start();

            Thread.Sleep(15000);

            Console.WriteLine("That's enough of the world. Now back to Dave");
            Console.WriteLine(">monitor stop");
            monitorTimer.Stop();
            Wait();

            Console.WriteLine(">monitor start Dave");
            monitoredEntity = "Dave";
            Wait();

            monitorTimer.Start();

            Thread.Sleep(15000);

            monitorTimer.Stop();

            Console.WriteLine("Now we stop spying Dave to see the world through the Hal's eyes... or visors... or whatever he uses to percieve the world around him");
            Console.WriteLine(">monitor stop");
            Wait();
            Wait();
            Wait();
            Wait();

            Console.WriteLine(">map Hal");
            Wait();

            world.Tell(new GetMapMessage("Hal"), console);
            Wait();
            Wait();

            Console.WriteLine("And just to show you, how the the map gets uncovered");
            Wait();

            Console.WriteLine(">add-drone Monolith 35 13 0.05 2");
            Console.WriteLine(">start-drone Monolith");
            Console.WriteLine(">monitor start Monolith");
            Wait();
            Wait();

            world.Tell(new AddDiscoveryDroneMessage(new DiscoveryDroneConfig("Monolith", 35, 12, 0.05f, 2, 1)));
            Wait();
            world.Tell(new StartMovingMessage("Monolith"));

            monitoredEntity = "Monolith";
            monitorTimer.Start();

            Thread.Sleep(25000);

            monitorTimer.Stop();

            Console.WriteLine("Ok, so now we will stop the monolith, and look for a short while on the whole world again.");
            Wait();
            Wait();
            Wait();
            Console.WriteLine(">stop-drone Monolith");
            Wait();

            world.Tell(new StopMovingMessage("Monolith"));

            Console.WriteLine(">monitor start world");

            monitoredEntity = "world";
            monitorTimer.Start();

            Thread.Sleep(20000);

            monitorTimer.Stop();

            Console.WriteLine("That's all folks!");

            Console.WriteLine();

            Console.WriteLine("press enter to exit");
            Console.ReadLine();

            Environment.Exit(0);
        }
    }
}

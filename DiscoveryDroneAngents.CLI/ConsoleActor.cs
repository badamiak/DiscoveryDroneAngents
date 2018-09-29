using Akka.Actor;
using DiscoveryDroneAgents.API.Messages.Responses;
using DiscoveryDroneAgents.API.Model;
using System;
using System.Linq;

namespace DiscoveryDroneAngents.CLI
{
    class ConsoleActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is GetMapResponseMessage)
            {
                var parsed = (message as GetMapResponseMessage);

                Console.WriteLine(MapHelper.GetMapRepresentation("World map", parsed.Map, parsed.SizeX, parsed.SizeY));
            }
        }
    }
}

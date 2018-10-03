using Akka.Actor;
using DiscoveryDroneAgents.API.Messages.Responses;
using DiscoveryDroneAngents.API;
using System;
using System.Diagnostics;
using System.IO;

namespace DiscoveryDroneAngents.CLI
{
    public class ConsoleActor : UntypedActor
    {
        //private Process consoleProcess;
        //private StreamWriter console => this.consoleProcess.StandardInput;


        protected override void PreStart()
        {
            base.PreStart();
            //this.consoleProcess = OpenNewConsole();
        }

        protected override void OnReceive(object message)
        {
            if (message is GetMapResponseMessage)
            {
                var parsed = (message as GetMapResponseMessage);

                Console.Clear();
                Console.WriteLine(MapHelper.GetMapRepresentation("World map", parsed.Map, parsed.SizeX, parsed.SizeY, parsed.DronesPositions));
            }
        }

        protected override void PostStop()
        {
            base.PostStop();
            //consoleProcess.Close();
        }

        private Process OpenNewConsole()
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd")
            {
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            return Process.Start(psi);
        }
    }
}

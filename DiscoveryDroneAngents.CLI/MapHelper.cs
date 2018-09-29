using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAngents.CLI
{
    class MapHelper
    {
        public static string GetMapRepresentation(string mapName, TileType[,] mapMatrix, int sizeX, int sizeY)
        {
            var mapArray = mapMatrix.Cast<TileType[]>();

            string map = $"{mapName}{Environment.NewLine}";

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    map += (char)mapMatrix[x, y];
                }
                map += Environment.NewLine;
            }

            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                map += $"{(char)type} - {type}{Environment.NewLine}";
            }

            return map;
        }
    }
}

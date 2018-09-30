using DiscoveryDroneAgents.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAngents.API
{
    public class MapHelper
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

        public static TileType[,] GetUnchartedMap(int sizeX, int sizeY)
        {
            var map = new TileType[sizeX,sizeY];
            for(int x = 0; x < sizeX; x++ )
            {
                for(int y = 0; y< sizeY; y++)
                {
                    if (x==0 | y==0 | x == sizeX-1| y == sizeY-1)
                    {
                        map[x, y] = TileType.HighObstacle;
                    }
                    else
                    {
                        map[x, y] = TileType.Uncharted;
                    }
                }
            }
            return map;
        }

        public static TileType[,] GetUpdatedMap(TileType[,] knownMap, TileType[,] mapUdate, int updatePositionX, int updatePositionY, int updateSizeX, int updateSizeY)
        {
            for(int x = 0; x < updateSizeX; x++)
            {
                for (int y = 0; y < updateSizeX; y++)
                    knownMap[x + updatePositionX, y + updatePositionY] = mapUdate[x, y];
            }
            return knownMap;
        }
    }
}

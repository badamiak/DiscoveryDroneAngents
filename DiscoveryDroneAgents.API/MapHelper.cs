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
        public static string GetMapRepresentation(string mapName, TileType[,] mapMatrix, int sizeX, int sizeY, List<DiscoveryDroneStatus> drones )
        {
            string header = $"{mapName}{Environment.NewLine}";
            string map = string.Empty;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if(drones.Any(drone=> drone.PositionX == x && drone.PositionY == y))
                    {
                        map += (char)TileType.Drone;
                    }
                    else
                    {
                        map += (char)mapMatrix[x, y];
                    }
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
                    if (x == 0
                        || x == 1
                        || x == sizeX - 1
                        || x == sizeX - 2
                        || y == 0
                        || y == 1
                        || y == sizeY - 1
                        || y == sizeY - 2)
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

        public static TileType[,] CorelateMaps(TileType[,] map1, TileType[,] map2, int sizeX, int sizeY)
        {
            var resultingMap = new TileType[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(map1[x,y] == TileType.Uncharted && map2[x,y] == TileType.Uncharted)
                    {
                        resultingMap[x,y] = map1[x, y];
                    }
                    else if (map1[x,y] == TileType.Uncharted)
                    {
                        resultingMap[x, y] = map2[x, y];
                    }
                    else
                    {
                        resultingMap[x, y] = map1[x, y];
                    }
                }
            }

            return resultingMap;
        }

        public static TileType[,] GetUpdatedMap(TileType[,] knownMap, MapUpdate patch)
        {
            for(int x = 0; x < patch.SizeX; x++)
            {
                for (int y = 0; y < patch.SizeY; y++)
                    knownMap[x + patch.PositionX, y + patch.PositionY] = patch.mapFragment[x, y];
            }
            return knownMap;
        }
    }
}

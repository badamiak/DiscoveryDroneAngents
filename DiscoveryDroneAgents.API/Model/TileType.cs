using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryDroneAgents.API.Model
{
    public enum TileType
    {
        /// <summary>
        /// Unknown ground, not yet mapped
        /// </summary>
        Uncharted = '░',
        /// <summary>
        /// Mapped and passabe, drone may move through that tile
        /// </summary>
        Passable = ' ',
        /// <summary>
        /// Low obstacle means an unpassable terraing but low enought to scan throug it
        /// </summary>
        LowObstacle = '▒',
        /// <summary>
        /// High Obstacle can't be passed and blocks scanning in that direction
        /// </summary>
        HighObstacle = '▓',
    }
}

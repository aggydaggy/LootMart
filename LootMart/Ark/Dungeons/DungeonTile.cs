using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Ark.Dungeons
{
    public enum TileType
    {
        NONE,
        TILE_FLOOR,
        TILE_WALL,
        TILE_ROCK
    }

    public struct DungeonTile
    {
        public TileType Type;
        public bool Reached;
    }
}

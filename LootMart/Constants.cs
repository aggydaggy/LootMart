using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart
{
    public static class Constants
    {
        public static readonly Random Random = new Random();

        public static class Fonts
        {
            public static readonly string FontFolder = @"Fonts/";
            public static readonly string TelegramaSmall = FontFolder + "TelegramaSmall";
        }

        public static class Sprites
        {
            public static readonly string SpriteFolder = @"Sprites/";
            public static readonly string Placeholder = SpriteFolder + "placeholder";
            public static readonly string PlaceHolderKey = "placeholder";
            public static readonly string Shieldbun = SpriteFolder + "shieldbunsprite";
            public static readonly string ItemSheet = SpriteFolder + "items";
            public static readonly string ItemSheetKey = "item";
            public const int ItemGrid = 16;
            public static readonly string MonsterSheet = SpriteFolder + "monsters";
            public static readonly string MonsterSheetKey = "monsters";
            public const int MonsterGrid = 24;
            public static readonly string TileSheet = SpriteFolder + "tiles";
            public static readonly string TileSheetKey = "tiles";
            public const int TileGrid = 24;
        }

        public static class IO
        {
            public static class GameSettings
            {
                public const string SettingsDirectory = @"Settings/";
                public const string CurrentSettings = SettingsDirectory + "Config.xml";
                public const string DefaultGameSettings = SettingsDirectory + "DefaultConfig.xml";
            }
        }

        public static class Tiles
        {
            public const int FloorCol = 5;
            public const int FloorRow = 12;
            public const int WallCol = 1;
            public const int WallRow = 5;
            public const int TileScale = 2;
        }


        public static class Ark
        {
            public const string ArkFolder = @"Ark/";
            public const string MonstersFolder = ArkFolder + @"Monsters/";
            public const string ItemsFolder = ArkFolder + @"Items/";

            public static class Monsters
            {
                public static readonly string Player = Ark.MonstersFolder + "Player.xml";
                public static readonly string TestNpc = Ark.MonstersFolder + "TestNpc.xml";
            }
        }

        public static class Loot
        {
            public const int MinimumDropFling = 50;
            public const int MaximumDropFling = 250;
        }

        public static class Systems
        {
            public static class Collision
            {
                public const int MapCellSize = 200;
            }
        }
    }
}

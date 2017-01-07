using LootMart.Ark.Dungeons;
using LootMart.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Systems
{
    public static class DisplaySystem
    {
        public static void DisplayEntity(SpriteBatch spriteBatch, Camera camera, Display displayInfo, Position positionInfo, Texture2D spriteSheet)
        {
            if (positionInfo != null && displayInfo != null && camera.IsInView(GetCameraBounds(displayInfo, positionInfo, camera)))
            {
                Rectangle spriteSource = new Rectangle(displayInfo.SpriteSheetSize * displayInfo.SpriteSheetCol, displayInfo.SpriteSheetSize * displayInfo.SpriteSheetRow, displayInfo.SpriteSheetSize, displayInfo.SpriteSheetSize);
                spriteBatch.Draw(spriteSheet, positionInfo.OriginPosition, spriteSource, displayInfo.Color * displayInfo.Opacity, displayInfo.Rotation, displayInfo.Origin, displayInfo.Scale, displayInfo.SpriteEffect, ((float)displayInfo.Layer) / 100);
            }
        }

        public static void DisplayLabel(SpriteBatch spriteBatch, Camera camera, Display displayInfo, Label labelInfo, Position positionInfo, SpriteFont font, Position playerPosition, Display playerDisplay)
        {
            if (positionInfo != null && displayInfo != null && labelInfo != null && camera.IsInView(GetCameraBounds(displayInfo, positionInfo, camera)))
            {
                int distance = Math.Abs((int)Vector2.Distance(playerPosition.OriginPosition + playerDisplay.Origin, positionInfo.OriginPosition + displayInfo.Origin));
                bool show = false;
                switch (labelInfo.WhenToShow)
                {
                    case WhenToShowLabel.ALWAYS:
                        show = true;
                        break;
                    case WhenToShowLabel.PLAYER_CLOSE:
                        show = distance <= labelInfo.DistanceRenderBuffer;
                        break;
                    case WhenToShowLabel.PLAYER_FAR:
                        show = distance >= labelInfo.DistanceRenderBuffer;
                        break;
                }

                if (show)
                {
                    Vector2 fontSize = font.MeasureString(labelInfo.Text);
                    spriteBatch.DrawString(font, labelInfo.Text, positionInfo.OriginPosition + labelInfo.Displacement - new Vector2(0, fontSize.Y), labelInfo.Color, labelInfo.Rotation, new Vector2(fontSize.X / 2, fontSize.Y / 2), labelInfo.Scale, labelInfo.SpriteEffect, 0f);
                }
            }
        }

        public static void DrawDungeon(Camera camera, SpriteBatch spriteBatch, DungeonTile[,] grid, Texture2D spriteSheet, int cols, int rows)
        {
            Rectangle gridTranslation = camera.TranslateToGrid(rows, cols, Constants.Sprites.TileGrid * Constants.Tiles.TileScale, 1);

            for (int i = gridTranslation.X; i < gridTranslation.Width; i++)
            {
                for (int j = gridTranslation.Y; j < gridTranslation.Height; j++)
                {
                    Rectangle sourceRect = new Rectangle();
                    switch (grid[i, j].Type)
                    {
                        case TileType.TILE_FLOOR:
                            sourceRect = new Rectangle(Constants.Tiles.FloorCol * Constants.Sprites.TileGrid, Constants.Tiles.FloorRow * Constants.Sprites.TileGrid, Constants.Sprites.TileGrid, Constants.Sprites.TileGrid);
                            break;
                        case TileType.TILE_WALL:
                            sourceRect = new Rectangle(Constants.Tiles.WallCol * Constants.Sprites.TileGrid, Constants.Tiles.WallCol * Constants.Sprites.TileGrid, Constants.Sprites.TileGrid, Constants.Sprites.TileGrid);
                            break;
                    }

                    spriteBatch.Draw(spriteSheet,
                        new Rectangle(i * Constants.Tiles.TileScale * Constants.Sprites.TileGrid, j * Constants.Tiles.TileScale * Constants.Sprites.TileGrid, Constants.Tiles.TileScale * Constants.Sprites.TileGrid, Constants.Tiles.TileScale * Constants.Sprites.TileGrid),
                        sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }
        }

        private static Rectangle GetCameraBounds(Display displayInfo, Position positionInfo, Camera camera)
        {
            Vector2 bottomRight = Vector2.Transform(new Vector2((positionInfo.OriginPosition.X) + (displayInfo.SpriteSheetSize * displayInfo.Scale), (positionInfo.OriginPosition.Y) + (displayInfo.SpriteSheetSize * displayInfo.Scale)), camera.CurrentMatrix);
            Vector2 topLeft = Vector2.Transform(new Vector2(positionInfo.OriginPosition.X, positionInfo.OriginPosition.Y), camera.CurrentMatrix);
            Rectangle cameraBounds = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)bottomRight.X - (int)topLeft.X, (int)bottomRight.Y - (int)topLeft.Y);
            return cameraBounds;
        }

        private static Rectangle GetCameraBounds(int posX, int posY, int size, int scale, Camera camera)
        {
            Vector2 bottomRight = Vector2.Transform(new Vector2((posX) + (size * scale), (posY) + (size * scale)), camera.CurrentMatrix);
            Vector2 topLeft = Vector2.Transform(new Vector2(posX, posY), camera.CurrentMatrix);
            Rectangle cameraBounds = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)bottomRight.X - (int)topLeft.X, (int)bottomRight.Y - (int)topLeft.Y);
            return cameraBounds;
        }
    }
}

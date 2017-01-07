using LootMart.Ark;
using LootMart.Ark.Dungeons;
using LootMart.Components;
using LootMart.IO.Settings;
using LootMart.States.Menus;
using LootMart.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.States.Levels
{
    public class TestLevel : IState
    {
        private Dictionary<string, Texture2D> _spriteSheets = new Dictionary<string, Texture2D>();
        private SpriteFont _labelFont;
        private ECSContainer _components;
        private ContentManager _content;
        private DungeonTile[,] _dungeonGrid;
        private int _gridCols;
        private int _gridRows;
        private List<Entity>[,] _collisionPartition;
        private Dictionary<DisplayLayer, List<Action>> drawSequence = new Dictionary<DisplayLayer, List<Action>>();

        public TestLevel(ContentManager content, Camera camera)
        {
            _content = new ContentManager(content.ServiceProvider, content.RootDirectory);
            _labelFont = _content.Load<SpriteFont>(Constants.Fonts.TelegramaSmall);
            _spriteSheets.Add(Constants.Sprites.MonsterSheetKey, _content.Load<Texture2D>(Constants.Sprites.MonsterSheet));
            _spriteSheets.Add(Constants.Sprites.ItemSheetKey, _content.Load<Texture2D>(Constants.Sprites.ItemSheet));
            _spriteSheets.Add(Constants.Sprites.TileSheetKey, _content.Load<Texture2D>(Constants.Sprites.TileSheet));
            _spriteSheets.Add(Constants.Sprites.PlaceHolderKey, _content.Load<Texture2D>(Constants.Sprites.Placeholder));
            this._components = new ECSContainer();
            this._dungeonGrid = DungeonGenerationSystem.GenerateDungeon(50, 100);
            this._gridCols = this._dungeonGrid.GetLength(0);
            this._gridRows = this._dungeonGrid.GetLength(1);
            this._collisionPartition = CollisionSystem.CreatePartitionGrid(this._gridCols, this._gridRows);
            drawSequence[DisplayLayer.BACKGROUND] = new List<Action>();
            drawSequence[DisplayLayer.FLOOR] = new List<Action>();
            drawSequence[DisplayLayer.FOREGROUND] = new List<Action>();
            drawSequence[DisplayLayer.NORMAL] = new List<Action>();
            drawSequence[DisplayLayer.SUPER] = new List<Action>();
            drawSequence[DisplayLayer.TOP] = new List<Action>();

            #region Debug Creation
            Guid playerId = ArkCreation.SpawnEntityWithOverrides(Constants.Ark.Monsters.Player, ref this._components, new BaseEntity(ComponentFlags.POSITION) { Position = new Position() { OriginPosition = new Vector2(20, 20) } });
            Guid testId = ArkCreation.SpawnEntityWithOverrides(Constants.Ark.Monsters.TestNpc, ref this._components, new BaseEntity(ComponentFlags.POSITION) { Position = new Position() { OriginPosition = new Vector2(20, 20) } });
            InventorySystem.GenerateRandomInventoryItemsForEntity(this._components, testId);
            camera.TargetEntity = playerId;
            #endregion
        }

        public void DrawContent(SpriteBatch spriteBatch, Camera camera)
        {
            Guid playerId = this._components.Entities.Where(c => c.HasComponents(ComponentFlags.IS_PLAYER)).FirstOrDefault().Id;
            // Draw Dungeon
            DisplaySystem.DrawDungeon(camera, spriteBatch, this._dungeonGrid, this._spriteSheets[Constants.Sprites.TileSheetKey], this._gridCols, this._gridRows);

            // Draw Sprites
            this._components.Entities.ForEach((c) =>
            {
                if (c.HasDrawableSprite())
                {
                    drawSequence[this._components.Displays[c.Id].Layer].Add(new Action(() =>
                    {
                        DisplaySystem.DisplayEntity(spriteBatch, camera, this._components.Displays[c.Id], this._components.Positions[c.Id], this._spriteSheets[this._components.Displays[c.Id].SpriteSheetKey]);
                    }));
                }
                if (c.HasDrawableLabel())
                {
                    DisplaySystem.DisplayLabel(spriteBatch, camera, this._components.Displays[c.Id], this._components.Labels[c.Id], this._components.Positions[c.Id], _labelFont, this._components.Positions[playerId], this._components.Displays[playerId]);
                }
            });
            foreach (DisplayLayer key in drawSequence.Keys)
            {
                drawSequence[key].ForEach(x => x.Invoke());
                drawSequence[key].Clear();
            }
        }

        public IState UpdateState(ref GameSettings gameSettings, GameTime gameTime, Camera camera, KeyboardState currentKey, KeyboardState prevKey, MouseState currentMouse, MouseState prevMouse)
        {
            Guid playerId = this._components.Entities.Where(c => c.HasComponents(ComponentFlags.IS_PLAYER)).FirstOrDefault().Id;
            // Level input
            if (currentKey.IsKeyDown(Keys.Escape) && prevKey.IsKeyUp(Keys.Escape))
            {
                return new PauseState(this._content, this);
            }
            if (currentKey.IsKeyDown(Keys.Q) && prevKey.IsKeyUp(Keys.Q))
            {
                return new TestLevel(this._content, camera);
            }

            if (currentKey.IsKeyDown(Keys.F) && prevKey.IsKeyUp(Keys.F))
            {
                this._components.DelayedActions.Add(new Action(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Guid testId = ArkCreation.SpawnEntityWithOverrides(Constants.Ark.Monsters.TestNpc, ref this._components, new BaseEntity(ComponentFlags.POSITION) { Position = new Position() { OriginPosition = new Vector2(Constants.Random.Next(20, this._gridCols * 48), Constants.Random.Next(0, this._gridRows * 48)) } });
                        InventorySystem.GenerateRandomInventoryItemsForEntity(this._components, testId);
                    }
                }));
            }
            if (currentKey.IsKeyDown(Keys.E) && prevKey.IsKeyUp(Keys.E))
            {
                this._components.DelayedActions.Add(new Action(() =>
                {
                    Guid testId = ArkCreation.SpawnEntityWithOverrides(Constants.Ark.Monsters.TestNpc, ref this._components, new BaseEntity(ComponentFlags.POSITION) { Position = new Position() { OriginPosition = this._components.Positions[playerId].OriginPosition } });
                    InventorySystem.GenerateRandomInventoryItemsForEntity(this._components, testId);
                }));
            }
            if (currentKey.IsKeyDown(Keys.R) && prevKey.IsKeyUp(Keys.R))
            {
                this._components.DelayedActions.Add(new Action(() =>
                {
                    Guid id = this._components.Entities.Where(x => x.HasDrawableSprite() && !x.HasComponents(ComponentFlags.IS_PLAYER) && x.HasComponents(ComponentFlags.INVENTORY)).First().Id;
                    InventorySystem.DropEntityInventory(this._components, id);
                    this._components.DestroyEntity(id);
                }));
            }

            // Camera Updates
            CameraSystem.ControlCamera(currentKey, prevKey, camera, gameTime);
            CameraSystem.PanCamera(camera, gameTime);

            // Entity Movement Updates
            this._components.Entities.ForEach(c =>
            {
                if (c.IsMovable())
                {
                    switch (this._components.Movements[c.Id].MovementType)
                    {
                        case MovementType.AI:
                            //AI Movement System Call
                            break;
                        case MovementType.INPUT:
                            MovementSystem.InputMovement(currentKey, prevKey, gameTime, this._components.Positions[c.Id], this._components.Movements[c.Id]);
                            break;
                        case MovementType.DIRECTED:
                            MovementSystem.UpdateMovingEntities(this._components.Movements[c.Id], this._components.Positions[c.Id], gameTime, this._components, c);
                            break;
                    }
                }
            });

            // Entity Information Updates
            // Collision
            CollisionSystem.CheckForCollisions(this._components, this._collisionPartition);
            CollisionSystem.HandleCollisions(this._components);

            // Set up for next frame
            CameraSystem.UpdateCameraTarget(this._components, camera);
            CollisionSystem.ResetCollisions(ref this._components);
            this._components.InvokeDelayedActions();

            return this;
        }

        public void DrawUI(SpriteBatch spriteBatch, Camera camera)
        {
            //Nothing here yet.
        }
    }
}

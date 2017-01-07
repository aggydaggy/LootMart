using LootMart.IO.Settings;
using LootMart.States;
using LootMart.States.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LootMart
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class LootMartGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState _prevKey;
        private MouseState _prevMouse;
        private Camera _camera;
        private IState _currentState;
        private GameSettings _gameSettings;
        private SpriteFont _debugText;

        public LootMartGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this._gameSettings = SettingsIO.LoadGameSettings();
            this._spriteBatch = new SpriteBatch(GraphicsDevice);
            this._debugText = Content.Load<SpriteFont>(Constants.Fonts.TelegramaSmall);

            this.IsMouseVisible = false;
            this.Window.IsBorderless = _gameSettings.Borderless;
            this.Window.AllowUserResizing = false;
            this._currentState = new TitleState(Content);
            this._graphics.PreferredBackBufferWidth = (int)_gameSettings.Resolution.X;
            this._graphics.PreferredBackBufferHeight = (int)_gameSettings.Resolution.Y;
            this.IsFixedTimeStep = this._gameSettings.Vsync;
            this._graphics.SynchronizeWithVerticalRetrace = this._gameSettings.Vsync;
            this._graphics.ApplyChanges();
            this._camera = new Camera(GraphicsDevice.Viewport, GraphicsDevice.Viewport.Bounds.Center.ToVector2(), 0f, 1f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                this._camera.CurrentMatrix = this._camera.GetMatrix();
                this._camera.CurrentInverseMatrix = this._camera.GetInverseMatrix();
                KeyboardState currentKey = Keyboard.GetState();
                MouseState currentMouse = Mouse.GetState();
                this._currentState = this._currentState.UpdateState(ref _gameSettings, gameTime, this._camera, currentKey, this._prevKey, currentMouse, this._prevMouse);
                this._prevKey = currentKey;
                this._prevMouse = currentMouse;

                if (this._currentState == null)
                {
                    this.Exit();
                }

                if (this._gameSettings.HasChanges)
                {
                    ResetGameSettings();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw Entities
            this._spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: this._camera.CurrentMatrix);
            this._currentState.DrawContent(this._spriteBatch, this._camera);
            this._spriteBatch.End();

            // Draw UI
            this._spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            this._currentState.DrawUI(this._spriteBatch, this._camera);
            this._spriteBatch.End();

            // Draw Debug
            this._spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            this._spriteBatch.DrawString(this._debugText, "FPS: " + Math.Round((1 / (decimal)gameTime.ElapsedGameTime.TotalSeconds), 2).ToString(), new Vector2(25, 25), Color.White);
            this._spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ResetGameSettings()
        {
            this.Window.IsBorderless = this._gameSettings.Borderless;
            this._graphics.PreferredBackBufferWidth = (int)this._gameSettings.Resolution.X;
            this._graphics.PreferredBackBufferHeight = (int)this._gameSettings.Resolution.Y;
            this._graphics.SynchronizeWithVerticalRetrace = this._gameSettings.Vsync;
            this.IsFixedTimeStep = this._gameSettings.Vsync;
            this._graphics.ApplyChanges();
            this._gameSettings.HasChanges = false;
            this.Window.ClientBounds.Offset(new Point((int)this._graphics.GraphicsDevice.DisplayMode.Width / 2 - (int)this._gameSettings.Resolution.X / 2, (int)this._graphics.GraphicsDevice.DisplayMode.Height / 2 - (int)this._gameSettings.Resolution.Y / 2));
            this._camera.FullViewport = GraphicsDevice.Viewport;
            this._camera.Bounds = GraphicsDevice.Viewport.Bounds;
        }
    }
}

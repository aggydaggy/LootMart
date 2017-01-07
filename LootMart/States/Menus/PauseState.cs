using LootMart.IO.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.States.Menus
{
    public class PauseState : IState
    {
        private enum Options
        {
            UNPAUSE = 0,
            GAME_SETTINGS = 1,
            SAVE = 2,
            SAVE_AND_QUIT = 3
        }

        private class OptionItem
        {
            public string Text { get; set; }
            public bool Enabled { get; set; }
        }

        private IState _previousState;
        private ContentManager _content;
        private SpriteFont _pauseText;
        private OptionItem[] _optionItems;
        private int _selectedOption;
        private Texture2D _screenDarken;

        public PauseState(ContentManager content, IState prevState)
        {
            this._content = new ContentManager(content.ServiceProvider, content.RootDirectory);
            this._previousState = prevState;
            this._pauseText = _content.Load<SpriteFont>(Constants.Fonts.TelegramaSmall);
            this._screenDarken = _content.Load<Texture2D>(Constants.Sprites.Placeholder);
            _selectedOption = 0;
            _optionItems = new OptionItem[Enum.GetNames(typeof(Options)).Length];
            _optionItems[(int)Options.UNPAUSE] = new OptionItem() { Enabled = true, Text = "Unpause" };
            _optionItems[(int)Options.GAME_SETTINGS] = new OptionItem() { Enabled = true, Text = "Game Settings" };
            _optionItems[(int)Options.SAVE] = new OptionItem() { Enabled = false, Text = "Save" };
            _optionItems[(int)Options.SAVE_AND_QUIT] = new OptionItem() { Enabled = false, Text = "Save & Quit" };
        }

        public void DrawContent(SpriteBatch spriteBatch, Camera camera)
        {
            if (_previousState != null)
            {
                _previousState.DrawContent(spriteBatch, camera);
            }
        }

        public void DrawUI(SpriteBatch spriteBatch, Camera camera)
        {
            if (_previousState != null)
            {
                _previousState.DrawUI(spriteBatch, camera);
                spriteBatch.Draw(_screenDarken, destinationRectangle: camera.Bounds, color: Color.Black * .75f);
            }

            string message = "[PAUSE]";
            Vector2 size = this._pauseText.MeasureString(message);
            spriteBatch.DrawString(_pauseText, message, new Vector2(20, 150), Color.MonoGameOrange, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            int optionNum = 0;

            foreach (OptionItem option in _optionItems)
            {
                message = (_selectedOption == optionNum ? ">" : string.Empty) + option.Text;
                size = _pauseText.MeasureString(message);
                Color optionColor = (_selectedOption == optionNum) ? Color.GhostWhite : Color.MonoGameOrange;
                spriteBatch.DrawString(_pauseText, message, new Vector2(20, 250 + (optionNum++ * size.Y)), option.Enabled ? optionColor : Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public IState UpdateState(ref GameSettings gameSettings, GameTime gameTime, Camera camera, KeyboardState currentKey, KeyboardState prevKey, MouseState currentMouse, MouseState prevMouse)
        {

            if (currentKey.IsKeyDown(Keys.Up) && prevKey.IsKeyUp(Keys.Up))
            {
                _selectedOption -= 1;
                this.HandleOptionChange(-1);
            }

            if (currentKey.IsKeyDown(Keys.Down) && prevKey.IsKeyUp(Keys.Down))
            {
                _selectedOption += 1;
                this.HandleOptionChange(1);
            }

            if (currentKey.IsKeyDown(Keys.Enter) && prevKey.IsKeyUp(Keys.Enter))
            {
                switch (_selectedOption)
                {
                    case (int)Options.UNPAUSE:
                        return this._previousState;
                    case (int)Options.GAME_SETTINGS:
                        return new SettingsState(ref gameSettings, this._content, this);
                    case (int)Options.SAVE:
                        break;
                    case (int)Options.SAVE_AND_QUIT:
                        return new TitleState(_content);
                }
            }

            if (currentKey.IsKeyDown(Keys.Escape) && prevKey.IsKeyUp(Keys.Escape))
            {
                return this._previousState;
            }

            return this;
        }

        private void HandleOptionChange(int changeDirection)
        {
            if (_selectedOption < 0)
            {
                _selectedOption = Enum.GetNames(typeof(Options)).Length - 1;
            }

            if (_selectedOption > Enum.GetNames(typeof(Options)).Length - 1)
            {
                _selectedOption = 0;
            }
            while (!_optionItems[_selectedOption].Enabled)
            {
                _selectedOption += changeDirection;

                if (_selectedOption < 0)
                {
                    _selectedOption = Enum.GetNames(typeof(Options)).Length - 1;
                }

                if (_selectedOption > Enum.GetNames(typeof(Options)).Length - 1)
                {
                    _selectedOption = 0;
                }
            }
        }
    }
}

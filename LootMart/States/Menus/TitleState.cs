using LootMart.States.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LootMart.IO.Settings;

namespace LootMart.States.Menus
{
    public class TitleState : IState
    {
        private IState _previousState;
        private ContentManager _content;
        private SpriteFont _titleFont;

        public TitleState(ContentManager content, IState previous = null)
        {
            _previousState = previous;
            _content = new ContentManager(content.ServiceProvider, content.RootDirectory);
            _titleFont = content.Load<SpriteFont>(Constants.Fonts.TelegramaSmall);
        }

        public void DrawContent(SpriteBatch spriteBatch, Camera camera)
        {
            // Unimplemented for Title
        }

        public IState UpdateState(ref GameSettings gameSettings, GameTime gameTime, Camera camera, KeyboardState currentKey, KeyboardState prevKey, MouseState currentMouse, MouseState prevMouse)
        {
            camera.ResetCamera();
            if (currentKey.IsKeyDown(Keys.Escape) && prevKey.IsKeyUp(Keys.Escape))
            {
                return null;
            }

            if (currentKey.IsKeyDown(Keys.Enter) && prevKey.IsKeyUp(Keys.Enter))
            {
                return new TestLevel(this._content, camera);
            }

            return this;
        }

        public void DrawUI(SpriteBatch spriteBatch, Camera camera)
        {
            string titleText = "LOOT PINYATA";
            string promptText = "Press Enter";
            spriteBatch.DrawString(_titleFont, titleText, camera.Bounds.Center.ToVector2(), Color.DarkRed, -0.261799f, titleText.GetStringOrigin(_titleFont), 4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_titleFont, promptText, camera.Bounds.Center.ToVector2() + new Vector2(0, 100), Color.Black, 0f, titleText.GetStringOrigin(_titleFont), 1f, SpriteEffects.None, 0f);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart
{
    public static class StringExtensions
    {
        public static Vector2 GetStringOrigin(this string message, SpriteFont font)
        {
            return font.MeasureString(message) / 2;
        }
    }
}

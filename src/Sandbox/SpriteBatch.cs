using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blocksGame.Extensions
{
    public static class SpriteBatchExtensions
    {
        private static string[] digits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static string[] charBuffer = new string[10];
        private static float[] xposBuffer = new float[10];
        private static readonly string minValue = Int32.MinValue.ToString(CultureInfo.InvariantCulture);

        /// <summary>  
        /// Extension method for SpriteBatch that draws an integer without allocating  
        /// any memory. This function avoids garbage collections that are normally caused  
        /// by calling Int32.ToString or String.Format.  
        /// </summary>  
        /// <param name="spriteBatch">The SpriteBatch instance whose DrawString method will be invoked.</param>  
        /// <param name="spriteFont">The SpriteFont to draw the integer value with.</param>  
        /// <param name="value">The integer value to draw.</param>  
        /// <param name="position">The screen position specifying where to draw the value.</param>  
        /// <param name="color">The color of the text drawn.</param>  
        /// <returns>The next position on the line to draw text. This value uses position.Y and position.X plus the equivalent of calling spriteFont.MeasureString on value.ToString(CultureInfo.InvariantCulture).</returns>  
        public static Vector2 DrawInt32(this SpriteBatch spriteBatch, SpriteFont spriteFont, int value, Vector2 position, Color color)
        {
            if (spriteBatch == null) throw new ArgumentNullException("spriteBatch");
            if (spriteFont == null) throw new ArgumentNullException("spriteFont");

            Vector2 nextPosition = position;

            if (value == Int32.MinValue)
            {
                nextPosition.X = nextPosition.X + spriteFont.MeasureString(minValue).X;
                spriteBatch.DrawString(spriteFont, minValue, position, color);
                position = nextPosition;
            }
            else
            {
                if (value < 0)
                {
                    nextPosition.X = nextPosition.X + spriteFont.MeasureString("-").X;
                    spriteBatch.DrawString(spriteFont, "-", position, color);
                    value = -value;
                    position = nextPosition;
                }

                int index = 0;

                do
                {
                    int modulus = value % 10;
                    value = value / 10;

                    charBuffer[index] = digits[modulus];
                    xposBuffer[index] = spriteFont.MeasureString(digits[modulus]).X;
                    index += 1;
                }
                while (value > 0);

                for (int i = index - 1; i >= 0; --i)
                {
                    nextPosition.X = nextPosition.X + xposBuffer[i];
                    spriteBatch.DrawString(spriteFont, charBuffer[i], position, color);
                    position = nextPosition;
                }
            }
            return position;
        }
    }  
}

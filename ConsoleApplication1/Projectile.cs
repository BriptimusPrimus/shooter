using System;
using Game2DKit;
using OpenTK;
using System.Drawing;

namespace MyGameStuff
{
    class Projectile
    {
        // Image representing the Projectile
        public Texture2D Texture;

        // Position of the Projectile relative to the upper left side of the screen
        public Vector2 Position;

        // State of the Projectile
        public bool Active;

        // The amount of damage the projectile can inflict to an enemy
        public int Damage;

        // Get the width of the projectile ship
        public int Width
        {
            get { return Texture.Width; }
        }

        // Get the height of the projectile ship
        public int Height
        {
            get { return Texture.Height; }
        }

        // Determines how fast the projectile moves
        float projectileMoveSpeed;


        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;            

            Active = true;

            Damage = 2;

            projectileMoveSpeed = 20f;
        }

        public void Update()
        {
            // Projectiles always move to the right
            Position.X += projectileMoveSpeed;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > Globals.ScreenDimensions.X)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}

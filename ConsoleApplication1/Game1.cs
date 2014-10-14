using System;   
using Game2DKit;
using Game2DKit.Input;
using Game2DKit.SoundMangement;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;

//using OpenTK.Audio;
//using OpenTK.Audio.OpenAL;
//using System.Media;

namespace MyGameStuff
{
    class Game1 : Game
    {
        SpriteBatch spriteBatch;

        // Represents the player 
        Player player;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        Texture2D mainBackground;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        List<Projectile> projectiles;

        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        Font font;

        //Time of game start
        TimeSpan StartTime;
        //Time since game start
        TimeSpan TimeFromStart;

        public Game1()
            : base()
        {
            Initialize();            
            this.Content.ImagesDirectory = "images";
            this.Content.SoundDirectory = "sound";
        }

        protected void Initialize()
        {
            // TODO: Add your initialization logic here                                 
            this.Title = "Shooter";

            // Initialize the player class
            player = new Player();

            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            bgLayer1 = new ParallaxingBackground(); 
            bgLayer2 = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.5f); 

            // Initialize our random number generator
            random = new Random();

            projectiles = new List<Projectile>();

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.30f);

            explosions = new List<Animation>();

            //Set player's score to zero
            score = 0;
        }

        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch();

            // TODO: use this.Content to load your game content here            

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.LoadTexture("shipAnimation.png");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(0, Globals.ScreenDimensions.Y / 2);            
            player.Initialize(playerAnimation, playerPosition);

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "bgLayer1.png", (int)Globals.ScreenDimensions.X, -1);
            bgLayer2.Initialize(Content, "bgLayer2.png", (int)Globals.ScreenDimensions.X, -2);

            mainBackground = Content.LoadTexture("mainbackground.png");

            enemyTexture = Content.LoadTexture("mineAnimation.png");

            projectileTexture = Content.LoadTexture("laser.png");

            explosionTexture = Content.LoadTexture("explosion.png");

            // Load the music
            gameplayMusic = Content.LoadSong("ObstacleCourse.wav", 40);

            // Load the laser and explosion sound effect
            laserSound = Content.LoadSoundEffect("laserFire.wav");
            explosionSound = Content.LoadSoundEffect("explosion.wav");

            // Start the music right away
            gameplayMusic.Start();

            // Load the score font
            font = new Font(FontFamily.GenericSansSerif, 16.0f);

            //start clock
            StartTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            TimeFromStart = StartTime;
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //stop the music before exit
            gameplayMusic.Finish();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                //stop the music before exit                
                Exit();
            }

            // TODO: Add your update logic here

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses            
            previousKeyboardState = currentKeyboardState;            

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Game2DKit.Input.Keyboard.GetState();

            // Update the enemies
            UpdateEnemies(e);

            //Update the player
            UpdatePlayer(e);

            // Update the parallaxing background
            bgLayer1.Update();
            bgLayer2.Update();

            // Update the collision
            UpdateCollision();

            // Update the projectiles
            UpdateProjectiles();

            // Update the explosions
            UpdateExplosions(e);

            // Update Clock
            UpdateClock();
        }

        private void UpdateExplosions(FrameEventArgs gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.IntersectsWith(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }

            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.IntersectsWith(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }

        }

        private void UpdateEnemies(FrameEventArgs gameTime)
        {
            // Spawn a new enemy every 1.5 seconds            
            TimeSpan Now = TimeSpan.FromTicks(DateTime.Now.Ticks);
            if (Now - previousSpawnTime > enemySpawnTime)
            {                
                previousSpawnTime = Now;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position - new Vector2(enemies[i].Width / 2, enemies[i].Height / 2));
                        // Play the explosion sound
                        explosionSound.Play();
                        //Add to the player's score
                        score += enemies[i].Value;
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(FrameEventArgs gameTime)
        {
            player.Update(gameTime);

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.Position.Y += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.Position.Y -= playerMoveSpeed;
            }
            
            // Make sure that the player does not go out of bounds
            if (player.Position.X - (Globals.ScreenDimensions.X - player.Width) > 0)
            {
                player.Position.X = Globals.ScreenDimensions.X - player.Width;    
            }
            if (player.Position.Y - (Globals.ScreenDimensions.Y - player.Height) > 0)
            {
                player.Position.Y = Globals.ScreenDimensions.Y - player.Height;
            }
            if (player.Position.X < 0)
            {
                player.Position.X = 0;
            }
            if (player.Position.Y < 0)
            {
                player.Position.Y = 0;
            }

            // Fire only every interval we set as the fireTime
            TimeSpan Now = TimeSpan.FromTicks(DateTime.Now.Ticks);
            if (Now - previousFireTime > fireTime)
            {
                // Reset our current time
                previousFireTime = Now;

                // Add the projectile, but add it to the front and center of the player
                AddProjectile(player.Position + new Vector2(player.Width / 2, player.Height / 2 - 10));

                // Play the laser sound
                laserSound.Play();
            }

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }

        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateClock()
        { 
            // Spawn a new enemy enemy every 1.5 seconds            
            TimeSpan Now = TimeSpan.FromTicks(DateTime.Now.Ticks);
            TimeFromStart = Now - StartTime;
        }

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(projectileTexture, position);
            projectiles.Add(projectile);
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2((int)Globals.ScreenDimensions.X + enemyTexture.Width / 2, random.Next(100, (int)Globals.ScreenDimensions.Y - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);            

            // TODO: Add your drawing code here
            //Begin
            {
                spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

                // Draw the moving background
                bgLayer1.Draw(spriteBatch);
                bgLayer2.Draw(spriteBatch);

                // Draw the Enemies
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Draw(spriteBatch);
                }

                // Draw the Player
                player.Draw(spriteBatch);

                // Draw the Projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].Draw(spriteBatch);
                }

                // Draw the explosions
                for (int i = 0; i < explosions.Count; i++)
                {
                    explosions[i].Draw(spriteBatch);
                }

                // Draw the score
                spriteBatch.DrawString(font, 
                    "score: " + score,                     
                    new RectangleF(5, 25, 0, 0),
                    Color.White);
                // Draw the player health
                spriteBatch.DrawString(font, 
                    "health: " + player.Health,                     
                    new RectangleF(5, 55, 0, 0),
                    Color.White);

                // Draw the Clock
                spriteBatch.DrawString(font,                    
                    "Time: " +  TimeFromStart.ToString(@"hh\:mm\:ss"),                    
                    new RectangleF(500, 25, 0, 0),
                    Color.White);
            }
            //End

            base.OnRenderFrame(e);
        }

    }
}

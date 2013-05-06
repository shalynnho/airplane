#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class PlaneGameScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);

        Random random = new Random();

        Utils.Timer m_kTimer = new Utils.Timer();

        Airplane m_kAirplane;
        List<Fan> fans = new List<Fan>();

        //public static Microsoft.Xna.Framework.Matrix CameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f), Vector3.Zero, Vector3.UnitY);
        //public static Microsoft.Xna.Framework.Matrix ProjectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);

        public static Microsoft.Xna.Framework.Matrix CameraMatrix;// = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f), Vector3.Zero, Vector3.UnitY);
        public static Microsoft.Xna.Framework.Matrix ProjectionMatrix;// = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);

        //Sound
        private static AudioEngine _audioEngine;
        private static WaveBank _waveBank;
        private static SoundBank _soundBank;

        //city-building
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Effect effect;
        Texture2D sceneryTexture;
        VertexBuffer cityVertexBuffer;
        int[,] floorPlan;
        int[] buildingHeights = new int[] { 0, 2, 2, 6, 5, 4 };
        Matrix viewMatrix;
        Matrix projectionMatrix;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PlaneGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            //this.device = ScreenManager.game.GraphicsDevice;
            //this.device = this.ScreenManager.GraphicsDevice;
             
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            //camera = new CameraComponent(ScreenManager.Game);

            gameFont = content.Load<SpriteFont>("gamefont");

            _audioEngine = new AudioEngine("Content/Sounds.xgs");
            _waveBank = new WaveBank(_audioEngine, "Content/XNAsteroids Waves.xwb");
            _soundBank = new SoundBank(_audioEngine, "Content/XNAsteroids Cues.xsb");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            //level building
            effect = content.Load<Effect>("effects");
            sceneryTexture = content.Load<Texture2D>("texturemap");
            SetUpCamera();
            LoadFloorPlan();
            SetUpVertices();

            m_kAirplane = new Airplane(ScreenManager.Game);
            m_kAirplane.WorldRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
            ScreenManager.Game.Components.Add(m_kAirplane);

            //_soundBank.PlayCue("Ship_Spawn");

        }

        private void SetUpCamera()
        {
            //viewMatrix = Matrix.CreateLookAt(new Vector3(20, 13, -5), new Vector3(8, 0, -7), new Vector3(0, 1, 0));
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.ScreenManager.GraphicsDevice.Viewport.AspectRatio, 0.2f, 500.0f);

            CameraMatrix = Matrix.CreateLookAt(new Vector3(20, 13, -5), new Vector3(8, 0, -7), new Vector3(0, 1, 0));
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.ScreenManager.GraphicsDevice.Viewport.AspectRatio, 0.2f, 500.0f);
        }

        private void LoadFloorPlan()
        {
            floorPlan = new int[,]
             {
                 {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,1,1,0,0,0,1,1,0,0,1,0,1},
                 {1,0,0,1,1,0,0,0,1,0,0,0,1,0,1},
                 {1,0,0,0,1,1,0,1,1,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,1,1,0,0,0,1,0,0,0,0,0,0,1},
                 {1,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,1,0,0,0,1,0,0,0,0,1},
                 {1,0,1,0,0,0,0,0,0,1,0,0,0,0,1},
                 {1,0,1,1,0,0,0,0,1,1,0,0,0,1,1},
                 {1,0,0,0,0,0,0,0,1,1,0,0,0,1,1},
                 {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
             };

            Random random = new Random();
            int differentBuildings = buildingHeights.Length - 1;
            for (int x = 0; x < floorPlan.GetLength(0); x++)
                for (int y = 0; y < floorPlan.GetLength(1); y++)
                    if (floorPlan[x, y] == 1)
                        floorPlan[x, y] = random.Next(differentBuildings) + 1;
        }

        private void SetUpVertices()
        {
            int differentBuildings = buildingHeights.Length - 1;
            float imagesInTexture = 1 + differentBuildings * 2;

            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentbuilding = floorPlan[x, z];

                    //floor or ceiling
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesInTexture, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    if (currentbuilding != 0)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    }
                }
            }

            cityVertexBuffer = new VertexBuffer(this.ScreenManager.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            cityVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                m_kTimer.Update(gameTime);
            }
        }

        private void CheckCollisions()
        {

        }

        /// <summary>
        /// Applies the fan force in their forward direction
        /// </summary>
        private void ApplyFanForces()
        {
            foreach (Fan f in fans)
            {

            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                //ScreenManager.AddScreen(new PauseMenuScreen());
                ScreenManager.AddScreen(new PauseMenuScreen(this));
            }

            if (input.AirplaneLaunch && !m_kAirplane.Launched)
            {
                m_kAirplane.Launched = true;
                m_kAirplane.Force = Vector3.Multiply(m_kAirplane.GetWorldFacing(), m_kAirplane.Thrust);
                m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kAirplane.CurrentAngle));
            }

            /* Old Ship controls
            if (input.ShipFire && _weaponCooldown <= 0 && !ShipDead)
            {
                _weaponCooldown = 1;

                Missile temp = new Missile(ScreenManager.Game, m_kShip);
  
                //m_kTimer.AddTimer("RemoveMissile" + temp.GetHashCode(), 5, removeMissile, false);
                
                ScreenManager.Game.Components.Add(temp);
                _soundBank.PlayCue("Ship_Missile");
                Missiles.Add(temp);

            }

            if (input.ShipForwardThrust)
            {

                //Vector3 vTempVelocity = Vector3.Zero;
                //vTempVelocity += Vector3.Multiply(m_kShip.GetWorldFacing(), m_kShip.Speed);
                //vTempVelocity.Z = 0;

                m_kShip.Force = Vector3.Multiply(m_kShip.GetWorldFacing(), m_kShip.Thrust);
                //m_kShip.Velocity = vTempVelocity; //Store the vector for wrapping around
                m_kShip.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kShip.CurrentAngle));
            }

            if (input.ShipReverseThrust)
            {

                m_kShip.Force = Vector3.Multiply(m_kShip.mWorldTransform.Backward, m_kShip.Thrust);
                m_kShip.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kShip.CurrentAngle));
            }

            if (input.ShipTurnLeft)
            {
                m_kShip.CurrentAngle += m_kShip.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_kShip.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kShip.CurrentAngle));
            }

            if (input.ShipTurnRight)
            {
                m_kShip.CurrentAngle -= m_kShip.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_kShip.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kShip.CurrentAngle));
            }

            //Let's mimic actual Asteroids and have a decelerating force to automatically slow the player down.
            if(!(input.ShipReverseThrust || input.ShipForwardThrust)){
                m_kShip.Force = Vector3.Zero;
                if (m_kShip.Velocity.Length() > 0)
                {
                    m_kShip.Velocity = Vector3.Subtract(m_kShip.Velocity, Vector3.Multiply(m_kShip.Velocity, .75f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }
            }
             * */

            else
            {
                /*
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Left))
                    movement.X--;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Right))
                    movement.X++;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Up))
                    movement.Y--;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = input.CurrentGamePadStates[0].ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * ((float) gameTime.ElapsedGameTime.TotalSeconds) * 60;
                 * */
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            /*ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);*/

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            //spriteBatch.DrawString(gameFont, "Magic Text", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);

            //spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 1"), new Vector2(20.0f, 500.0f), Color.Blue);
            //spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 2"), new Vector2(20.0f, 550.0f), Color.White);
            //spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 3"), new Vector2(20.0f, 600.0f), Color.White);
            //spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 4"), new Vector2(20.0f, 650.0f), Color.Blue);
            //spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 5"), new Vector2(20.0f, 700.0f), Color.White);

            spriteBatch.End();

            DrawCity();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        private void DrawCity()
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(CameraMatrix);
            effect.Parameters["xProjection"].SetValue(ProjectionMatrix);
            effect.Parameters["xTexture"].SetValue(sceneryTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.ScreenManager.GraphicsDevice.SetVertexBuffer(cityVertexBuffer);
                this.ScreenManager.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, cityVertexBuffer.VertexCount / 3);
            }
        }

        #endregion

        #region Timer Test Functions
        void TimerOneShot()
        {
            Console.WriteLine("TimerOneShot fired!");
        }

        void TimerLoop()
        {
            Console.WriteLine("TimerLoop fired!");
        }

        void TimerLoopRemove()
        {
            Console.WriteLine("TimerLoopRemove fired!");
            m_kTimer.RemoveTimer("Timer 3");
        }

        string MakeTimerDebugString(string sTimerName)
        {
            if (m_kTimer.GetTriggerCount(sTimerName) != -1)
                return sTimerName + " - Time: " + m_kTimer.GetRemainingTime(sTimerName).ToString("f3")
                    + " Count: " + m_kTimer.GetTriggerCount(sTimerName);
            else
                return sTimerName + " not found! ";
        }
        #endregion
    }
}

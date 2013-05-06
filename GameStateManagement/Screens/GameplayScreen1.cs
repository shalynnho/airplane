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
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        CameraComponent camera;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

		Utils.Timer m_kTimer = new Utils.Timer();

        Airplane m_kAirplane;
        float launchPitch;
        float launchYaw;

        List<Fan> fans = new List<Fan>();

        //Sound
        private static AudioEngine _audioEngine;
        private static WaveBank _waveBank;
        private static SoundBank _soundBank;
        private Boolean goalSoundPlayed;

        //city-building
        Effect effect;
        Texture2D sceneryTexture;
        VertexBuffer cityVertexBuffer;
        int[,] floorPlan;
        int[] buildingHeights = new int[] { 0, 2, 2, 3, 1, 4 };

        private BasicEffect basicEffect;

        Goal m_kGoal;
        BoundingBox goalBox;

        Bonus m_kBonus;

        BoundingBox[] buildingBoundingBoxes;
        BoundingBox levelBoundingBox;

        MouseState previousState;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            launchPitch = 0;
            launchYaw = 0;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

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
            LoadFloorPlan();
            SetUpVertices();
            SetUpBoundingBoxes();
            
            m_kAirplane = new Airplane(ScreenManager.Game);

            camera = new CameraComponent(ScreenManager.Game, m_kAirplane);
            m_kAirplane.Camera = camera;

            ScreenManager.Game.Components.Add(camera);

			ScreenManager.Game.Components.Add(m_kAirplane);
            //_soundBank.PlayCue("Ship_Spawn");

            m_kAirplane.WorldPosition = new Vector3(8, 1, -3);

            m_kGoal = new Goal(ScreenManager.Game);
            m_kGoal.Camera = camera;
            ScreenManager.Game.Components.Add(m_kGoal);
            m_kGoal.WorldPosition = new Vector3(5.5f,2.0f,-10.5f);
            SetUpGoalBoundingBox();
            goalSoundPlayed = false;

            m_kBonus = new Bonus(ScreenManager.Game);
            m_kBonus.Camera = camera;
            ScreenManager.Game.Components.Add(m_kBonus);
            m_kBonus.WorldPosition = new Vector3(8.5f,2.5f,-6.5f);

            ScreenManager.Game.IsMouseVisible = true;

            basicEffect = new BasicEffect(ScreenManager.GraphicsDevice);
            


            //m_kAirplane.WorldPosition = new Vector3(8, 0, -7);
            //camera.View = Matrix.CreateLookAt(new Vector3(3, 5, 2), new Vector3(2, 0, -1), new Vector3(0, 1, 0));
        }

        private void Retry()
        {
            m_kAirplane.Dispose();
            _soundBank.PlayCue("Ship_Death");
            m_kAirplane = new Airplane(ScreenManager.Game);

            ScreenManager.Game.Components.Add(m_kAirplane);
            //_soundBank.PlayCue("Ship_Spawn");

            m_kAirplane.WorldPosition = new Vector3(8, 1, -3);
            m_kAirplane.PitchAngle = launchPitch;
            m_kAirplane.YawAngle = launchYaw;
            m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_kAirplane.YawAngle), Quaternion.CreateFromAxisAngle(Vector3.Normalize(m_kAirplane.mWorldTransform.Right), m_kAirplane.PitchAngle));

            launchPitch = 0;
            launchYaw = 0;

            //ScreenManager.Game.Components.Remove(camera);
            //camera = new CameraComponent(ScreenManager.Game, m_kAirplane);
            m_kAirplane.Camera = camera;
            camera.Airplane = m_kAirplane;
            //ScreenManager.Game.Components.Add(camera);
            camera.Reset();

            goalSoundPlayed = false;
        }

        private void LoadFloorPlan()
        {
            floorPlan = new int[,]
             {  //0,1,2,3,4,5,6,7,8,9,0,1,2,3,4
                 {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,2,2,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,2,2,2,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,2,0,0,0,0,1},
                 {1,0,0,0,0,4,0,0,0,2,0,0,0,0,1},
                 {1,0,0,0,0,0,0,2,2,3,2,2,2,2,1},
                 {1,0,0,0,2,2,2,2,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                 {1,0,0,0,0,0,0,1,0,0,0,0,0,0,1},
                 {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
             };

            Random random = new Random();
            int differentBuildings = buildingHeights.Length - 1;
            for (int x = 0; x < floorPlan.GetLength(0); x++)
                for (int y = 0; y < floorPlan.GetLength(1); y++)
                    if (floorPlan[x, y] == 1)
                    {
                        //floorPlan[x, y] = random.Next(differentBuildings) + 1;
                    }
                        

            
            
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

        private void SetUpGoalBoundingBox()
        {            
            Vector3[] buildingPoints = new Vector3[2];
            buildingPoints[0] = new Vector3(m_kGoal.WorldPosition.X-0.5f, m_kGoal.WorldPosition.Y, m_kGoal.WorldPosition.Z+0.5f);
            buildingPoints[1] = new Vector3(m_kGoal.WorldPosition.X + 0.5f, m_kGoal.WorldPosition.Y + 1, m_kGoal.WorldPosition.Z - 0.5f);
            BoundingBox buildingBox = BoundingBox.CreateFromPoints(buildingPoints);
            goalBox = buildingBox;
        }

        private void SetUpBoundingBoxes()
        {
            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);


            List<BoundingBox> bbList = new List<BoundingBox>(); for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int buildingType = floorPlan[x, z];
                    if (buildingType != 0)
                    {
                        int buildingHeight = buildingHeights[buildingType];
                        Vector3[] buildingPoints = new Vector3[2];
                        buildingPoints[0] = new Vector3(x, 0, -z);
                        buildingPoints[1] = new Vector3(x + 1, buildingHeight, -z - 1);
                        BoundingBox buildingBox = BoundingBox.CreateFromPoints(buildingPoints);
                        bbList.Add(buildingBox);
                    }
                }
            }
            buildingBoundingBoxes = bbList.ToArray();

            Vector3[] boundaryPoints = new Vector3[2];
            boundaryPoints[0] = new Vector3(0, 0, 0);
            boundaryPoints[1] = new Vector3(cityWidth, 20, -cityLength);
            levelBoundingBox = BoundingBox.CreateFromPoints(boundaryPoints);
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
            CheckMouseClick();
            CheckCollisions();
            ApplyFanForces(gameTime);
            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(200, 200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
				m_kTimer.Update(gameTime);

            }
        }

        private void CheckCollisions()
        {
            //check collisions with buildings/level
            for (int i = 0; i < buildingBoundingBoxes.Length; i++)
            {
                if (buildingBoundingBoxes[i].Contains(m_kAirplane.WorldBoundingSphere) != ContainmentType.Disjoint)
                {
                    Retry();
                    System.Diagnostics.Debug.WriteLine("BUILDING HIT");
                }
            }
            if (levelBoundingBox.Contains(m_kAirplane.WorldBoundingSphere) != ContainmentType.Contains)
            {
                Retry();
            }

            if(m_kAirplane.WorldBoundingSphere.Intersects(goalBox))
            {
                if (!goalSoundPlayed) { 
                    _soundBank.PlayCue("Ship_Spawn"); 
                    goalSoundPlayed = true;
                    Retry();
                }
                System.Diagnostics.Debug.WriteLine("YOU HIT THE GOAL");
                System.Diagnostics.Debug.WriteLine("Pitch: " + m_kAirplane.PitchAngle + " Yaw: " + m_kAirplane.YawAngle);
            }

            /*
            //Ship colliding with asteroids
            for (int i = 0; i < m_kSpawnManager.AsteroidList.Count; i++)
            {
                    if (m_kSpawnManager.AsteroidList[i].WorldBoundingSphere.Intersects(m_kShip.WorldBoundingSphere) && !m_kShip.Invincible && !ShipDead)
                    {
                        ScreenManager.Game.Components.Remove(m_kShip);
                        System.Diagnostics.Debug.WriteLine("Dead!", "GAME OVER");
                        m_kTimer.RemoveTimer("ShipRespawn");
                        m_kTimer.AddTimer("ShipRespawn", 5, spawnShip, false);
                        ShipDead = true;
                        break;
                    }
            }
             * */

        }

        void CheckMouseClick()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
            {

                Ray pickRay = GetPickRay();


                //Nullable<float> result = pickRay.Intersects(triangleBB);
                int selectedIndex = -1;
                float selectedDistance = float.MaxValue;
                for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                {
                    Nullable<float> result = pickRay.Intersects(buildingBoundingBoxes[i]);
                    if (result.HasValue == true)
                    {
                        if (result.Value < selectedDistance)
                        {
                            selectedIndex = i;
                            selectedDistance = result.Value;
                        }
                    }
                }
                if (selectedIndex > -1)
                {
                    //selected grid
                    //buildingBoundingBoxes[selectedIndex].
                    //buildingBoundingBoxes[selectedIndex] = selectedTexture;
                    System.Diagnostics.Debug.WriteLine(selectedIndex);

                    bool fanExists = false;
                    Vector3 fanPosition = new Vector3((buildingBoundingBoxes[selectedIndex].GetCorners()[0].X + buildingBoundingBoxes[selectedIndex].GetCorners()[1].X)/2, buildingBoundingBoxes[selectedIndex].GetCorners()[0].Y, (buildingBoundingBoxes[selectedIndex].GetCorners()[0].Z + buildingBoundingBoxes[selectedIndex].GetCorners()[4].Z)/2);
                    foreach (Fan f in fans)
                    {
                        if(f.WorldPosition == fanPosition)
                        {
                            System.Diagnostics.Debug.WriteLine(f.WorldPosition);                            
                            f.CurrentAngle += MathHelper.PiOver4;
                            fanExists = true;                            
                            System.Diagnostics.Debug.WriteLine("ROTATED");
                            System.Diagnostics.Debug.WriteLine(f.WorldPosition);
                        }
                    }
                    if(!fanExists)
                    {
                        Fan temp = new Fan(ScreenManager.Game);
                        temp.Camera = camera;
                        temp.WorldRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);                    
                        temp.WorldPosition = new Vector3((buildingBoundingBoxes[selectedIndex].GetCorners()[0].X + buildingBoundingBoxes[selectedIndex].GetCorners()[1].X)/2, buildingBoundingBoxes[selectedIndex].GetCorners()[0].Y, (buildingBoundingBoxes[selectedIndex].GetCorners()[0].Z + buildingBoundingBoxes[selectedIndex].GetCorners()[4].Z)/2);
                        
                        ScreenManager.Game.Components.Add(temp);
                        fans.Add(temp);
                    }
                    
                    
                }

            }

            if (mouseState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released)
            {

                Ray pickRay = GetPickRay();


                //Nullable<float> result = pickRay.Intersects(triangleBB);
                int selectedIndex = -1;
                float selectedDistance = float.MaxValue;
                for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                {
                    Nullable<float> result = pickRay.Intersects(buildingBoundingBoxes[i]);
                    if (result.HasValue == true)
                    {
                        if (result.Value < selectedDistance)
                        {
                            selectedIndex = i;
                            selectedDistance = result.Value;
                        }
                    }
                }
                if (selectedIndex > -1)
                {
                    //selected grid
                    //buildingBoundingBoxes[selectedIndex].
                    //buildingBoundingBoxes[selectedIndex] = selectedTexture;
                    System.Diagnostics.Debug.WriteLine(selectedIndex);

                    bool fanExists = false;
                    Vector3 fanPosition = new Vector3((buildingBoundingBoxes[selectedIndex].GetCorners()[0].X + buildingBoundingBoxes[selectedIndex].GetCorners()[1].X) / 2, buildingBoundingBoxes[selectedIndex].GetCorners()[0].Y, (buildingBoundingBoxes[selectedIndex].GetCorners()[0].Z + buildingBoundingBoxes[selectedIndex].GetCorners()[4].Z) / 2);
                    Fan toRemove = null;
                    foreach (Fan f in fans)
                    {
                        if (f.WorldPosition == fanPosition)
                        {
                            //fans.Remove(f);
                            toRemove = f;
                            ScreenManager.Game.Components.Remove(f);
                            System.Diagnostics.Debug.WriteLine("REMOVED");
                        }
                    }
                    if (toRemove != null)
                    {
                        fans.Remove(toRemove);
                    }
                    
                    if (!fanExists)
                    {
                        
                    }


                }

            }



            previousState = mouseState;
        }
        
        Ray GetPickRay()
        {
            MouseState mouseState = Mouse.GetState();

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
            Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = ScreenManager.GraphicsDevice.Viewport.Unproject(nearsource, camera.Projection, camera.View, world);

            Vector3 farPoint = ScreenManager.GraphicsDevice.Viewport.Unproject(farsource, camera.Projection, camera.View, world);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            return pickRay;
        }

        /// <summary>
        /// Applies the fan force in their forward direction
        /// </summary>
        private void ApplyFanForces(GameTime g)
        {
            float? intersectionDistance = 0;
            if (m_kAirplane.Launched)
            {
                foreach (Fan f in fans)
                {
                    for (int i = 0; i < f.rays.Length; i++)
                    {
                        intersectionDistance = f.rays[i].Intersects(m_kAirplane.WorldBoundingSphere);
                        if (intersectionDistance != null)
                        {
                            if (!_soundBank.IsInUse)
                            {
                                _soundBank.PlayCue("Ship_Swoosh");
                            }
                            float fanDistance = (float)intersectionDistance;
                            if (fanDistance > 0)
                            {
                                float magnitude = (float) (f.FanPower / Math.Pow(MathHelper.E, 3 * (double)(intersectionDistance)));
                                m_kAirplane.ApplyWindForce(Vector3.Divide(f.rays[i].Direction, 2), (magnitude > 25) ? 25 : magnitude, g);
                                //System.Diagnostics.Debug.WriteLine("Intersection with fan ray at: " + fanDistance);
                            }
                        }  
                    }
                                     
                }
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
                ScreenManager.AddScreen(new PauseMenuScreen(this));
            }

            if (input.AirplaneLaunch && !m_kAirplane.Launched)
            {
                launchPitch = m_kAirplane.PitchAngle;
                launchYaw = m_kAirplane.YawAngle;
                m_kAirplane.Launched = true;
                m_kAirplane.Force += Vector3.Multiply(m_kAirplane.GetWorldFacing(), m_kAirplane.Thrust);
                camera.Launching = true;
                //_soundBank.PlayCue("Ship_Throw");
                // m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2),
                //    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, m_kAirplane.CurrentAngle));
            }

            if (input.RightArrowHeld)
            {
                if (camera.FreeRoam)
                {
                    camera.RotateYRightFromCamera(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    camera.RotateYRightAroundPlane(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (input.LeftArrowHeld)
            {
                if (camera.FreeRoam)
                {
                    camera.RotateYLeftFromCamera(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    camera.RotateYLeftAroundPlane(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (input.UpArrowHeld)
            {
                if (camera.FreeRoam)
                {
                    camera.RotateXUpFromCamera(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    camera.RotateXCCWAroundPlane(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (input.DownArrowHeld)
            {
                if (camera.FreeRoam)
                {
                    camera.RotateXDownFromCamera(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    camera.RotateXCWAroundPlane(camera.Angle_increment * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (input.RButtonPressed)
            {
                Retry();
            }
            if (input.TButtonPressed)
            {
                if (camera.FreeRoam)
                {
                    camera.FreeRoam = false;
                }
                else
                {
                    camera.FreeRoam = true;
                }
            }


            //Allow the airplane to be pointed in a direction before launching it
            if (!m_kAirplane.Launched)
            {
                if (input.AirplaneLeft)
                {
                    if (!camera.FreeRoam)
                    {
                        m_kAirplane.YawAngle += (m_kAirplane.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, m_kAirplane.PitchAngle), Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_kAirplane.YawAngle));
                    }
                }

                if (input.AirplaneRight)
                {
                    if (!camera.FreeRoam)
                    {
                        m_kAirplane.YawAngle -= (m_kAirplane.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, m_kAirplane.PitchAngle), Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_kAirplane.YawAngle));
                    }
                }

                if (input.AirplaneUp)
                {
                    if (camera.FreeRoam)
                    {
                        camera.MoveForward(camera.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        m_kAirplane.PitchAngle -= (m_kAirplane.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, m_kAirplane.PitchAngle), Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_kAirplane.YawAngle));
                    }
                }

                if (input.AirplaneDown)
                {
                    if (camera.FreeRoam)
                    {
                        camera.MoveBackward(camera.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        m_kAirplane.PitchAngle += (m_kAirplane.RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        m_kAirplane.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, m_kAirplane.PitchAngle), Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_kAirplane.YawAngle));
                    }
                }

                if (input.PButtonPressed)
                {
                    m_kAirplane.PitchAngle = (float) 0.6365855;
                    m_kAirplane.YawAngle =  (float) -0.3565314;
                }

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Azure, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Texture2D pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Begin();
            
            spriteBatch.End();
            //ScreenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            DrawCity();

            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
            basicEffect.CurrentTechnique.Passes[0].Apply();

            
            foreach (Fan f in fans)
            {
                f.BuildRay();
                foreach (Ray r in f.rays)
                {
                    Vector3 start = r.Position;
                    Vector3 end = ScreenManager.GraphicsDevice.Viewport.Project(start, camera.Projection, camera.View, Matrix.Identity);
                    Vector3 end2 = r.Position + (r.Direction * 100);
                    //spriteBatch.Draw(pixel, new Vector2(end.X, end.Y), Color.White);
                    //LineBatch.DrawLine(spriteBatch, Color.White, start, end);
                    var vertices = new[] { new VertexPositionColor(start, Color.White), new VertexPositionColor(end2, Color.White) };
                    ScreenManager.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                }
                
            }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        private void DrawCity()
        {
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            Matrix world = Matrix.CreateTranslation(new Vector3(-((float)floorPlan.GetLength(0) / 2.0f), 0, ((float)floorPlan.GetLength(1) / 2.0f)));
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(camera.View);
            effect.Parameters["xProjection"].SetValue(camera.Projection);
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

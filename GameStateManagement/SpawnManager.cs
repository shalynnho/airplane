#region File Description
//-----------------------------------------------------------------------------
// SpawnManager.cs
//
// Spawns fans. The fans should have been placed by the player prior to
// starting the game. 
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Timers;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpawnManager : Microsoft.Xna.Framework.GameComponent
    {

        //int AsteroidCount;
        const float MAXFANS = 5.0f;
        const float TIMERMAX = 5; //const value
        Random RNG;

        List<Fan> fans = new List<Fan>();

        public SpawnManager(Game game)
            : base(game)
        {
            //AsteroidCount = 0;
            RNG = new Random();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code 
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            /*
            //timer for spawning asteroids
            fSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fSpawnTimer <= 0 && Asteroids.Count < MAXASTEROIDS)
            {

                //Random X
                double xRand = RNG.NextDouble()*(1024) - 512;
                /*while(xRand - 50 < -512 || xRand + 50 > 512){
                    xRand = RNG.NextDouble() * (1024) - 512;
                }

             * //Random Y
                double yRand = RNG.NextDouble() *(768) - 384;
                while (yRand - 50 < -384 || yRand + 50 > 384)
                {
                    yRand = RNG.NextDouble() * (768) - 384;
                }

                Asteroid temp = new Asteroid(Game);
                //System.Diagnostics.Debug.WriteLine("Creating asteroid");
                temp.WorldRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
                temp.WorldPosition = new Vector3((float)xRand,(float)yRand, 1);

                Game.Components.Add(temp);
                Asteroids.Add(temp);

                //AsteroidCount++;
                fSpawnTimer = TIMERMAX; //Reset to 5s
            }
                 * */

            base.Update(gameTime);
        }

        public List<Fan> FanList
        {
            get { return fans; }
        }

        public void removeFan(Fan a)
        {
            fans.Remove(a);
        }
    }
}

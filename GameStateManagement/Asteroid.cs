#region File Description
//-----------------------------------------------------------------------------
// Asteroid.cs
//
// Asteroid class 
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


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Asteroid : Actor
    {
        Random rand = new Random();
        //private const float _SPEED = 50.0f;
        private const float _FORCE = 500.0f;
        private const float _ROTATIONSPEED = 0.2f;
        private Random random;

        private const float _BOUNCETIMERMAX = 1.0f; //Prevents asteroids from sticking together
        private const float _BOUNCETIMERMIN = 3.0f;

        private float BounceTimer;

        public Asteroid(Game game)
            : base(game)
        {
            base.SetActorMeshName("Asteroid");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            // TODO: Add your initialization code here
            random = new Random();
            //base.Velocity = new Vector3((float) rand.NextDouble() * (2 * _SPEED) - _SPEED, (float) rand.NextDouble() * (2 * _SPEED) - _SPEED, 0);
            base.vForce = new Vector3((float)rand.NextDouble() * (2 * _FORCE) - _FORCE, (float)rand.NextDouble() * (2 * _FORCE) - _FORCE, 0);
            base.fTerminalVelocity = 75.0f;
            base.fMass = 30.0f;
            base.bPhysicsDriven = true;
            BounceTimer = 1.0f;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            CurrentAngle += (_ROTATIONSPEED * (float)gameTime.ElapsedGameTime.TotalSeconds) % MathHelper.TwoPi;
            WorldRotation = Quaternion.CreateFromAxisAngle(WorldForward, CurrentAngle);
            //WorldPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (BounceTimer > 0) BounceTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public void Bounce(Vector3 otherVelocity, Vector3 otherForce)
        {
            if (BounceTimer <= 0)
            {

                System.Diagnostics.Debug.WriteLine(GetHashCode() + " Bouncing");
                //base.vForce = Vector3.Multiply(base.vForce, Vector3.Negate(Vector3.Normalize(Velocity);
                base.Force = Vector3.Multiply(Vector3.Negate(base.Velocity), (float)(rand.NextDouble() * (100000 - 20000) + 20000));
                //base.Velocity = Vector3.Negate(base.Velocity);
                //base.Velocity = Vector3.Negate(base.Velocity);
                BounceTimer = (float)(random.NextDouble() * (_BOUNCETIMERMAX - _BOUNCETIMERMIN) + _BOUNCETIMERMIN);

                /* Old convoluted logic
                   //base.vForce = Vector3.Multiply(base.vForce, Vector3.Negate(Vector3.Normalize(Velocity);
                   base.Force = Vector3.Multiply(Vector3.Negate(base.Velocity), (float) (rand.NextDouble()*(10000-2000) + 2000));
                   //base.Velocity = Vector3.Negate(base.Velocity);
                   //base.Velocity = Vector3.Negate(base.Velocity);
                   BounceTimer = _BOUNCETIMERMAX;
                   base.Force = (Vector3.Dot(Vector3.Normalize(base.Force), Vector3.Normalize(otherForce)) == -1) ? base.Force : Vector3.Negate(base.Force);
                       //new Vector3((otherForce.X / Math.Abs(otherForce.X) != base.Force.X / Math.Abs(base.Force.X)) ? base.Force.X : -base.Force.X, 
                       //(otherForce.Y / Math.Abs(otherForce.Y) != base.Force.Y / Math.Abs(base.Force.Y)) ? base.Force.Y : -base.Force.Y, 0)s;
                   base.Velocity = (Vector3.Dot(Vector3.Normalize(base.Velocity), Vector3.Normalize(otherVelocity)) == -1) ? base.Velocity : Vector3.Negate(base.Velocity);
                       //new Vector3((otherVelocity.X / Math.Abs(otherVelocity.X) != base.Velocity.X / Math.Abs(base.Velocity.X)) ? base.Velocity.X : -base.Velocity.X,
                       //(otherVelocity.Y / Math.Abs(otherVelocity.Y) != base.Velocity.Y / Math.Abs(base.Velocity.Y)) ? base.Velocity.Y : -base.Velocity.Y, 0);
                   BounceTimer =  (float) (random.NextDouble() * (_BOUNCETIMERMAX - _BOUNCETIMERMIN) + _BOUNCETIMERMIN);
                 * */

            }
        }
    }
}

#region File Description
//-----------------------------------------------------------------------------
// Missile.cs
//
// Missile class 
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
    public class Missile : Actor
    {
        private float TimeToLive = 5; //Causes missiles to be removed after 5s.
        private const float _SPEED = 75.0f;

        public Missile(Game game, Ship ship)
            : base(game)
        {
            base.SetActorMeshName("Missile");
            base.WorldPosition = ship.WorldPosition;
            base.WorldRotation = Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, ship.CurrentAngle + MathHelper.Pi));
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.WorldScale = 1.25f;
            base.vForce = new Vector3(mWorldTransform.Backward.X * 500.0f, mWorldTransform.Backward.Y * 500.0f, 0);
            base.fTerminalVelocity = 400.0f;
            base.fMass = 5.0f;
            base.bPhysicsDriven = true;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Velocity = Vector3.Multiply(mWorldTransform.Backward, _SPEED);
            //System.Diagnostics.Debug.WriteLine("Backward: " + mWorldTransform.Backward);
            //System.Diagnostics.Debug.WriteLine("Velocity: " + Velocity);
            //WorldPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(TimeToLive > 0) TimeToLive -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public float TTL{
            get { return TimeToLive; }
        }
    }
}

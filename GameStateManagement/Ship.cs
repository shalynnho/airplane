#region File Description
//-----------------------------------------------------------------------------
// Ship.cs
//
// Ship class 
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
    public class Ship : Actor
    {

        private const float _ROTATIONSPEED = 2.0f;
        private const float _THRUST = 2000.0f;
        private bool _bInvincible; //Prevents insta-death from stray asteroids

        public Ship(Game game)
            : base(game)
        {
            base.SetActorMeshName("Ship");
            //base.mWorldTransform = Matrix.CreateRotationX(MathHelper.PiOver2);
            _bInvincible = true;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.WorldPosition = new Vector3(0,0,1);
            base.fTerminalVelocity = 150.0f;
            base.fMass = 20.0f;
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
            //System.Diagnostics.Debug.WriteLine("Ship Forward " + GetWorldFacing());
            base.Update(gameTime);
        }

        public float RotationSpeed
        {
            get { return _ROTATIONSPEED; }
        }

        public float Thrust
        {
            get { return _THRUST; }
        }

        public bool Invincible { 
            get { return _bInvincible; } 
            set { _bInvincible = value; } 
        }


    }
}

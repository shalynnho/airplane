#region File Description
//-----------------------------------------------------------------------------
// Airplane2.cs
//
// The user-controlled airplane. This is a glider and is less affected by 
// by gravity. The tradeoff is it moves slower and is more affected by 
// wind
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
    public class Airplane2 : Airplane
    {
        public Airplane2(Game game)
            : base(game)
        {
            base.SetActorMeshName("Glider");
            base._launched = false;
            base._GRAVITY = Vector3.Zero;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            base.SetActorMeshName("Glider");
            base.WorldPosition = new Vector3(0, 0, 1);
            //base.WorldScale = .003f;
            base.WorldScale = .04f;
            base.fTerminalVelocity = 1.25f;
            base.fMass = 1.0f;
            base.bPhysicsDriven = true;

            base._GRAVITY = Vector3.Down * 2.5f;

            ambientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            specularColor = new Vector3(1.0f, 1.0f, 1.0f);
            specularPower = 0.0f;
            lightDirection = new Vector3(0.5f, 0.2f, 0.0f);
            lightDiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
        }

        protected override void LoadContent()
        {
            
            //effect = Game.Content.Load<Effect>("Texture");
            //texture = Game.Content.Load<Texture2D>("PaperTexture");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (Launched)
            {
                //System.Diagnostics.Debug.WriteLine("Airplane vel: " + base.Velocity);
                PointTowardsMovementDirection();
                ApplyGravity(gameTime);
            }

            /** For specular lighting **/

            Vector3 cameraLocation = distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
            Vector3 cameraTarget = new Vector3(0, 0, 0);
            viewVector = Vector3.Transform(cameraTarget - cameraLocation, Matrix.CreateRotationY(0));
            viewVector.Normalize();
            view = Matrix.CreateLookAt(cameraLocation, cameraTarget, new Vector3(0, 1, 0));

            /**/
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //System.Diagnostics.Debug.WriteLine("I'm a paper airplane!");
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            actorModel.CopyAbsoluteBoneTransformsTo(actorBoneTransforms);
            foreach (ModelMesh mesh in actorModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = actorBoneTransforms[mesh.ParentBone.Index] * mWorldTransform;
                    effect.View = camera.View;
                    //System.Diagnostics.Debug.WriteLine("Actor view: " + effect.View);
                    effect.Projection = camera.Projection;
                    //effect.Texture = texture;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;


                    effect.AmbientLightColor = ambientLightColor;
                    //effect.SpecularColor = specularColor;
                    //effect.SpecularPower = specularPower;
                    // effect.DirectionalLight0.Direction = lightDirection;
                    // effect.DirectionalLight0.DiffuseColor = lightDiffuseColor;


                }
                mesh.Draw();
            }
            // DrawModelWithEffect(model, world, view, projection);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Applies a fan's wind force to the airplane. 
        /// </summary>
        /// <param name="direction">A unit vector pointing in the fan's direction</param>
        /// <param name="magnitude"></param>
       public void ApplyWindForce(Vector3 direction, float magnitude, GameTime g)
        {
            float elapsedTime = (float)g.ElapsedGameTime.TotalSeconds;
            base.Force += Vector3.Multiply(direction, magnitude*1.4f) * elapsedTime;
        }

    }
}
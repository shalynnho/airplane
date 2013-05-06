#region File Description
//-----------------------------------------------------------------------------
// Bonus.cs
//
// Bonus objects for the airplane to collect
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
    public class Bonus : Actor
    {

        private Boolean _visible = true;

        public Bonus(Game game)
            : base(game)
        {
            base.SetActorMeshName("Coin");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.WorldRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
            //base.WorldPosition = new Vector3(8.0f, 1.0f, -4.0f);
            base.fTerminalVelocity = 0.0f;
            base.fMass = 0.5f;
            base.WorldScale = .015f;
            ambientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            specularColor = new Vector3(1.0f, 1.0f, 1.0f);
            specularPower = 80.0f;
            lightDirection = new Vector3(1.0f, 0.5f, 0.5f);
            lightDiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            effect = Game.Content.Load<Effect>("Toon");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            WorldRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.05f);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //System.Diagnostics.Debug.WriteLine("COIN DRAW NOW");
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
                    effect.SpecularColor = specularColor;
                    //effect.SpecularPower = specularPower;
                    effect.DirectionalLight0.Direction = lightDirection;
                    effect.DirectionalLight0.DiffuseColor = lightDiffuseColor;
                }
                if (Visible) { mesh.Draw(); } 
            }
            //DrawModelWithEffect(model, world, view, projection);
            base.Draw(gameTime);
        }
        
        private void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in actorModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(actorBoneTransforms[mesh.ParentBone.Index] * mWorldTransform);
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    //effect.Parameters["ViewVector"].SetValue(viewVector);
                    //effect.Parameters["ModelTexture"].SetValue(texture);
                    effect.Parameters["Texture"].SetValue(texture);
                } mesh.Draw();
                
            }
        }

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
    }

    }

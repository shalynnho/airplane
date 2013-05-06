#region File Description
//-----------------------------------------------------------------------------
// Goal.cs
//
// The goal the airplane must reach
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
    public class Goal : Actor
    {

        public Goal(Game game)
            : base(game)
        {
            base.SetActorMeshName("garbagecan"); //TODO: Change once we have a model
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            //base.WorldPosition = new Vector3(0, 0, 1);
            base.fTerminalVelocity = 150.0f;
            base.fMass = 20.0f;
            base.bPhysicsDriven = true;
            base.WorldScale = 0.0055f;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            effect = Game.Content.Load<Effect>("Toon");
            texture = Game.Content.Load<Texture2D>("TrashCanTexture");
            base.LoadContent();


        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            actorModel.CopyAbsoluteBoneTransformsTo(actorBoneTransforms);
            /*foreach (ModelMesh mesh in actorModel.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.World = actorBoneTransforms[mesh.ParentBone.Index] * mWorldTransform;
                    basicEffect.View = Camera.View;
                    //System.Diagnostics.Debug.WriteLine("Actor view: " + effect.View);
                    basicEffect.Projection = Camera.Projection;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.PreferPerPixelLighting = true;

                }
                mesh.Draw();
            }*/
            DrawModelWithEffect(model, world, view, projection);
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
        }}

    }

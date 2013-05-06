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
    public class Fan : Actor
    {
        private WindParticleSystem wind;
        private Game game;
        private float m_fFanPower;
        private Vector3 m_vFanDirection;
        private Ray m_rWindRay;
        private Ray m_rWindRayT;
        private Ray m_rWindRayB;
        private Ray m_rWindRayL;
        private Ray m_rWindRayR;
        private Ray m_rWindRayLT;
        private Ray m_rWindRayRT;
        private Ray m_rWindRayLTT;
        private Ray m_rWindRayRTT;
        private Ray m_rWindRayTT;
        public Ray[] rays = new Ray[10];
        private bool m_bForceApplied;

        public Fan(Game game)
            : base(game)
        {
            base.SetActorMeshName("ToonFan");
            this.game = game;
            m_fFanPower = 5000;
            m_vFanDirection = base.WorldForward;
            m_bForceApplied = false;
            base.WorldScale = 0.02f;
            wind = new WindParticleSystem(game, game.Content, Vector3.UnitZ);
            
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            WorldRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, CurrentAngle);
            
            base.Initialize();
            BuildRayInitial();

        }

        public void BuildRayInitial()
        {
            m_rWindRay = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayT = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayB = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.2f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayL = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayR = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayLT = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayRT = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayTT = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayLTT = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            m_rWindRayRTT = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            
            rays[0] = m_rWindRay;
            rays[1] = m_rWindRayT;
            rays[2] = m_rWindRayB;
            rays[3] = m_rWindRayL;
            rays[4] = m_rWindRayR;
            rays[5] = m_rWindRayLT;
            rays[6] = m_rWindRayRT;
            rays[7] = m_rWindRayTT;
            rays[8] = m_rWindRayLTT;
            rays[9] = m_rWindRayRTT;
            //System.Diagnostics.Debug.WriteLine("Built initial rays");
        }

        public void BuildRay()
        {
            if (this.CurrentAngle == MathHelper.PiOver2 || this.CurrentAngle == MathHelper.PiOver2 * 3)
            {
                //System.Diagnostics.Debug.WriteLine("FOUND IT");
                rays[0] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[1] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[2] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.2f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[3] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z - .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[4] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z + .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[5] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z - .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[6] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z + .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[7] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[8] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z - .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[9] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z - .2f), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            }
            else
            {
                rays[0] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[1] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[2] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.2f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[3] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[4] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.4f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[5] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[6] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.6f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[7] = new Ray(new Vector3(this.WorldPosition.X, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[8] = new Ray(new Vector3(this.WorldPosition.X - .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
                rays[9] = new Ray(new Vector3(this.WorldPosition.X + .2f, this.WorldPosition.Y + 0.8f, this.WorldPosition.Z), Vector3.Normalize(new Vector3(-(this.WorldForward.X), this.WorldForward.Y, -(this.WorldForward.Z))));
            }

        
        }

        protected override void LoadContent()
        {

           // effect = Game.Content.Load<Effect>("Toon");
           // texture = Game.Content.Load<Texture2D>("PaperTexture");
           base.LoadContent();

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (CurrentAngle == MathHelper.Pi * 2)
                CurrentAngle = 0;
            
            WorldRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.05f);
            UpdateWind();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            actorModel.CopyAbsoluteBoneTransformsTo(actorBoneTransforms);
            foreach (ModelMesh mesh in actorModel.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.World = actorBoneTransforms[mesh.ParentBone.Index] * mWorldTransform;
                    basicEffect.View = Camera.View;
                    //System.Diagnostics.Debug.WriteLine("Actor view: " + effect.View);
                    basicEffect.Projection = Camera.Projection;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.PreferPerPixelLighting = true;

                   // effect.AmbientLightColor = ambientLightColor;
                    //effect.SpecularColor = specularColor;
                    //effect.SpecularPower = specularPower;
                    // effect.DirectionalLight0.Direction = lightDirection;
                    // effect.DirectionalLight0.DiffuseColor = lightDiffuseColor;

     
                }
                mesh.Draw();
            }
            //DrawModelWithEffect(model, world, view, projection);
            base.Draw(gameTime);
        }

        /*public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            actorModel.CopyAbsoluteBoneTransformsTo(actorBoneTransforms);
            DrawModelWithEffect(model, world, view, projection);
            base.Draw(gameTime);
        }*/

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
                    System.Diagnostics.Debug.WriteLine("Hey");
                } mesh.Draw();

            }
        }

        private void UpdateWind()
        {
            // This is trivial: we just create one wind particle per frame.
            Vector3 shiftedUpPosition = Vector3.Add(this.GetWorldPosition(), new Vector3(0, 0.0f, 0));
            Vector3 shiftedUpAndForwardPosition = Vector3.Add(shiftedUpPosition, Vector3.Multiply(FanDirection, 0.1f));
            wind.AddParticle(shiftedUpAndForwardPosition, Vector3.Zero);
        }


        /// <summary>
        /// Use this to set the fan's power
        /// </summary>
        public float FanPower
        {
            get { return m_fFanPower; }
            set { m_fFanPower = value; }
        }

        public Vector3 FanDirection
        {
            get { return rays[0].Direction; }
        }

        public override float CurrentAngle
        {
            get { return base.CurrentAngle; }
            set 
            { 
                base.CurrentAngle = value;
                Quaternion rotHack = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.PiOver4);
                wind = new WindParticleSystem(game, game.Content, Vector3.Transform(FanDirection, rotHack));
            }
        }

        public Ray WindRay
        {
            get { return m_rWindRay; }
            set { m_rWindRay = value; } 
        }

        public bool ForceApplied
        {
            get { return m_bForceApplied; }
            set { m_bForceApplied = value; }
        }

        public WindParticleSystem Wind
        {
            get { return wind; }
            set { wind = value; }
        }
    }
}

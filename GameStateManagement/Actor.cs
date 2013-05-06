#region File Description
//-----------------------------------------------------------------------------
// Actor.cs
//
// Base class for game objects 
//-----------------------------------------------------------------------------
#endregion

#region Using statements
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
#endregion


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Actor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected Model actorModel;
        public Microsoft.Xna.Framework.Matrix mWorldTransform;
        private float _worldScale;
        private Vector3 _worldPosition;
        private Quaternion _worldRotation;
        public float _currentAngle = 0;
        private Vector3 _velocity;

        protected CameraComponent camera;

        protected string sActorMeshName;
        protected Utils.Timer actorTimer;
        protected Matrix[] actorBoneTransforms;

        //Verlet velocity
        protected float fMass;
        protected float fTerminalVelocity;
        protected Vector3 vForce;
        protected Vector3 vAcceleration;
        protected bool bPhysicsDriven;

        //Collision detection
        BoundingSphere ModelBounds;
        BoundingSphere WorldBounds;
        BoundingBox WorldBoxBounds;

        //For shader
        protected Model model;
        protected Effect effect;

        protected Matrix world = Matrix.CreateTranslation(0, 0, 0);
        protected Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        protected Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        protected float angle = 0;
        protected float distance = 10;


        protected Vector3 viewVector;  // for specualr lighting

        protected Texture2D texture;

        public static Vector3 ambientLightColor;
        public static Vector3 specularColor;
        public static float specularPower;
        public static Vector3 lightDirection;
        public static Vector3 lightDiffuseColor;
        

        public Actor(Game game)
            : base(game)
        {
            actorTimer = new Utils.Timer();
            mWorldTransform = Matrix.Identity;

            _worldScale = 1.0f;
            _worldPosition = Vector3.Zero;
            _worldRotation = Quaternion.Identity;
            _velocity = Vector3.Zero;

            fMass = 1;
            fTerminalVelocity = 0;
            vForce = Vector3.Zero;
            vAcceleration = Vector3.Zero;
            bPhysicsDriven = false;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            actorTimer.Update(gameTime);

            if (bPhysicsDriven)
            {
                float fDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;
                _velocity += vAcceleration * (fDelta / 2.0f);
                checkTerminalVelocity();
                _worldPosition += _velocity * fDelta;
                //checkWorldPositionValid(fDelta);
                vAcceleration = vForce / fMass;
                _velocity += vAcceleration * fDelta / 2.0f;
                checkTerminalVelocity();
                CalculateWorldTransform();
            }

            

            base.Update(gameTime);
        }

        /// <summary>
        /// Make sure that actor velocity can't go above terminal
        /// </summary>
        protected void checkTerminalVelocity()
        {
            
            if (_velocity.Length() > fTerminalVelocity)
            {
                _velocity = Vector3.Multiply(Vector3.Normalize(_velocity), fTerminalVelocity);
            }
        }

        protected override void LoadContent()
        {
            actorModel = Game.Content.Load<Model>(sActorMeshName);
            actorBoneTransforms = new Matrix[actorModel.Bones.Count];
    
            foreach (ModelMesh mesh in actorModel.Meshes)
            {
                ModelBounds = BoundingSphere.CreateMerged(ModelBounds, mesh.BoundingSphere);
            }
            
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }

        public Vector3 GetWorldFacing()
        {
            return mWorldTransform.Forward;
        }

        public Vector3 WorldForward
        {
            get { return mWorldTransform.Forward; }
            set
            {
                if (mWorldTransform.Forward != value)
                {
                    mWorldTransform.Forward = value;
                    CalculateWorldTransform();
                }
            }
        }

        public Vector3 Force
        {
            get { return vForce; }
            set { vForce = value; }
        }

        public Vector3 Acceleration
        {
            get { return vAcceleration; }
            set { vAcceleration = value; }
        }

        public Vector3 GetWorldPosition()
        {
            return mWorldTransform.Translation;
        }

        public void SetActorMeshName(String name)
        {
            this.sActorMeshName = name;
        }

        public float WorldScale
        {
            get { return _worldScale; }
            set
            {
                if (_worldScale != value)
                {
                    _worldScale = value;
                    CalculateWorldTransform();
                }
            }
        }

        public BoundingBox WorldBoundingBox
        {
            get { return WorldBoxBounds; }
        }

        public BoundingSphere WorldBoundingSphere
        {
            get { return WorldBounds; }
        }

        public Vector3 WorldPosition
        {
            get { return _worldPosition; }
            set
            {
                if (_worldPosition != value)
                {
                    _worldPosition = value;
                    CalculateWorldTransform();
                    if (sActorMeshName == "Coin")
                    {
                        System.Diagnostics.Debug.WriteLine("CHANGING COIN POSITION: " + value);
                    }
                }
            }
        }

        public Quaternion WorldRotation
        {
            get { return _worldRotation; }
            set
            {

                if (_worldRotation != value)
                {
                    _worldRotation = value;
                    CalculateWorldTransform();
                }
            }
        }

        public virtual float CurrentAngle
        {
            get { return _currentAngle; }
            set { _currentAngle = value; }
        }

        public Vector3 Velocity
        {
            get { return _velocity; }
            set {_velocity = value; }
        }

        public CameraComponent Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public void CalculateWorldTransform()
        {
            WorldBounds.Center = WorldPosition;
            WorldBounds.Radius = ModelBounds.Radius * WorldScale;
            mWorldTransform = Matrix.Multiply(Matrix.Multiply(Matrix.CreateScale(_worldScale), Matrix.CreateFromQuaternion(_worldRotation)), Matrix.CreateTranslation(_worldPosition));
            //mWorldTransform = Matrix.CreateScale(_worldScale) * Matrix.CreateFromQuaternion(_worldRotation) * Matrix.CreateTranslation(_worldPosition); 
        }

       
    }

}

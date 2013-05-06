using System;
using System.Collections;
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
    public class CameraComponent : Microsoft.Xna.Framework.GameComponent
    {
        // default follow values
        private const float DIST_FROM_TARGET = 2.8f;
        private const float DIST_ABOVE_TARGET = 0.1f;
        private static Vector3 FOLLOW_CAM_OFFSET = new Vector3(0, DIST_ABOVE_TARGET, DIST_FROM_TARGET);
        private static Vector3 AIRPLANE_START_POS = new Vector3(8.0f, 1.0f, -3.0f);
        // city angle offset
        private Vector3 CITY_ANGLE_OFFSET = new Vector3(8.0f, 1.0f, -3.0f);

        // View & Projection matrices
        private static Matrix view;
        private static Matrix proj;

        // Rotation angle increment
        private float angle_increment = 1.5f*MathHelper.PiOver4;
        // Move speed
        private float moveSpeed = 6.0f;

        // Direction the camera points without rotation.
        private static Vector3 cameraReference;
        private static Vector3 cameraPosition;
        private static Vector3 cameraLookat;
        private static Matrix cameraRotation;
        // Direction the camera is currently pointing
        private static Vector3 transformedReference;
        private static Vector3 transformedUpVector;
        private static Vector3 transformedRotateAbout;

        // View angle of the camera
        private float viewAngle = MathHelper.PiOver4;

        // Distance from the camera of the near and far clipping planes.
        static float nearClip = 1.0f;
        static float farClip = 1000.0f;

        // viewport, aspectRatio
        Viewport viewport;
        float aspectRatio;

        // the airplane - for follow cam
        Airplane airplane;
        Boolean launching = false;
        Boolean freeRoam = false;

        // list of wind systems
        private ArrayList windList = new ArrayList();


        public CameraComponent(Game game)
            : base(game)
        {
            viewport = Game.GraphicsDevice.Viewport;
            aspectRatio = (float)viewport.Width / (float)viewport.Height;

            cameraLookat = new Vector3(8.0f, 1.0f, -3.0f);
            cameraPosition = Vector3.Add(cameraLookat, FOLLOW_CAM_OFFSET);
            cameraRotation = Matrix.Identity;

            CalculateMatrices(cameraPosition, cameraLookat, Vector3.UnitY);

            cameraReference = -Vector3.UnitZ;
            transformedReference = -Vector3.UnitZ;
            transformedUpVector = Vector3.UnitY;
            transformedRotateAbout = Vector3.UnitX;

            //System.Diagnostics.Debug.WriteLine("W/O plane. cameraLookat: " + cameraLookat + ", cameraPosition: " + cameraPosition);
        }

        public CameraComponent(Game game, Airplane plane)
            : base(game)
        {
            airplane = plane;
            viewport = Game.GraphicsDevice.Viewport;
            aspectRatio = (float)viewport.Width / (float)viewport.Height;            
            //cameraPosition = new Vector3(8.0f, 1.0f, -1.5f);
            //cameraLookat = Vector3.Add(airplane.WorldPosition, CITY_ANGLE_OFFSET);
            AIRPLANE_START_POS = airplane.WorldPosition;
            cameraLookat = airplane.WorldPosition;
            cameraPosition = Vector3.Add(cameraLookat, FOLLOW_CAM_OFFSET);
            cameraRotation = Matrix.Identity;

            CalculateMatrices(cameraPosition, cameraLookat, Vector3.UnitY);

            cameraReference = -Vector3.UnitZ;
            transformedReference = -Vector3.UnitZ;
            transformedUpVector = Vector3.UnitY;
            transformedRotateAbout = Vector3.UnitX;

            //System.Diagnostics.Debug.WriteLine("With plane. cameraLookat: " + cameraLookat + ", cameraPosition: " + cameraPosition + ", planePos: " + airplane.WorldPosition);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (launching)
            {
                Vector3 planePos = airplane.WorldPosition;
                Vector3 dif = planePos - cameraLookat;

                cameraLookat = planePos;
                cameraPosition += dif;

                CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);
            }

            foreach (WindParticleSystem wind in windList)
            {
                wind.SetCamera(view, proj);
            }
            base.Update(gameTime);
        }

        // ********** FREE ROAM MOVEMENT/ROTATION ********** //

        public void RotateYRightFromCamera(float yaw)
        {
            cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Up, -yaw);
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            /*Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedUpVector, -yaw);
            transformedReference = Vector3.Transform(transformedReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);
            transformedRotateAbout = Vector3.Transform(transformedRotateAbout, rotationQuaternion);
            cameraLookat = cameraPosition + transformedReference;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }
        public void RotateYLeftFromCamera(float yaw)
        {
            cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Up, yaw);
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            /*Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedUpVector, yaw);
            transformedReference = Vector3.Transform(transformedReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);
            transformedRotateAbout = Vector3.Transform(transformedRotateAbout, rotationQuaternion);
            cameraLookat = cameraPosition + transformedReference;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }
        public void RotateUpFromCamera(float pitch)
        {
            cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Right, pitch);
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            /*Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedRotateAbout, pitch);
            transformedReference = Vector3.Transform(transformedReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);
            transformedRotateAbout = Vector3.Transform(transformedRotateAbout, rotationQuaternion);
            cameraLookat = cameraPosition + transformedReference;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }
        public void RotateDownFromCamera(float pitch)
        {
            cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Right, -pitch);
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            /*Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedRotateAbout, -pitch);
            transformedReference = Vector3.Transform(transformedReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);
            transformedRotateAbout = Vector3.Transform(transformedRotateAbout, rotationQuaternion);
            cameraLookat = cameraPosition + transformedReference;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }
        public void RotateRollFromCamera(float roll)
        {
            cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Forward, roll);
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
        }
        public void MoveForward(float d)
        {
            cameraPosition += cameraRotation.Forward * d;
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            System.Diagnostics.Debug.WriteLine("cam move forward: " + d);
            /*cameraPosition += transformedReference * d;
            cameraLookat += transformedReference * d;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }
        public void MoveBackward(float d)
        {
            cameraPosition -= cameraRotation.Forward * d;
            cameraLookat = cameraPosition + cameraRotation.Forward;
            CalculateMatrices(cameraPosition, cameraLookat, cameraRotation.Up);
            /*cameraPosition -= transformedReference * d;
            cameraLookat -= transformedReference * d;
            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);*/
        }


        // ********** ROTATIONS AROUND THE PLANE ********** //

        /** 
         * z and x values change
         * camera rotates around the plane
         **/
        public void RotateYRightAroundPlane(float yaw)
        {
            Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw);

            // vector pointing the direction the camera is facing.
            transformedReference = -Vector3.Transform(-transformedReference, rotationQuaternion);
            transformedUpVector = -Vector3.Transform(-transformedUpVector, rotationQuaternion);
            transformedRotateAbout = -Vector3.Transform(-transformedRotateAbout, rotationQuaternion);

            // position the camera is looking from.
            cameraPosition = Vector3.Transform(cameraPosition - cameraLookat, rotationQuaternion) + cameraLookat;

            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);
        }

        /** 
         * z and x values change
         * camera rotates around the plane
         **/
        public void RotateYLeftAroundPlane(float yaw)
        {
            Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -yaw);

            // vector pointing the direction the camera is facing.
            transformedReference = -Vector3.Transform(-transformedReference, rotationQuaternion);
            transformedUpVector = -Vector3.Transform(-transformedUpVector, rotationQuaternion);
            transformedRotateAbout = -Vector3.Transform(-transformedRotateAbout, rotationQuaternion);

            // position the camera is looking at.
            cameraPosition = Vector3.Transform(cameraPosition - cameraLookat, rotationQuaternion) + cameraLookat;

            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);
        }

        
        /** 
         * z and y values change
         * camera rotates around the plane
         * (up arrow)
         **/
        public void RotateXCCWAroundPlane(float pitch)
        {
            Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedRotateAbout, -pitch);

            // vector pointing the direction the camera is facing.
            transformedReference = -Vector3.Transform(-cameraReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);

            // position the camera is looking at.
            cameraPosition = Vector3.Transform(cameraPosition - cameraLookat, rotationQuaternion) + cameraLookat;

            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);
        }

        /** 
         * z and y values change
         * camera rotates around the plane
         * (down arrow)
         **/
        public void RotateXCWAroundPlane(float pitch)
        {
            Quaternion rotationQuaternion = Quaternion.CreateFromAxisAngle(transformedRotateAbout, pitch);

            // vector pointing the direction the camera is facing.
            transformedReference = -Vector3.Transform(-cameraReference, rotationQuaternion);
            transformedUpVector = Vector3.Transform(transformedUpVector, rotationQuaternion);

            // position the camera is looking at.
            cameraPosition = Vector3.Transform(cameraPosition - cameraLookat, rotationQuaternion) + cameraLookat;

            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);
        }

        public void Reset()
        {
            cameraRotation = Matrix.Identity;
            cameraLookat = AIRPLANE_START_POS;
            //cameraLookat = new Vector3(8.0f, 1.0f, -3.0f);
            cameraPosition = Vector3.Add(AIRPLANE_START_POS, FOLLOW_CAM_OFFSET);
            transformedUpVector = Vector3.UnitY;
            transformedRotateAbout = Vector3.UnitX;
            transformedReference = -Vector3.UnitZ;
            freeRoam = false;
            launching = false;

            CalculateMatrices(cameraPosition, cameraLookat, transformedUpVector);

            cameraReference = -Vector3.UnitZ;
            transformedReference = -Vector3.UnitZ;
        }

        public void addWindComponent(WindParticleSystem wind)
        {
            windList.Add(wind);
        }

        public void removeWindComponent(WindParticleSystem wind)
        {
            windList.Remove(wind);
            wind.Dispose();
        }
        // ***** HELPER METHODS ***** //

        private void CalculateMatrices(Vector3 camPos, Vector3 camLookat, Vector3 upVector)
        {
            // view matrix and projection matrix.
            view = Matrix.CreateLookAt(cameraPosition, cameraLookat, upVector);
            proj = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);
            cameraRotation.Forward.Normalize();
            cameraRotation.Up.Normalize();
            cameraRotation.Right.Normalize();
        }

        /**
         * Creates a rotation quaternion using the angle between a source and target vector.
         * 
         **/
        private Quaternion CreateRotationQuaternion(Vector3 start, Vector3 end)
        {
            Vector3 planeFacing = airplane.WorldForward;
            float dotProduct = Vector3.Dot(-Vector3.UnitZ, planeFacing);
            float angle = (float)Math.Acos(dotProduct);
            Vector3 crossProduct = Vector3.Cross(-Vector3.UnitZ, planeFacing);
            crossProduct.Normalize();
            return Quaternion.CreateFromAxisAngle(crossProduct, angle);
        }

        /***** ACCESSORS & MUTATORS *****/

        public Matrix View
        {
            get { return CameraComponent.view; }
            set { CameraComponent.view = value; }
        }
        public Matrix Projection
        {
            get { return CameraComponent.proj; }
            set { CameraComponent.proj = value; }
        }

        public float Angle_increment
        {
            get { return angle_increment; }
            set { angle_increment = value; }
        }

        public Vector3 CameraPosition
        {
            get { return CameraComponent.cameraPosition; }
            set { CameraComponent.cameraPosition = value; }
        }
        public Boolean Launching
        {
            get { return launching; }
            set { launching = value; }
        }
        public Airplane Airplane
        {
            get { return airplane; }
            set { airplane = value; }
        }
        public Boolean FreeRoam
        {
            get { return freeRoam; }
            set 
            { 
                freeRoam = value;
                if (!freeRoam)
                {
                    Reset();
                }
            }
        }
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }
    }
}

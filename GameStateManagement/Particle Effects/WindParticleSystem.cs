#region File Description
//-----------------------------------------------------------------------------
// WindParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting wind.
    /// </summary>
    public class WindParticleSystem : ParticleSystem
    {
        private Vector3 gravity;
        public WindParticleSystem(Game game, ContentManager content, Vector3 gravity)
            : base(game, content)
        {
            this.gravity = gravity;
        }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "wind";

            settings.MaxParticles = 10000;

            settings.Duration = TimeSpan.FromSeconds(.5f);

            //settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 1f;
            settings.MaxHorizontalVelocity = 1.1f;

            settings.MinVerticalVelocity = -1f;
            settings.MaxVerticalVelocity = 1.1f;

            settings.Gravity = Vector3.Multiply(gravity, 5);
            //settings.Gravity = 40 * Vector3.UnitY;

            settings.EndVelocity = .1f;

            Color WhiteTransparent = Color.White * 0.25f;
            Color LightCyanTransparent = Color.LightCyan * 1f;
            settings.MinColor = WhiteTransparent;
            settings.MinColor = LightCyanTransparent;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = .2f;
            settings.MaxStartSize = .5f;

            settings.MinEndSize = .2f;
            settings.MaxEndSize = .5f;

            settings.BlendState = BlendState.AlphaBlend;
        }

    }
}

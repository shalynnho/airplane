#region File Description
//-----------------------------------------------------------------------------
// FrameRateCounter.cs
//
// Displays the average/high/low FPS counter in the bottom right corner of the 
// screen
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Utils
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private SpriteBatch m_kSpriteBatch;
        private SpriteFont m_kFont;

        private Vector2 m_vPosition;

        private Queue<float> m_kLastFrames = new Queue<float>();

        private const int m_iFrameRateWindowSize = 1000;

		private float m_fCurrentFrameRate;
        private float m_fAverageFrameRate;
        private float m_fLowestFrameRate;
        private float m_fHighestFrameRate;

        private float _fpsCounter = 0;
        private TimeSpan _elapsedGameTime = TimeSpan.Zero;
        
        public FrameRateCounter(Game game, Vector2 vPosition)
            : base(game)
        {
            m_vPosition = vPosition;
            m_fCurrentFrameRate = 0;
            m_fLowestFrameRate = 0;
            m_fHighestFrameRate = 0;
            ResetFPSCount();
            DrawOrder = 1000;
        }

        protected override void LoadContent()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            m_kSpriteBatch = new SpriteBatch(graphicsService.GraphicsDevice);
            m_kFont = Game.Content.Load<SpriteFont>("fpsfont");

            base.LoadContent();
        }
        
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        
        public override void Update(GameTime gameTime)
        {

            _elapsedGameTime += gameTime.ElapsedGameTime;
            if (_elapsedGameTime > TimeSpan.FromMilliseconds(1000))
            {
                _elapsedGameTime -= TimeSpan.FromMilliseconds(1000);
                m_fCurrentFrameRate = _fpsCounter;
                _fpsCounter = 0;
            }
            CalcAverageFrameRate();

			base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            //_fpsCounter++;

            m_kSpriteBatch.Begin();
            
			// Color this based on the framerate
            Color DrawColor = Color.Green;
			if (m_fCurrentFrameRate < 15.0f)
                DrawColor = Color.Red;
			else if (m_fCurrentFrameRate < 30.0f)
                DrawColor = Color.Yellow;

            m_kSpriteBatch.DrawString(m_kFont, "FPS Info: "
                + "\r\nCurrent: " + m_fCurrentFrameRate.ToString("f0")
                + "\r\nAverage: " + m_fAverageFrameRate.ToString("f0")
                + "\r\nHighest: " + m_fHighestFrameRate.ToString("f0")
                + "\r\nLowest: " + m_fLowestFrameRate.ToString("f0")
                /* + "\r\nWindow width: " + m_kLastFrames.Count */, m_vPosition, DrawColor);
            m_kSpriteBatch.End();

            base.Draw(gameTime);
        }

        public void CalcAverageFrameRate()
        {
            double TotalFPS = 0;

            while (m_kLastFrames.Count > m_iFrameRateWindowSize)
            {
                //Drop the oldest FPS
                m_kLastFrames.Dequeue();
            }

            m_kLastFrames.Enqueue(m_fCurrentFrameRate);

            //High is set to 0, low is set to current. Both should be overwritten in the foreach.
            m_fHighestFrameRate = 0;
            m_fLowestFrameRate = m_fCurrentFrameRate;

            foreach (float f in m_kLastFrames) 
            {

                if (f > m_fHighestFrameRate){
                    m_fHighestFrameRate = f;
                }

                if (f < m_fLowestFrameRate)
                {
                    m_fLowestFrameRate = f;
                }

                TotalFPS += f;
            }

            if (m_kLastFrames.Count > 0)
            {
                m_fAverageFrameRate = (float) TotalFPS / m_kLastFrames.Count;
            }

            else
            {
                //This should only happen at the beginning
                m_fAverageFrameRate = 0;
            }
        }

		public void ResetFPSCount()
		{
            m_kLastFrames.Clear();
		}

        public float FPSCounter { 
            get { return _fpsCounter; } 
            set { _fpsCounter = value; } 
        }
    }
}

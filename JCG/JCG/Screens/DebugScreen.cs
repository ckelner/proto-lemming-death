#region File Description
//-----------------------------------------------------------------------------
// DebugScreen.cs
//
// Author - Ckelner
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
#endregion

namespace GameStateManagement
{
    class DebugScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteBatch sB;
        SpriteFont sF;
        int fRate = 0;
        int fCount = 0;
        TimeSpan elapsed = TimeSpan.Zero;
        long memUse;
        int memOffSet = 200;
        int memCount = 201;
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        float cpuDiag = 0;
        float memDiag = 0;
        int cpuOffSet = 200;
        int cpuCount = 201;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public DebugScreen(String name)
        {
            HumanReadableName = name;
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
                sB = ScreenManager.SpriteBatch;
                sF = content.Load<SpriteFont>(@"debug");
            }
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// 
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            calcFrames(gameTime);
            updateCPU();
            updateMem();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the debug screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            fCount++;
            string f = string.Format("FPS: {0}", fRate);
            string r = string.Format("RAM WorkingSet: {0}mb", memUse);
            string rD = string.Format("RAM Sys Diag: {0}mb", memDiag);
            string c = string.Format("CPU Sys Diag: {0}%", cpuDiag);
            sB.Begin();
            sB.DrawString(sF, f, new Vector2(13, 3), Color.Black);
            sB.DrawString(sF, f, new Vector2(12, 2), Color.White);
            sB.DrawString(sF, r, new Vector2(13, 18), Color.Black);
            sB.DrawString(sF, r, new Vector2(12, 17), Color.White);
            sB.DrawString(sF, rD, new Vector2(13, 33), Color.Black);
            sB.DrawString(sF, rD, new Vector2(12, 32), Color.White);
            sB.DrawString(sF, c, new Vector2(13, 48), Color.Black);
            sB.DrawString(sF, c, new Vector2(12, 47), Color.White);
            sB.End();
        }
        #endregion

        #region Utility Functions
        private void calcFrames(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime;
            if (elapsed > TimeSpan.FromSeconds(1))
            {
                elapsed -= TimeSpan.FromSeconds(1);
                fRate = fCount;
                fCount = 0;
            }
        }

        private void getRAM()
        {
            memUse = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024;
            memDiag = ramCounter.NextValue();
        }

        private void getCPU()
        {
            cpuDiag = cpuCounter.NextValue();
        }

        private void getVmem()
        {
            // TODO: Difficult to find any way to do this so far... DX can, but seems silly
            // to use DX if we aren't using it for something else already
        }

        private void updateCPU()
        {
            if (cpuCount > cpuOffSet)
            {
                getCPU();
                cpuCount = 0;
            }
            else
            {
                cpuCount++;
            }
        }

        private void updateMem()
        {
            if (memCount > memOffSet)
            {
                getRAM();
                memCount = 0;
            }
            else
            {
                memCount++;
            }
        }
        #endregion
    }
}

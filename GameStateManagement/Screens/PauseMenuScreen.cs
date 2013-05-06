#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        GameScreen source;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(GameScreen g)
            : base("Paused")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            source = g;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry switchAirplaneMenuEntry = new MenuEntry("Switch airplanes");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            switchAirplaneMenuEntry.Selected += SwitchAirplaneMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(switchAirplaneMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }

        /// <summary>
        /// Switches the airplane between the dart and the glider
        /// </summary>
        void SwitchAirplaneMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen gs in ScreenManager.GetScreens()){
                if(gs.GetType().Equals(typeof(GameplayScreen))){
                    ((GameplayScreen)gs).swapAirplane();
                }
            }
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            
            //source.ExitScreen();
            //source.UnloadContent();
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(),
                                                     new MainMenuScreen());
            foreach (GameScreen gs in ScreenManager.GetScreens())
            {
                if (gs.GetType().Equals(typeof(GameplayScreen)))
                {
                    ScreenManager.RemoveScreen(gs);
                    ((GameplayScreen)gs).Dispose();
                    break;
                }
            }

            MediaPlayer.Stop();
            MediaPlayer.Play(ScreenManager.getMenuMusic());
            

        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            base.Draw(gameTime);
        }


        #endregion
    }
}

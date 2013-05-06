#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class LevelSelectScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public LevelSelectScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playLevelOneEntry = new MenuEntry("Play Level 1");
            MenuEntry playLevelTwoEntry = new MenuEntry("Play Level 2");
            MenuEntry playLevelThreeEntry = new MenuEntry("Play Level 3");
            MenuEntry playLevelFourEntry = new MenuEntry("Play Level 4");
            MenuEntry playLevelFiveEntry = new MenuEntry("Play Level 5");

            // Hook up menu event handlers.
            playLevelOneEntry.Selected += PlayLevelOneSelected;
            playLevelTwoEntry.Selected += PlayLevelTwoSelected;
            playLevelThreeEntry.Selected += PlayLevelThreeSelected;
            playLevelFourEntry.Selected += PlayLevelFourSelected;
            playLevelFiveEntry.Selected += PlayLevelFiveSelected;

            // Add entries to the menu.
            MenuEntries.Add(playLevelOneEntry);
            MenuEntries.Add(playLevelTwoEntry);
            MenuEntries.Add(playLevelThreeEntry);
            MenuEntries.Add(playLevelFourEntry);
            MenuEntries.Add(playLevelFiveEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayLevelOneSelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen("Content/LevelOneMap.txt", "Content/LevelOneData.txt", 1, false, 0));

        }

        void PlayLevelTwoSelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen("Content/LevelTwoMap.txt", "Content/LevelTwoData.txt", 2, false, 0));
        }

        void PlayLevelThreeSelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen("Content/LevelThreeMap.txt", "Content/LevelThreeData.txt", 3, false, 0));
        }

        void PlayLevelFourSelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen("Content/LevelFourMap.txt", "Content/LevelFourData.txt", 4, false, 0));
        }

        void PlayLevelFiveSelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen("Content/LevelFiveMap.txt", "Content/LevelFiveData.txt", 5, false, 0));
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}

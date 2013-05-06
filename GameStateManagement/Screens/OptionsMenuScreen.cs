#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry ungulateMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry frobnicateMenuEntry;
        MenuEntry elfMenuEntry;
        MenuEntry QWES;
        MenuEntry arrowKeys;
        MenuEntry WASD;
        MenuEntry spacebar;
        MenuEntry mouseLeftClick;
        MenuEntry mouseRightClick;
        MenuEntry others;

        enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            Llama,
        }

        static Ungulate currentUngulate = Ungulate.Dromedary;

        static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int currentLanguage = 0;

        static bool frobnicate = true;

        static int elf = 23;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Controls")
        {
            // Create our menu entries.
            ungulateMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);
            frobnicateMenuEntry = new MenuEntry(string.Empty);
            elfMenuEntry = new MenuEntry(string.Empty);
            QWES = new MenuEntry(string.Empty);
            WASD = new MenuEntry(string.Empty);
            arrowKeys = new MenuEntry(string.Empty);
            spacebar = new MenuEntry(string.Empty);
            mouseLeftClick = new MenuEntry(string.Empty);
            mouseRightClick = new MenuEntry(string.Empty);
            others = new MenuEntry(string.Empty);


            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            /*ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
            elfMenuEntry.Selected += ElfMenuEntrySelected;
           */ backMenuEntry.Selected += OnCancel;
            
            
            // Add entries to the menu.
           // MenuEntries.Add(ungulateMenuEntry);
           // MenuEntries.Add(languageMenuEntry);
           // MenuEntries.Add(frobnicateMenuEntry);
           // MenuEntries.Add(elfMenuEntry);

            
            MenuEntries.Add(mouseLeftClick);
            MenuEntries.Add(mouseRightClick);
            
            MenuEntries.Add(arrowKeys);
            MenuEntries.Add(WASD);
            MenuEntries.Add(spacebar);
            MenuEntries.Add(QWES);
            MenuEntries.Add(others);
            MenuEntries.Add(backMenuEntry);
            
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
           // ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
            //languageMenuEntry.Text = "Language: " + languages[currentLanguage];
            //frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            //elfMenuEntry.Text = "elf: " + elf;
            
            mouseLeftClick.Text = "Left Click : Place Fan, Rotate Fan";
            mouseRightClick.Text = "Right Click : Remove Fan";
            QWES.Text = "QWES : Move camera";
            arrowKeys.Text = "Arrow Keys :  Rotate Camera";
            WASD.Text = "WASD : Rotate Plane";
            spacebar.Text = "Spacebar : Launch plane";
            others.Text = "T : Free Roam, R : Retry";




        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
       /* void UngulateMenuEntrySelected(object sender, EventArgs e)
        {
            currentUngulate++;

            if (currentUngulate > Ungulate.Llama)
                currentUngulate = 0;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void LanguageMenuEntrySelected(object sender, EventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void FrobnicateMenuEntrySelected(object sender, EventArgs e)
        {
            frobnicate = !frobnicate;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void ElfMenuEntrySelected(object sender, EventArgs e)
        {
            elf++;

            SetMenuEntryText();
        }

        */
        #endregion
    }
}

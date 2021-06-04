using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PyroSquidUniLib;
using PyroSquidUniLib.Database;
using Serilog;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Application = System.Windows.Application;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using PyroSquidUniLib.Extensions;
using Button = System.Windows.Controls.Button;
using System.ComponentModel;

namespace Svendepr_ve_V9_2k21
{
    public partial class MainWindow
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<MainWindow>();

        /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
        public MainWindow()
        {
            InitializeComponent();

            VerifyLogsFolder();
            SetTitle();
        }

        #region Title
        /// <summary> Gets our AssemblyInformationalVersion to put the Semantic versioning into effect.</summary>
        private static string GetVersion()
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            }
            catch (Exception ex)
            {

                Log.Error(ex, "An error occured while fetching the version");
                return string.Empty;
            }
        }

        /// <summary>Applies our Version and changes the title of our program.</summary>        
        private void SetTitle()
        {
            try
            {
                Title = "Svendepr_ve_V9_2k21 V" + GetVersion();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured while setting the application title");
            }
        }
        #endregion

        #region Logging
        /// <summary>Verifies if the logs folder exists.</summary>
        private static void VerifyLogsFolder()
        {
            try
            {
                var folderPath = PropertiesExtension.Get<string>("LogsPath");

                switch (Directory.Exists(folderPath))
                {
                    case true:
                        return;

                    case false:
                        Directory.CreateDirectory(folderPath);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "The path could not be made or found");
            }
        }

        /// <summary>Handles the OnItemClick event of the HamburgerMenu control.</summary>
        /// <param name="sender">The source of the event.</param>,
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        private void Menu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // Set content
                Menu.Content = e.ClickedItem;
                // Close pane
                Menu.IsPaneOpen = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error using the menuitem with index \"{Index}\"", Menu.SelectedIndex);
            }
        }
        #endregion

        private void EULA(object sender, RoutedEventArgs e)
        {
            //https://drive.google.com/file/d/1vsE3ZIjDY9sEnh_tGcfKN-f3PC39U6LF/view?usp=sharing

            System.Diagnostics.Process.Start("https://drive.google.com/file/d/1vsE3ZIjDY9sEnh_tGcfKN-f3PC39U6LF/view?usp=sharing");
        }
    }
}

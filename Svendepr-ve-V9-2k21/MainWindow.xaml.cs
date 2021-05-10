using MahApps.Metro.Controls;
using System;

namespace Svendepr_ve_V9_2k21
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
                Console.WriteLine(ex + ": Error using the menuitem with index \"{Index}\"", Menu.SelectedIndex);
            }
        }
    }
}

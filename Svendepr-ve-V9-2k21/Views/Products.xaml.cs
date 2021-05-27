using System;
using System.Windows;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Products
    {
        public Products()
        {
            InitializeComponent();
        }

        private void ProductGrid_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenAddProductDialogButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductDialog.IsOpen = true;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearTextSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void CancelAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductDialog.IsOpen = false;
        }

        private void FinalAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductDialog.IsOpen = false;
        }

        private void ProductPriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}

using Svendepr_ve_V9_2k21;
using System;
using PyroSquidUniLib.Database;
using System.Diagnostics;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Customers
    {

        public Customers()
        {
            InitializeComponent();
        }

        private void ClearTextSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void CountySearch_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void MonthSearch_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ContainServicesToYearOnly_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ContainServicesToYearOnly_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ReloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void OpenAddServiceDialogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = true;
        }

        private void AddCustomerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddCustomerDialog.IsOpen = true;
        }

        private void DeleteCustomerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CustomerGrid_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void CustomerGrid_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {

        }

        private void CancelAddService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = false;
        }

        private void FinalAddService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = false;
        }

        private void FinalAddAndPrintStandartService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = false;
        }

        private void FinalAddAndPrintGiroService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = false;
        }

        private void ListViewPriceTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void RemoveProduct_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void AddProduct_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ContainPriceCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ContainPriceCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void PriceTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CancelAddCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddCustomerDialog.IsOpen = false;
        }

        private void FinalAddCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void NewCustomersServices_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void NewCustomersZipCode_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void NewCustomersCity_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void NewCustomersCity_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void NewCustomersZipCode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void NewCustomersZipCode_PreviewTextInput_1(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void NewCustomersZipCode_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Models;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Routes
    {
        private Route[] _routeData;
        private City[] _cityData;
        private Customer[] _customerData;
        private RouteCustomer[] _printRouteCustomerData;

        private int _routeSelectId;
        private readonly List<string[]> _customerDataList = new List<string[]>();
        // 0 = CustomerID
        // 1 = Services
        // 2 = Chimneys
        // 3 = Pipes
        // 4 = KW
        // 5 = Lightning
        // 6 = Height
        // 7 = Dia
        // 8 = Length
        // 9 = Type

        //Route Print
        private bool _didServiceDataChange;
        private bool _didServiceDataExist;
        private int _routeCustomerNum;
        private int _routeCustomerAmount;
        private int _routeSelected;

        //Month Print
        private readonly List<string[]> _monthPrintList = new List<string[]>();
        private int AreaAmount = 9;
        private int _areaNum = 1;
        private bool _isNewPage;

        public Routes()
        {
            InitializeComponent();
        }



        private void ClearTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            var cities = City.CreateFromJson(await ApiHelper.GetDataAsync("Cities"));
            _cityData = cities;
            CityGrid.ItemsSource = _cityData;
        }

        private void PrintMonthSheatButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintRouteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void DeleteCountyButton_Click(object sender, RoutedEventArgs e)
        {
            var cities = CityGrid.SelectedItems;

            if (cities != null)
            {
                try
                {
                    foreach (City city in cities)
                    {
                        var data = $"value={city.Postnr}";
                        await ApiHelper.RemoveDataAsync("City", data);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void DeleteRouteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRouteCustomerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddCountyButton_Click(object sender, RoutedEventArgs e)
        {
            AddCountyDialog.IsOpen = true;
        }

        private void CreateRouteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRouteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RouteGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void RouteGrid_CurrentCellChanged(object sender, System.EventArgs e)
        {

        }

        private async void CityGrid_CurrentCellChanged(object sender, System.EventArgs e)
        {
            try
            {
                var grid = sender as DataGrid;
                var city = grid.SelectedItem as City;

                if (city != null)
                {
                    var data = $"zip={city.Postnr}&city={city.By}";
                    await ApiHelper.EditDataAsync("Edit/City", data);
                }
            }
            catch (Exception)
            {

            }
        }

        private void CustomerGrid_CurrentCellChanged(object sender, System.EventArgs e)
        {

        }

        private async void FinalAddCounty_Click(object sender, RoutedEventArgs e)
        {
            var data = $"zip={NewCityZip.Text}&city={NewCityName.Text}";
            await ApiHelper.EditDataAsync("Add/City", data, "POST");

            AddCountyDialog.IsOpen = false;
        }

        private void CancelAddCounty_Click(object sender, RoutedEventArgs e)
        {
            AddCountyDialog.IsOpen = false;
        }

        private void ClearTextRouteCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RemoveCustomer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FinalAddRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelAddRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearTextEditRouteCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddEditCustomer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveEditCustomer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FinalEditRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelEditRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrevServiceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextServiceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CustomerServiceDataTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FinalPrintRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrevPrintRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextPrintRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EmptyPrintRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelPrintRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AreaMonthTextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FinalPrintSheat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrevPrintSheat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextPrintSheat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelPrintSheat_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

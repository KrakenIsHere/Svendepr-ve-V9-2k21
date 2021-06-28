using Svendepr_ve_V9_2k21;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Documents;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.WPFControls;
using PyroSquidUniLib.Models;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Routes
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Routes>();

        private Route[] _routeData;
        private City[] _cityData;
        private RouteCustomer[] _routeCustomerData;
        private DataTable _printRouteCustomerData = new DataTable();

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

            SetRouteData();
            SetCityData();
            AddCustomersToDataGrid();
            AddMonthsToCombobox();
        }

        #region Customers List

        /// <summary>Adds the products from the database to the ProductComboBox.</summary>
        private async void AddCustomersToDataGrid()
        {
            try
            {
                var input = ClearTextRouteCustomerSearch.Text;

                var customers = Customer.CreateFromJson(await ApiHelper.GetDataAsync("Customers"));

                switch (!string.IsNullOrWhiteSpace(input))
                {
                    case true:
                        {
                            customers = customers.Where(x => 
                                x.Fornavn.Contains(input) ||
                                x.Efternavn.Contains(input) ||
                                x.ID.ToString().Contains(input) ||
                                x.Adresse.Contains(input) ||
                                x.Fejninger.ToString().Contains(input)).ToArray();
                            break;
                        }
                }

                var listData = VariableManipulation.GetIntegerValuesInColumnFromListView<Customer>(CustomerList);

                customers = customers.Where(x => !listData.Contains(x.ID)).ToArray();

                AddCustomerGrid.ItemsSource = customers;

                Log.Information($"Successfully filled AddCustomerGrid ItemSource");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while adding a new customer to listview");
            }
        }

        private async void FinalAddRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (!string.IsNullOrWhiteSpace(RouteAreaTextBox.Text) &&
                        CustomerList.Items.Count != 0 &&
                        CustomerList.Items.Count != -1)
                {
                    case true:
                        {
                            switch (AddNewRouteData().Result)
                            {
                                case true:
                                    {
                                        AddRouteDialog.IsOpen = false;
                                        SetRouteData();
                                        break;
                                    }
                            }
                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Data mangler");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void CancelAddRoute_Click(object sender, RoutedEventArgs e)
        {
            AddRouteDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        private async void CreateRouteButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAddRouteDialog();

            AddRouteDialog.IsOpen = true;

            await Task.FromResult(true);
        }

        private async void ClearTextRouteCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddCustomersToDataGrid();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the AddProduct control.
        /// </para>
        ///   <para>Adds a product to a list for a new service</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = AddCustomerGrid.SelectedItem as Customer;
                
                CustomerList.Items.Add(row);

                AddCustomersToDataGrid();
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(true);
                Log.Error(ex, "Unexpected Error");
            }
        }


        /// <summary>
        ///   <para>
        ///  Handles the Click event of the RemoveProduct control.
        /// </para>
        ///   <para>Removes a selected product from a list for a new service</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void RemoveCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerList.Items.RemoveAt(CustomerList.SelectedIndex);

                AddCustomersToDataGrid();
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Route

        #region Standard

        /// <summary>Adds the new route data.</summary>
        private async Task<bool> AddNewRouteData()
        {
            try
            {
                var rData = $"" +
                    $"Name={RouteAreaTextBox.Text}&" +
                    $"Description=&" +
                    $"Month={RouteMonthComboBox.Text}&" +
                    $"Year={RouteYearTextBox.Text}&";

                switch (!await ApiHelper.PutDataAsync("Route", rData))
                {
                    case true:
                        {
                            throw new Exception("Unable to post new Route to database under at AddNewRouteData() Routes.xaml.cs");
                        }
                }

                //Gets all saved routes from a database
                var routes = Route.CreateFromJson(await ApiHelper.GetDataAsync("Routes"));

                //Finds the last route that was just created and gets it's ID
                var id = routes.LastOrDefault().ID.ToString();

                var rows = CustomerList.Items;

                var i = 0;
                var orderNum = 0;
                foreach (var item in rows)
                {
                    orderNum++;
                    var data = (RouteCustomer)CustomerList.Items[i];

                    Random random = new Random();

                    var rcData = $"" +
                        $"RouteID={data.ID}&" +
                        $"CustomerID={id}&" +
                        $"CityID={random.Next(1, 251)}";

                    await ApiHelper.PostDataAsync("RouteCustomer", rcData);

                    i++;
                }
                Log.Information($"Successfully created a new route: #{id} {RouteAreaTextBox.Text} with {i} customers");
                return true;
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
                MessageBox.Show("Rute kunne ikke tilføjes", "FEJL");
            }
            return false;
        }

        /// <summary>Sets route data to RouteGrid.</summary>
        private async void SetRouteData()
        {
            try
            {
                Debug.WriteLine(await ApiHelper.GetDataAsync("Routes"));

                var routes = Route.CreateFromJson(await ApiHelper.GetDataAsync("Routes"));
                _routeData = routes;
                RouteGrid.ItemsSource = _routeData;

                CustomerGrid.ItemsSource = null;

                Log.Information($"Successfully filled RouteGrid ItemSource");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Warning(ex, "Something went wrong setting the data for the RouteGrid");
            }
        }

        /// <summary>Updates Route data to the database.</summary>
        private async void UpdateRouteData()
        {
            try
            {
                try
                {
                    var route = RouteGrid.SelectedItem as Route;

                    if (route != null)
                    {
                        var data = $"" +
                            $"id={route.ID}&" +
                            $"Name={route.Navn}&" +
                            $"Description={route.Beskrivelse}&" +
                            $"Month={route.Maaned}&" +
                            $"Year={route.Aar}";
                        await ApiHelper.PutDataAsync("Route", data);
                    }
                }
                catch (Exception)
                {

                }
                await Task.FromResult(true);
                Log.Information($"Successfully updated route data");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "an error occured while upding city data");
            }
        }

        /// <summary>Deletes the selected route from the RouteGrid.</summary>
        /// <param name="row">The row.</param>
        private async void DeleteRoutes()
        {
            try
            {
                var routes = RouteGrid.SelectedItems;
                string message = $"Er du sikker du vil slette {routes.Count} ruter?";
                string caption = "Advarsel";
                MessageBoxButton buttons = MessageBoxButton.YesNo;

                // Displays the MessageBox.
                var result = MessageBox.Show(message, caption, buttons);

                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            foreach (Route row in routes)
                            {
                                var routeCustomers = _routeCustomerData.Where(x => x.RuteID == row.ID);
                                foreach(RouteCustomer customer in routeCustomers)
                                {
                                    var RCD = $"id={customer.ID}";
                                    if (await ApiHelper.DeleteDataAsync("RouteCustomer", RCD))
                                    {
                                        Log.Information($"Successfully deleted RouteCustomer #{row.ID}");
                                    }
                                }

                                var data = $"id={row.ID}";
                                if (await ApiHelper.DeleteDataAsync("Route", data))
                                {
                                    Log.Information($"Successfully deleted route #{row.ID}");
                                }
                            }

                            MessageBox.Show($"{routes.Count} Ruter blev slettet");
                            break;
                        }
                }

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                MessageBox.Show("En uventet fejl er sket", "FEJL");
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the SelectedCellsChanged event of the RouteGrid control.
        /// </para>
        ///   <para>Sets product data in ProductGrid accroding to the selected cell's row</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectedCellsChangedEventArgs"/> instance containing the event data.</param>
        private async void RouteGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SetCustomerData();
            DisableCollumnsForRoutesGrid();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the CurrentCellChanged event of the RouteGrid control.
        /// </para>
        ///   <para>Updates route data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RouteGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            UpdateRouteData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the DeleteRouteButton control.
        /// </para>
        ///   <para>Deletes the selected route from the RouteGrid</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void DeleteRouteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (RouteGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            DeleteRoutes();
                            SetRouteData();

                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en rute");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void ResetAddRouteDialog()
        {
            try
            {
                CustomerList.Items.Clear();
                RouteAreaTextBox.Text = "";
                RouteMonthComboBox.SelectedIndex = -1;

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Print Route

        private async Task<string[]> DataInput()
        {
            try
            {
                string[] routeCustomerData = new string[12];

                routeCustomerData[0] = CustomerIDTextBox.Text;
                routeCustomerData[1] = CustomerServiceNumTextBox.Text;
                routeCustomerData[2] = CustomerChimneysTextBox.Text;
                routeCustomerData[3] = CustomerPipesTextBox.Text;
                routeCustomerData[4] = CustomerKWTextBox.Text;
                routeCustomerData[5] = CustomerLightingTextBox.Text;
                routeCustomerData[6] = CustomerHeightTextBox.Text;
                routeCustomerData[7] = CustomerDiaTextBox.Text;
                routeCustomerData[8] = CustomerLengthTextBox.Text;
                routeCustomerData[9] = CustomerTypeTextBox.Text;
                routeCustomerData[10] = CustomerCommentTextBox.Text;
                routeCustomerData[11] = ContainCommentCheck.IsChecked.ToString();         

                await Task.FromResult(true);
                return routeCustomerData;
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
            return null;
        }

        private async void ResetPrintRouteDialog(bool buttonClick = false)
        {
            switch (!buttonClick)
            {
                case true:
                    {
                        _didServiceDataExist = false;
                        _didServiceDataChange = false;

                        //Customer
                        CustomerIDTextBox.Text = "";
                        CustomerNameTextBox.Text = "";
                        CustomerAddressTextBox.Text = "";
                        CustomerZipCodeTextBox.Text = "";
                        CustomerCountyTextBox.Text = "";
                        CustomerServicesTextBox.Text = "";
                        CustomerServiceNumTextBox.Text = "";

                        CustomerCommentTextBox.Text = "";
                        CustomerCommentTextBox.IsEnabled = false;
                        ContainCommentCheck.IsChecked = false;
                        ContainCommentCheck.IsEnabled = false;
                        break;
                    }
            }

            //Service
            //Amount
            CustomerChimneysTextBox.Text = "";
            CustomerPipesTextBox.Text = "";
            CustomerKWTextBox.Text = "";

            CustomerLightingTextBox.Text = "";
            CustomerHeightTextBox.Text = "";

            //Pipe
            CustomerDiaTextBox.Text = "";
            CustomerLengthTextBox.Text = "";

            CustomerTypeTextBox.Text = "";

            await Task.FromResult(true);
        }

        private async void SetupPrintRouteDialog(DataTable table, bool isNext = true)
        {
            try
            {
                switch (isNext)
                {
                    case true:
                        {
                            _routeCustomerNum++;
                            break;
                        }
                    case false:
                        {
                            _routeCustomerNum--;
                            break;
                        }
                }

                int year;

                switch (DateTime.Now.Month == 12 && DateTime.Now.Day > 20)
                {
                    case true:
                        {
                            year = DateTime.Now.Year + 1;
                            break;
                        }
                    default:
                        {
                            year = DateTime.Now.Year;
                            break;
                        }
                }

                var query = $"SELECT * FROM `user_services` " +
                    $"WHERE `Kunde ID` = {table.Rows[_routeCustomerNum - 1]["Kunde ID"]} AND `Aar` = {year}";

                int serviceNum;

                try
                {
                    var data = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services"));

                    serviceNum = data.Length;

                    //if (serviceNum == null || serviceNum == 0)
                    //{
                    //    serviceNum = 0;
                    //}

                    Log.Information($"Amount of services: {serviceNum}");
                    Log.Information($"Query: {query}");

                }
                catch (Exception)
                {
                    serviceNum = 0;
                }

                var comment = table.Rows[_routeCustomerNum - 1]["Kommentar"].ToString();

                var serviceNeeded = table.Rows[_routeCustomerNum - 1]["Fejninger"].ToString();

                try
                {
                    var _ = int.Parse(serviceNeeded);
                }
                catch (Exception)
                {
                    serviceNeeded = (1).ToString();
                }

                Console.WriteLine("Needed: " + serviceNeeded);

                //Customer
                CustomerIDTextBox.Text = table.Rows[_routeCustomerNum - 1]["Kunde ID"].ToString();
                CustomerNameTextBox.Text = table.Rows[_routeCustomerNum - 1]["Fornavn"] + " " + table.Rows[_routeCustomerNum - 1]["Efternavn"];
                CustomerAddressTextBox.Text = table.Rows[_routeCustomerNum - 1]["Adresse"].ToString();
                CustomerZipCodeTextBox.Text = table.Rows[_routeCustomerNum - 1]["Postnr"].ToString();
                CustomerCountyTextBox.Text = table.Rows[_routeCustomerNum - 1]["By"].ToString();
                CustomerServicesTextBox.Text = serviceNeeded;
                CustomerServiceNumTextBox.Text = (serviceNum + 1).ToString();
                try
                {
                    switch (!string.IsNullOrWhiteSpace(comment))
                    {
                        case true:
                            {
                                CustomerCommentTextBox.Text = comment;
                                CustomerCommentTextBox.IsEnabled = true;
                                ContainCommentCheck.IsEnabled = true;

                                switch (_customerDataList[_routeCustomerNum - 1][11] == "True")
                                {
                                    case true:
                                        {
                                            ContainCommentCheck.IsChecked = true;
                                            break;
                                        }
                                }
                                break;
                            }
                    }

                    //Prints the list to console for debugging
                    //for (int i = 0; i < _customerDataList.Count - 1; i++)
                    //{
                    //    Console.WriteLine($"{i}/{_routeCustomerNum - 1} : " + String.Join(", ", _customerDataList[i]));
                    //}
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected Error");
                }

                var customerId = table.Rows[_routeCustomerNum - 1]["Kunde ID"].ToString();

                SetServiceData(int.Parse(serviceNeeded), serviceNum, int.Parse(customerId));

                _routeCustomerAmount = table.Rows.Count;

                CustomerInfoGroupBox.Header = $"Kunde {_routeCustomerNum} / {_routeCustomerAmount}";
                _didServiceDataChange = false;

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void SetServiceData(int serviceNeeded, int serviceNum, int customerId, bool manual = false)
        {
            var serviceData = CustomerServiceData.CreateFromJson(await ApiHelper.GetDataAsync("ServiceData"));

            if (serviceNeeded <= serviceNum || manual)
            {
                serviceData = serviceData.Where(x => x.CustomerID == customerId && x.ServiceNumber == serviceNum).ToArray();
            }
            else
            {
                serviceData = serviceData.Where(x => x.CustomerID == customerId && x.ServiceNumber == serviceNum +1).ToArray();
            }

            if (serviceNum < 1)
            {
                PrevServiceButton.IsEnabled = false;
            }
            else
            {
                PrevServiceButton.IsEnabled = true;
            }

            switch (serviceData.Length > 0)
            {
                case true:
                    {

                        _didServiceDataExist = true;
                        //Amount
                        CustomerChimneysTextBox.Text = serviceData[0].Chimneys.ToString();
                        CustomerPipesTextBox.Text = serviceData[0].Pipes.ToString();
                        CustomerKWTextBox.Text = serviceData[0].KW.ToString();

                        CustomerLightingTextBox.Text = serviceData[0].Lighting.ToString();
                        CustomerHeightTextBox.Text = serviceData[0].Height.ToString();

                        //Pipe
                        CustomerDiaTextBox.Text = serviceData[0].Diameter.ToString();
                        CustomerLengthTextBox.Text = serviceData[0].Length.ToString();

                        CustomerTypeTextBox.Text = serviceData[0].Type.ToString();
                        break;

                    }
                default:
                    {
                        if (manual)
                        {
                            //Amount
                            CustomerChimneysTextBox.Text = "";
                            CustomerPipesTextBox.Text = "";
                            CustomerKWTextBox.Text = "";

                            CustomerLightingTextBox.Text = "";
                            CustomerHeightTextBox.Text = "";

                            //Pipe
                            CustomerDiaTextBox.Text = "";
                            CustomerLengthTextBox.Text = "";

                            CustomerTypeTextBox.Text = "";
                        }
                        break;
                    }
            }
        }

        private async void AddCustomerServiceData(string[] array)
        {
            try
            {
                var data = $"" +
                    $"CustomerID={array[0]}&" +
                    $"ServiceNumber={int.Parse(array[1])}&" +
                    $"Chimneys={array[2]}&" +
                    $"Pipes={array[3]}&" +
                    $"KW={array[4]}&" +
                    $"Lighting={array[5]}&" +
                    $"Height={array[6]}" +
                    $"Diameter={array[7]}&" +
                    $"Length={array[8]}&" +
                    $"Type={array[9]}&";

                await ApiHelper.PostDataAsync("ServiceData", data);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void UpdateCustomerServiceData(string[] array)
        {
            try
            {
                var data = $"" +
                    $"CustomerID={array[0]}&" +
                    $"ServiceNumber={int.Parse(array[1])}&" +
                    $"Chimneys={array[2]}&" +
                    $"Pipes={array[3]}&" +
                    $"KW={array[4]}&" +
                    $"Lighting={array[5]}&" +
                    $"Height={array[6]}" +
                    $"Diameter={array[7]}&" +
                    $"Length={array[8]}&" +
                    $"Type={array[9]}&";

                await ApiHelper.PutDataAsync("ServiceData", data);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void DoNextCustomerRow(bool isFinalPrint = false)
        {
            try
            {
                var table = _printRouteCustomerData;

                switch (table != null)
                {
                    case true:
                        {
                            var array = await DataInput();

                            switch (_didServiceDataChange)
                            {
                                case true:
                                    {
                                        switch (_didServiceDataExist)
                                        {
                                            case true:
                                                {
                                                    UpdateCustomerServiceData(array);
                                                    break;
                                                }
                                            default:
                                                {
                                                    AddCustomerServiceData(array);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            try
                            {
                                //Sets data input for the previous cutomer before changeing
                                _customerDataList[_routeCustomerNum - 1] = array;
                            }
                            catch (ArgumentOutOfRangeException AOOREx)
                            {
                                Log.Error(AOOREx, $"_routeCustomerNum - 1 = {_routeCustomerNum - 1}; Which is out of index (_customerDataList.Count = {_customerDataList.Count}) At Next");
                            }

                            switch (!isFinalPrint)
                            {
                                case true:
                                    {
                                        ResetPrintRouteDialog();

                                        SetupPrintRouteDialog(table);
                                        break;
                                    }
                            }

                            switch (_routeCustomerNum >= _routeCustomerAmount)
                            {
                                case true:
                                    {
                                        NextPrintRoute.IsEnabled = false;
                                        FinalPrintRoute.IsEnabled = true;
                                        break;
                                    }
                            }

                            switch (_routeCustomerNum > 1)
                            {
                                case true:
                                    {
                                        PrevPrintRoute.IsEnabled = true;
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            CustomerIDTextBox.Focus();
                            break;
                        }
                }
                table.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Next route customer error");
            }
        }

        private async void DoPrevCustomerRow()
        {
            try
            {
                var table = _printRouteCustomerData;

                switch (table != null)
                {
                    case true:
                        {
                            var array = await DataInput();

                            switch (_didServiceDataChange)
                            {
                                case true:
                                    {
                                        switch (_didServiceDataExist)
                                        {
                                            case true:
                                                {
                                                    UpdateCustomerServiceData(array);
                                                    break;
                                                }
                                            default:
                                                {
                                                    AddCustomerServiceData(array);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            try
                            {
                                //Sets data input for the previous cutomer before changeing
                                _customerDataList[_routeCustomerNum - 1] = array;
                            }
                            catch (ArgumentOutOfRangeException AOOREx)
                            {
                                Log.Error(AOOREx, $"_routeCustomerNum - 1 = {_routeCustomerNum - 1}; Which is out of index (_customerDataList.Count = {_customerDataList.Count}) At Prev");
                            }

                            ResetPrintRouteDialog();

                            SetupPrintRouteDialog(table, false);

                            switch (_routeCustomerNum < _routeCustomerAmount)
                            {
                                case true:
                                    {
                                        NextPrintRoute.IsEnabled = true;
                                        FinalPrintRoute.IsEnabled = false;
                                        break;
                                    }
                            }

                            switch (_routeCustomerNum <= 1)
                            {
                                case true:
                                    {
                                        NextPrintRoute.IsEnabled = true;
                                        PrevPrintRoute.IsEnabled = false;
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            CustomerIDTextBox.Focus();
                            break;
                        }
                }
                table.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Previous route customer error");
            }

        }
        
        private async void CustomerServiceDataTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _didServiceDataChange = true;

            try
            {
                //Must be an IF
                if (!(sender is TextBox textBox) || textBox.Text.Length <= 0) return;

                TextBoxExtrasHelper.FixedLineLength(5, textBox);
                TextBoxExtrasHelper.FixedLineAmount(4, textBox);
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Customer Service Data Error");
            }
        }

        private async void PrevPrintRoute_Click(object sender, RoutedEventArgs e)
        {
            DoPrevCustomerRow();
            await Task.FromResult(true);
        }

        private async void NextPrintRoute_Click(object sender, RoutedEventArgs e)
        {
            DoNextCustomerRow();
            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the PrintRouteButton control.
        /// </para>
        ///   <para>Prints the current data in CustomerGrid to a route schema</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void PrintRouteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (RouteGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            var table = VariableManipulation.DataGridtoDataTable(CustomerGrid);
                            _printRouteCustomerData = table; //VariableManipulation.SortDataTable(table, "Opstilling ASC");

                            foreach (DataRow row in _printRouteCustomerData.Rows)
                            {
                                Console.WriteLine(row.ItemArray[2]);
                            }

                            _routeCustomerNum = 0;
                            _routeSelected = RouteGrid.SelectedIndex;

                            ResetPrintRouteDialog();

                            _customerDataList.Clear();

                            for (int i = 0; i < _printRouteCustomerData.Rows.Count; i++)
                            {
                                _customerDataList.Add(new string[12]);
                            }

                            SetupPrintRouteDialog(_printRouteCustomerData, true);

                            PrintRouteDialog.IsOpen = true;

                            switch (_printRouteCustomerData.Rows.Count > 1)
                            {
                                case true:
                                    {
                                        FinalPrintRoute.IsEnabled = false;
                                        NextPrintRoute.IsEnabled = true;
                                        break;
                                    }
                                default:
                                    {
                                        FinalPrintRoute.IsEnabled = true;
                                        NextPrintRoute.IsEnabled = false;
                                        break;
                                    }

                            }
                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en rute");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void FinalPrintRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var table = VariableManipulation.DataGridtoDataTable(RouteGrid);

                DoNextCustomerRow(true);

                var _customerData = new DataSet();

                PrintHelper.SetupRoutePrint(
                    _customerData,
                    table.Rows[_routeSelected]["Maaned"].ToString(),
                    table.Rows[_routeSelected]["Aar"].ToString(),
                    _customerDataList,
                    table.Rows[_routeSelected]["Navn"].ToString()
                    );

                UpdateRouteData();

                PrintRouteDialog.IsOpen = false;
                Log.Information($"Successfully printed route {table.Rows[_routeSelected]["Navn"].ToString()}");
                table.Dispose();
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void CancelPrintRoute_Click(object sender, RoutedEventArgs e)
        {
            PrintRouteDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        private async void EmptyPrintRoute_Click(object sender, RoutedEventArgs e)
        {
            ResetPrintRouteDialog(true);

            await Task.FromResult(true);
        }

        #endregion

        #region Edit Route

        private async void EditRouteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (RouteGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            var route = RouteGrid.SelectedItem as Route;

                            _routeSelectId = route.ID;

                            ClearAddEditCustomerDialog();

                            AddEditCustomersToDataGrid();

                            EditRouteDialog.IsOpen = true;

                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en rute");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void AddEditCustomersToDataGrid()
        {
            try
            {
                //Get existing customer ids
                var ids = VariableManipulation.GetIntegerValuesInColumnFromListView<Customer>(EditCustomerList).ToList();
                //Get new customer ids
                var newCustomers = CustomerGrid.ItemsSource as RouteCustomer[];

                foreach(var customer in newCustomers)
                {
                    ids.Add(customer.KundeID);
                }

                Debug.WriteLine(ids.Count());

                var input = ClearTextEditRouteCustomerSearch.Text;

                //Get table
                var customers = Customer.CreateFromJson(await ApiHelper.GetDataAsync("Customers"));

                //Sort data
                if (!string.IsNullOrEmpty(input))
                {
                    customers = customers.Where(x => x.Adresse.Contains(input) || x.ID.ToString().Contains(input)).ToArray();
                }

                //Remove Ids and use source
                customers = customers.Where(x => !ids.Contains(x.ID)).ToArray();

                Debug.WriteLine(customers.Length);

                EditCustomerGrid.ItemsSource = customers;

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async Task<int[]> GetCustormerIdsFormRouteCustomersList(DataTable data)
        {
            try
            {
                List<int> list = new List<int>();

                foreach (DataRowView row in data.Rows)
                {
                    list.Add(int.Parse(row["ID"].ToString()));
                }

                data.Dispose();

                await Task.FromResult(true);

                return list.ToArray();
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
                return null;
            }
        }

        private async void ClearTextEditRouteCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEditCustomersToDataGrid();

            await Task.FromResult(true);
        }

        private async void ClearAddEditCustomerDialog()
        {
            EditCustomerList.Items.Clear();
            ClearTextEditRouteCustomerSearch.Text = "";

            await Task.FromResult(true);
        }

        private async void AddEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (EditCustomerGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            var row = EditCustomerGrid.SelectedItem as Customer;

                            EditCustomerList.Items.Add(row);

                            AddEditCustomersToDataGrid();
                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en kunde");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void RemoveEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditCustomerList.Items.RemoveAt(EditCustomerList.SelectedIndex);

                AddEditCustomersToDataGrid();
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Adds the new route data.</summary>
        private async Task<bool> AddNewRouteCustomerData()
        {
            try
            {
                //Finds the last route that was just created and gets it's ID
                var id = _routeSelectId;

                var rows = EditCustomerList.Items;

                var i = 0;
                var orderNum = CustomerGrid.Items.Count;
                foreach (var item in rows)
                {
                    orderNum++;

                    var data = (Customer)EditCustomerList.Items[i];

                    Random random = new Random();

                    var rcData = $"" +
                        $"RouteID={id}&" +
                        $"CustomerID={data.ID}&" +
                        $"CityID={random.Next(1, 251)}";

                    switch(await ApiHelper.PostDataAsync("RouteCustomer", rcData))
                    {
                        case false:
                            {
                                Debug.WriteLine("An error Occored");
                                break;
                            }
                    }

                    i++;

                    Log.Information($"Successfully added {i} new customers to existing route #{id}");
                }
                await Task.FromResult(true);

                return true;
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Warning(ex, "Customer could not be added to route");
                MessageBox.Show("Kunde kunne ikke tilføjes til Rute", "FEJL");
            }
            return false;
        }

        private async void FinalEditRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (EditCustomerList.Items.Count != 0 &&
                    EditCustomerList.Items.Count != -1)
                {
                    case true:
                        {
                            switch (AddNewRouteCustomerData().Result)
                            {
                                case true:
                                    {
                                        EditRouteDialog.IsOpen = false;
                                        SetCustomerData();
                                        break;
                                    }
                            }
                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Data mangler");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void CancelEditRoute_Click(object sender, RoutedEventArgs e)
        {
            EditRouteDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        private async void DeleteRouteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (CustomerGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            DeleteRouteCustomers(CustomerGrid.SelectedItems as RouteCustomer[]);

                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en kunde");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void DeleteRouteCustomers(RouteCustomer[] rows)
        {
            try
            {
                string message = $"Er du sikker du vil slette {rows.Length} kunder fra ruten?";
                string caption = "Advarsel";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            foreach (var row in rows)
                            {
                                var data = $"id={row.ID}";
                                await ApiHelper.DeleteDataAsync("RouteCustomer", data);

                                SetCustomerData();
                                Log.Information($"Successfully deleted customer {row.KundeID} from route {row.RuteID}");
                            }

                            MessageBox.Show($"{rows.Length} Kunder blev slettet fra rute nr:{rows[0].RuteID}");
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                MessageBox.Show("En uventet fejl er sket", "FEJL");
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #endregion

        #region City / County

        /// <summary>Sets city data to CityGrid.</summary>
        private async void SetCityData()
        {
            try
            {
                var cities = City.CreateFromJson(await ApiHelper.GetDataAsync("Cities"));
                _cityData = cities;
                CityGrid.ItemsSource = _cityData;

                Log.Information($"Successfully filled CityGrid ItemSource");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Something went wrong setting the data for the CityGrid");
            }
        }

        /// <summary>Updates city data to the database.</summary>
        private async void UpdateCityData(DataGrid grid)
        {
            try
            {
                try
                {
                    var city = grid.SelectedItem as City;

                    if (city != null)
                    {
                        var data = $"" +
                            $"id={city.id}&" +
                            $"ZipCode={city.Postnr}&" +
                            $"City={city.By}";

                        //Json
                        data = $"{{\n" +
                            $"\"id\": {city.id},\n" +
                            $"\"ZipCode\": {city.Postnr},\n" +
                            $"\"City\": \"{city.By}\"\n}}";

                        if (await ApiHelper.JPutDataAsync("City", data))
                        {
                            Log.Information($"Successfully updated city data");
                        }
                    }
                }
                catch (Exception)
                {

                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>Deletes selected county from database</para>
        /// </summary>
        /// <param name="row">The row.</param>
        private async void DeleteCounties()
        {
            try
            {
                var cities = CityGrid.SelectedItems;

                string message = $"Er du sikker du vil slette {cities.Count} Områder?";
                string caption = "Advarsel";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            if (cities != null)
                            {
                                try
                                {
                                    foreach (City city in cities)
                                    {
                                        var data = $"id={city.id}";
                                        if(await ApiHelper.DeleteDataAsync("City", data))
                                        {
                                            Log.Information($"Successfully deleted city #{city.id} ({city.By})");
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }

                            MessageBox.Show($"{cities.Count} Områder blev slettet");
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                MessageBox.Show("En uventet fejl er sket", "FEJL");
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>Adds new county data to the database</para>
        /// </summary>
        /// <returns>success</returns>
        private async Task<bool> AddNewCountyData()
        {
            try
            {
                switch (!string.IsNullOrWhiteSpace(NewCityZip.Text) &&
                    !string.IsNullOrWhiteSpace(NewCityName.Text))
                {
                    case true:
                        {

                            var data = $"ZipCode={NewCityZip.Text}&City={NewCityName.Text}";
                            if (await ApiHelper.PostDataAsync("City", data))
                            {
                                Log.Information($"Successfully added a new city");
                            }

                            await Task.FromResult(true);
                            return true;
                        }
                }

                MessageBox.Show("Data Mangler");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
            return false;
        }

        private async void ClearAddCountyDialog()
        {
            NewCityZip.Text = "";
            NewCityName.Text = "";

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the AddCountyButton control.
        /// </para>
        ///   <para>Opens the AddCountyDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void AddCountyButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAddCountyDialog();

            AddCountyDialog.IsOpen = true;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the CancelAddCounty control.
        /// </para>
        ///   <para>Closes the AddCountyDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CancelAddCounty_Click(object sender, RoutedEventArgs e)
        {
            AddCountyDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the FinalAddCounty control.
        /// </para>
        ///   <para>Adds new county data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddCounty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (AddNewCountyData().Result)
                {
                    case true:
                        {
                            AddCountyDialog.IsOpen = false;

                            SetCityData();

                            await Task.FromResult(true);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the DeleteCountyButton control.
        /// </para>
        ///   <para>Deletes selected county from database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void DeleteCountyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (CityGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            DeleteCounties();

                            AddCountyDialog.IsOpen = false;
                            SetCityData();

                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg et Område");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the CurrentCellChanged event of the CityGrid control.
        /// </para>
        ///  <para>Updates city data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void CityGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = sender as DataGrid;

            UpdateCityData(grid);

            await Task.FromResult(true);
        }

        #endregion

        #region Customer

        /// <summary>Sets customer data to CustomerGrid.</summary>
        /// <param name="row">The row.</param>
        private async void SetCustomerData()
        {
            try
            {
                var route = RouteGrid.SelectedItem as Route;
                _routeCustomerData = RouteCustomer.CreateFromJson(await ApiHelper.GetDataAsync("RouteCustomers"));
                CustomerGrid.ItemsSource = _routeCustomerData.Where(x => x.RuteID == route.ID).ToArray();

                await Task.FromResult(true);
                Log.Information($"Successfully filled CustomerGrid ItemSource");
            }
            catch (NullReferenceException)
            {
                Log.Information("While setting the data for the CustomerGrid, _routeSelectId was liekly NULL which is ok");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Warning(ex, "Something went wrong setting the data for the CustomerGrid");
            }
        }

        private async void UpdateCustomerData()
        {
            try
            {
                
                await Task.FromResult(true);
                Log.Information($"Successfully updated route customer data");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        private void CustomerGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                var grid = sender as DataGrid;

                int i = 0;
                foreach (DataGridColumn column in grid.Columns)
                {
                    //Must be an IF
                    if (column.IsReadOnly)
                    {
                        break;
                    }

                    switch (i == 3)
                    {
                        case true:
                            {
                                column.IsReadOnly = false;
                                break;
                            }
                        default:
                            {
                                column.IsReadOnly = true;
                                break;
                            }
                    }
                    i++;
                }

                UpdateCustomerData();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Setup
        /// <summary>Adds months from database to combobox.</summary>
        private async void AddMonthsToCombobox()
        {
            try
            {
                var months = new string[]
                 {
                    "Januar",
                    "Februar",
                    "Marts",
                    "April",
                    "Maj",
                    "Juni",
                    "Juli",
                    "August",
                    "September",
                    "Oktober",
                    "November",
                    "December"
                 };

                foreach (var month in months)
                {
                    RouteMonthComboBox.Items.Add(month);
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Enables the collumns for RouteGrid.</summary>
        private async void DisableCollumnsForRoutesGrid()
        {
            try
            {
                switch (RouteGrid.ItemsSource != null)
                {
                    case true:
                        {
                            RouteGrid.Columns[0].IsReadOnly = true;
                            RouteGrid.Columns[2].IsReadOnly = true;
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while disabling columns");
            }
        }

        #endregion

        #region Other Event handlers
        /// <summary>
        ///   <para>
        ///  Handles the Click event of the ReloadButton control.
        /// </para>
        ///  <para>Reloads data to RouteGrid</para>
        /// </summary> 
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            SetRouteData();
            SetCityData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the TextChanged event of the ClearTextSearch control.
        /// </para>
        ///   <para>Lets you search for data in clear text</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private async void ClearTextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetRouteData();

            await Task.FromResult(true);
        }
        #endregion

        #region Print Months
        private async Task<string[]> GetMonthPrintValues()
        {
            try
            {
                var result = new string[7];

                result[0] = Area1TextBox.Text;
                result[1] = Collumn1Row1TextBox.Text;
                result[2] = Collumn2Row1TextBox.Text;
                result[3] = Collumn3Row1TextBox.Text;
                result[4] = Collumn4Row1TextBox.Text;
                result[5] = Collumn5Row1TextBox.Text;
                result[6] = Collumn6Row1TextBox.Text;

                await Task.FromResult(true);

                return result;
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
            return null;
        }

        private async void DoNextMonthRow(bool isFinalMonth = false)
        {
            try
            {
                _monthPrintList[_areaNum - 1] = await GetMonthPrintValues();

                switch (!isFinalMonth)
                {
                    case true:
                        {
                            _areaNum++;

                            switch (_isNewPage)
                            {
                                case true:
                                    {
                                        ResetCollumnsInput();
                                        break;
                                    }
                                default:
                                    {
                                        SetCollumnsInput();
                                        break;
                                    }
                            }

                            MonthsArea.Header = $"Område {_areaNum} / {AreaAmount}";
                            break;
                        }
                }

                switch (_areaNum >= AreaAmount)
                {
                    case true:
                        {
                            NextPrintSheat.IsEnabled = false;
                            FinalPrintSheat.IsEnabled = true;
                            break;
                        }
                }
                switch (_areaNum > 1)
                {
                    case true:
                        {
                            PrevPrintSheat.IsEnabled = true;
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Next Month Error");
            }
        }

        private async void DoPrevMonthRow(bool isFirstMonth = false)
        {
            try
            {
                _monthPrintList[_areaNum - 1] = await GetMonthPrintValues();

                switch (!isFirstMonth)
                {
                    case true:
                        {
                            _areaNum--;

                            SetCollumnsInput();

                            _isNewPage = false;

                            MonthsArea.Header = $"Område {_areaNum} / {AreaAmount}";
                            break;
                        }
                }

                switch (_areaNum < AreaAmount)
                {
                    case true:
                        {
                            NextPrintSheat.IsEnabled = true;
                            FinalPrintSheat.IsEnabled = false;
                            break;
                        }
                }

                switch (_areaNum <= 1)
                {
                    case true:
                        {
                            NextPrintSheat.IsEnabled = true;
                            PrevPrintSheat.IsEnabled = false;
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Previous Month Error");
            }

        }

        private async void SetCollumnsInput()
        {
            try
            {
                Area1TextBox.Text = _monthPrintList[_areaNum - 1][0];
                Collumn1Row1TextBox.Text = _monthPrintList[_areaNum - 1][1];
                Collumn2Row1TextBox.Text = _monthPrintList[_areaNum - 1][2];
                Collumn3Row1TextBox.Text = _monthPrintList[_areaNum - 1][3];
                Collumn4Row1TextBox.Text = _monthPrintList[_areaNum - 1][4];
                Collumn5Row1TextBox.Text = _monthPrintList[_areaNum - 1][5];
                Collumn6Row1TextBox.Text = _monthPrintList[_areaNum - 1][6];

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void ResetCollumnsInput()
        {
            Area1TextBox.Text = "";
            Collumn1Row1TextBox.Text = "";
            Collumn2Row1TextBox.Text = "";
            Collumn3Row1TextBox.Text = "";
            Collumn4Row1TextBox.Text = "";
            Collumn5Row1TextBox.Text = "";
            Collumn6Row1TextBox.Text = "";

            await Task.FromResult(true);
        }

        private async void PrintMonthSheatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetCollumnsInput();

                _areaNum = 1;
                AreaAmount = int.Parse(MonthPrintValue.Value.ToString());

                MonthsArea.Header = $"Område {_areaNum} / {AreaAmount}";

                switch (!(AreaAmount <= 1))
                {
                    case true:
                        {
                            FinalPrintSheat.IsEnabled = false;
                            NextPrintSheat.IsEnabled = true;
                            break;
                        }
                    default:
                        {
                            FinalPrintSheat.IsEnabled = true;
                            NextPrintSheat.IsEnabled = false;
                            break;
                        }
                }

                _monthPrintList.Clear();

                for (var i = 0; i < AreaAmount; i++)
                {
                    _monthPrintList.Add(GetMonthPrintValues().Result);
                }

                MonthSheatYear.Value = DateTime.Now.Year;

                PrintMonthDialog.IsOpen = true;
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void CancelPrintSheat_Click(object sender, RoutedEventArgs e)
        {
            PrintMonthDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        private async void FinalPrintSheat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoNextMonthRow(true);

                PrintHelper.SetupMonthsPrint(_monthPrintList, int.Parse(MonthSheatYear.Value.ToString()));

                PrintMonthDialog.IsOpen = false;

                await Task.FromResult(true);
                Log.Information($"Successfully printed month sheat");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void NextPrintSheat_Click(object sender, RoutedEventArgs e)
        {
            DoNextMonthRow();

            await Task.FromResult(true);
        }

        private async void PrevPrintSheat_Click(object sender, RoutedEventArgs e)
        {
            DoPrevMonthRow();

            await Task.FromResult(true);
        }

        private async void AreaMonthTextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;

                switch (textBox?.Text.Length > 0)
                {
                    case true:
                        {
                            TextBoxExtrasHelper.FixedLineLength(20, textBox);
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Area Month TextBox Error");
            }
        }
        #endregion

        private async void PrevService_Click(object sender, RoutedEventArgs e)
        {
            var data = await DataInput();

            if (!_didServiceDataExist)
            {
                AddCustomerServiceData(data);
            }
            if (_didServiceDataChange)
            {
                UpdateCustomerServiceData(data);
            }

            var num = int.Parse(CustomerServiceNumTextBox.Text);

            num -= 1;

            if (num > 0)
            {
                CustomerServiceNumTextBox.Text = num.ToString();

                SetServiceData(int.Parse(CustomerServicesTextBox.Text), int.Parse(CustomerServiceNumTextBox.Text), int.Parse(CustomerIDTextBox.Text), true);
            }
            else
            {
                PrevServiceButton.IsEnabled = false;
            }
            await Task.FromResult(true);
        }
        private async void NextService_Click(object sender, RoutedEventArgs e)
        {
            PrevServiceButton.IsEnabled = true;

            var data = await DataInput();

            if (!_didServiceDataExist)
            {
                AddCustomerServiceData(data);
            }
            if (_didServiceDataChange)
            {
                UpdateCustomerServiceData(data);
            }

            var num = int.Parse(CustomerServiceNumTextBox.Text);

            num += 1;

            if (num > 0)
            {
                CustomerServiceNumTextBox.Text = num.ToString();

                SetServiceData(int.Parse(CustomerServicesTextBox.Text), int.Parse(CustomerServiceNumTextBox.Text), int.Parse(CustomerIDTextBox.Text), true);
            }
            await Task.FromResult(true);
        }
    }
}

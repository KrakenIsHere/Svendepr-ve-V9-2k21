using Svendepr_ve_V9_2k21;
using System;
using Serilog;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Documents;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.Models;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Customers
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Customers>();

        Customer[] data;
        Service[] Servicedata;

        public Customers()
        {
            InitializeComponent();

            InsertDataToCountyDropdown();
            AddObjectsToPaymentCombo();
            AddObjectsToInvoiceCombo();
            AddMonthsToCombobox(true);
            PrintHelper.FillInvoiceDesignCombo(InvoiceDesignCombo);
            SetData();
        }

        #region Customer

        /// <summary>Sets the data for the CustomerGrid ServiceGrid.</summary>
        private async void SetData()
        {
            try
            {
                var input = ClearTextSearch.Text;

                var customers = Customer.CreateFromJson(await ApiHelper.GetDataAsync("Customers"));
                if (!string.IsNullOrEmpty(input))
                {
                    customers = customers.Where(x => x.Adresse.Contains(input) || x.ID.ToString().Contains(input) || x.By.Contains(input)).ToArray();
                }
                if (!(CountySearch.SelectedIndex <= 0))
                {
                    customers = customers.Where(x => x.By.Contains(CountySearch.Text.Split(" ")[1])).ToArray();
                }
                if (!(MonthSearch.SelectedIndex <= 0))
                {
                    customers = customers.Where(x => x.Maaned == MonthSearch.Text).ToArray();
                }

                data = customers;

                CustomerGrid.ItemsSource = data;

                ServiceGrid.ItemsSource = null;

                Log.Information("Successfully filled CustomerGrid ItemSource");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Something went wrong setting the data for the CustomerGrid");
            }
        }

        /// <summary>Updates the cutomer data to the database.</summary>
        private async void UpdateData(DataGrid grid)
        {
            try
            {
                var cities = City.CreateFromJson(await ApiHelper.GetDataAsync("Cities"));

                var customer = grid.SelectedItem as Customer;

                if (customer != null)
                {
                    //URL
                    var data = $"" +
                        $"id={customer.ID}&" +
                        $"FirstName={customer.Fornavn}&" +
                        $"LastName={customer.Efternavn}&" +
                        $"Address={customer.Adresse}&" +
                        $"City={cities.Where(x => x.By == customer.By).FirstOrDefault().id}&" +
                        $"ServicesNeeded={customer.Fejninger}&" +
                        $"Comment={customer.Kommentar}&" +
                        $"MobilePhoneNumber={customer.Mobil}&" +
                        $"HomePhoneNumber={customer.Hjemme}&" +
                        $"Email={customer.Email}&" +
                        $"Month={customer.Maaned}";

                    //Json
                    data = $"{{\n" +
                        $"\"id\": {customer.ID},\n" +
                        $"\"FirstName\": \"{customer.Fornavn}\",\n" +
                        $"\"LastName\": \"{customer.Efternavn}\",\n" +
                        $"\"Address\": \"{customer.Adresse.Replace("\n", "\\n")}\",\n" +
                        $"\"City\": \"{customer.By}\",\n" +
                        $"\"ServicesNeeded\": {customer.Fejninger},\n" +
                        $"\"Comment\": \"{customer.Kommentar}\",\n" +
                        $"\"MobilePhoneNumber\": \"{customer.Mobil}\",\n" +
                        $"\"HomePhoneNumber\": \"{customer.Hjemme}\",\n" +
                        $"\"Email\": \"{customer.Email}\",\n" +
                        $"\"Month\": \"{customer.Maaned}\"\n" +
                        $"}}";

                    Debug.WriteLine(data);

                    await ApiHelper.JPutDataAsync("Customer", data);
                }
                Log.Information("Successfully updated customer data");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Something went wrong updating customer data");
            }
        }

        /// <summary>Deletes selected customers from CustomerGrid</summary>
        private async void DeleteCustomers()
        {
            try
            {
                string message = $"Er du sikker du vil slette {CustomerGrid.SelectedItems.Count} kunde(r)?";
                string caption = "Advarsel";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            var customers = CustomerGrid.SelectedItems;

                            if (customers != null)
                            {
                                try
                                {
                                    foreach (Customer customer in customers)
                                    {
                                        var data = $"value={customer.ID}";
                                        await ApiHelper.DeleteDataAsync("Customer", data);

                                        Log.Information($"Successfully deleted a customer #{customer.ID}");
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }

                            MessageBox.Show($"{CustomerGrid.SelectedItems.Count} Kunder blev slettet");
                            SetData();
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                MessageBox.Show("En uventet fejl er sket", "FEJL");
                Log.Error(ex, "An error occured while deleting a customer");
            }
        }
        /// <summary>Clears the AddCutomer Dialog</summary>
        private async void ClearAddCustomerDialog()
        {
            NewCustomersFirstname.Text = "";
            NewCustomersLastname.Text = "";
            NewCustomersAdress.Text = "";
            NewCustomersZipCode.Text = "";
            NewCustomersCity.Text = "";
            NewCustomersMail.Text = "";
            NewCustomersHome.Text = "";
            NewCustomersMobile.Text = "";
            NewCustomersComment.Text = "";
            NewCustomersServices.Text = "1";

            NewCustomersMonth1.SelectedIndex = 1;
            NewCustomersMonth2.SelectedIndex = 0;
            NewCustomersMonth3.SelectedIndex = 0;
            NewCustomersMonth4.SelectedIndex = 0;
            NewCustomersMonth5.SelectedIndex = 0;
            NewCustomersMonth6.SelectedIndex = 0;

            await Task.FromResult(true);
        }

        /// <summary>Adds new customer data to the database</summary>
        /// <returns>success</returns>
        private async Task<bool> AddNewCustomerData()
        {
            try
            {
                switch (!string.IsNullOrWhiteSpace(NewCustomersAdress.Text) &&
                        !string.IsNullOrWhiteSpace(NewCustomersZipCode.Text) &&
                        !string.IsNullOrWhiteSpace(NewCustomersCity.Text))
                {
                    case true:
                        {
                            string[] months = new string[] {
                                NewCustomersMonth1.SelectedValue.ToString(),
                                NewCustomersMonth2.SelectedValue.ToString(),
                                NewCustomersMonth3.SelectedValue.ToString(),
                                NewCustomersMonth4.SelectedValue.ToString(),
                                NewCustomersMonth5.SelectedValue.ToString(),
                                NewCustomersMonth6.SelectedValue.ToString()
                            };

                            var monthResult = "";

                            foreach (string str in months)
                            {
                                switch (!string.IsNullOrWhiteSpace(str) && str != "System.Windows.Controls.ComboBoxItem: Ingen Valgt" && str != "Ingen Valgt")
                                {
                                    case true:
                                        {
                                            monthResult += $"{str}, ";
                                            break;
                                        }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(monthResult))
                            {
                                monthResult = monthResult.Remove((monthResult.Length - 2), 2);
                            }

                            var NewServiceNum = NewCustomersServices.Text;

                            switch (string.IsNullOrWhiteSpace(NewServiceNum))
                            {
                                case true:
                                    {
                                        NewServiceNum = "0";
                                        break;
                                    }
                            }


                            var data = $"" +
                                $"FirstName={NewCustomersFirstname.Text}&" +
                                $"LastName={NewCustomersLastname.Text}&" +
                                $"Address={NewCustomersAdress.Text}&" +
                                $"City={NewCustomersCity.Text}&" +
                                $"ServicesNeeded={NewServiceNum}&" +
                                $"Comment={NewCustomersComment.Text}&" +
                                $"MobilePhoneNumber={NewCustomersMobile.Text}&" +
                                $"HomePhoneNumber={NewCustomersHome.Text}&" +
                                $"Email={NewCustomersMail.Text}&" +
                                $"Month={monthResult}";

                            await ApiHelper.PostDataAsync("Customer", data);

                            Log.Information("Successfully Added a new customer");
                            return true;
                        }
                    default:
                        {
                            MessageBox.Show("Data Mangler");
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while adding a new customer");
            }

            return false;
        }

        /// <summary>
        ///   <para>
        ///  Handles the SelectedCellsChanged event of the CustomerGrid control.
        /// </para>
        ///   <para>Updates ServiceGrid data</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectedCellsChangedEventArgs"/> instance containing the event data.</param>
        private async void CustomerGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SetServiceData();
            DisableCollumnsForDataGrid();

            await Task.FromResult(true);
        }


        /// <summary>
        ///   <para>
        ///  Handles the CurrentCellChanged event of the CustomerGrid control.
        /// </para>
        ///   <para>Updates changed cell to database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void CustomerGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            UpdateData(sender as DataGrid);

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the DeleteCustomerButton control.
        /// </para>
        ///   <para>Deletes selected customer from CustomerGrid</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            switch (CustomerGrid.SelectedIndex != -1)
            {
                case true:
                    {
                        DeleteCustomers();
                        SetData();
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Venligst vælg en kunde");
                        break;
                    }
            }
            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the AddCustomerButton control.
        /// </para>
        ///   <para>Opens the AddCustomerDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            AddMonthsToCombobox();
            SetCountyDataToCombos();

            ClearAddCustomerDialog();

            AddCustomerDialog.IsOpen = true;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the CancelAddCustomer control.
        /// </para>
        ///   <para>Closes the AddCustomerDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CancelAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the FinalAddCustomer control.
        /// </para>
        ///   <para>Adds new customer data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            switch (AddNewCustomerData().Result)
            {
                case true:
                    {
                        AddCustomerDialog.IsOpen = false;
                        break;
                    }
            }

            SetData();

            await Task.FromResult(true);
        }

        #endregion

        #region Service

        /// <summary>Adds the new service data.</summary>
        private async Task<bool> AddNewServiceData()
        {
            try
            {
                var queryFind =  $"{InvoiceMethod.SelectedItem.ToString()} {InvoiceNum.Text}";
                var services = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services"));

                switch (services.Where(x => x.Nummer == queryFind).Count() > 0)
                {
                    case true:
                        {
                            MessageBox.Show($"{InvoiceMethod.SelectedItem.ToString()} {InvoiceNum.Text}\nExistere allerede!");
                            return false;
                        }
                }


                var year = 0;

                switch (DateTime.Now.Month == 12 && DateTime.Now.Day > 26)
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

                var payment = "";

                switch (PaymentMethod.SelectedIndex != 0 && PaymentMethod.SelectedIndex != -1)
                {
                    case true:
                        {
                            payment = PaymentMethod.SelectedValue.ToString();
                            break;
                        }
                }

                var data = $"" +
                    $"id={NewServiceID.Text}&" +
                    $"CustomerID={CustomerID}&" +
                    $"Date={DateSelect.Text}&" +
                    $"Year={year}&" +
                    $"PaymentForm={payment}&" +
                    $"InvoiceNumber={InvoiceMethod.SelectedItem.ToString()} {InvoiceNum.Text}&" +
                    $"TimesSwept={Times.Text}";

                switch (!await ApiHelper.PostDataAsync("Service", data))
                {
                    case true:
                        {
                            throw new Exception("Unable to post new Service to database under at AddNewServiceData() Customers.xaml.cs");
                        }
                }

                foreach (ServiceProduct obj in ProductList.Items)
                {
                    data = $"" +
                        $"ServiceID={NewServiceID.Text}&" +
                        $"ProductID={obj.ID}";

                    await ApiHelper.PostDataAsync("ServiceProducts", data);
                }

                PropertiesExtension.Set("LastInvoiceNum", InvoiceNum.Text);

                await Task.FromResult(true);
                Log.Information($"Successfully added a service to customer #{CustomerID.Text}");
                return true;
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Something went wrong adding new service data");
            }
            return false;
        }

        /// <summary>Sets the data to ServiceGrid.</summary>
        /// <param name="row">The row.</param>
        private async void SetServiceData()
        {
            try
            {
                var customer = CustomerGrid.SelectedItem as Customer;

                Servicedata = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services"));

                switch (ContainServicesToYearOnly.IsChecked == true)
                {
                    case true:
                        {
                            Servicedata = Servicedata.Where(x => x.aar == DateTime.Now.Year).ToArray();
                            break;
                        }
                }

                ServiceGrid.ItemsSource = Servicedata.Where(x => x.KundeID == customer.ID);

                await Task.FromResult(true);
                Log.Information("Successfully filled ServiceGrid ItemSource");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Something went wrong setting the ServiceGrid data");
            }
        }

        private void ContainServicesToYearOnly_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SetServiceData();
            }
            catch (Exception)
            {
                //Do Nothing
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the CancelAddService control.
        /// </para>
        ///   <para>Closes the AddService Dialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CancelAddService_Click(object sender, RoutedEventArgs e)
        {
            AddServiceDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the FinalAddService control.
        /// </para>
        ///   <para>Adds the service to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddService_Click(object sender, RoutedEventArgs e)
        {
            switch (AddNewServiceData().Result)
            {
                case true:
                    {
                        AddServiceDialog.IsOpen = false;
                        SetServiceData();
                        break;
                    }
            }
            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the FinalAddAndPrintStandardService control.
        /// </para>
        ///   <para>Adds the service to the database and prints invoice</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddAndPrintStandardService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (InvoiceDesignCombo.SelectedIndex != 0 && InvoiceDesignCombo.SelectedIndex != -1)
                {
                    case true:
                        {
                            switch (!string.IsNullOrWhiteSpace(PayDateSelect.Text) && !string.IsNullOrWhiteSpace(PriceTextBox.Text))
                            {
                                case true:
                                    {
                                        switch (AddNewServiceData().Result)
                                        {
                                            case true:
                                                {
                                                    PrintHelper.SetupStandardInvoicePrint(
                                                        InvoiceDesignCombo.SelectedValue.ToString(),
                                                        PriceTextBox.Text,
                                                        PayDateSelect.Text,
                                                        long.Parse(InvoiceNum.Text),
                                                        ContainPayDateCheckBox,
                                                        ContainPriceCheckBox,
                                                        TaxWithPriceCheckBox,
                                                        DateSelect.Text,
                                                        InvoiceNum.Text,
                                                        int.Parse(CustomerID.Text),
                                                        ProductList,
                                                        IncludeProductsCheckBox
                                                        );

                                                    AddServiceDialog.IsOpen = false;
                                                    Log.Information("Successfully printed standard invoice");
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        MessageBox.Show("Data mangler");
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Venligst vælg et design");
                            break;
                        }
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
        ///   <para>
        ///  Handles the Click event of the FinalAddAndPrintService control.
        /// </para>
        ///   <para>Adds the service to the database and prints Giro-card</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddAndPrintService_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                switch (AddNewServiceData().Result)
                {
                    case true:
                        {
                            PrintHelper.SetupGiroPrint(
                            PriceTextBox.Text,
                            PayDateSelect.Text,
                            long.Parse(InvoiceNum.Text),
                            int.Parse(CustomerID.Text),
                            ContainPriceCheckBox,
                            ContainPayDateCheckBox,
                            TaxWithPriceCheckBox,
                            ProductList,
                            IncludeProductsCheckBox
                            );

                            AddServiceDialog.IsOpen = false;
                            Log.Information("Successfully printed giro invoice");
                            break;
                        }
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
        ///   <para>
        ///  Handles the Click event of the OpenAddServiceDialogButton control.
        /// </para>
        ///   <para>Opens the AddService dialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OpenAddServiceDialogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (CustomerGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            var row = CustomerGrid.SelectedItem as Customer;

                            var CID = row.ID;
                            int ServicesNeeded;
                            try
                            {
                                ServicesNeeded = row.Fejninger;
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "No service amount");
                                ServicesNeeded = 0;
                            }

                            CustomerID.Text = CID.ToString();

                            var query = $"SELECT * FROM `services` WHERE Service_YEAR = {DateTime.Now.Year} AND Customer_ID = {CID};";

                            var services = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services"));

                            var amountOfRows = services.Where(x => x.aar == DateTime.Now.Year && x.KundeID == CID).Count();

                            Times.Text = (amountOfRows + 1).ToString();

                            string message = "Denne kunde har allerede alle deres fejninger\nVil du fortsætte?";
                            string caption = "Advarsel";
                            System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                            System.Windows.MessageBoxResult result = System.Windows.MessageBoxResult.Yes;

                            switch (amountOfRows >= ServicesNeeded)
                            {
                                case true:
                                    {
                                        // Displays the MessageBox.
                                        result = MessageBox.Show(message, caption, buttons);
                                        break;
                                    }
                            }

                            switch (result == System.Windows.MessageBoxResult.Yes)
                            {
                                case true:
                                    {
                                        var date = DateTime.Now.Date;

                                        int amountOfTotalServices = 1;

                                        try
                                        {
                                            amountOfTotalServices = int.Parse(services.Last().ID.ToString()) + 1;
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error(ex, "Unable to get any Service Data");
                                        }

                                        NewServiceID.Text = amountOfTotalServices.ToString();

                                        DateSelect.Text = date.ToString("dd-MMM-yy");
                                        date = date.AddMonths(1);
                                        PayDateSelect.Text = date.ToString("dd-MMM-yy");

                                        //This number can be 16 digits long!!
                                        var newInvoiceNumber = long.Parse(PropertiesExtension.Get<string>("LastInvoiceNum")) + 1;

                                        InvoiceNum.Text = newInvoiceNumber.ToString();
                                        ProductList.Items.Clear();
                                        AddProductsToCombo();

                                        AddServiceDialog.IsOpen = true;
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            System.Windows.MessageBox.Show("Venligst vælg en kunde");
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region City/County

        /// <summary>Adds every county to combo boxes</summary>
        private async void SetCountyDataToCombos()
        {
            try
            {
                var cities = City.CreateFromJson(await ApiHelper.GetDataAsync("Cities"));

                NewCustomersCity.Items.Clear();
                NewCustomersZipCode.Items.Clear();

                foreach (City city in cities)
                {
                    NewCustomersCity.Items.Add(city.By);
                    NewCustomersZipCode.Items.Add(city.Postnr);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Page Setup

        /// <summary>Adds the objects to payment combobox.</summary>
        private async void AddObjectsToPaymentCombo()
        {
            try
            {
                string[] payment = new string[]
                {
                "Bank Overførsel",
                "Girokort",
                "Faktura"
                };

                foreach (object obj in payment)
                {
                    PaymentMethod.Items.Add(obj);
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Adds the objects to invoice combobox.</summary>
        private async void AddObjectsToInvoiceCombo()
        {
            try
            {
                string[] payment = new string[]
                {
                "Gironr.",
                "Faktura."
                };

                foreach (object obj in payment)
                {
                    InvoiceMethod.Items.Add(obj);
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Inserts the data to county dropdown.</summary>
        private async void InsertDataToCountyDropdown()
        {
            try
            {
                var cities = City.CreateFromJson(await ApiHelper.GetDataAsync("Cities"));

                foreach (var row in cities)
                {
                    CountySearch.Items.Add(row.Postnr.ToString() + " " + row.By);
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Disables the collumns for datagrids.</summary>
        private async void DisableCollumnsForDataGrid()
        {
            try
            {
                CustomerGrid.Columns[0].IsReadOnly = true;

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while disabling columns");
            }
        }

        /// <summary>Adds months from database to combobox.
        ///     <para>Makes sure the monthsearch is clear</para>
        /// </summary>
        /// <param name="isOnLoad">Holds onLoad booleam</param>
        private async void AddMonthsToCombobox(bool isOnLoad = false)
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

                NewCustomersMonth1.Items.Clear();
                NewCustomersMonth2.Items.Clear();
                NewCustomersMonth3.Items.Clear();
                NewCustomersMonth4.Items.Clear();
                NewCustomersMonth5.Items.Clear();
                NewCustomersMonth6.Items.Clear();

                if (isOnLoad)
                {
                    MonthSearch.Items.Clear();
                    MonthSearch.Items.Add("Ingen Valgt");
                }

                NewCustomersMonth1.Items.Add("Ingen Valgt");
                NewCustomersMonth2.Items.Add("Ingen Valgt");
                NewCustomersMonth3.Items.Add("Ingen Valgt");
                NewCustomersMonth4.Items.Add("Ingen Valgt");
                NewCustomersMonth5.Items.Add("Ingen Valgt");
                NewCustomersMonth6.Items.Add("Ingen Valgt");

                foreach (string month in months)
                {
                    NewCustomersMonth1.Items.Add(month);
                    NewCustomersMonth2.Items.Add(month);
                    NewCustomersMonth3.Items.Add(month);
                    NewCustomersMonth4.Items.Add(month);
                    NewCustomersMonth5.Items.Add(month);
                    NewCustomersMonth6.Items.Add(month);

                    if (isOnLoad)
                    {
                        MonthSearch.Items.Add(month);
                    }
                }

                NewCustomersMonth1.SelectedIndex = 0;
                NewCustomersMonth2.SelectedIndex = -1;
                NewCustomersMonth3.SelectedIndex = -1;
                NewCustomersMonth4.SelectedIndex = -1;
                NewCustomersMonth5.SelectedIndex = -1;
                NewCustomersMonth6.SelectedIndex = -1;
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Product List

        /// <summary>Adds the products from the database to the ProductComboBox.</summary>
        private async void AddProductsToCombo()
        {
            try
            {
                var data = Product.CreateFromJson(await ApiHelper.GetDataAsync("Products"));

                ProductCombo.Items.Clear();

                switch (ProductCombo.Items.Count <= 0)
                {
                    case true:
                        {
                            foreach (Product row in data)
                            {
                                ProductCombo.Items.Add(row.Navn);
                            }

                            ProductCombo.SelectedIndex = 0;
                            break;
                        }
                }
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
        ///  Handles the Click event of the AddProduct control.
        /// </para>
        ///   <para>Adds a product to a list for a new service</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pData = Product.CreateFromJson(await ApiHelper.GetDataAsync("Products")).Where(x => x.Navn == ProductCombo.SelectedValue.ToString()).FirstOrDefault();

                ProductList.Items.Add(pData);

                foreach (var data in ProductList.Items)
                {
                    Debug.WriteLine(data);
                }

                PrintHelper.CalculatePrice(ProductList, PriceTextBox);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }


        /// <summary>
        ///   <para>
        ///  Handles the Click event of the RemoveProduct control.
        /// </para>
        ///   <para>
        ///     Removes a selected product from a list for a new service
        ///   </para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductList.Items.RemoveAt(ProductList.SelectedIndex);

                PrintHelper.CalculatePrice(ProductList, PriceTextBox);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Other Event Handlers

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the RemoveProduct control.
        /// </para>
        ///   <para>
        ///     Runs when the ContainPriceCheckBox gets Checked
        ///   </para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ContainPriceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TaxWithPriceCheckBox.IsEnabled = true;
            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the RemoveProduct control.
        /// </para>
        ///   <para>
        ///     Runs when the ContainPriceCheckBox gets Unchecked
        ///   </para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ContainPriceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TaxWithPriceCheckBox.IsEnabled = false;
            TaxWithPriceCheckBox.IsChecked = false;

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
            SetData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the SelectionChanged event of the CountySearch control.
        /// </para>
        ///   <para>Lets you search by county</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private async void CountySearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the ReloadButton control.
        /// </para>
        ///   <para>Gets the data from the database with the current search parameters</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            SetData();

            await Task.FromResult(true);
        }


        /// <summary>
        ///   <para>
        ///  Handles the LostFocus event of the PriceTextBox control.
        /// </para>
        ///   <para>Shows the price for the current service</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void PriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintHelper.FixPriceText(PriceTextBox);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the PreviewTextInput event of the NumbersOnly control.
        /// </para>
        ///   <para>Gets only number input</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.TextCompositionEventArgs"/> instance containing the event data.</param>
        private async void NumbersOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>
        ///   <para>
        ///  Handles the SelectionChanged event of the MonthSearch control.
        /// </para>
        ///   <para>Changes search by month</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private async void MonthSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the RemoveProduct control.
        /// </para>
        ///   <para>
        ///     Runs when the ListViewPriceTextBox loses focus
        ///   </para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ListViewPriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintHelper.CalculatePrice(ProductList, PriceTextBox);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion
    }
}

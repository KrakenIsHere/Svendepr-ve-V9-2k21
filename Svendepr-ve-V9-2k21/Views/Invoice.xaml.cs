using Svendepr_ve_V9_2k21;

using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Documents;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.Models;
using System.Linq;
using System.Collections.Generic;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Invoice
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Invoice>();

        Service data;
        ServiceProduct[] productData;
        bool customerExists;

        public Invoice()
        {
            InitializeComponent();

            InvoiceSearchYearValue.Value = DateTime.Now.Year;

            AddObjectsToPaymentCombo();
            PrintHelper.FillInvoiceDesignCombo(InvoiceDesignCombo);
            SetData();
        }

        #region Service

        /// <summary>Sets up the PrintService dialog.</summary>
        /// <param name="ServiceRow">The service row.</param>
        /// <param name="ProductsTable">The products table.</param>
        private async void SetupPrintServiceDialog(Service ServiceRow, ServiceProduct[] Products)
        {
            //ServiceRow Array form:
            //"ID = 0"
            //"Kunde ID = 1"
            //"Dato = 2"
            //"Antal Gange = 3"
            //"Betaling = 5"
            //"Nummer = 6"

            try
            {
                var giro = ServiceRow.Nummer.ToString().Replace(" ", "").Split('.')[0];
                var number = "";

                switch (ServiceRow.Nummer.ToString().Replace(" ", "").Split('.')[1] != null)
                {
                    case true:
                        {
                            number = ServiceRow.Nummer.ToString().Replace(" ", "").Split('.')[1];
                            break;
                        }
                }

                ServiceID.Text = ServiceRow.ID.ToString();
                CustomerID.Text = ServiceRow.KundeID.ToString();
                Times.Text = ServiceRow.AntalGange.ToString();
                DateSelect.Text = ServiceRow.Dato.ToString();
                InvoiceNum.Text = number;

                var date = DateTime.Now.Date;
                date = date.AddMonths(1);
                PayDateSelect.Text = date.ToString("dd-MMM-yy");

                InvoiceMethod.Items.Add(giro);
                InvoiceMethod.SelectedIndex = InvoiceMethod.Items.Count - 1;

                switch (!string.IsNullOrWhiteSpace(ServiceRow.Betaling.ToString()))
                {
                    case true:
                        {
                            PaymentMethodText.Items.Add(ServiceRow.Betaling.ToString());
                            PaymentMethodText.SelectedIndex = PaymentMethodText.Items.Count - 1;
                            break;
                        }
                }

                ProductList.Items.Clear();

                switch (Products != null)
                {
                    case true:
                        {
                            foreach (ServiceProduct row in Products)
                            {
                                ProductList.Items.Add(new ServiceProduct
                                {
                                    ID = row.ID,
                                    ProduktNavn = row.ProduktNavn,
                                    Pris = row.Pris,
                                    Beskrivelse = row.Beskrivelse
                                });
                            }
                            break;
                        }
                }

                PrintHelper.CalculatePrice(ProductList, PriceTextBox);
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(true);
                Log.Error(ex, "An error occured while setting up the PrintService Dialog");
            }
        }

        /// <summary>Updates the database with the changes made to the ServiceGrid.</summary>
        private async void UpdateServiceData()
        {
            try
            {
                var service = ServiceGrid.SelectedItem as Service;

                var data = $"" +
                    $"id={service.ID}&" +
                    $"CustomerID={service.KundeID}&" +
                    $"Date={service.Dato}&" +
                    $"Year={service.aar}&" +
                    $"PaymentForm={service.Betaling}&" +
                    $"InvoiceNumber={service.Nummer}&" +
                    $"TimesSwept={service.AntalGange}";

                switch (!await ApiHelper.PutDataAsync("Service", data))
                {
                    case true:
                        {
                            throw new Exception("Unable to post new Service to database under at UpdateServiceData() Invoice.xaml.cs");
                        }
                }

                await Task.FromResult(true);
                Log.Information("Successfully updated service data");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Something went wrong updating the service data");
            }
        }

        /// <summary>Sets the data to the ServiceGrid.</summary>
        private async void SetData()
        {
            try
            {
                var input = ClearTextSearch.Text;

                var services = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services"));

                switch (!string.IsNullOrWhiteSpace(input))
                {
                    case true:
                        {
                            services = services.Where(x => x.ID.ToString().Contains(input) || x.KundeID.ToString().Contains(input) || x.Nummer.Contains(input)).ToArray();

                            UnpayedOnlyCheck.IsEnabled = true;
                            switch (PaySearch.SelectedIndex != 0 && PaySearch.SelectedIndex != -1)
                            {
                                case true:
                                    {
                                        UnpayedOnlyCheck.IsEnabled = false;
                                        UnpayedOnlyCheck.IsChecked = false;
                                        services = services.Where(x => x.Betaling == PaySearch.SelectedItem.ToString()).ToArray();
                                        break;
                                    }
                                default:
                                    {
                                        switch (UnpayedOnlyCheck.IsChecked == true)
                                        {
                                            case true:
                                                {

                                                    services = services.Where(x => x.Betaling == "").ToArray();
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            switch (ThisYearOnlyCheck.IsChecked == true)
                            {
                                case true:
                                    {
                                        services = services.Where(x => x.aar == InvoiceSearchYearValue.Value).ToArray();
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (PaySearch.SelectedIndex != 0 && PaySearch.SelectedIndex != -1)
                            {
                                case true:
                                    {
                                        services = services.Where(x => x.Betaling == PaySearch.SelectedItem.ToString()).ToArray();
                                        break;
                                    }
                                default:
                                    {
                                        switch (UnpayedOnlyCheck.IsChecked == true)
                                        {
                                            case true:
                                                {
                                                    services = services.Where(x => x.Betaling == "").ToArray();

                                                    switch (ThisYearOnlyCheck.IsChecked == true)
                                                    {
                                                        case true:
                                                            {
                                                                services = services.Where(x => x.aar == InvoiceSearchYearValue.Value).ToArray();
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    switch (ThisYearOnlyCheck.IsChecked == true)
                                                    {
                                                        case true:
                                                            {
                                                                try
                                                                {
                                                                    services = services.Where(x => x.aar == InvoiceSearchYearValue.Value).ToArray();
                                                                }
                                                                catch (NullReferenceException)
                                                                {
                                                                    //Do Nothing
                                                                }
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                }

                try
                {
                    ServiceGrid.ItemsSource = services;
                    ProductGrid.ItemsSource = null;
                    Log.Information("Successfully filled ServiceGrid ItemSource");
                }
                catch (NullReferenceException)
                {
                    //Do Nothing
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Something went wrong setting the data for the ServiceGrid");
            }
        }

        /// <summary>Disables the collumns for datagrids.</summary>
        private async void DisableCollumnsForDataGrid()
        {
            try
            {
                ServiceGrid.Columns[0].IsReadOnly = true;

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while disabling columns");
            }
        }

        /// <summary>Marks the selected service data as payed.</summary>
        private async void MarkServiceDataAsPayed(Service row)
        {
            try
            {
                switch (string.IsNullOrWhiteSpace(row.Betaling) || row.Betaling != PaymentMethod.SelectedValue.ToString())
                {
                    case true:
                        {
                            var paymentFormIndex = PaymentMethod.SelectedIndex;

                            switch (paymentFormIndex != 0 && paymentFormIndex != -1)
                            {
                                case true:
                                    {
                                        var paymentForm = PaymentMethod.SelectedValue;

                                        var data = $"" +
                                            $"id={row.ID}&" +
                                            $"CustomerID={row.KundeID}&" +
                                            $"Date={row.Dato}&" +
                                            $"Year={row.aar}&" +
                                            $"PaymentForm={paymentForm}&" +
                                            $"InvoiceNumber={row.Nummer}&" +
                                            $"TimesSwept={row.AntalGange}";

                                        switch (!await ApiHelper.PutDataAsync("Service", data))
                                        {
                                            case true:
                                                {
                                                    throw new Exception("Unable to post new Service to database under at UpdateServiceData() Invoice.xaml.cs");
                                                }
                                        }

                                        UpdateServiceData();

                                        MessageBox.Show($"Betaling til fejning nr:{row.ID} er nu opdateret");

                                        Log.Information($"Successfully set service #{row.ID} as payed with: {paymentForm}");
                                        break;
                                    }
                                default:
                                    {
                                        MessageBox.Show($"Venligst vælg en betalingsform");
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            MessageBox.Show($"Denne Fejning allerede står som betalt");
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                MessageBox.Show("Venligst vælg en fejning");
                Log.Warning(ex, "No service picked");
            }
        }

        private async void InsertInvoiceCustomerInfo(Service service)
        {
            try
            {
                CustomerInfoAddress.Text = "";
                CustomerInfoFirstname.Text = "";
                CustomerInfoLastname.Text = "";
                CustomerInfoID.Text = "";
                CustomerInfoNeededServices.Text = "";
                CustomerInfoMail.Text = "";
                CustomerInfoMobile.Text = "";
                CustomerInfoHome.Text = "";
                CustomerInfoServicesGotten.Text = "";

                var userData = Customer.CreateFromJson(await ApiHelper.GetDataAsync("Customers")).Where(x => x.ID == service.KundeID).ToArray();

                customerExists = userData.Length > 0;

                CustomerInfoAddress.Text = userData.FirstOrDefault().Adresse; // 1
                CustomerInfoFirstname.Text = userData.FirstOrDefault().Fornavn; // 2
                CustomerInfoLastname.Text = userData.FirstOrDefault().Efternavn; // 3
                CustomerInfoID.Text = userData.FirstOrDefault().ID.ToString(); // 4
                CustomerInfoNeededServices.Text = userData.FirstOrDefault().Fejninger.ToString(); // 5
                CustomerInfoMail.Text = userData.FirstOrDefault().Email; // 6
                CustomerInfoMobile.Text = userData.FirstOrDefault().Mobil; // 7
                CustomerInfoHome.Text = userData.FirstOrDefault().Hjemme; // 8

                var serviceData = Service.CreateFromJson(await ApiHelper.GetDataAsync("Services")).Where(x => x.KundeID == userData.FirstOrDefault().ID && x.aar == DateTime.Now.Year).ToArray();

                CustomerInfoServicesGotten.Text = serviceData.Length.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Deletes the selected service from ServiceGrid.</summary>
        /// <param name="row">The row.</param>
        private async void DeleteSelectedServices(Service[] rows)
        {
            try
            {
                string message = $"Er du sikker du vil slette {rows.Count()} fejninger?";
                string caption = "Advarsel";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            foreach (Service row in rows)
                            {
                                var products = ServiceProduct.CreateFromJson(await ApiHelper.GetDataAsync("ServiceProducts")).Where(x => x.FejningsID == row.ID);

                                foreach (var product in products)
                                {
                                    var dData = $"id={product.ID}";
                                    await ApiHelper.DeleteDataAsync("ServiceProduct", dData);
                                }

                                var data = $"id={row.ID}";
                                await ApiHelper.DeleteDataAsync("Service", data);

                                Log.Information($"Successfully deleted service #{row.ID}");
                            }
                            MessageBox.Show($"{rows.Length} Fejninger blev slettet");
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

        private void ThisYearOnlyCheck_Checked(object sender, RoutedEventArgs e)
        {
            SetData();
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the InvoicePrintButton control.
        /// </para>
        ///   <para>Gets service information</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void InvoicePrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (customerExists)
                {
                    case true:
                        {
                            switch (ServiceGrid.SelectedIndex != -1)
                            {
                                case true:
                                    {
                                        PrintServiceDialog.IsOpen = true;
                                        var service = ServiceGrid.SelectedItem as Service;
                                        var products = ServiceProduct.CreateFromJson(await ApiHelper.GetDataAsync("ServiceProducts")).Where(x => x.FejningsID == service.ID).ToArray();

                                        switch (products != null)
                                        {
                                            case true:
                                                {
                                                    SetupPrintServiceDialog(service, products);
                                                    break;
                                                }
                                        }
                                        await Task.FromResult(true);
                                        break;
                                    }
                                default:
                                    {
                                        await Task.FromResult(false);
                                        MessageBox.Show("Venligst vælg en fejning");
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Kunden for denne fejning er blevet slettet");
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
        ///  Handles the Click event of the FinalAddAndPrintStandardService control.
        /// </para>
        ///   <para>Adds the service to the database and prints invoice</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void PrintStandardService_Click(object sender, RoutedEventArgs e)
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

                                        PrintServiceDialog.IsOpen = false;
                                        Log.Information($"Successfully printed standard invoice");
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
        private async void PrintService_Click(object sender, RoutedEventArgs e)
        {
            try
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

                PrintServiceDialog.IsOpen = false;
                Log.Information($"Successfully printed giro invoice");
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
        ///  Handles the Click event of the CancelAddService control.
        /// </para>
        ///   <para>Closes the PrintService dialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CancelAddService_Click(object sender, RoutedEventArgs e)
        {
            PrintServiceDialog.IsOpen = false;
            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the InvoiceDeleteButton control.
        /// </para>
        ///   <para>Deletes a selected service from the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void InvoiceDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (ServiceGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            DeleteSelectedServices(ServiceGrid.SelectedItems as Service[]);
                            SetData();
                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg en fejning");
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
        ///  Handles the SelectedCellsChanged event of the ServiceGrid control.
        /// </para>
        ///   <para>Updates ProductsGrid data</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectedCellsChangedEventArgs"/> instance containing the event data.</param>
        private async void ServiceGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Log.Information("Selected Cell Changed");

            try
            {
                SetProductsInGrid(ServiceGrid.SelectedItem as Service);
            }
            catch (NullReferenceException NREx)
            {
                Log.Warning(NREx, "No products for invoice");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }

            try
            {
                UpdateServiceData();
                InsertInvoiceCustomerInfo(ServiceGrid.SelectedItem as Service);
                DisableCollumnsForDataGrid();

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Products

        /// <summary>Sets the products from the selected service to ProductsGrid.</summary>
        /// <param name="row">The Selected Row in the ServiceGrid.</param>
        private async void SetProductsInGrid(Service row)
        {
            try
            {
                productData = ServiceProduct.CreateFromJson(await ApiHelper.GetDataAsync("ServiceProducts")).Where(x => x.FejningsID == row.ID).ToArray();

                ProductGrid.ItemsSource = productData;

                await Task.FromResult(true);
                Log.Information($"Successfully filled ProductGrid ItemSource");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion

        #region Setup

        /// <summary>Adds the payment methods to payment combo.</summary>
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
                    PaySearch.Items.Add(obj);
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

        #region Other Event Handlers

        private async void ContainPriceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TaxWithPriceCheckBox.IsEnabled = true;
            await Task.FromResult(true);
        }

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
        ///  Handles the SelectionChanged event of the PaySearch control.
        /// </para>
        ///   <para>Lets you search by payment method</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private async void PaySearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnpayedOnlyCheck.IsChecked = false;
            SetData();

            await Task.FromResult(true);
        }


        /// <summary>
        ///   <para>
        ///  Handles the Checked event of the UnpayedCheckBox control.
        /// </para>
        ///   <para>Lets you search by unpayed services only</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void UnpayedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PaySearch.SelectedIndex = 0;
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
        ///  Handles the Click event of the MarkAsPayedButton control.
        /// </para>
        ///   <para>Marks the selected service data as payed.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void MarkAsPayedButton_Click(object sender, RoutedEventArgs e)
        {
            MarkServiceDataAsPayed(ServiceGrid.SelectedItem as Service);
            SetData();

            await Task.FromResult(true);
        }

        private void InvoiceSearchYearValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            SetData();
        }

        #endregion
    }
}

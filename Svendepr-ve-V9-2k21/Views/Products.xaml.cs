using Svendepr_ve_V9_2k21;

using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Documents;
using System.Collections.Generic;
using PyroSquidUniLib.Models;
using System.Linq;
using System.Diagnostics;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Products
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Products>();

        Product[] productsData;

        public Products()
        {
            InitializeComponent();
            SetData();
        }

        #region Product

        /// <summary>Sets data from the database to ProductGrid.</summary>
        private async void SetData()
        {
            try
            {
                var input = ClearTextSearch.Text;
                if (productsData == null)
                {
                    productsData = Product.CreateFromJson(await ApiHelper.GetDataAsync("Products"));
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                   ProductGrid.ItemsSource = productsData.Where(x => x.Navn.Contains(input)).ToArray();
                }
                else
                {
                    ProductGrid.ItemsSource = productsData;
                }

                Log.Information($"Successfully filled ProductsGrid ItemSource");
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Warning(ex, "Something went wrong setting the data for the ProductGrid");
            }
        }

        /// <summary>Updates data to the database.</summary>
        private async void UpdateData()
        {
            try
            {
                var product = ProductGrid.SelectedItem as Product;

                var data = $"" +
                    $"id={product.id}&" +
                    $"Name={product.Navn}&" +
                    $"Price={product.Pris}&" +
                    $"Description={product.Beskrivelse}";
                Debug.WriteLine(data);
                if (await ApiHelper.PutDataAsync("Products", data))
                {
                    Log.Information($"Successfully updated product data");
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "An error occured while updating data");
            }
        }

        /// <summary>
        ///   <para>Add new product data to the database</para>
        /// </summary>
        /// <returns>success</returns>
        private async Task<bool> AddNewProductData()
        {
            try
            {
                switch (!string.IsNullOrWhiteSpace(ProductNameTextBox.Text) &&
                    !string.IsNullOrWhiteSpace(ProductPriceTextBox.Text))
                {
                    case true:
                        {
                            var data = $"" +
                                    $"Name={ProductNameTextBox.Text}&" +
                                    $"Price={ProductPriceTextBox.Text.Replace(",", ".")}&" +
                                    $"Description={ProductDescriptionTextBox.Text}";

                            await ApiHelper.PostDataAsync("Product", data);

                            await Task.FromResult(true);
                            Log.Information($"Successfully added a new product");
                            return true;
                        }
                    default:
                        {
                            MessageBox.Show("Data Mangler");
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }

            return false;
        }

        /// <summary>
        ///   <para>Deletes selected product from the database</para>
        /// </summary>
        /// <param name="row">The row.</param>
        private async void DeleteProducts()
        {
            try
            {
                var products = ProductGrid.SelectedItems;

                string message = $"Er du sikker du vil slette {products.Count} produkter?";
                string caption = "Advarsel";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                switch (result == System.Windows.MessageBoxResult.Yes)
                {
                    case true:
                        {
                            foreach (Product product in products)
                            {
                                var data = $"id={product.id}";

                                if (await ApiHelper.DeleteDataAsync("Product", data))
                                {
                                    Log.Information($"Successfully deleted product #{product.id} ({product.Navn})");
                                }
                            }
                            MessageBox.Show($"{products.Count} Produkter blev slettet");
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
        ///  Handles the CurrentCellChanged event of the ProductGrid control.
        /// </para>
        ///   <para>Updates product data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void ProductGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            UpdateData();
            ProductGrid.Columns[0].IsReadOnly = true;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the DeleteProductButton control.
        /// </para>
        ///   <para>Deletes selected product from the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (ProductGrid.SelectedIndex != -1)
                {
                    case true:
                        {
                            DeleteProducts();

                            SetData();

                            await Task.FromResult(true);
                            break;
                        }
                    default:
                        {
                            await Task.FromResult(false);
                            MessageBox.Show("Venligst vælg et produkt");
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
        ///  Handles the Click event of the OpenAddProductDialogButton control.
        /// </para>
        ///   <para>Opens the AddProductDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OpenAddProductDialogButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductDialog.IsOpen = true;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the FinalAddProduct control.
        /// </para>
        ///   <para>Add new product data to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void FinalAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (AddNewProductData().Result)
                {
                    case true:
                        {
                            AddProductDialog.IsOpen = false;

                            SetData();

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

        #endregion

        #region Other Event Handlers

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the ReloadButton control.
        /// </para>
        ///   <para>Reloads data to ProductGrid</para>
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
        ///  Handles the TextChanged event of the ClearTextSearch control.
        /// </para>
        ///   <para>Lets you search for data in clear text</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private async void ClearTextSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SetData();

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the CancelAddProduct control.
        /// </para>
        ///   <para>Closes the AddProductDialog</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CancelAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductDialog.IsOpen = false;

            await Task.FromResult(true);
        }

        /// <summary>
        ///   <para>
        ///  Handles the LostFocus event of the ProductPriceTextBox control.
        /// </para>
        ///   <para>Shows the price for the current service</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ProductPriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintHelper.FixPriceText(ProductPriceTextBox);

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        #endregion
    }
}

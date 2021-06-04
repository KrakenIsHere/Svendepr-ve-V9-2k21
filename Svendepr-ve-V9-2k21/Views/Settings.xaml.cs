﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.FileSystem;
using Serilog;

namespace Svendepr_ve_V9_2k21.Views
{
    public partial class Settings
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Settings>();

        public Settings()
        {
            InitializeComponent();

            GetLogsPath();
            InsertCurrentDatabaseData();
            GetAndInsertGiroSettings();
        }

        #region Database Settings
        /// <summary>Gets the database connection status.</summary>
        /// <returns>Status String</returns>
        private async Task<string> GetDatabaseConnectionStatus()
        {
            var output = "";

            try
            {
                var isWorking = true;

                switch (isWorking)
                {
                    case true:
                        {
                            output = "Forbundet";
                            break;
                        }
                    case false:
                        {
                            output = "Ingen Forbindelse";
                            break;
                        }
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }

            return output;
        }

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the CheckDatabaseButton control.
        /// </para>
        ///   <para>Checks the status of the connection to the database</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CheckDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckDatabaseButton.IsEnabled = false;

                ConnectionStatusLabel.Content = await GetDatabaseConnectionStatus();
 
                CheckDatabaseButton.IsEnabled = true;
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Sets the new connection string to user settings.</summary>
        private async void SetNewConnectionString()
        {
            try
            { 
                PropertiesExtension.Set("ConnString", $"server={DatabaseIpBox.Text};user={DataUserNameBox.Text};database=Svendepr_ve_V9_2k21;port={DatabasePortBox.Text};password={DataUserPasswordBox.Password};");
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Error(ex, "Unexpected Error");
            }
        }

        /// <summary>Inserts the current database data to textboxes.</summary>
        private async void InsertCurrentDatabaseData()
        {
            try
            {
                var connString = PropertiesExtension.Get<string>("ConnString");

                var stringArray1 = connString.Split(';');

                CurrentDatabaseIp.Text = stringArray1[0].Split('=')[1];
                CurrentDatabasePort.Text = stringArray1[3].Split('=')[1];
                CurrentDatabaseUser.Text = stringArray1[1].Split('=')[1];

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
        ///  Handles the Click event of the SetNewDatabaseButton control.
        /// </para>
        ///   <para>Sets new connection string in user settings</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SetNewDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            SetNewConnectionString();

            InsertCurrentDatabaseData();

            await Task.FromResult(true);
        }
        #endregion

        #region Giro & Invoice Settings

        /// <summary>
        ///   <para>
        ///  Handles the Click event of the SaveInvoiceSettingsButton control.
        /// </para>
        ///   <para>Saves the giro setup to user settings</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SaveInvoiceSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetGiroSettings();

            await Task.FromResult(true);
        }

        /// <summary>Gets the and insert giro setup from user settings.</summary>
        private async void GetAndInsertGiroSettings()
        {
            try
            {
                var creditor = PropertiesExtension.Get<string>("CreditorNum");
                var invoice = PropertiesExtension.Get<string>("LastInvoiceNum");
                var address = PropertiesExtension.Get<string>("AddressLine");
                var reg = PropertiesExtension.Get<string>("Regnr");
                var account = PropertiesExtension.Get<string>("Accountnr");
                var message = PropertiesExtension.Get<string>("InvoiceMessage");

                var company = address.Split('+')[0];
                var road = address.Split('+')[1];
                var zip = address.Split('+')[2];

                CreditorNumTextBox.Text = creditor;
                InvoiceNumTextBox.Text = invoice;
                CompanyNameTextBox.Text = company;
                AddressLineTextBox.Text = road;
                ZipCodeAndCityTextBox.Text = zip;
                RegNumTextBox.Text = reg;
                AccountNumTextBox.Text = account;
                InvoiceMessageTextBox.Text = message.Replace("+", "\n");

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private static async Task<string> InvoiceMessageFormat(string input)
        {
            await Task.FromResult(true);
            return input.Replace(Environment.NewLine, "+");
        }

        /// <summary>Sets the new giro setup to user settings.</summary>
        private async void SetGiroSettings()
        {
            try
            {
                PropertiesExtension.Set("CreditorNum", CreditorNumTextBox.Text);
                PropertiesExtension.Set("LastInvoiceNum", InvoiceNumTextBox.Text);
                PropertiesExtension.Set("AddressLine", $"{CompanyNameTextBox.Text}+{AddressLineTextBox.Text}+{ZipCodeAndCityTextBox.Text}");
                PropertiesExtension.Set("Regnr", RegNumTextBox.Text);
                PropertiesExtension.Set("Accountnr", AccountNumTextBox.Text);
                PropertiesExtension.Set("InvoiceMessage", await InvoiceMessageFormat(InvoiceMessageTextBox.Text));

                GiroErrorLabel.Content = "Ændringerne er gemt";

                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Log.Warning(ex, "Chagnes could not be saved");
                GiroErrorLabel.Content = "Kunne ikke gemme ændringer";
            }
        }

        private void SetLogoButton_Click(object sender, RoutedEventArgs e)
        {
            SetNewLogo();
        }

        private void SetNewLogo()
        {
            try
            {
                var imageDialog = FileSystemHelper.imageFileDialog;

                imageDialog.FileName = "Vælg en fil";
                imageDialog.Filter = "Billed filer (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                imageDialog.Title = "Åben et billed";

                switch (imageDialog.ShowDialog() == DialogResult.OK)
                {
                    case true:
                        {
                            List<string> pathArray = new List<string>();
                            pathArray.AddRange(imageDialog.FileName.Split((char)92)); //(char)92 = \
                            var fileName = pathArray[pathArray.Count - 1];

                            pathArray.RemoveAt(pathArray.Count - 1);

                            var path = String.Join(@"\", pathArray.ToArray());
                            var logo = FileSystemHelper.GetImageFile(path + @"\", fileName);

                            switch (logo.Width > 1725 || logo.Height > 230)
                            {
                                case true:
                                    {
                                        System.Windows.MessageBox.Show("Billed for stort\nMax H: 230 Pixels\nMax B: 1725 Pixels");
                                        LogoErrorLabel.Content = "Størrelses fejl";
                                        break;
                                    }
                                default:
                                    {
                                        FileSystemHelper.CreateImage($@"{Environment.CurrentDirectory}\Assets\Company\Logo.png", logo);
                                        LogoErrorLabel.Content = "Logo tilføjet";
                                        break;
                                    }
                            }
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

        #region Logs
        private async void GetLogsPath()
        {
            LogsPath.Text = PropertiesExtension.Get<string>("LogsPath");
            await Task.FromResult(true);
        }

        private async void ClearLogs_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentLog = "Svendepr_ve_V9_2k21_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
                var info = new DirectoryInfo(PropertiesExtension.Get<string>("LogsPath"));

                foreach (var file in info.GetFiles())
                {
                    if (file.Name == currentLog) continue;
                    file.Delete();
                }

                await Task.FromResult(true);
                Log.Information("Logs was cleared");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private async void LogsPathBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentLog = "Svendepr_ve_V9_2k21_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
                var info = new DirectoryInfo(PropertiesExtension.Get<string>("LogsPath"));

                using (var fd = new FolderBrowserDialog())
                {
                    var result = fd.ShowDialog();

                    if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fd.SelectedPath)) return;

                    foreach (var file in info.GetFiles())
                    {
                        if (file.Name == currentLog) continue;
                        File.Move(file.FullName, fd.SelectedPath + "/" + file.Name);
                    }

                    LogsPath.Text = fd.SelectedPath;
                    PropertiesExtension.Set("LogsPath", fd.SelectedPath);
                }
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected Error");
            }
        }

        private void OpenLogs_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = LogsPath.Text;

            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                var ex = $"Directory does not exist {folderPath}";

                Log.Error(ex, "Unexpected Error");
            }
        }
        #endregion

        #region Save Settings

        private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var text = $"" +
                $"{PropertiesExtension.Get<string>("CreditorNum")}\n" +
                $"{PropertiesExtension.Get<string>("LastInvoiceNum")}\n" +
                $"{PropertiesExtension.Get<string>("AddressLine")}\n" +
                $"{PropertiesExtension.Get<string>("Regnr")}\n" +
                $"{PropertiesExtension.Get<string>("Accountnr")}\n" +
                $"{PropertiesExtension.Get<string>("InvoiceMessage")}\n" +
                $"{PropertiesExtension.Get<string>("ConnString")}";

            var path = $@"{DefaultDirectories.AppData}\Svendepr_ve_V9_2k21\Settings\";

            AsyncFileSystemHelper.CreateRewriteTextFileAsync(path, "Settings.txt", text);
            await Task.FromResult(true);
        }

        private async void RestoreSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = $@"{DefaultDirectories.AppData}\Svendepr_ve_V9_2k21\Settings\Settings.txt";

                var array = await AsyncFileSystemHelper.ReadTextFileToArrayAsync(path);

                PropertiesExtension.Set("CreditorNum", array[0]);
                PropertiesExtension.Set("LastInvoiceNum", array[1]);
                PropertiesExtension.Set("AddressLine", array[2]);
                PropertiesExtension.Set("Regnr", array[3]);
                PropertiesExtension.Set("Accountnr", array[4]);
                PropertiesExtension.Set("InvoiceMessage", array[5]);
                PropertiesExtension.Set("ConnString", array[6]);

                InsertCurrentDatabaseData();

                GetAndInsertGiroSettings();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Couldn't recover settings");
            }
        }

        #endregion

    }
}

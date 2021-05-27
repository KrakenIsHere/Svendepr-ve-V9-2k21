using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PyroSquidUniLib.Database;
using PyroSquidUniLib.FileSystem;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.Models;

namespace PyroSquidUniLib.Documents
{
    public class PrintHelper
    {
        /// <summary>Fills the invoice design combo with design file names.</summary>
        /// <param name="invoiceDesignCombo"></param>
        public static void FillInvoiceDesignCombo(System.Windows.Controls.ComboBox invoiceDesignCombo)
        {
            try
            {
                var imagesPath = $@"{Environment.CurrentDirectory}\Assets\Invoice Template\Designs\";

                var files = Directory.GetFiles(imagesPath);

                foreach (var path in files)
                {
                    var lastIndex = path.Split('\\').Count() - 1;

                    invoiceDesignCombo.Items.Add(path.Split('\\')[lastIndex]);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>Corrects the price text format.</summary>
        /// <param name="priceTextBox"></param>
        /// <returns>Formatted price text</returns>
        public static string FixPriceText(System.Windows.Controls.TextBox priceTextBox)
        {
            try
            {
                priceTextBox.Text = priceTextBox.Text.Replace(',', '.');
                switch (!priceTextBox.Text.Contains('.'))
                {
                    case true:
                        {
                            priceTextBox.Text += ".00";
                            break;
                        }
                }
                switch (priceTextBox.Text.Split('.')[1].Length <= 1)
                {
                    case true:
                        {
                            priceTextBox.Text += "0";
                            break;
                        }
                }
                switch (priceTextBox.Text.Split('.')[1].Length > 2)
                {
                    case true:
                        {
                            var cents = priceTextBox.Text.Split('.')[1];

                            priceTextBox.Text.Replace(cents, cents.Remove(2));
                            break;
                        }
                }

                return priceTextBox.Text.Replace('.', ',');
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return "";
            }
        }

        /// <summary>Calculates the full price.</summary>
        /// <param name="productList"></param>
        /// <param name="priceTextBox"></param>
        public static void CalculatePrice(System.Windows.Controls.ListView productList, System.Windows.Controls.TextBox priceTextBox)
        {
            try
            {
                var price = 0.00;

                foreach (ServiceProduct obj in productList.Items)
                {
                    price += double.Parse(obj.Price.Replace(',', '.'));
                }

                priceTextBox.Text = price.ToString();
                FixPriceText(priceTextBox);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Months
        /// <summary>Setups the route print.</summary>
        /// <param name="data">The data.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SetupMonthsPrint(List<string[]> inputRows, int year = 0, string fileName = "Turseddel Template.png")
        {
            try
            {
                var PrintPath = "";

                var imageFilePath = $@"{Environment.CurrentDirectory}\Assets\";

                //load the image file
                var originalBmp = (Bitmap)System.Drawing.Image.FromFile(imageFilePath + fileName);

                switch (FileSystemHelper.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    case true:
                        {
                            var outputPath = FileSystemHelper.folderBrowserDialog.SelectedPath;
                            int i = 0;
                            int pageNum = 1;
                            int monthNum = 1;
                            bool stamped = false;
                            List<Bitmap> pages = new List<Bitmap>();

                            var point = new PointF(1060, 60);
                            var tempBmp = new Bitmap(originalBmp);

                            while (i < inputRows.Count)
                            {
                                using (Graphics graphics = Graphics.FromImage(tempBmp))
                                {
                                    using (Font arialFont = new Font("Arial", 20))
                                    {
                                        switch (!stamped)
                                        {
                                            case true:
                                                {
                                                    using (Font arialFontLarge = new Font("Arial", 30, FontStyle.Bold))
                                                    {
                                                        graphics.DrawString($"{year}", arialFontLarge, System.Drawing.Brushes.Black, point);

                                                        point.X = 1150;
                                                        point.Y = 1500;

                                                        graphics.DrawString($"{pageNum}", arialFontLarge, System.Drawing.Brushes.Black, point);
                                                    }
                                                    stamped = true;

                                                    point.X = 70;
                                                    point.Y = 290;
                                                    break;
                                                }
                                        }

                                        /*
                                         0 = Area
                                         1 = x1
                                         2 = x2
                                         3 = x3
                                         4 = x4
                                         5 = x5
                                         6 = x6
                                         */

                                        //Reset Point
                                        point.X = 70;

                                        //Months
                                        #region Write Row in image

                                        //Area
                                        graphics.DrawString($"{inputRows[i][0]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 370.0f;

                                        // x1
                                        graphics.DrawString($"{inputRows[i][1]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 310.0f;

                                        // x2
                                        graphics.DrawString($"{inputRows[i][2]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 280.0f;

                                        // x3
                                        graphics.DrawString($"{inputRows[i][3]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 320.0f;

                                        // x4
                                        graphics.DrawString($"{inputRows[i][4]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 320.0f;

                                        // x5
                                        graphics.DrawString($"{inputRows[i][5]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 300.0f;

                                        // x6
                                        graphics.DrawString($"{inputRows[i][6]}", arialFont, System.Drawing.Brushes.Black, point);

                                        #endregion

                                        //New Line
                                        point.Y += 120.0f;

                                        //New Page
                                        switch ((monthNum % (9 * pageNum) == 0))
                                        {
                                            case true:
                                                {
                                                    pageNum++;
                                                    pages.Add(tempBmp);

                                                    //Point Reset
                                                    point.X = 1060;
                                                    point.Y = 60;

                                                    //Bitmap Reset
                                                    tempBmp = new Bitmap(originalBmp);
                                                    stamped = false;
                                                    break;
                                                }
                                        }
                                        i++;
                                        monthNum++;
                                    }
                                }
                            }

                            pages.Add(tempBmp);

                            PrintPath = outputPath + @"\FilerTilPrinter";

                            //save the image file
                            foreach (Bitmap map in pages)
                            {
                                FileSystemHelper.CreateFolder(PrintPath);

                                var outputFileName = $@"\NewPrintFile{pageNum}.png";

                                FileSystemHelper.CreateImage(PrintPath + outputFileName, map);

                                pageNum++;
                            }

                            PdfHelper.ConvertImageArrayToPdf(PrintPath, outputPath, $@"\" + VariableManipulation.CorrectFileName($@"Maaneds Oversigt_{year}"), IOerrorMessage: "Er dokumentet åbent?");

                            FileSystemHelper.DeleteFolderAndContent(PrintPath);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Route
        /// <summary>Setups the route print.</summary>
        /// <param name="data">The data.</param>
        /// <param name="routeName"></param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="customerInputData"></param>
        public static void SetupRoutePrint(DataSet data, string month, string year, List<string[]> customerInputData, string routeName, string fileName = "Køreseddel Template.png")
        {
            try
            {
                var PrintPath = "";

                var pages = new List<Bitmap>();
                ;
                var imageFilePath = $@"{Environment.CurrentDirectory}\Assets\";

                //load the image file
                var originalBmp = (Bitmap)System.Drawing.Image.FromFile(imageFilePath + fileName);

                var table = VariableManipulation.SortDataTable(data.Tables[0], "Opstilling ASC");

                var i = 0;
                var pageNum = 1;
                var customerNum = 0;
                var newArea = true;
                //var lastRowOfLastTabel = false;
                var phone = "";

                switch (FileSystemHelper.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    case true:
                        {
                            var outputPath = FileSystemHelper.folderBrowserDialog.SelectedPath;
                            var point = new PointF(35, 385);
                            var tempBmp = new Bitmap(originalBmp);
                            var Stamped = false;
                            customerNum = 1;
                            pageNum = 1;

                            //Point Reset
                            point.X = 30;
                            point.Y = 385;

                            foreach (DataRow row in table.Rows)
                            {
                                newArea = false;

                                using (Graphics graphics = Graphics.FromImage(tempBmp))
                                {
                                    using (Font arialFont = new Font("Arial", 20))
                                    {
                                        switch (!Stamped)
                                        {
                                            case true:
                                                {
                                                    graphics.DrawString($"" +
                                                        $"{routeName}", new Font("Arial", 30), System.Drawing.Brushes.Black, 190, 150);
                                                    graphics.DrawString($"" +
                                                        $"{year}", new Font("Arial", 30), System.Drawing.Brushes.Black, 1090, 150);
                                                    graphics.DrawString($"" +
                                                        $"{month}", new Font("Arial", 30), System.Drawing.Brushes.Black, 1992, 150);

                                                    graphics.DrawString($"" +
                                                        $"{pageNum}", new Font("Arial", 30), System.Drawing.Brushes.Black, 1150, 1500);
                                                    Stamped = true;
                                                    break;
                                                }
                                        }

                                        phone = "";

                                        #region Phone Setup
                                        try
                                        {
                                            switch (!string.IsNullOrWhiteSpace(row["Mobil"].ToString()) && !string.IsNullOrWhiteSpace(row["Hjemme"].ToString()))
                                            {
                                                case true:
                                                    {
                                                        phone = row["Mobil"].ToString() + " / " + row["Hjemme"].ToString();
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        switch (!string.IsNullOrWhiteSpace(row["Mobil"].ToString()))
                                                        {
                                                            case true:
                                                                {
                                                                    phone = row["Mobil"].ToString();
                                                                    break;
                                                                }
                                                            default:
                                                                {
                                                                    switch (!string.IsNullOrWhiteSpace(row["Hjemme"].ToString()))
                                                                    {
                                                                        case true:
                                                                            {
                                                                                phone = row["Hjemme"].ToString();
                                                                                break;
                                                                            }
                                                                    }
                                                                    break;
                                                                }
                                                        }
                                                        break;
                                                    }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex);
                                        }

                                        #endregion

                                        //Write Row in image
                                        graphics.DrawString($""
                                            + $"{row["Fornavn"].ToString()} "
                                            + $"{row["Efternavn"].ToString()}\n"
                                            + $"{row["Adresse"].ToString()}\n"
                                            + $"{row["Postnr"].ToString()} "
                                            + $"{row["By"].ToString()}\n"
                                            + $"Tlf: {phone}"
                                            + $"", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 500.0f;
                                        point.Y += 30.0f;



                                        //Services
                                        graphics.DrawString($"{row["Fejninger"].ToString()}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 170.0f;
                                        //Chimneys
                                        graphics.DrawString($"{customerInputData[i][2]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 130.0f;
                                        //Pipes
                                        graphics.DrawString($"{customerInputData[i][3]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 85.0f;
                                        //KW
                                        graphics.DrawString($"{customerInputData[i][4]}", arialFont, System.Drawing.Brushes.Black, point);

                                        var messured = MessureLineLenthInString(5, customerInputData[i][5]);

                                        //Makes space if a line is too big based on messurement
                                        switch (!messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 120.0f;
                                                    break;
                                                }
                                            default:
                                                {
                                                    point.X += 100.0f;
                                                    break;
                                                }
                                        }

                                        var multiline = CountLinesInString(3, customerInputData[i][5]);

                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y -= 30.0f;
                                                    break;
                                                }
                                        }

                                        //Light
                                        graphics.DrawString($"{customerInputData[i][5]}", arialFont, System.Drawing.Brushes.Black, point);


                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y += 30.0f;
                                                    break;
                                                }
                                        }

                                        //Adding the missing distance
                                        switch (messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 20.0f;
                                                    break;
                                                }
                                        }

                                        point.X += 120.0f;
                                        //Hight
                                        graphics.DrawString($"{customerInputData[i][6]}", arialFont, System.Drawing.Brushes.Black, point);

                                        point.X += 120.0f;
                                        //Diameter
                                        graphics.DrawString($"{customerInputData[i][7]}", arialFont, System.Drawing.Brushes.Black, point);

                                        messured = MessureLineLenthInString(5, customerInputData[i][8]);

                                        //Makes space if a line is too big based on messurement
                                        switch (!messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 120.0f;
                                                    break;
                                                }
                                            default:
                                                {
                                                    point.X += 100.0f;
                                                    break;
                                                }
                                        }

                                        multiline = CountLinesInString(3, customerInputData[i][8]);

                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y -= 30.0f;
                                                    break;
                                                }
                                        }

                                        //Lenght
                                        graphics.DrawString($"{customerInputData[i][8]}", arialFont, System.Drawing.Brushes.Black, point);

                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y += 30.0f;
                                                    break;
                                                }
                                        }

                                        //Adding the missing distance
                                        switch (messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 20.0f;
                                                    break;
                                                }
                                        }

                                        messured = MessureLineLenthInString(5, customerInputData[i][9]);

                                        //Makes space if a line is too big based on messurement
                                        switch (!messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 120.0f;
                                                    break;
                                                }
                                            default:
                                                {
                                                    point.X += 100.0f;
                                                    break;
                                                }
                                        }

                                        multiline = CountLinesInString(3, customerInputData[i][9]);

                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y -= 30.0f;
                                                    break;
                                                }
                                        }

                                        //Type
                                        graphics.DrawString($"{customerInputData[i][9]}", arialFont, System.Drawing.Brushes.Black, point);

                                        switch (multiline)
                                        {
                                            case true:
                                                {
                                                    point.Y += 30.0f;
                                                    break;
                                                }
                                        }

                                        //Adding the missing distance
                                        switch (messured)
                                        {
                                            case true:
                                                {
                                                    point.X += 20.0f;
                                                    break;
                                                }
                                        }

                                        Console.WriteLine(customerInputData[i][11]);
                                        switch (customerInputData[i][11] == "True")
                                        {
                                            case true:
                                                {
                                                    point.Y -= 30.0f;
                                                    point.X += 115.0f;
                                                    //Type
                                                    graphics.DrawString($"{customerInputData[i][10]}", arialFont, System.Drawing.Brushes.Black, point);
                                                    point.Y += 30.0f;
                                                    break;
                                                }
                                        }

                                        //Start a new Row
                                        point.X = 30.0f;
                                        point.Y += 146.5f - 30.0f;

                                        switch (customerNum == table.Rows.Count)
                                        {
                                            case true:
                                                {
                                                    newArea = true;
                                                    break;
                                                }
                                        }

                                        switch ((customerNum % (7 * pageNum) == 0) || newArea)
                                        {
                                            case true:
                                                {
                                                    pageNum++;
                                                    pages.Add(tempBmp);

                                                    //Point Reset
                                                    point.X = 30;
                                                    point.Y = 385;

                                                    //Bitmap Reset
                                                    tempBmp = new Bitmap(originalBmp);

                                                    Stamped = false;
                                                    break;
                                                }
                                        }
                                        customerNum++;
                                        i++;
                                    }
                                }
                            }
                            tempBmp.Dispose();
                            pageNum = 1;

                            PrintPath = outputPath + @"\FilerTilPrinter";

                            //save the image file
                            foreach (Bitmap map in pages)
                            {
                                FileSystemHelper.CreateFolder(PrintPath);

                                var outputFileName = $@"\NewPrintFile{pageNum}.png";

                                FileSystemHelper.CreateImage(PrintPath + outputFileName, map);

                                pageNum++;
                            }

                            PdfHelper.ConvertImageArrayToPdf(PrintPath, outputPath, $@"\" + VariableManipulation.CorrectFileName($@"Rute_{routeName}_{month}_{year}"), IOerrorMessage: "Er dokumentet åbent?");

                            FileSystemHelper.DeleteFolderAndContent(PrintPath);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Invoice
        /// <summary>Sets up the standard invoice print.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="priceText">The price text.</param>
        /// <param name="containTaxPriceCheck"></param>
        /// <param name="payDate">The pay date.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="productList">The product list.</param>
        /// <param name="includeProductsCheck"></param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="invoiceDate"></param>
        /// <param name="containPriceCheck"></param>
        public static void SetupStandardInvoicePrint(string fileName, string priceText, string invoiceDate, long num, System.Windows.Controls.CheckBox containPayDate, System.Windows.Controls.CheckBox containPriceCheck, System.Windows.Controls.CheckBox containTaxPriceCheck, string payDate, string invoiceNumber, int customerId, System.Windows.Controls.ListView productList, System.Windows.Controls.CheckBox includeProductsCheck, string imageFilePath = "")
        {
            try
            {
                var printPath = "";

                var query = $"SELECT * FROM `customers` WHERE Customer_ID = {customerId};";

                var rows = MySqlHelper.GetDataFromDatabase<Customer>(query, "ConnString");

                var logoFilePath = $@"{Environment.CurrentDirectory}\Assets\Company\";

                var logo = FileSystemHelper.GetImageFile(logoFilePath, $@"Logo.png");

                switch (string.IsNullOrWhiteSpace(imageFilePath))
                {
                    case true:
                        {
                            imageFilePath = $@"{Environment.CurrentDirectory}\Assets\Invoice Template\Designs\";
                            break;
                        }
                }

                //load the image file
                var originalBmp = (Bitmap)System.Drawing.Image.FromFile(imageFilePath + fileName);

                switch (FileSystemHelper.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    case true:
                        {
                            var outputPath = FileSystemHelper.folderBrowserDialog.SelectedPath;
                            var point = new PointF(35, 300);
                            Bitmap tempBmp = new Bitmap(originalBmp);

                            using (PrivateFontCollection pfcoll = new PrivateFontCollection())
                            {
                                var fontName = "ocr-b.ttf";
                                //put a font file under a Fonts directory within your application root
                                pfcoll.AddFontFile($@"{Environment.CurrentDirectory}\Assets\Fonts\" + fontName);
                                System.Drawing.FontFamily ff = pfcoll.Families[0];

                                foreach (Customer row in rows)
                                {
                                    using (Graphics graphics = Graphics.FromImage(tempBmp))
                                    {
                                        using (Font arialFont = new Font("Arial", 35))
                                        {
                                            using (Font ocr = new Font(ff, 28))
                                            {
                                                switch (logo == null)
                                                {
                                                    case false:
                                                        {
                                                            // Logo Max Height = 230px Max Lenght = 1725
                                                            graphics.DrawImage(logo, new Point(10, 10));
                                                            break;
                                                        }
                                                }

                                                var taxLessprice = "";
                                                var tax = 0.0d;
                                                var taxedPrice = "";

                                                switch (containPriceCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {

                                                            taxLessprice = priceText.Replace(',', '.');
                                                            tax = Math.Round((double.Parse(taxLessprice) * 0.25), 2);
                                                            taxedPrice = (Math.Round((double.Parse(taxLessprice) + tax), 2).ToString());
                                                            break;
                                                        }
                                                }

                                                //Top

                                                //Name & Address
                                                graphics.DrawString($"" +
                                                    $"{row.Customer_FIRSTNAME} " +
                                                    $"{row.Customer_LASTNAME}\n" +
                                                    $"{row.Customer_ADDRESS}\n" +
                                                    $"{row.Customer_ZIPCODE} " +
                                                    $"{row.Customer_CITY}\n" +
                                                    $"", arialFont, System.Drawing.Brushes.Black, point);

                                                //Company Address Line
                                                point.X = 910;
                                                point.Y = 300;
                                                var address = PropertiesExtension.Get<string>("AddressLine");

                                                graphics.DrawString($"" +
                                                    $"{address.Replace("+", "\n")}", arialFont, System.Drawing.Brushes.Black, point);

                                                var numb = invoiceNumber;

                                                point.Y += 200;
                                                graphics.DrawString($"Fakturadato...: {payDate}", arialFont, System.Drawing.Brushes.Black, point);
                                                point.Y += 50;
                                                graphics.DrawString($"Fakturanr.......: {numb}", arialFont, System.Drawing.Brushes.Black, point);
                                                point.Y += 50;

                                                if (containPayDate.IsChecked == false)
                                                {
                                                    invoiceDate = "";
                                                }

                                                graphics.DrawString($"Betalingsdato.: {invoiceDate}", arialFont, System.Drawing.Brushes.Black, point);

                                                point.Y = 720;
                                                point.X = 60;

                                                switch (includeProductsCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            //Products

                                                            foreach (ServiceProduct obj in productList.Items)
                                                            {
                                                                graphics.DrawString($"{obj.Name}", arialFont, System.Drawing.Brushes.Black, point);
                                                                point.X += 1350;
                                                                graphics.DrawString($"{obj.Price}", arialFont, System.Drawing.Brushes.Black, point);
                                                                point.X = 60;
                                                                point.Y += 55;
                                                            }
                                                            break;
                                                        }
                                                }

                                                point.X += 1340;

                                                switch (containPriceCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            Pen pen = new Pen(Color.Black)
                                                            {
                                                                Width = 3
                                                            };

                                                            switch (containTaxPriceCheck.IsChecked == false)
                                                            {
                                                                case true:
                                                                    {
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 20;
                                                                        graphics.DrawString($"{taxLessprice}", arialFont, System.Drawing.Brushes.Black, point);
                                                                        point.Y += 50;
                                                                        point.X -= 20;
                                                                        switch (tax.ToString().Length > 6)
                                                                        {
                                                                            case true:
                                                                                {
                                                                                    point.X -= 35;
                                                                                    graphics.DrawString($"+{tax} moms", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    point.X += 35;
                                                                                    break;
                                                                                }
                                                                            default:
                                                                                {
                                                                                    graphics.DrawString($"+{tax} moms", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    break;
                                                                                }
                                                                        }
                                                                        point.X += 20;
                                                                        point.Y += 50;

                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 25;
                                                                        switch (taxedPrice.Length > 6)
                                                                        {
                                                                            case true:
                                                                                {
                                                                                    point.X -= 30;
                                                                                    graphics.DrawString($"{taxedPrice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    point.X += 30;
                                                                                    break;
                                                                                }
                                                                            default:
                                                                                {                                                                                    
                                                                                    graphics.DrawString($"{taxedPrice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    break;
                                                                                }
                                                                        }
                                                                        point.Y += 55;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 7;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 25;
                                                                        switch (taxLessprice.Length > 6)
                                                                        {
                                                                            case true:
                                                                                {
                                                                                    point.X -= 30;
                                                                                    graphics.DrawString($"{taxLessprice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    point.X += 30;
                                                                                    break;
                                                                                }
                                                                            default:
                                                                                {
                                                                                    graphics.DrawString($"{taxLessprice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                                    break;
                                                                                }
                                                                        }
                                                                        point.Y += 55;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 7;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                }

                                                //Message
                                                point.X = 50;
                                                point.Y = 1920;
                                                var message = PropertiesExtension.Get<string>("InvoiceMessage").Replace("+", "\n");
                                                graphics.DrawString($"{message}", arialFont, System.Drawing.Brushes.Black, point);

                                                //Transfer
                                                point.X = 1070;
                                                point.Y = 2020;

                                                graphics.DrawString($"Betalingsdato: {invoiceDate}", arialFont, System.Drawing.Brushes.Black, point);

                                                var reg = PropertiesExtension.Get<string>("Regnr");
                                                var account = PropertiesExtension.Get<string>("Accountnr");
                                                point.Y += 60;

                                                graphics.DrawString($"HUSK: Fakturanr i overførselen.", arialFont, System.Drawing.Brushes.Black, point);
                                                point.Y += 55;
                                                graphics.DrawString($"Reg: {reg} - Konto: {account}", ocr, System.Drawing.Brushes.Black, point);
                                            }
                                        }
                                    }
                                }
                            }
                            printPath = outputPath + @"\FilerTilPrinter";

                            FileSystemHelper.CreateFolder(printPath);

                            var outputFileName = $@"\NewPrintFile.png";

                            FileSystemHelper.CreateImage(printPath + outputFileName, tempBmp);

                            tempBmp.Dispose();

                            PdfHelper.ConvertImageToPdf(printPath, outputPath, $@"\" + VariableManipulation.CorrectFileName($@"{rows[0].Customer_ADDRESS}_{num}_Faktura"), "Invoice", "Er dokumentet åbent?");

                            FileSystemHelper.DeleteFolderAndContent(printPath);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Giro
        /// <summary>Sets up the giro print.</summary>
        /// <param name="priceText">The price text.</param>
        /// <param name="payDate">The pay date.</param>
        /// <param name="creditorNumber">The creditor number.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="CustomerID">The customer identifier.</param>
        /// <param name="prepCombo">The prep combo.</param>
        /// <param name="productList">The product list.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SetupGiroPrint(string priceText, string payDate, long num, int CustomerID, System.Windows.Controls.CheckBox ContainPriceCheck, System.Windows.Controls.CheckBox ContainPayDateCheck, System.Windows.Controls.CheckBox ContainTaxPriceCheck, System.Windows.Controls.ListView productList, System.Windows.Controls.CheckBox includeProductsCheck, string imageFilePath = "", string fileName = "A5 Template.PNG")
        {
            try
            {
                var PrintPath = "";

                var query = $"SELECT * FROM `customers` WHERE Customer_ID = {CustomerID};";

                var rows = MySqlHelper.GetDataFromDatabase<Customer>(query, "ConnString");

                switch (string.IsNullOrWhiteSpace(imageFilePath))
                {
                    case true:
                        {
                            imageFilePath = $@"{Environment.CurrentDirectory}\Assets\Invoice Template\";
                            break;
                        }
                }

                //load the image file
                var originalBmp = (Bitmap)System.Drawing.Image.FromFile(imageFilePath + fileName);

                switch (FileSystemHelper.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    case true:
                        {
                            var outputPath = FileSystemHelper.folderBrowserDialog.SelectedPath;
                            var point = new PointF(35, 350);
                            Bitmap tempBmp = new Bitmap(originalBmp);

                            using (PrivateFontCollection pfcoll = new PrivateFontCollection())
                            {
                                var fontName = "ocr-b.ttf";
                                //put a font file under a Fonts directory within your application root
                                pfcoll.AddFontFile($@"{Environment.CurrentDirectory}\Assets\Fonts\" + fontName);
                                System.Drawing.FontFamily ff = pfcoll.Families[0];

                                foreach (Customer row in rows)
                                {
                                    using (Graphics graphics = Graphics.FromImage(tempBmp))
                                    {

                                        graphics.Clear(Color.White);

                                        using (Font arialFont = new Font("Arial", 35))
                                        {
                                            using (Font ocr = new Font(ff, 36))
                                            {
                                                #region Standard Info

                                                var taxLessprice = "";
                                                var tax = 0.0d;
                                                var taxedPrice = "";
                                                var cents = "";
                                                var dollars = "";

                                                switch (ContainPriceCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {

                                                            taxLessprice = priceText.Replace(',', '.');
                                                            tax = Math.Round((double.Parse(taxLessprice) * 0.25), 2);
                                                            taxedPrice = (Math.Round((double.Parse(taxLessprice) + tax), 2).ToString());

                                                            switch (ContainTaxPriceCheck.IsChecked == false)
                                                            {
                                                                case true:
                                                                    {
                                                                        cents = taxedPrice.Replace(",", ".").Split('.')[1];
                                                                        dollars = taxedPrice.Replace(",", ".").Split('.')[0];
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        cents = taxLessprice.Replace(",", ".").Split('.')[1];
                                                                        dollars = taxLessprice.Replace(",", ".").Split('.')[0];
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                }
                                                //Top

                                                //Name & Address
                                                graphics.DrawString($"" +
                                                    $"{row.Customer_FIRSTNAME} " +
                                                    $"{row.Customer_LASTNAME}\n" +
                                                    $"{row.Customer_ADDRESS}\n" +
                                                    $"{row.Customer_ZIPCODE} " +
                                                    $"{row.Customer_CITY}\n" +
                                                    $"", arialFont, System.Drawing.Brushes.Black, point);

                                                point.X = 710;
                                                point.Y = 895;

                                                switch (ContainPriceCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            //Price

                                                            switch (cents.Length == 1)
                                                            {
                                                                case true:
                                                                    {
                                                                        cents += "0";
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        switch (cents.Length == 0)
                                                                        {
                                                                            case true:
                                                                                {
                                                                                    cents += "00";
                                                                                    break;
                                                                                }
                                                                        }
                                                                        break;
                                                                    }
                                                            }
                                                            

                                                            graphics.DrawString($"{cents.ToCharArray()[1]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X -= 60;
                                                            graphics.DrawString($"{cents.ToCharArray()[0]}", ocr, System.Drawing.Brushes.Black, point);

                                                            point.X -= 90;
                                                            foreach (char ch in dollars.ToCharArray().Reverse())
                                                            {
                                                                graphics.DrawString($"{ch}", ocr, System.Drawing.Brushes.Black, point);
                                                                point.X -= 60;
                                                            }
                                                            break;
                                                        }
                                                }

                                                switch (ContainPayDateCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            //Date
                                                            var day = payDate.Split('-')[0];
                                                            var month = GetMonthFormat(payDate.Split('-')[1]);
                                                            var year = payDate.Split('-')[2];

                                                            point.X = 987;

                                                            //Day
                                                            graphics.DrawString($"{day.ToCharArray()[0]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X += 60;
                                                            graphics.DrawString($"{day.ToCharArray()[1]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X += 60;

                                                            //Month
                                                            graphics.DrawString($"{month.ToCharArray()[0]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X += 60;
                                                            graphics.DrawString($"{month.ToCharArray()[1]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X += 60;
                                                            //Year
                                                            graphics.DrawString($"{year.ToCharArray()[0]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X += 60;
                                                            graphics.DrawString($"{year.ToCharArray()[1]}", ocr, System.Drawing.Brushes.Black, point);
                                                            break;
                                                        }
                                                }

                                                switch (includeProductsCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            //Products
                                                            point.Y = 1320;
                                                            point.X = 60;

                                                            foreach (ServiceProduct obj in productList.Items)
                                                            {
                                                                graphics.DrawString($"{obj.Name}", arialFont, System.Drawing.Brushes.Black, point);
                                                                point.X += 510;
                                                                graphics.DrawString($"{obj.Price}", arialFont, System.Drawing.Brushes.Black, point);
                                                                point.X = 60;
                                                                point.Y += 55;
                                                            }
                                                            point.X += 500;

                                                            Pen pen = new Pen(Color.Black)
                                                            {
                                                                Width = 3
                                                            };

                                                            switch (ContainTaxPriceCheck.IsChecked == false)
                                                            {
                                                                case true:
                                                                    {
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 20;
                                                                        graphics.DrawString($"{taxLessprice}", arialFont, System.Drawing.Brushes.Black, point);
                                                                        point.Y += 50;
                                                                        point.X -= 20;
                                                                        graphics.DrawString($"+{tax} (moms)", arialFont, System.Drawing.Brushes.Black, point);
                                                                        point.X += 20;
                                                                        point.Y += 50;

                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 25;
                                                                        graphics.DrawString($"{taxedPrice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                        point.Y += 55;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 7;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 25;
                                                                        graphics.DrawString($"{taxLessprice},- DKK", arialFont, System.Drawing.Brushes.Black, point);
                                                                        point.Y += 55;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        point.Y += 7;
                                                                        graphics.DrawLine(pen, point.X, point.Y, point.X + 200, point.Y);
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            //Lines
                                                            point.Y = 1320;
                                                            point.X = 60;

                                                            graphics.DrawString($"Skorsten: ", arialFont, System.Drawing.Brushes.Black, point);
                                                            point.Y += 130;
                                                            graphics.DrawString($"Syn: ", arialFont, System.Drawing.Brushes.Black, point);
                                                            point.Y += 130;
                                                            graphics.DrawString($"Moms: ", arialFont, System.Drawing.Brushes.Black, point);
                                                            break;
                                                        }
                                                }

                                                //Bottom
                                                point.X = 1000;
                                                point.Y = 1750;

                                                graphics.DrawString($"" +
                                                    $"{row.Customer_FIRSTNAME} " +
                                                    $"{row.Customer_LASTNAME}\n" +
                                                    $"{row.Customer_ADDRESS}\n" +
                                                    $"{row.Customer_ZIPCODE} " +
                                                    $"{row.Customer_CITY}\n" +
                                                    $"", arialFont, System.Drawing.Brushes.Black, point);

                                                switch (ContainPriceCheck.IsChecked == true)
                                                {
                                                    case true:
                                                        {
                                                            point.X = 1665;
                                                            point.Y = 2030;
                                                            graphics.DrawString($"{cents.ToCharArray()[1]}", ocr, System.Drawing.Brushes.Black, point);
                                                            point.X -= 60;
                                                            graphics.DrawString($"{cents.ToCharArray()[0]}", ocr, System.Drawing.Brushes.Black, point);


                                                            point.X -= 90;
                                                            foreach (char numb in dollars.ToCharArray().Reverse())
                                                            {
                                                                graphics.DrawString($"{numb}", ocr, System.Drawing.Brushes.Black, point);
                                                                point.X -= 60;
                                                            }
                                                            break;
                                                        }
                                                }
                                                #endregion

                                                //Company Address Line
                                                point.X = 1040;
                                                point.Y = 250;
                                                var address = PropertiesExtension.Get<string>("AddressLine");
                                            }
                                        }
                                    }
                                }
                            }
                            PrintPath = outputPath + @"\FilerTilPrinter";

                            FileSystemHelper.CreateFolder(PrintPath);

                            var outputFileName = $@"\NewPrintFile.png";

                            FileSystemHelper.CreateImage(PrintPath + outputFileName, tempBmp);

                            tempBmp.Dispose();

                            PdfHelper.ConvertImageToPdf(PrintPath, outputPath, $@"\" + VariableManipulation.CorrectFileName($@"{rows[0].Customer_ADDRESS}_{num}_Giro"), "Giro", "Er dokumentet åbent?");

                            FileSystemHelper.DeleteFolderAndContent(PrintPath);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>Sorts the Rows in a DataTable.</summary>
        /// <param name="dataRows">The data rows.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Sorted data rows</returns>
        private static EnumerableRowCollection<DataRow> SortDataRows(DataTable dataRows, string columnName)
        {
            try
            {
                var collection = dataRows.AsEnumerable();
                return collection.OrderBy(r => r[columnName]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>Gets the split tables after sorting.</summary>
        /// <param name="oriTable">The ori table.</param>
        /// <param name="splitByColumnName">Name of the split by column.</param>
        /// <returns>Sorted DataTable Array</returns>
        public static DataTable[] GetSplitTabels(DataTable oriTable, string splitByColumnName)
        {
            try
            {
                List<DataTable> tables = new List<DataTable>();

                var rows = SortDataRows(oriTable, splitByColumnName);

                var PreviousCityName = "";
                var newTable = oriTable.Clone();
                newTable.Clear();
                oriTable.Dispose();

                int i = 0;
                foreach (DataRow row in rows)
                {
                    var newRow = row;
                    switch (i == 0)
                    {
                        case true:
                            {
                                newTable.TableName = newRow["By"].ToString();
                                break;
                            }
                    }

                    switch (newRow["By"].ToString().Replace(" ", "") == PreviousCityName)
                    {
                        case true:
                            {
                                PreviousCityName = newRow["By"].ToString().Replace(" ", "");

                                newTable.ImportRow(newRow);
                                break;
                            }
                        default:
                            {
                                PreviousCityName = newRow["By"].ToString().Replace(" ", "");

                                tables.Add(newTable);

                                newTable = newTable.Clone();
                                newTable.Clear();
                                newTable.TableName = newRow["By"].ToString();

                                newTable.ImportRow(newRow);
                                break;
                            }
                    }
                    i++;
                }
                tables.Add(newTable);

                return tables.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


        /// <summary>Gets the month format.</summary>
        /// <param name="input">The Months short name.</param>
        /// <returns>Month number</returns>
        private static string GetMonthFormat(string input)
        {
            var returnValue = "";

            try
            {
                switch (input)
                {
                    case "Jan":
                    {
                        return returnValue = "01";
                    }
                    case "Feb":
                    {
                        return returnValue = "02";
                    }
                    case "Mar":
                    {
                        return returnValue = "03";
                    }
                    case "Apr":
                    {
                        return returnValue = "04";
                    }
                    case "May":
                    {
                        return returnValue = "05";
                    }
                    case "Jun":
                    {
                        return returnValue = "06";
                    }
                    case "Jul":
                    {
                        return returnValue = "07";
                    }
                    case "Aug":
                    {
                        return returnValue = "08";
                    }
                    case "Sep":
                    {
                        return returnValue = "09";
                    }
                    case "Oct":
                    {
                        return returnValue = "10";
                    }
                    case "Nov":
                    {
                        return returnValue = "11";
                    }
                    case "Dec":
                    {
                        return returnValue = "12";
                    }
                    default:
                    {
                        return returnValue = input;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return returnValue;
        }

        /// <summary>Returns true if line length is greater than maxlenght</summary>
        /// <param name="maxLength">Maximum line length</param>
        /// /// <param name="str">string with lines</param>
        /// <returns>boolean</returns>
        private static bool MessureLineLenthInString(int maxLength, string str)
        {
            var array = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach(string line in array)
            {
                switch (line.Length >= maxLength)
                {
                    case true:
                        {
                            return true;
                        }
                }
            }
            return false;
        }

        private static bool CountLinesInString(int maxLines, string str)
        {
            var array = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            switch (array.Count() > maxLines)
            {
                case true:
                    {
                        return true;
                    }
            }

            return false;
        }
    }
}
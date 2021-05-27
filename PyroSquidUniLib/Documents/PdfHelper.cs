using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PyroSquidUniLib.Extensions;
using PyroSquidUniLib.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PyroSquidUniLib.Documents
{
    class AsyncPdfHelper
    {

        #region Images

        /// <summary>Converts a single bitmap image to a single PDF file</summary>
        /// <param name="imagesPath">The images path.</param>
        /// <param name="invoiceType">The Invoice Type</param>
        public static async void ConvertImageToPdfAsync(string imagesPath, string deploypath, string FileName, string invoiceType = "None", string IOerrorMessage = "Is the printed document is already open?")
        {
            try
            {
                string num;
                string pastePath;

                switch (invoiceType != "None")
                {
                    case true:
                        {
                            num = PropertiesExtension.Get<string>("LastInvoiceNum");
                            break;
                        }
                }

                pastePath = deploypath + FileName + ".pdf";


                //Convert folder with images to PDF files
                PdfDocument doc = new PdfDocument();

                var files = FileSystemHelper.GetFilePathsFromFolderAsArray(imagesPath);

                XSize size = new XSize(420, 595);
                var page = new PdfPage()
                {
                    Size = PageSize.A4
                };

                switch (invoiceType != "None")
                {
                    case true:
                        {
                            switch (invoiceType == "Giro")
                            {
                                case true:
                                    {
                                        page.Size = PageSize.A5;
                                        break;
                                    }
                                default:
                                    {
                                        switch (invoiceType == "Invoice")
                                        {
                                            case true:
                                                {
                                                    size = new XSize(595, 842); //Sets dimentions
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }
                            
                            break;
                        }
                    default:
                        {
                            page.Orientation = PageOrientation.Landscape;
                            size = new XSize(842, 595); //Sets dimentions
                            break;
                        }
                }

                var point = new XPoint(0, 0);
                var rect = new XRect(point, size);

                doc.Pages.Add(page);

                using (XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]))
                {
                    using (XImage img = XImage.FromFile(files[0]))
                    {
                        xgr.DrawImage(img, rect);
                    }
                }
                doc.Save(pastePath);
                doc.Close();

                //Show produced PDF in Acrobat Reader
                System.Diagnostics.Process.Start(pastePath);
                await Task.FromResult(true);
            }
            catch (IOException IOEx)
            {
                Console.WriteLine(IOEx);
                MessageBox.Show(IOerrorMessage);
                await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Console.WriteLine(ex);
            }
        }

        /// <summary>Converts an array of bitmap images to a single PDF file</summary>
        /// <param name="imagesPath">The images path.</param>
        public static async void ConvertImageArrayToPdfAsync(string imagesPath, string deploypath, string FileName, string IOerrorMessage = "Is the printed document is already open?")
        {
            try
            {
                var pastePath = deploypath + FileName + ".pdf";

                //Convert folder with images to PDF files
                PdfDocument doc = new PdfDocument();

                var files = FileSystemHelper.GetFilePathsFromFolderAsArray(imagesPath);

                var size = new XSize(842, 595); //Sets dimentions
                var point = new XPoint(0, 0);
                var rect = new XRect(point, size);

                int i = 0;
                foreach (string path in files.OrderBy(x => int.Parse(String.Join("", x.Split('\\')[x.Split('\\').Length - 1].ToCharArray().Where(Char.IsDigit)))))
                {

                    var page = new PdfPage()
                    {
                        Orientation = PageOrientation.Landscape,
                        Size = PageSize.A4
                    };

                    doc.Pages.Add(page);

                    using (XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[i]))
                    {
                        using (XImage img = XImage.FromFile(path))
                        {
                            xgr.DrawImage(img, rect);
                        }
                    }

                    i++;
                }
                doc.Save(pastePath);
                doc.Close();

                //Show produced PDF in Acrobat Reader
                System.Diagnostics.Process.Start(pastePath);
                await Task.FromResult(true);
            }
            catch (IOException IOEx)
            {
                Console.WriteLine(IOEx);
                MessageBox.Show(IOerrorMessage);
                await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                await Task.FromResult(false);
                Console.WriteLine(ex);
            }
        }

        #endregion

    }

    class PdfHelper
    {
        #region Images

        /// <summary>Converts a single bitmap image to a single PDF file</summary>
        /// <param name="imagesPath">The images path.</param>
        /// <param name="invoiceType">The Invoice Type</param>
        public static void ConvertImageToPdf(string imagesPath, string deploypath, string FileName, string invoiceType = "None", string IOerrorMessage = "Is the printed document is already open?")
        {
            try
            {
                string num;
                string pastePath;

                switch (invoiceType != "None")
                {
                    case true:
                        {
                            num = PropertiesExtension.Get<string>("LastInvoiceNum");
                            break;
                        }
                }

                pastePath = deploypath + FileName + ".pdf";


                //Convert folder with images to PDF files
                PdfDocument doc = new PdfDocument();

                var files = FileSystemHelper.GetFilePathsFromFolderAsArray(imagesPath);

                XSize size = new XSize(420, 595);
                var page = new PdfPage()
                {
                    Size = PageSize.A4
                };

                switch (invoiceType != "None")
                {
                    case true:
                        {
                            switch (invoiceType == "Giro")
                            {
                                case true:
                                    {
                                        page.Size = PageSize.A5;
                                        break;
                                    }
                                default:
                                    {
                                        switch (invoiceType == "Invoice")
                                        {
                                            case true:
                                                {
                                                    size = new XSize(595, 842); //Sets dimentions
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            break;
                        }
                    default:
                        {
                            page.Orientation = PageOrientation.Landscape;
                            size = new XSize(842, 595); //Sets dimentions
                            break;
                        }
                }

                var point = new XPoint(0, 0);
                var rect = new XRect(point, size);

                doc.Pages.Add(page);

                using (XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]))
                {
                    using (XImage img = XImage.FromFile(files[0]))
                    {
                        xgr.DrawImage(img, rect);
                    }
                }
                doc.Save(pastePath);
                doc.Close();

                //Show produced PDF in Acrobat Reader
                System.Diagnostics.Process.Start(pastePath);
            }
            catch (IOException IOEx)
            {
                Console.WriteLine(IOEx);
                MessageBox.Show(IOerrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>Converts an array of bitmap images to a single PDF file</summary>
        /// <param name="imagesPath">The images path.</param>
        public static void ConvertImageArrayToPdf(string imagesPath, string deploypath, string FileName, string IOerrorMessage = "Is the printed document is already open?")
        {
            try
            {
                var pastePath = deploypath + FileName + ".pdf";

                //Convert folder with images to PDF files
                PdfDocument doc = new PdfDocument();

                var files = FileSystemHelper.GetFilePathsFromFolderAsArray(imagesPath);

                var size = new XSize(842, 595); //Sets dimentions
                var point = new XPoint(0, 0);
                var rect = new XRect(point, size);

                int i = 0;
                foreach (string path in files.OrderBy(x => int.Parse(String.Join("", x.Split('\\')[x.Split('\\').Length - 1].ToCharArray().Where(Char.IsDigit)))))
                {

                    var page = new PdfPage()
                    {
                        Orientation = PageOrientation.Landscape,
                        Size = PageSize.A4
                    };

                    doc.Pages.Add(page);

                    using (XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[i]))
                    {
                        using (XImage img = XImage.FromFile(path))
                        {
                            xgr.DrawImage(img, rect);
                        }
                    }

                    i++;
                }
                doc.Save(pastePath);
                doc.Close();

                //Show produced PDF in Acrobat Reader
                System.Diagnostics.Process.Start(pastePath);
            }
            catch (IOException IOEx)
            {
                Console.WriteLine(IOEx);
                MessageBox.Show(IOerrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

    }
}

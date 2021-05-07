using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PyroSquidUniLib.WPFControls
{
    public class TextBoxExtrasHelper
    {
        #region Lines

        #region Line Amount

        public static void FixedLineAmount(int maxLines, object textBox)
        {
            TextBox box = textBox as TextBox;

            FixedLineAmount(maxLines, box);
        }

        public static void FixedLineAmount(int maxLines, TextBox textBox)
        {
            var array = textBox.Text.Split(new string[] { "\n" }, StringSplitOptions.None);

            try
            {
                // Gives textbox maxlines if maxLines is 0
                if (array.Length > maxLines && maxLines > 0)
                {
                    array = array.Take(maxLines).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            textBox.Text = String.Join("\n", array);
            textBox.SelectionStart = textBox.Text.Length;
        }

        #endregion

        #region Line Lenght

        public static void FixedLineLength(int maxChars, object textBox)
        {
            TextBox box = textBox as TextBox;

            FixedLineLength(maxChars, box);
        }

        public static void FixedLineLength(int maxChars, TextBox textBox)
        {
            var array = textBox.Text.Split(new string[] { "\n" }, StringSplitOptions.None);

            try
            {
                int i = 0;
                foreach (string str in array)
                {
                    if (str.Length > maxChars)
                    {
                        if (!array[i].Contains("\n"))
                        {
                            array[i] = str.Remove(str.Length - 1, 1);
                        }
                    }
                    i++;
                }

                textBox.Text = String.Join("\n", array);
                textBox.SelectionStart = textBox.Text.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #endregion
    }
}

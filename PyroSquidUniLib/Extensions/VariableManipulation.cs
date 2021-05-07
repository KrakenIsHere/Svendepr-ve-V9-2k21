using PyroSquidUniLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PyroSquidUniLib.Extensions
{
    public static class VariableManipulation
    {
        #region General Arrays

        public static T[] RemoveAtIndex<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            switch (index > 0)
            {
                case true:
                    {
                        Array.Copy(source, 0, dest, 0, index);
                        break;
                    }
            }

            switch (index < source.Length - 1)
            {
                case true:
                    {
                        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
                        break;
                    }
            }
            return dest;
        }

        public static T[] RemoveAllWithValue<T>(this T[] source, T value)
        {
            T[] values = source.Where(val => !EqualityComparer<T>.Default.Equals(val, value)).ToArray();

            return values;
        }

        #endregion

        #region DataTable

        #region Remove Rows

        public static void RemoveRowsFromDataTableWithValue(DataTable table, string ColumnName, string value)
        {
            try
            {
                foreach (DataRow row in table.Select())
                {
                    try
                    {
                        switch (row[ColumnName].ToString() == value)
                        {
                            case true:
                                {
                                    table.Rows.Remove(row);
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                table.AcceptChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void RemoveRowsFromDataTableWhereValuesApear(DataTable table, string ColumnName, string[] values)
        {
            try
            {
                foreach (DataRow row in table.Select())
                {
                    foreach (string str in values)
                    {
                        try
                        {
                            switch (row[ColumnName].ToString() == str)
                            {
                                case true:
                                    {
                                        table.Rows.Remove(row);
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                table.AcceptChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static DataTable GetDataTableFromListView(ListView list)
        {
            try
            {
                DataTable dt = new DataTable();
                bool createColumns = true;
                foreach (var eachobj in list.ItemsSource)
                {
                    Type t = eachobj.GetType();
                    PropertyInfo[] propInfos = t.GetProperties();
                    if (createColumns)
                    {
                        foreach (PropertyInfo eachProp in propInfos)
                        {
                            dt.Columns.Add(eachProp.Name, typeof(object));
                        }
                        createColumns = false;
                    }
                    object[] data = new object[propInfos.Length];
                    for (int i = 0; i < propInfos.Length; i++)
                    {
                        PropertyInfo propInfo = propInfos[i];
                        data[i] = propInfo.GetValue(eachobj, null);
                    }
                    dt.Rows.Add(data);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static int[] GetIntegerValuesInColumnFromListView(ListView list)
        {
            try
            {
                List<int> intList = new List<int>();

                foreach (RouteCustomer item in list.Items)
                {
                    intList.Add(int.Parse(item.ID));
                }

                return intList.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static async void RemoveRowsFromDataTableWhereIntValueIsSingleRow(DataTable table, string ColumnName, int[] values)
        {
            try
            {
                var deletedRowsAmount = 0;

                values = SortIntegerArray(values);

                var totalValLenght = values.Length;

                foreach (DataRow row in table.Rows)
                {
                    foreach (int num in values)
                    {
                        //Console.WriteLine(deletedRowsAmount + " : " + String.Join(", ", values));

                        try
                        {
                            switch (deletedRowsAmount >= totalValLenght)
                            {
                                case true:
                                    {
                                        table.AcceptChanges();
                                        return;
                                    }
                            }
                            if (row[ColumnName].ToString() == num.ToString())
                            {
                                row.Delete();
                                values = values.RemoveAllWithValue(num);
                                deletedRowsAmount++;
                                break;
                            }
                        }
                        catch (DeletedRowInaccessibleException)
                        {
                            //It Works!!!
                        }
                        catch (RowNotInTableException)
                        {
                            //Do Nothing
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                table.AcceptChanges();
                await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Task.FromResult(false);
            }
        }

        public static void RemoveRowsFromDataTableWhereStringValueIsSingleRow(DataTable table, string ColumnName, string[] values)
        {
            try
            {
                var deletedRowsAmount = 0;
                foreach (DataRow row in table.Select())
                {
                    foreach (string str in values)
                    {
                        try
                        {
                            switch (deletedRowsAmount >= values.Length)
                            {
                                case true:
                                    {
                                        table.AcceptChanges();
                                        return;
                                    }
                            }
                            switch (row[ColumnName].ToString() == str)
                            {
                                case true:
                                    {
                                        table.Rows.Remove(row);
                                        deletedRowsAmount++;
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                table.AcceptChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #endregion

        #region Ingeters

        #region Arrays

        public static int[] SortIntegerArray(int[] array)
        {
            List<int> list = new List<int>();

            list.AddRange(array);

            list.Sort();

            return list.ToArray();
        }

        #endregion

        #endregion

        #region Strings

        #region Files

        public static string CorrectFileName(string fileName)
        {
            fileName = fileName
                .Replace("<", "-")
                .Replace(">", "-")
                .Replace(":", "-")
                .Replace("\"", "-")
                .Replace("/", "-")
                .Replace("|", "-")
                .Replace("?", "-")
                .Replace("*", "-");

            return fileName;
        }

        #endregion

        private static IEnumerable<string> GraphemeClusters(this string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }
        public static string ReverseGraphemeClusters(this string s) //Reverse a string
        {
            return string.Join("", s.GraphemeClusters().Reverse().ToArray());
        }

        #endregion

        #region DataGrids

        public static DataTable DataGridtoDataTable(System.Windows.Controls.DataGrid dg)
        {
            try
            {
                dg.SelectAllCells();
                dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, dg);
                dg.UnselectAllCells();
                String result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);
                string[] Lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                DataTable dt = new DataTable();

                //1st row must be column names; force lower case to ensure matching later on.
                for (int i = 0; i < Cols; i++)
                    dt.Columns.Add(Fields[i].ToUpper(), typeof(string));
                DataRow Row;
                for (int i = 1; i < Lines.GetLength(0) - 1; i++)
                {
                    Fields = Lines[i].Split(new char[] { ',' });
                    Row = dt.NewRow();
                    for (int f = 0; f < Cols; f++)
                    {
                        Row[f] = Fields[f];
                    }
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            catch (NullReferenceException NRex)
            {
                Console.WriteLine(NRex);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region Sorting Data

        #region DataTable Output

        public static DataTable SortDataTable(DataTable table, string sortingColumn)
        {
            try
            {
                var dv = table.DefaultView;
                dv.Sort = sortingColumn;
                return dv.ToTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        public static DataTable SortDataSetToTable(DataSet set, string sortingColumn)
        {
            try
            {
                var dv = set.Tables[0].DefaultView;
                dv.Sort = sortingColumn;
                return dv.ToTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        #endregion

        #region DataView Output
        public static DataView SortDataSetToView(DataSet set, string sortingColumn)
        {
            try
            {
                var dv = set.Tables[0].DefaultView;
                dv.Sort = sortingColumn;
                return dv;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static DataView SortDataTableToView(DataTable table, string sortingColumn)
        {
            try
            {
                var dv = table.DefaultView;
                dv.Sort = sortingColumn;
                return dv;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region String Array Output
        public static string[] SortDataSetToStringArray(DataSet set, string sortingColumn, string outputColumn)
        {
            try
            {
                var list = new List<string>();

                var dv = set.Tables[0].DefaultView;
                dv.Sort = sortingColumn;
                dv.ToTable();

                foreach (DataRow row in dv.ToTable().Rows)
                {
                    list.Add(row[outputColumn].ToString());
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        public static string[] SortDataTableToStringArray(DataTable table, string sortingColumn, string outputColumn)
        {
            try
            {
                var list = new List<string>();

                var dv = table.DefaultView;
                dv.Sort = sortingColumn;
                dv.ToTable();

                foreach (DataRow row in dv.Table.Rows)
                {
                    list.Add(row[outputColumn].ToString());
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        #endregion

        #endregion

    }
}

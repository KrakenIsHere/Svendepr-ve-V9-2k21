using System;


namespace PyroSquidUniLib.Extensions
{
    public class PropertiesExtension
    {
        /// <summary>Save variable.</summary>
        /// <param name="key">Name of the variable in the properties section.</param>
        /// <param name="value">The saved content.</param>
        public static void Set(string key, object value)
        {
            try
            {
                PyroSquidUniLib.Properties.Settings.Default[key] = value;
                PyroSquidUniLib.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>Get variable</summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="key">Name of the variable you want to fetch.</param>
        /// <returns>Returns the variable from the properties section.</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return (T)PyroSquidUniLib.Properties.Settings.Default[key];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return default;
            }
        }
    }
}

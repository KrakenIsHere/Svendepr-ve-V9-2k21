using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace PyroSquidUniLib.Verification
{
    public class RegHelper
    {
        #region Registry Key section

        /// <summary>Add a new key</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        public static void AddRegKey(string key, string subKey)
        {
            if (CheckKeyExistence(key, "")) return;
            using (var regKey = Registry.CurrentUser.OpenSubKey(key, true))
            {
                regKey?.CreateSubKey(subKey);
            }
        }

        /// <summary>Delete the specified key</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        public static void DeleteRegKey(string key, string subKey)
        {
            if (CheckKeyExistence(key, "")) return;
            using (var regKey = Registry.CurrentUser.OpenSubKey(key, true))
            {
                regKey?.DeleteSubKey(subKey);
            }
        }


        /// <summary>Check the existence of a specified key</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        /// <returns>Return true or false, telling us if the key exists</returns>
        public static bool CheckKeyExistence(string key, string subKey)
        {
            var tempBool = false;
            var subKeys = new List<string>();

            using (var regKey = Registry.CurrentUser.OpenSubKey(key))
            {
                if (regKey == null)
                {
                    Console.WriteLine($"Registry key '{key}' was not found!");
                }

                subKeys.AddRange(regKey?.GetSubKeyNames() ?? throw new InvalidOperationException());

                switch (subKeys.Contains(subKey))
                {
                    case true:
                        tempBool = true;
                        break;

                    case false:
                        Console.WriteLine($"'{subKey}' was not found in '{key}'!");
                        break;
                }
            }

            return tempBool;
        }
        #endregion

        #region Registry Value Section
        /// <summary>With this method you can add a value or edit an existing one.</summary>
        /// <param name="keyPath">The KeyPath is the Key and Sub key combined eg. 'Software\Microsoft\Windows'</param>
        /// <param name="valueName">The name of the value you want to add/edit</param>
        /// <param name="value">type in your desired new value</param>
        /// <param name="regValueKind">Specifies the data type to use eg 'Binary, DWord, ExpandString, MultiString, None, QWord, String Or Unknown'</param>
        public static void SetRegValue(string keyPath, string valueName, string value, RegistryValueKind regValueKind)
        {
            var path = @"HKEY_CURRENT_USER\" + keyPath;

            Registry.SetValue(path, valueName, value, regValueKind);
        }

        /// <summary>Delete the specified value</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        /// <param name="value">The value that you want to delete in the key/sub key</param>
        public static void DeleteValue(string key, string subKey, string value)
        {
            if (CheckValueExistence(key, subKey, value) != true) return;
            using (var regKey = Registry.CurrentUser.OpenSubKey(key + subKey, true))
            {
                regKey?.DeleteValue(value);
            }
        }

        /// <summary>  Take a value and convert it to a readable string</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        /// <param name="value">The specified value that you want to read</param>
        /// <returns>Return a string containing the value</returns>
        public static string Readvalue(string key, string subKey, string value)
        {
            var tempString = "";

            if (CheckValueExistence(key, subKey, value) != true) return tempString;
            using (var regKey = Registry.CurrentUser.OpenSubKey(key + subKey, true))
            {
                tempString = regKey?.GetValue(value).ToString();
            }

            return tempString;
        }

        /// <summary>Check the existence of the specified value</summary>
        /// <param name="key">The key is the root path eg. 'Software\Microsoft\'</param>
        /// <param name="subKey">The sub key is the sub directory of our key eg. 'Windows'. Key + Sub key will then be 'Software\Microsoft\Windows'</param>
        /// <param name="value">The value in question</param>
        /// <returns>Return true or false, telling ud whether or not it exists</returns>
        public static bool CheckValueExistence(string key, string subKey, string value)
        {
            var tempBool = false;
            var values = new List<string>();

            if (CheckKeyExistence(key, subKey) != true) return false;

            using (var regKey = Registry.CurrentUser.OpenSubKey(key + subKey))
            {
                if (regKey == null)
                {
                    Console.WriteLine("Registry key '{RegKey}' was not found!", key + subKey);
                }

                values.AddRange(regKey.GetValueNames());

                switch (values.Contains(value))
                {
                    case true:
                        tempBool = true;
                        break;

                    case false:
                        Console.WriteLine("'{Value}' was not found in '{SubKey}'!", value, subKey);
                        break;
                }
            }

            return tempBool;
        }
        #endregion
    }
}

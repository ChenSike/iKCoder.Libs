using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;

namespace iKCoder_Platform_SDK_Kit
{
    public class Base_Registry
    {
        /// <summary>
        /// SetKeyForLocalMachineNode
        /// </summary>
        /// <param name="subNodePath">SAMPLE:SOFTWARE\Inspriation\MyKey</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetKeyForLocalMachineNode(string subNodePath, string valueKeyName, string value)
        {
            string keyValue = string.Empty;
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subNodePath, true))
                {
                    if (key == null)
                    {
                        using (RegistryKey myKey = Registry.LocalMachine.CreateSubKey(subNodePath))
                        {
                            myKey.SetValue(valueKeyName == "" ? "REGKEY" : valueKeyName, value);
                        }
                    }
                    else
                    {
                        key.SetValue(valueKeyName == "" ? "REGKEY" : valueKeyName, value);
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool GetKeyValueFromLocalMachineNode(string subNodePath, string valueKeyName,out string outValue)
        {
            try
            {
                using (RegistryKey currentKey = Registry.LocalMachine.OpenSubKey(subNodePath, false))
                {
                    if (currentKey == null)
                    {
                        outValue = "";
                        return false;
                    }
                    else
                    {
                        outValue = currentKey.GetValue(valueKeyName == "" ? "REGKEY" : valueKeyName).ToString();
                        return true;
                    }
                }
            }
            catch
            {
                outValue = "";
                return false;
            }
        }

    }
}

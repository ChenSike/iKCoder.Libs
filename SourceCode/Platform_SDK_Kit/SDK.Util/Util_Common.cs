using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace iKCoder_Platform_SDK_Kit
{
    public class Util_Common
    {
        public static void Action_ClearAllInputControls(Control.ControlCollection activeFormControls)
        {
            foreach (Control activeControl in activeFormControls)
            {
                Type activeType = activeControl.GetType();
                if (activeType.FullName.Contains("TextBox"))
                    ((TextBox)activeControl).Text = "";
                else if (activeType.FullName.Contains("ComboBox"))
                    ((ComboBox)activeControl).Items.Clear();
                else if (activeType.FullName.Contains("CheckBox"))
                    ((CheckBox)activeControl).Checked = false;
                else if (activeType.FullName.Contains("ListView"))
                    ((ListView)activeControl).Items.Clear();
            }
        }

        public static string Encoder_Base64(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;
            else
            {
                try
                {
                    byte[] bytes = Encoding.Default.GetBytes(data);
                    string str = Convert.ToBase64String(bytes);
                    return str;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static string Decoder_Base64(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;
            else
            {
                try
                {
                    byte[] outputb = Convert.FromBase64String(data);
                    string orgStr = Encoding.Default.GetString(outputb);
                    return orgStr;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static object DeserializeObj(string SerializationString)
        {
            if (SerializationString == "")
                return null;
            else
            {
                try
                {
                    byte[] tmpBuffer = Convert.FromBase64String(SerializationString);
                    MemoryStream tmpMS = new MemoryStream(tmpBuffer, 0, tmpBuffer.Length);
                    BinaryFormatter activeBF = new BinaryFormatter();
                    object activeToken = activeBF.Deserialize(tmpMS);
                    return activeToken;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string SerializationObj(object activeConnectTokenObject)
        {
            if (activeConnectTokenObject == null)
                return "";
            else
            {
                try
                {
                    BinaryFormatter activeBF = new BinaryFormatter();
                    MemoryStream tmpMS = new MemoryStream();
                    activeBF.Serialize(tmpMS, activeConnectTokenObject);
                    byte[] result = new byte[tmpMS.Length];
                    result = tmpMS.ToArray();
                    string strResult = Convert.ToBase64String(result);
                    tmpMS.Flush();
                    tmpMS.Close();
                    return strResult;
                }
                catch
                {
                    return "";
                }

            }
        }

    }
}

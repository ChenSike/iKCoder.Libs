using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace iKCoder_Platform_SDK_Kit
{
    public class Base_DataStructs
    {
        public List<string> GetPropertiesNameForSet()
        {
            List<string> result = new List<string>();
            PropertyInfo[] propertyInfoLst = this.GetType().GetProperties(BindingFlags.SetProperty);
            foreach(PropertyInfo activeProperty in propertyInfoLst)
            {
                if (result.Contains(activeProperty.Name))
                    result.Add(activeProperty.Name);
            }
            return result;
        }

        public List<string> GetPropertiesNameForGet()
        {
            List<string> result = new List<string>();
            PropertyInfo[] propertyInfoLst = this.GetType().GetProperties(BindingFlags.GetProperty);
            foreach (PropertyInfo activeProperty in propertyInfoLst)
            {
                if (result.Contains(activeProperty.Name))
                    result.Add(activeProperty.Name);
            }
            return result;
        }

        public string GetValue(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo activeProperty = this.GetType().GetProperty(propertyName);
                if (activeProperty != null)                
                    return activeProperty.GetValue(this, null).ToString();                
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public bool SetValue(string propertyName,string value)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo activeProperty = this.GetType().GetProperty(propertyName);
                if (activeProperty != null)
                {
                    activeProperty.SetValue(this, value);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        
    }
}

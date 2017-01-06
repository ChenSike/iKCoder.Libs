using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace iKCoder_Platform_SDK_Kit
{  

    public class class_Util_LabelsController
    {
        protected string pathOfLabels = "";
        protected XmlDocument labelsDocument;
        protected enum_Language activeLanguage = enum_Language.Chinese;

        public static class_Util_LabelsController CreateInstance(string labelsOfFilename)
        {
            class_Util_LabelsController obj = new class_Util_LabelsController();
            if (!obj.LoadDocument(labelsOfFilename))
                return null;
            else
                return obj;
        }

        public void SetLanguage(enum_Language activeLanguage)
        {
            this.activeLanguage = activeLanguage;
        }

        public string GetString(string group , string code)
        {
            XmlNode activeItemNode = this.labelsDocument.SelectSingleNode("/root/group[@type='" + group + "']/item[@code='" + code + "']");
            if (activeItemNode != null)
            {
                string result = "";
                if (activeLanguage == enum_Language.Chinese)
                    result = class_XmlHelper.GetNodeValue("@chinese", activeItemNode);
                else if (activeLanguage == enum_Language.English)
                    result = class_XmlHelper.GetNodeValue("@english", activeItemNode);
                return result;
            }
            else
                return "";
        }

        public bool LoadDocument(string labelsOfFilename)
        {
            try
            {
                if (labelsOfFilename != "")
                {
                    pathOfLabels = labelsOfFilename;
                    labelsDocument = new XmlDocument();
                    labelsDocument.Load(labelsOfFilename);
                    return true;
                }
                else
                    if (pathOfLabels != "")
                    {
                        labelsDocument = new XmlDocument();
                        labelsDocument.Load(labelsOfFilename);
                        return true;
                    }
                    else
                        return false;                
            }
            catch
            {
                return false;
            }

        }
    }
}

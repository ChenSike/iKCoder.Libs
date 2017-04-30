using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Security_CheckCode
    {

        public string CheckCode = "";

        public string NextCode(int codeLength)
        {

            string so = "1,2,3,4,5,6,7,8,9,0";
            string[] strArr = so.Split(',');
            string code = "";
            Random rand = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                code += strArr[rand.Next(0, strArr.Length)];
            }
            CheckCode = code;
            return code;
        }

        public byte[] CreateImage(Color borderColor,int width = 60,int height =30)
        {

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            WebColorConverter ww = new WebColorConverter();
            g.Clear((Color)ww.ConvertFromString("#FFFFFF"));
            
            Random random = new Random();
            /*
            for (int i = 0; i < 12; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
            }
            */
            Font font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic);
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, image.Width, image.Height), Color.Black, Color.Gray, 1.2f, true);
            g.DrawString(this.CheckCode, font, brush, 0, (height-20)/4);
            /*
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image.SetPixel(x, y, Color.White);
            }*/

            g.DrawRectangle(new Pen(borderColor), 0, 0, image.Width - 1, image.Height - 1);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);          
            g.Dispose();
            image.Dispose();
            return ms.ToArray();
        }

    }
}

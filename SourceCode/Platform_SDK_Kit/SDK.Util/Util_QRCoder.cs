﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace iKCoder_Platform_SDK_Kit
{
    public class Util_QRCoder
    {
        public byte[] CreateQRToByteArr(string content)
        {
            Gma.QrCodeNet.Encoding.QrEncoder obj = new Gma.QrCodeNet.Encoding.QrEncoder();
            Gma.QrCodeNet.Encoding.QrCode qrcode;
            obj.TryEncode("http://ikcoder.iok.la:24525/IKCoder/Data/GET_UrlMap.aspx?mapkey=reg&fulurl=1", out qrcode);
            QuietZoneModules QuietZones = QuietZoneModules.Two;
            Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer newRender = new Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer(new FixedModuleSize(12, QuietZones));
            FileStream fs = new FileStream("test.png", FileMode.Create);
            newRender.WriteToStream(qrcode.Matrix, System.Drawing.Imaging.ImageFormat.Png, fs);
            fs.Close();
        }
    }
}

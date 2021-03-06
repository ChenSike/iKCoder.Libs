﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace iKCoder_Platform_SDK_Kit
{
    public class Util_QRCoder
    {
        public byte[] CreateQRToByteArr(string content, ImageFormat imageFormat)
        {
            Gma.QrCodeNet.Encoding.QrEncoder obj = new Gma.QrCodeNet.Encoding.QrEncoder();
            Gma.QrCodeNet.Encoding.QrCode qrcode;
            obj.TryEncode(content, out qrcode);
            QuietZoneModules QuietZones = QuietZoneModules.Two;
            Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer newRender = new Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer(new FixedModuleSize(12, QuietZones));
            MemoryStream newStream = new MemoryStream();
            newRender.WriteToStream(qrcode.Matrix, imageFormat, newStream);
            byte[] dataBuffer = new byte[newStream.Length];
            newStream.Seek(0, SeekOrigin.Begin);
            newStream.Read(dataBuffer, 0, (int)newStream.Length);            
            newStream.Close();
            return dataBuffer;
        }

        public void CreateQRToFile(string content,ImageFormat imageFormat,string objectFile)
        {
            Gma.QrCodeNet.Encoding.QrEncoder obj = new Gma.QrCodeNet.Encoding.QrEncoder();
            Gma.QrCodeNet.Encoding.QrCode qrcode;
            obj.TryEncode(content, out qrcode);
            QuietZoneModules QuietZones = QuietZoneModules.Two;
            Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer newRender = new Gma.QrCodeNet.Encoding.Windows.Render.GraphicsRenderer(new FixedModuleSize(12, QuietZones));
            FileStream newFS = new FileStream(objectFile, FileMode.Create);
            newRender.WriteToStream(qrcode.Matrix, imageFormat, newFS);
            newFS.Flush();
            newFS.Close();
        }

    }    
}

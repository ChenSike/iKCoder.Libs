using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace iKCoder_Platform_SDK_Kit
{
    public class class_Util_DrawingServices
    {

        public static Bitmap CreateImage(string imageFilePath)
        {
            if (string.IsNullOrEmpty(imageFilePath))
                return null;
            Bitmap returnBitmap = new Bitmap(imageFilePath);
            return returnBitmap;
        }

        public static Bitmap CreateImage(byte[] imageDataBuffer)
        {
            MemoryStream memStream = new MemoryStream(imageDataBuffer);            
            Bitmap returnBitMap = new Bitmap(memStream);
            return returnBitMap;
        }

        public static byte[] GetBytesFromBitmap(Bitmap sourceImage,string imageType)
        {
            MemoryStream newStream = new MemoryStream();
            ImageFormat activeImageType = ImageFormat.Jpeg;
            switch(imageType)
            {
                case "jpg":
                case "JPG":
                case "jpeg":
                case "JPEG":
                    activeImageType=ImageFormat.Jpeg;
                    break;
                case "png":
                case "PNG":
                    activeImageType = ImageFormat.Png;
                    break;
                case "gif":
                case "GIF":
                    activeImageType = ImageFormat.Gif;
                    break;
                case "bmp":
                case "BMP":
                    activeImageType = ImageFormat.Bmp;
                    break;                   
                    
            }
            sourceImage.Save(newStream, activeImageType);
            byte[] dataBuffer = newStream.GetBuffer();
            return dataBuffer;
        }

        public static string GetBase64FromBitmap(Bitmap sourceImage, string imageType)
        {
            byte[] imageData = GetBytesFromBitmap(sourceImage, imageType);
            return class_CommonUtil.Encoder_Base64(imageData);            
        }

        public static Bitmap ClipImage(Bitmap sourceImage,int StartX,int StartY,int Width,int Height)
        {
            if (sourceImage == null)
                return null;            
            Bitmap outImage = new Bitmap(Width, Height);
            Graphics tmpObjGraphics = Graphics.FromImage(outImage);
            tmpObjGraphics.DrawImage(sourceImage, new Rectangle(0, 0, Width, Height), new Rectangle(StartX, StartY, Width, Height), GraphicsUnit.Pixel);
            return outImage;
        }

        public static Bitmap CompressImage(Bitmap sourrceImage,int compressRate,string fileType)
        {
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter mycoder = new EncoderParameter(encoder, compressRate);
            EncoderParameters myCoders = new EncoderParameters(1);
            myCoders.Param[0] = mycoder;
            ImageFormat activeFormat = ImageFormat.Jpeg;
            switch(fileType)
            {
                case "jpg":
                case "JPG":
                case "jpeg":
                case "JPEG":
                    activeFormat = ImageFormat.Jpeg;
                    break;
                case "png":
                case "PNG":
                    activeFormat = ImageFormat.Png;
                    break;
                case "gif":
                case "GIF":
                    activeFormat = ImageFormat.Gif;
                    break;
                case "bmp":
                case "BMP":
                    activeFormat = ImageFormat.Bmp;
                    break;
            }
            ImageCodecInfo jpgInfo = GetEncoder(activeFormat);
            MemoryStream newStream = new MemoryStream();
            sourrceImage.Save(newStream, activeFormat);
            Image bmpObject = Image.FromStream(newStream);
            MemoryStream compreeStream = new MemoryStream();
            bmpObject.Save(compreeStream, jpgInfo, myCoders);
            byte[] dataBuffer = compreeStream.GetBuffer();
            return CreateImage(dataBuffer);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}

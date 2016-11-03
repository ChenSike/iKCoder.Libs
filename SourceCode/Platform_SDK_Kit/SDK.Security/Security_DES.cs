using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace iKCoder_Platform_SDK_Kit
{
    public class Security_DES
    {
        
        public int bufferLength = 4096;
        private const string iv = "1234";
        private string key = "ASDFGHJK";
        private Encoding MyEncoding = new UnicodeEncoding();
        private SymmetricAlgorithm provider = SymmetricAlgorithm.Create("TripleDES");

        public Security_DES(string OptionalKey)
        {
            try
            {
                this.key = OptionalKey;
                this.provider.Key = this.MyEncoding.GetBytes(this.key);
                this.provider.IV = this.MyEncoding.GetBytes("1234");
            }
            catch
            {
                this.key = "ASDFGHJK";
                this.provider.Key = this.MyEncoding.GetBytes(this.key);
                this.provider.IV = this.MyEncoding.GetBytes("1234");
            }
        }

        public Security_DES()
        {
            try
            {
                this.provider.Key = this.MyEncoding.GetBytes(this.key);
                this.provider.IV = this.MyEncoding.GetBytes("1234");
            }
            catch
            {
                this.key = "ASDFGHJK";
                this.provider.Key = this.MyEncoding.GetBytes(this.key);
                this.provider.IV = this.MyEncoding.GetBytes("1234");
            }
        }

        public bool DESCoding(string TransData, out string Result)
        {
            try
            {
                if (TransData.Length == 0)
                {
                    Result = "";
                    return false;
                }
                ICryptoTransform transform = this.provider.CreateEncryptor();
                if (transform == null)
                {
                    Result = "";
                    return false;
                }
                Stream stream = new MemoryStream(this.MyEncoding.GetBytes(TransData));
                Stream stream2 = new MemoryStream();
                CryptoStream stream3 = new CryptoStream(stream2, transform, CryptoStreamMode.Write);
                byte[] buffer = new byte[this.bufferLength];
                if (buffer == null)
                {
                    Result = "";
                    return false;
                }
                int count = 0;
                while ((count = stream.Read(buffer, 0, this.bufferLength)) > 0)
                {
                    stream3.Write(buffer, 0, count);
                }
                stream3.FlushFinalBlock();
                stream2.Position = 0L;
                byte[] buffer2 = new byte[stream2.Length];
                stream2.Read(buffer2, 0, buffer2.Length);
                Result = Convert.ToBase64String(buffer2);
                return true;
            }
            catch
            {
                Result = null;
                return false;
            }
        }

        public bool DESDecoding(string TransData, out string Result)
        {
            try
            {
                Result = "";
                if (TransData.Length == 0)
                {
                    return false;
                }
                ICryptoTransform transform = this.provider.CreateDecryptor();
                if (transform == null)
                {
                    return false;
                }
                Stream stream = new MemoryStream(Convert.FromBase64String(TransData));
                Stream stream2 = new MemoryStream();
                CryptoStream stream3 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                byte[] buffer = new byte[this.bufferLength];
                if (buffer == null)
                {
                    return false;
                }
                int count = 0;
                while ((count = stream3.Read(buffer, 0, this.bufferLength)) > 0)
                {
                    stream2.Write(buffer, 0, count);
                }
                stream2.Position = 0L;
                byte[] buffer2 = new byte[stream2.Length];
                stream2.Read(buffer2, 0, buffer2.Length);
                Result = this.MyEncoding.GetString(buffer2);
                return true;
            }
            catch
            {
                Result = null;
                return false;
            }
        }       
    }        
    
}


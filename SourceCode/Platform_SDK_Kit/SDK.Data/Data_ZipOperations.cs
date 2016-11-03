using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Threading;

namespace iKCoder_Platform_SDK_Kit
{

    public class zipFileItem
    {
        public byte[] dataBuffer
        {
            set;
            get;
        }

        public string fileName
        {
            set;
            get;
        }
    }

    public enum zipFileMode
    {
        NewWrite,
        Write,
        Read
    }

    public class Data_ZipOperations
    {
        private Queue<zipFileItem> _itemsQueue = new Queue<zipFileItem>();
        private ZipInputStream _activeInZipFile = null;
        private ZipOutputStream _activeOutZipFile = null;
        private Thread _monitorThread;

        public string ActiveZipFile
        {
            set;
            get;
        }
        private zipFileMode _actionMode
        {
            set;
            get;
        }

        

        public static Data_ZipOperations CreateInstance(string filename,zipFileMode actionMode)
        {
            if (filename != "")
            {
                if (!filename.EndsWith(".zip"))
                {
                    filename = filename + ".zip";
                }
                Data_ZipOperations Obj = new Data_ZipOperations();
                Obj._actionMode = actionMode;
                if (actionMode == zipFileMode.NewWrite)
                {
                    if (Obj.CreateNewFileForOut(filename))
                        return Obj;
                    else
                        return null;
                }
                else if (actionMode == zipFileMode.Read)
                {
                    if (Obj.OpenZipFileForReading(filename))
                        return Obj;
                    else
                        return null;
                }
                else if (actionMode == zipFileMode.Write)
                {
                    if (Obj.OpenZipFileForWriting(filename))
                        return Obj;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }       

        public bool CreateNewFileForOut(string filename)
        {
            try
            {
                if (!filename.EndsWith(".zip"))
                {
                    filename = filename + ".zip";
                }
                this.ActiveZipFile = filename;
                try
                {
                    this._activeOutZipFile = new ZipOutputStream(File.Create(filename));
                }
                catch
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool OpenZipFileForReading(string filename)
        {
            try
            {
                this.ActiveZipFile = filename;
                this._activeInZipFile = new ZipInputStream(File.Open(filename, FileMode.Open));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool OpenZipFileForWriting(string filename)
        {
            try
            {

                this._activeOutZipFile = new ZipOutputStream(File.Open(filename, FileMode.Append));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CloseActiveZipFile()
        {
            try
            {
                if (_actionMode == zipFileMode.NewWrite || _actionMode == zipFileMode.Write)
                    this._activeOutZipFile.Close();
                else if (_actionMode == zipFileMode.Read)
                    this._activeInZipFile.Close();
                return true;
            }
            catch
            {                
                return false;
            }
        }

        public bool InsertFileItem(byte[] buffer,string filename)
        {
            if (buffer.Length < -0 || filename == "")
                return false;
            else
            {
                zipFileItem newItem = new zipFileItem();
                newItem.dataBuffer = buffer;
                newItem.fileName = filename;
                _itemsQueue.Enqueue(newItem);
                return true;
            }
        }

        public void Start()
        {
            _monitorThread = new Thread(new ThreadStart(WritingZipFile));
            _monitorThread.Start();
        }

        public void Stop()
        {
            while(true)
            {
                if(_itemsQueue.Count==0)
                {
                    _monitorThread.Abort();
                    CloseActiveZipFile();                    
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void WritingZipFile()
        {
            while(true)
            {
                if (_itemsQueue.Count > 0)
                {
                    zipFileItem activeItem = _itemsQueue.Dequeue();
                    AddFileToZipFile(activeItem.dataBuffer, activeItem.fileName);
                }
                else
                    Thread.Sleep(200);
            }
        }    

        public void AddFileToZipFile(byte[] buffer, string filename)
        {
            try
            {

                if (buffer != null)
                {
                    Crc32 crc = new Crc32();
                    ZipEntry entry = new ZipEntry(filename)
                    {
                        DateTime = DateTime.Now,
                        Size = buffer.Length
                    };
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    lock (_activeOutZipFile)
                    {
                        this._activeOutZipFile.PutNextEntry(entry);
                        this._activeOutZipFile.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {                
            }
        }   

        public Dictionary<string, byte[]> GetAllDataFromZip()
        {
            Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
            if (this._activeInZipFile != null)
            {
                ZipEntry activeEntry;
                int bufferSize = 1024 * 1024;
                while ((activeEntry = this._activeInZipFile.GetNextEntry()) != null)
                {
                    byte[] tmpBuffer = new byte[bufferSize];
                    byte[] streatBuffer = new byte[bufferSize];
                    MemoryStream ms = new MemoryStream(streatBuffer);
                    BinaryWriter bw = new BinaryWriter(ms);
                    int size = 1024 * 1024;
                    size = _activeInZipFile.Read(tmpBuffer, activeEntry.offset, tmpBuffer.Length);
                    if (size > 0)
                    {
                        bw.Write(tmpBuffer);
                        if (!data.ContainsKey(activeEntry.Name))
                        {
                            data.Add(activeEntry.Name, streatBuffer);
                        }
                    }

                }
            }
            return data;
        }

  
    }
}

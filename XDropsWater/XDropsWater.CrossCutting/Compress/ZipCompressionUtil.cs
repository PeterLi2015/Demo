using XDropsWater.CrossCutting.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Compress
{
    public class ZipCompressionUtil
    {
        public static void ZipFile(string FileToZip, string ZipedFile, string pwd, int CompressionLevel, int BlockSize)
        {
            //Get all DirectoryInfo
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipStream.Password = pwd;
            ZipEntry ZipEntry = new ZipEntry("ZippedFile");
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }

        //Get all DirectoryInfo
        private static void Direct(DirectoryInfo di, ref ZipOutputStream s, Crc32 crc)
        {
            //DirectoryInfo di = new DirectoryInfo(filenames);
            DirectoryInfo[] dirs = di.GetDirectories("*");

            //±éÀúÄ¿Â¼ÏÂÃæµÄËùÓÐµÄ×ÓÄ¿Â¼
            foreach (DirectoryInfo dirNext in dirs)
            {
                //½«¸ÃÄ¿Â¼ÏÂµÄËùÓÐÎÄ¼þÌí¼Óµ½ ZipOutputStream s Ñ¹ËõÁ÷ÀïÃæ
                FileInfo[] a = dirNext.GetFiles();
                WriteStream(ref s, a, crc);

                //µÝ¹éµ÷ÓÃÖ±µ½°ÑËùÓÐµÄÄ¿Â¼±éÀúÍê³É
                Direct(dirNext, ref s, crc);
            }
        }

        private static int bufferSize = 2048;
        private static void WriteStream(ref ZipOutputStream s, FileInfo[] a, Crc32 crc)
        {
            foreach (FileInfo fi in a)
            {
                //string fifn = fi.FullName;
                FileStream fs = fi.OpenRead();

                try
                {
                    //ZipEntry entry = new ZipEntry(file);    Path.GetFileName(file)
                    //string file = fi.FullName;
                    //file = file.Replace(cutStr, "");

                    ZipEntry entry = new ZipEntry(Path.GetFileName(fi.FullName));

                    entry.DateTime = DateTime.Now;

                    // set Size and the crc, because the information
                    // about the size and crc should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    // (in this case size == crc == -1 in the header)
                    // Some ZIP programs have problems with zip files that don't store
                    // the size and crc in the header.
                    entry.Size = fs.Length;

                    // ²ÉÓÃ·Ö¿éÐ´Èë±ÜÃâÄÚ´æÏûºÄ¹ý´óµÄÎÊÌâ£¬Òò´ËÎÞ·¨ÔÚ¿ªÊ¼¾Í¼ÆËãcrc
                    //crc.Reset();
                    //crc.Update(buffer);
                    //entry.Crc = crc.Value;

                    s.PutNextEntry(entry);

                    byte[] buffer = new byte[bufferSize];
                    int readSize = 0;
                    while ((readSize = fs.Read(buffer, 0, buffer.Length)) > 0)
                        s.Write(buffer, 0, readSize);
                    fs.Close();
                }
                catch (Exception)
                {
                    fs.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Ñ¹ËõÖ¸¶¨Ä¿Â¼ÏÂÖ¸¶¨ÎÄ¼þ(°üÀ¨×ÓÄ¿Â¼ÏÂµÄÎÄ¼þ)
        /// </summary>
        /// <param name="zippath">args[0]ÎªÄãÒªÑ¹ËõµÄÄ¿Â¼ËùÔÚµÄÂ·¾¶ 
        /// ÀýÈç£ºD:\\temp\\   (×¢Òâtemp ºóÃæ¼Ó \\ µ«ÊÇÄãÐ´³ÌÐòµÄÊ±ºòÔõÃ´ÐÞ¸Ä¶¼¿ÉÒÔ)</param>
        /// <param name="zipfilename">args[1]ÎªÑ¹ËõºóµÄÎÄ¼þÃû¼°ÆäÂ·¾¶
        /// ÀýÈç£ºD:\\temp.zip</param>
        /// <param name="fileFilter">ÎÄ¼þ¹ýÂË, ÀýÈç*.xml,ÕâÑùÖ»Ñ¹Ëõ.xmlÎÄ¼þ.</param>
        ///
        public static bool ZipFileMain(string zippath, string zipfilename, string fileFilter, string pwd)
        {
            ZipOutputStream s = null;
            try
            {
                //string filenames = Directory.GetFiles(args[0]);

                Crc32 crc = new Crc32();
                s = new ZipOutputStream(File.Create(zipfilename));
                s.Password = pwd;
                s.SetLevel(6); // 0 - store only to 9 - means best compression

                DirectoryInfo di = new DirectoryInfo(zippath);

                FileInfo[] a = di.GetFiles(fileFilter);

                //Ñ¹ËõÕâ¸öÄ¿Â¼ÏÂµÄËùÓÐÎÄ¼þ
                WriteStream(ref s, a, crc);
                //Ñ¹ËõÕâ¸öÄ¿Â¼ÏÂ×ÓÄ¿Â¼¼°ÆäÎÄ¼þ
                Direct(di, ref s, crc);

                s.Finish();
                s.Close();
            }
            catch
            {
                if (s != null)
                    s.Close();
                throw;
            }
            return true;
        }

        /// <summary>
        /// ½âÑ¹ËõÎÄ¼þ(Ñ¹ËõÎÄ¼þÖÐº¬ÓÐ×ÓÄ¿Â¼)
        /// </summary>
        /// <param name="zipfilepath">´ý½âÑ¹ËõµÄÎÄ¼þÂ·¾¶</param>
        /// <param name="unzippath">½âÑ¹Ëõµ½Ö¸¶¨Ä¿Â¼</param>
        public static void UnZip(string zipfilepath, string unzippath)
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipfilepath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    //Éú³É½âÑ¹Ä¿Â¼
                    DirectoryUtil.CreateOpenDir(unzippath);
                    string filePath = Path.Combine(unzippath, Path.GetFileName(theEntry.Name));
                    if (filePath != string.Empty)
                    {
                        //Èç¹ûÎÄ¼þµÄÑ¹Ëõºó´óÐ¡Îª0ÄÇÃ´ËµÃ÷Õâ¸öÎÄ¼þÊÇ¿ÕµÄ,Òò´Ë²»ÐèÒª½øÐÐ¶Á³öÐ´Èë
                        if (theEntry.CompressedSize == 0)
                            break;

                        using (FileStream streamWriter = File.Create(filePath))
                        {
                            int size = 64 * 1024;
                            byte[] data = new byte[size];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                        }
                    }
                }
                s.Close();
            }
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace UTF_8
{
    class Program
    {

        /// <summary>
        /// Using C# port of Mozilla Universal Charset Detector for charset detection
        /// </summary>
        /// <param name="filename">physical path to file</param>
        /// <returns>file encoding</returns>
        public static Encoding DetectEncoding(string filename)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                var cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();
                return Encoding.GetEncoding(cdet.Charset);   
            }

        }

        /// <summary>
        /// Convert all .cshtml files in selected folder and its subdfolders (AllDirectories settings)
        /// from Windows-1250 to UTF-8 encoding
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {
            foreach (var f in new DirectoryInfo(@"_your_file_path_").GetFiles("*.cshtml", SearchOption.AllDirectories))
            {
                Encoding detectedEncoding = DetectEncoding(f.FullName);


                // Mozilla Universal Charset Detector detects 1250 as 1252, 
                // since I'm from central Europe (1250) I added this condition  

                if (detectedEncoding == Encoding.GetEncoding(1250) || detectedEncoding == Encoding.GetEncoding(1252))
                {
                    detectedEncoding = Encoding.GetEncoding(1250);
                    Encoding utf8 = Encoding.UTF8;
                    byte[] fileBytes = File.ReadAllBytes(f.FullName);
                    byte[] utf8Bytes = Encoding.Convert(detectedEncoding, utf8, fileBytes);
                    string utf8String = Encoding.UTF8.GetString(utf8Bytes);
                    File.WriteAllText(f.FullName, utf8String);
                    Console.WriteLine(f.FullName);
                }
            }

            Console.ReadKey();
        }
    }
}

using System;
using System.IO;
using System.Linq;

namespace Task2
{
    public class Program
    {
        /// <summary>
        /// Напишите программу, которая считает размер папки на диске (вместе со всеми вложенными папками и файлами). 
        /// На вход метод принимает URL директории, в ответ — размер в байтах.
        /// Считаем, что размер папки/диска не превосходит ulong; не используем checked или drcimal
        /// </summary>
        static void Main(string[] args)
        {
            const string path = @"C:\Scan";
            Console.WriteLine($"Calculating size of {path}");
            if (Directory.Exists(path))       
            {
                long dirSize = GetDirSize(path);
                Console.WriteLine($"Size of {path} is {dirSize:0,0} bytes");
            }
            else
            {
                Console.WriteLine($"path {path} does not exists");
            }
            Console.WriteLine("-- End --");
        }

        public static long GetDirSize(string path)
        {
            long size = 0;
            try
            {
                FileInfo[] files = new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when accessing files: {ex.Message}");
            }
            return size;
        }

        public static long GetDirectorySize(string parentDirectory)     // Очень красиво - да здравствует LINQ!
        {
            return new DirectoryInfo(parentDirectory).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
    }
}

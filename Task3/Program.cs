using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task3
{
    class Program
    {
        /// <summary>
        /// Доработайте программу из задания 1, используя ваш метод из задания 2.
        /// При запуске программа должна:
        /// - Показать, сколько весит папка до очистки.Использовать метод из задания 2. 
        /// - Выполнить очистку.
        /// - Показать сколько файлов удалено и сколько места освобождено.  - надеюсь, тут можно вычесть из начальных значений конечные, 
        ///                                                                    если нужно считать размер в явном виде - пишите, исправлю
        /// - Показать, сколько папка весит после очистки.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string path = @"C:\Scan\ToClean";
            const int ageInMinutes = 1;

            if (Directory.Exists(path))
            {
                // К сожалению, просто использовать код задачи 1 не получается - доступ к файлам для определения их размера изменяет значение свойства "время доступа"
                // Поэтому сначала сохраняем список "старых" директорий
                string[] agedDirs = GetAgedDirs(Directory.GetDirectories(path), ageInMinutes); // Пришлось делать массив старых каталогов -
                                                                         // при использовании LINQ-звпроса в нужный момент список оказывался пустым
                                                                         // (надеюсь, позже на курсе мы узнаем, почему так происходит; вероятно, из-за lazy)
                                                                         // Может быть, есть способ лучше?

                FileInfo[] agedFiles = ((new DirectoryInfo(path).GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    .Where(file => file.LastAccessTime < (DateTime.Now - TimeSpan.FromMinutes(ageInMinutes)))).ToArray();

                int initFileCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
                long initTotalDirSize = Task2.Program.GetDirSize(path);     

                Console.WriteLine($"Initial size of {path} is {initTotalDirSize:0,0} bytes; there are {initFileCount} files inside");
                Console.WriteLine($"\nInspecting {path} for files/folders not accessed during last {ageInMinutes} minute(s) to delete them");

                DeleteAgedDirs(agedDirs);
                DeleteAgedFiles(agedFiles);
                int finalFileCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
                long finalTotalDirSize = Task2.Program.GetDirSize(path);

                Console.WriteLine($"\nResulting size of {path} after cleaning is {finalTotalDirSize:0,0} bytes;  there are {finalFileCount} files inside" +
                    $"\ndeleted {initFileCount - finalFileCount} files; cleaned {initTotalDirSize - finalTotalDirSize:0,0} bytes");
            }
            else
            {
                Console.WriteLine($"Directory {path} does not exists - nothing to do");
            }

            Console.WriteLine("-- End --");
        }

        private static void DeleteAgedFiles(FileInfo[] agedFiles)
        {
            foreach (FileInfo f in agedFiles)
            {
                try
                {
                    Console.WriteLine($"Deleting {f.Name}");
                    f.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete file {f}: {ex.Message}");
                }
            }
        }

        private static string[] GetAgedDirs(string[] allDirs, int ageInMinutes)
        {
            try
            {
                var agedDirs = allDirs.Where(d => Directory.GetLastAccessTime(d) < (DateTime.Now - TimeSpan.FromMinutes(ageInMinutes)));
                string[] agedDirArray = new string[agedDirs.Count()];
                int i = 0;
                foreach (string dir in agedDirs)
                {
                    agedDirArray[i++] = dir;
                }
                return agedDirArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return new string[0];       // А что лучше возвращать в таком случае?

            }
        }

        private static long GetAgedDirSize(IEnumerable<string> agedDirs)
        {
            long size = 0;
            foreach (string dir in agedDirs)
            {
                size += Task2.Program.GetDirSize(dir);
            }
            return size;
        }

        private static void DeleteAgedDirs(IEnumerable<string> agedDirs)
        {
            foreach (string dir in agedDirs)
            {
                Console.WriteLine($"Deleting aged dir {dir}");
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while deleting {dir}: {ex.Message}");
                }
            }
        }
    }
}

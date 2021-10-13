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
        /// - Показать сколько файлов удалено и сколько места освобождено. 
        /// - Показать, сколько папка весит после очистки.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string path = @"C:\Scan\ToClean";
            const int ageInMinutes = 1;

            if (Directory.Exists(path))
            {
                // К сожалению, просто использовать код задач 1 и 2 не получается - доступ к файлам для определения их размера изменяет время доступа
                // Поэтому сначала сохраняем список "старых" директорий
                string[] allDirs = Directory.GetDirectories(path);
                string[] agedDirs = GetAgedDirs(allDirs, ageInMinutes); // Пришлось делать массив старых каталогов - при использовании LINK-звпроса в нужный момент список оказывался пустым
                                                                        // (надеюсь, позже на курсе мы узнаем, почему так происходит)
                                                                        // Вероятно, есть способ лучше?

                long totalDirSize = Task2.Program.GetDirSize(path);
                Console.WriteLine($"Initial size of {path} is {totalDirSize:0,0} bytes");
                long agedDirSize = GetAgedDirSize(agedDirs);    // Здесь хорошо бы использовать метод из задачи 2, но не получается - разные типы параметров

                Console.WriteLine($"\nInspecting {path} for files/folders not accessed during last {ageInMinutes} minute(s) and delete them");
                Console.WriteLine($"Size of aged directories: {agedDirSize:0,0}");
                // Task1.Program.CleanFolder(path, ageInMinutes);  // вот тут приходится отказаться от использования метода из 1й задачи - он не сработает
                // Поэтому сделаем новый метод
                DeleteAgedDirs(agedDirs);
                totalDirSize = Task2.Program.GetDirSize(path);
                Console.WriteLine($"\nResulting size of {path} after cleaning is {totalDirSize:0,0} bytes");
            }
            else
            {
                Console.WriteLine($"Directory {path} does not exists - nothing to do");
            }

            Console.WriteLine("-- End --");
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

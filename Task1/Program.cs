using System;
using System.IO;
using System.Linq;

namespace Task1
{
    /// <summary>
    /// Напишите программу, которая чистит нужную нам папку от файлов  и папок, которые не использовались более timeInMinutes минут 
    /// На вход программа принимает путь до папки.
    /// При разработке постарайтесь предусмотреть возможные ошибки (нет прав доступа, папка по заданному адресу не существует, передан некорректный путь) 
    /// и уведомить об этом пользователя.
    /// Критерии оценки
    /// 2 балла (хорошо): код должен удалять папки рекурсивно (если в нашей папке лежит папка со вложенными файлами, не должно возникнуть проблем с её удалением) - сделано.
    /// 4 балла(отлично) : предусмотрена проверка на наличие папки по заданному пути(строчка if directory.Exists);  -- сделано
    /// предусмотрена обработка исключений при доступе к папке(блок try-catch, а также логирует исключение в консоль). -- сделано, но жду комментариев по поводу использования try/catch
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const string folderToClean = @"C:\Scan\ToClean";    // Папка, которую мы собираемся чистить
            const int timeInMinutes = 1;                       // Если доступ к папке или файлу был раньше указанного здесь, папка или файл удаляются
            Console.WriteLine($"Чистим папку {folderToClean} от файлов и папок, которые не использовались более {timeInMinutes} минут");
            if (Directory.Exists(folderToClean))        // Достаточно ли проверки корневого каталога, или безопаснее проверять каждый каталог?
                                                        // состояние диска может измениться, пока программа работает 
            {
                CleanFolder(folderToClean, timeInMinutes);             // Где правильнее сделать try/catch - здесь или внутри метода?
            }
            else
            {
                Console.WriteLine($"Directory {folderToClean} does not exists - nothing to clean");
            }
            Console.WriteLine("-- End --");
        }

        private static void CleanFolder(string folderToClean, int timeInMinutes, string indent = "")    // indent был придуман для варианта с рекурсией, чтобы вывод был более красивым
        {
            string[] files = Directory.GetFiles(folderToClean); // Начинаем с удаления файлов
            foreach (string file in files.Where(f => File.GetLastAccessTime(f) < (DateTime.Now - TimeSpan.FromMinutes(timeInMinutes))))
            {
                Console.WriteLine($"{indent}{file} accessed at {File.GetLastAccessTime(file)} - deleting");
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{indent}Failed to delete {file}: {ex.Message}");
                }
            }

            string[] dirs = Directory.GetDirectories(folderToClean);    // Теперь удаляем каталоги

            // Насколько я понял, манипуляции с файлами и папками внутри интересующей нас папки (при любой глубине вложенности) меняют и "время последнего доступа" к этой папке.
            // Поэтому рекурсия с проверкой времени доступа ко всем вложенным папкам и файлам не нужна (этот код ниже закомментирован),
            // и можно удалить папку и все содержимоt, используя Directory.Delete(dir, true)
            // Если я не прав - пишите, исправлю..
            foreach (string dir in dirs.Where(d => Directory.GetLastAccessTime(d) < (DateTime.Now - TimeSpan.FromMinutes(timeInMinutes)) ))
            {
                //Console.WriteLine($"\n{indent}Cleaning directory {dir}");
                Console.WriteLine($"{indent}{dir} was accessed at {Directory.GetLastAccessTime(dir)} - deleting");
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can't delete {dir}: {ex.Message}");
                }


                //CleanFolder(dir, indent + "  ");   // Чистим папку и подпапки
                // Теперь удаляем папку, если она пуста и не использовалась более timeInMinutes минут

                //if ((Directory.GetLastAccessTime(dir) < (DateTime.Now - TimeSpan.FromMinutes(timeInMinutes))) )  //&& IsDirectoryEmpty(dir))
                //{
                //    Console.WriteLine($"{indent}{dir} is empty and accessed at {File.GetLastAccessTime(dir)} - deleting ");
                //    try
                //    {
                //        Directory.Delete(dir);
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"{indent}Failed to delete {dir}: {ex.Message}");
                //    }
                //}
            }

        }
        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}

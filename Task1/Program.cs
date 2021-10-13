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
    public class Program      // Это сделано public для использования в задаче 3. Вопрос: есть ли лучшее решение?
    {
        static void Main(string[] args)
        {
            const string folderToClean = @"C:\Scan\ToClean";    // Папка, которую мы собираемся чистить
            const int ageInMinutes = 1;                       // Если доступ к папке или файлу был раньше указанного здесь, папка или файл удаляются
            Console.WriteLine($"Inspecting {folderToClean} for files/folders not accessed during last {ageInMinutes} minute(s) and delete them");
            if (Directory.Exists(folderToClean))       
            {
                CleanFolder(folderToClean, ageInMinutes);             // Где правильнее сделать try/catch - здесь или внутри метода?
            }
            else
            {
                Console.WriteLine($"Directory {folderToClean} does not exists - nothing to clean");
            }
            Console.WriteLine("-- End --");
        }

        public static void CleanFolder(string folderToClean, int timeInMinutes)  
        {
            // Вопрос: что правильнее - делать один большой try/catch или свой для каждой операции, способной вызвать исключение?
            // Например, один для .GetFiles и один для каждого .Delete
            // И нормальна ли конструкция со вложенными try/catch, созданная ниже? 
            try     // Удаление файлов
            {
                string[] files = Directory.GetFiles(folderToClean);         
                foreach (string file in files.Where(f => File.GetLastAccessTime(f) < (DateTime.Now - TimeSpan.FromMinutes(timeInMinutes))))
                {
                    Console.WriteLine($"{file} accessed at {File.GetLastAccessTime(file)} - deleting");
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete File {file}: {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get list of files in {folderToClean} - aged files will not be deleted: {ex.Message} ");
            }

            try     // Удаление каталогов
            {
                string[] dirs = Directory.GetDirectories(folderToClean);    

                // Насколько я понял, манипуляции с файлами и папками внутри интересующей нас папки (при любой глубине вложенности) меняют и "время последнего доступа" к этой папке.
                // Поэтому рекурсия с проверкой времени доступа ко всем вложенным папкам и файлам не нужна (этот код ниже закомментирован),
                // и можно удалить папку и все содержимоt, используя Directory.Delete(dir, true)
                // Если я не прав - пишите, исправлю..
                foreach (string dir in dirs.Where(d => Directory.GetLastAccessTime(d) < (DateTime.Now - TimeSpan.FromMinutes(timeInMinutes))))
                {
                    Console.WriteLine($"{dir} was accessed at {Directory.GetLastAccessTime(dir)} - deleting");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get directory list in {folderToClean} - aged dirs will not be deleted : {ex.Message}");

            }
        }
    }
}

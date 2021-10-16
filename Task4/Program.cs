using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace FinalTask
{
    class Program
    {

        // Подскажите, пожалуйста: есть ли у задачи другое решение кроме переименования проекта?


        /// <summary>
        /// Написать программу-загрузчик данных из бинарного формата в текст.
        /// На вход программа получает бинарный файл, предположительно, это база данных студентов.
        /// Свойства сущности Student: Имя — Name(string); Группа — Group(string); Дата рождения — DateOfBirth(DateTime).
        /// 
        /// Создать на рабочем столе директорию Students. Внутри раскидать всех студентов из файла по группам(каждая группа-отдельный текстовый файл), 
        /// в файле группы студенты перечислены построчно в формате "Имя, дата рождения".
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string binFilePath = @"Data\Students.dat";
            const string outputDir = @"Data";   // Не люблю засорять десктоп. Но если нужно разместить все именно там,
                                                // надо закомментировать эту строку и снять комментарий со следующей  
                                                //const string outputDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //TestWithMyData();                 //Это попытки провести сериализацию и десериализацию собственных данных, чтобы понять,
                                                //что не так в предоставленном для тестирования файле

            Student[] studArray = Deserialize(binFilePath);
            WriteToConsole(studArray);
            var groups = studArray.Select(stud => stud.Group).Distinct();
            foreach(string g in groups)
            {
                CreateGroupFile(studArray, g, outputDir);
            }

            Console.WriteLine("-- End --");
        }

        private static void WriteToConsole(Student[] studArray)
        {
            Console.WriteLine($"Deserialized list of students ({studArray.Length} persons)");
            foreach (Student st in studArray)
            {
                Console.WriteLine($"{st.Group}\t{String.Format("{0,-12}", st.Name)}\t{st.DateOfBirth:d}");
            }
        }

        private static void CreateGroupFile(Student[] studArray, string group, string outputDir)
        {
            var groupContent = studArray.Where(student => student.Group == group);
            Console.WriteLine($"Creating file for group \"{group}\"  ({groupContent.Count()} person(s)) in {outputDir}");
            try
            {
                string path = outputDir + "\\" + group + ".txt";
                using StreamWriter writer = new(path);
                foreach (Student st in groupContent)
                {
                    writer.WriteLine($"{st.Name}, {st.DateOfBirth:d}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when creating file for {group} : {ex.Message}");
            }

        }

        private static Student[] Deserialize(string binFilePath)
        {
            // Вопрос: стоит ли тут отдельно проверять наличие файла, или наличие try делает это ненужным?
            BinaryFormatter formatter = new();
            try
            {
                using var fs = new FileStream(binFilePath, FileMode.Open);
                return (Student[])formatter.Deserialize(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error - exit: {ex.Message}");
                System.Environment.Exit(1);
                return null;
            }

        }

        #region Test
        /// <summary>
        /// Сериализация своих данных - попытка понять, почему не программа не работает на предоставленном файле
        /// Сейчас не используется
        /// </summary>
        private static void TestWithMyData()   
        {
            List<Student> st = CreateStudents();
            Student[] stAr = st.ToArray();
            BinaryFormatter formatter = new();
            using (var fs = new FileStream(@"Data\myStudents.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, stAr);
            }
            using (var fs = new FileStream(@"Data\myStudents.dat", FileMode.Open))
            {
                Student[] newSt = (Student[])formatter.Deserialize(fs);
                Console.WriteLine("-- Deserialized --");
                foreach (Student s in newSt)
                {
                    s.Show();
                }
            }
        }

        private static List<Student> CreateStudents()
        {
            return new List<Student> { new Student("Игорь", "G-152", new DateTime(2000, 12, 14)), new Student("Анна", "G-Unknow", new DateTime(2020, 10, 15)),
                                       new Student("Василиса", "G-153", new DateTime(2000, 12, 14)), new Student ("Алина", "G-Unknow", new DateTime(2020, 10, 15)),
             new Student("Инна", "G-Unknown", new DateTime(2000, 12, 14)), new Student("Клавдия", "G-150", new DateTime(2020, 10, 15)),
            new Student ("Кристина", "G-Unknown", new DateTime(2000, 12, 14)), new Student("Егор", "G-Unknow", new DateTime(2020, 10, 15)),
            new Student ("Василий", "G-Unknown", new DateTime(2000, 12, 14)), new Student("Виолетта", "G-Unknow", new DateTime(2020, 10, 15)),
             new Student("Тимур", "G-152", new DateTime(2000, 12, 14)), new Student("Галина", "G-Unknow", new DateTime(2020, 10, 15)),
             new Student("Никита", "G-153", new DateTime(2000, 12, 14)), new Student("Денис", "G-Unknow", new DateTime(2020, 10, 15)),
             new Student("Ольга", "G-150", new DateTime(2000, 12, 14)), new Student("Елизавета", "G-151", new DateTime(2020, 10, 15)) };
        }
        #endregion Test
    }
}

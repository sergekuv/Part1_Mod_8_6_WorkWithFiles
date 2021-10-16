using System;
using System.IO;
using System.Net;

namespace ReadFromUli
{
    class Program
    {
        static void Main(string[] args)
        {
            //var webRequest = WebRequest.Create(@"https://lms.skillfactory.ru/assets/courseware/v1/4aca073bd021bc0f48b90174e445cb58/asset-v1:SkillFactory+CSHARP+2020+type@asset+block/Students.dat");

            WebRequest objRequest = System.Net.HttpWebRequest.Create("https://lms.skillfactory.ru/assets/courseware/v1/4aca073bd021bc0f48b90174e445cb58/asset-v1:SkillFactory+CSHARP+2020+type@asset+block/Students.dat");
            var objResponse = objRequest.GetResponse();
            byte[] buffer = new byte[32768];
            using (Stream input = objResponse.GetResponseStream())
            {
                using (FileStream output = new FileStream("test.doc", FileMode.CreateNew))
                {
                    int bytesRead;

                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }

        }

    }
}

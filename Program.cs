using Ins_Assignment_3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Ins_Assignment_3
{
    class Program
    {
        static string url = "ftp://waws-prod-dm1-127.ftp.azurewebsites.windows.net/bdat1001-10983";
        static string username = @"bdat100119f\bdat1001";
        static string password = "bdat1001";

        public static object Newtonsoft { get; private set; }
        // Newtonsoft.Json.JsonConvert.SerializeObject(student);

        static void Main(string[] args)
        {

            Console.WriteLine(GetDirectory(url));

            string directories = GetDirectory(url);
            List<string> studentdir = directories.Split("\r\n", StringSplitOptions.None).ToList();
            List<Student> students = new List<Student>();

            foreach (var studentdirs in studentdir)
            {
                string[] studentProps = studentdirs.Split(" ", StringSplitOptions.None);

                if (studentProps.Length >= 1 && !String.IsNullOrEmpty(studentProps[0]))
                {
                    Student student = new Student();
                    student.StudentId = studentProps[0];
                    student.FirstName = studentProps[1];
                    student.LastName = studentProps[2];
                    students.Add(student);
                    Console.WriteLine($"{student.StudentId} {student.FirstName} {student.LastName}");
                }
            }
            Console.WriteLine($" {students.Count()} students in the list ");

            var studentwith2004 = students.Where(x => x.StudentId.StartsWith("2004"));
            Console.WriteLine($"There are {studentwith2004.Count()} that start with 2004");

            var studentWith2005 = students.Where(x => x.StudentId.Contains("2005"));
            Console.WriteLine($"There are {studentWith2005.Count()} that contains 2005");

            Student myrecord = students.SingleOrDefault(x => x.StudentId == "200450776");
            Console.WriteLine($" My record is {myrecord.FirstName} {myrecord.LastName}");

            double averageAge = students.Average(x => x.Age);
            Console.WriteLine($"The average age is => {averageAge.ToString("0")}");

            int highestMax = students.Max(x => x.Age);
            Console.WriteLine($"The highest Age in the list is {highestMax}");

            int lowestMax = students.Min(x => x.Age);
            Console.WriteLine($"The lowest Age in the list is {lowestMax}");


            List<string> studentsCSV = new List<string>();
            CsvFileReaderWriter reader = new CsvFileReaderWriter();

            foreach (var student in students)
            {
                Console.WriteLine($"{student.StudentId} {student.FirstName} {student.LastName} ");
                Console.WriteLine(student);
                Console.WriteLine(student.ToCSV());

                studentsCSV.Add(student.ToCSV());
            }

            using (StreamWriter sw = new StreamWriter(@"C:\Users\Jenisha\Desktop\students.csv"))
            {
                sw.WriteLine("StudentId, FirstName, LastName");
                foreach (var studentCSV in studentsCSV)
                {
                    sw.WriteLine(studentCSV);
                }
            }

            // Newtonsoft.Json.JsonConvert.SerializeObject(students);
            //string json = JsonConvert.SerializeObject(students);
            string json = JsonConvert.SerializeObject(students);

            using (StreamWriter sw = new StreamWriter(@"C:\Users\Jenisha\Desktop\students.json"))
            {
                sw.WriteLine(json);
            }


            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));

            using (Stream fs = new FileStream(@"C:\Users\Jenisha\Desktop\students.xml", FileMode.Create))
            {
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                serializer.Serialize(writer, students);
            }
            string uploadcsv = @"C:\Users\Jenisha\Desktop\students.csv";

            string csvfile = "/200450776%20Jenisha%20Thummar/students.csv";
            string uploadjson = @"C:\Users\Jenisha\Desktop\students.json";
            string jsonfile = "/200450776%20Jenisha%20Thummar/students.json";
            string uploadxml = @"C:\Users\Jenisha\Desktop\students.xml";
            string xmlfile = "/200450776%20Jenisha%20Thummar/students.xml";
            Console.WriteLine(UploadFile(uploadcsv, url + csvfile));
            Console.WriteLine(UploadFile(uploadjson, url + jsonfile));
            Console.WriteLine(UploadFile(uploadxml, url + xmlfile));

        }
        static string UploadFile(string uploadfile, string url)
        {
            string output = "";

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);

            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(uploadfile))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            //Get the length or size of the file
            request.ContentLength = fileContents.Length;

            //Write the file to the stream on the server
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            //Send the request
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                output = $"Upload File Complete, status {response.StatusDescription}";
            }

            return (output);
        }


        static string GetDirectory(string url)
        {
            string output;

            //string directories = GetDirectory(url);

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            //request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    output = reader.ReadToEnd();
                }
            }

            Console.WriteLine($"Directory List Complete with status code: {response.StatusDescription}");

            return (output);
        }
    }
}

using System;

namespace Students
{
    public class Student
    {
        public string Name;
        public string NIM;
        public double IPK;

        public Student(string name, string nim, double ipk)
        {
            Name = name;
            NIM = nim;
            IPK = ipk;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Nama: {Name}, NIM: {NIM}, IPK: {IPK}");
        }

        public bool CheckIpk()
        {
            return IPK >= 3.0;
        }


    }
}
using System;
using System.IO;

namespace DDrop.Utility.FileOperations
{
    public static class FileOperations
    {
        public static void ClearDirectory(string name)
        {
            string path = Path.Combine(Environment.CurrentDirectory, name);

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void CreateDirectory(string name)
        {
            string path = Path.Combine(Environment.CurrentDirectory, name);

            Directory.CreateDirectory(path);
        }
    }
}

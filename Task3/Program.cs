using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp10
{
    class Program
    {
        // Метод для получения размера папки
        static long GetDirectorySize(DirectoryInfo directory)
        {
            long size = 0;
            try
            {
                var files = directory.GetFiles();
                foreach (var file in files)
                {
                    size += file.Length;
                }

                var directories = directory.GetDirectories();
                foreach (var dir in directories)
                {
                    size += GetDirectorySize(dir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return size;
        }

        // Метод для обработки файлов в папке
        static (int, long) LookFile(DirectoryInfo directory, TimeSpan interval)
        {
            int filesDeleted = 0;
            long spaceFreed = 0;

            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                if (CheckFile(file, interval))
                {
                    filesDeleted++;
                    spaceFreed += file.Length;
                }
            }

            DirectoryInfo[] dirs = directory.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                var result = LookFile(dir, interval);
                filesDeleted += result.Item1;
                spaceFreed += result.Item2;
            }

            return (filesDeleted, spaceFreed);
        }

        // Метод для проверки файла
        static bool CheckFile(FileInfo file, TimeSpan interval)
        {
            if (DateTime.Now - file.LastAccessTime > interval)
            {
                try
                {
                    file.Delete();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            TimeSpan interval = TimeSpan.FromMinutes(30);
            Console.WriteLine("Введите путь к папке");

            string path = Console.ReadLine();
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                // Размер папки до очистки
                long sizeBeforeCleanup = GetDirectorySize(dirInfo);
                Console.WriteLine($"Размер папки до очистки: {sizeBeforeCleanup / 1024.0 / 1024.0:F2} MB");

                // Очистка
                var (filesDeleted, spaceFreed) = LookFile(dirInfo, interval);
                Console.WriteLine($"Удалено файлов: {filesDeleted}");
                Console.WriteLine($"Освобождено места: {spaceFreed / 1024.0 / 1024.0:F2} MB");

                // Размер папки после очистки
                long sizeAfterCleanup = GetDirectorySize(dirInfo);
                Console.WriteLine($"Размер папки после очистки: {sizeAfterCleanup / 1024.0 / 1024.0:F2} MB");
            }
            else
            {
                Console.WriteLine("Путь к папке не найден");
            }
        }
    }
}

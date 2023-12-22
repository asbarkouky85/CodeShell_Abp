using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Linq;

namespace Codeshell.Abp.Files
{
    public class FileUtils
    {
        public static void DeleteEmptyDirectories(string folder)
        {
            var dir = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories);
            foreach (var d in dir)
            {
                if (Directory.Exists(d) && !Directory.GetFiles(d, "*", SearchOption.AllDirectories).Any())
                {
                    DeleteDirectory(d);
                }
            }
        }

        public static bool DeleteDirectory(string path)
        {
            try
            {
                ClearDirectory(path);
                Directory.Delete(path, true);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool ClearDirectory(string path)
        {
            try
            {
                var fls = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (var f in fls)
                    File.Delete(f);
                var dirs = Directory.GetDirectories(path);
                foreach (var f in dirs)
                    Directory.Delete(f, true);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static string GetFileFolder(string file)
        {
            FileInfo info = new FileInfo(file);
            return info.Directory.FullName;
        }

        public static void CompressDirectory(string folderPath, string targetPath, bool includeBaseDirectory = false)
        {
            if (File.Exists(targetPath))
                File.Delete(targetPath);
            ZipFile.CreateFromDirectory(folderPath, targetPath, CompressionLevel.Optimal, includeBaseDirectory);
        }

        public static void DecompressDirectory(string file, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string dir = Path.GetDirectoryName(folder);
            string newDir = Path.Combine(dir, Guid.NewGuid().ToString());

            ZipFile.ExtractToDirectory(file, newDir, Encoding.UTF8);

            string[] files = Directory.GetFiles(newDir, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var relative = f.Replace(newDir + "\\", "");
                string newFile = Path.Combine(folder, relative);
                if (File.Exists(newFile))
                    File.Delete(newFile);
                else
                    Utils.CreateFolderForFile(newFile);
                File.Move(f, newFile);
            }
            DeleteDirectory(newDir);
        }
    }
}

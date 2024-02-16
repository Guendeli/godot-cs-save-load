
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SimpleSaveLoadSystem
{
    public static class FolderUtils
    {
        public static void OpenDirectory(string path)
        {
            //UnityEngine.Debug.Log(($"Converted paths: {Path.GetFullPath(path)}"));
            Process.Start("explorer.exe", Path.GetFullPath(path));
        }

        public static void OpenSelectDirectory(string path)
        {
            Process.Start("explorer.exe", "/select, \"" + path + "\"");
        }

        public static void DeleteDirectory(string path, bool recursive = false)
        {
            if (DirectoryExists(path))
                Directory.Delete(path, recursive);
        }

        public static List<string> GetFilesInFolder(string path, bool includeSubDirectories = false,
            string pattern = "*.*")
        {
            List<string> files = new List<string>();
            if (DirectoryExists(path))
            {
                SearchOption options =
                    (includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                files.AddRange(Directory.GetFiles(path, pattern, options));
            }

            return files;
        }

        public static bool IsFolder(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static void DeleteDirectoryContent(string path)
        {
            if (DirectoryExists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.IsReadOnly = false;
                    fi.Delete();
                }
            }
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
    
    }
}


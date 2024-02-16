
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Godot;

namespace SimpleSaveLoadSystem
{
    public static class FileUtils
    {
        public static string SaveDataDirectory => _userDataPath + "Save/";

        private static string _userDataPath = ProjectSettings.GlobalizePath("user://");
        
         /// <summary>
        /// Determine whether a given path is a directory.
        /// </summary>
        public static bool PathIsDirectory(string absolutePath)
        {
            FileAttributes attr = File.GetAttributes(absolutePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            else
                return false;
        }

        public static void CopyFile(string source, string dest, bool overwrite, bool forceCreateDir = false)
        {
            if (forceCreateDir)
            {
                var directoryInfo = new FileInfo(dest).Directory;
                if (directoryInfo != null)
                {
                    FolderUtils.CreateDirectory(directoryInfo.FullName);
                }
            }

            if (FileExists(source))
            {
                File.Copy(source, dest, overwrite);
            }
            else
            {
                GD.PrintErr(string.Format("Can't move file, File: {0} does not exist ", source));
            }
        }

        public static void OpenFile(string file)
        {
            if (FileExists(file))
                Process.Start(file);
            else
            {
                GD.PrintErr(string.Format("Could not open file, File: {0} does not exist!", file));
            }
        }

        public static void SetReadOnlyProperty(string file, bool readOnly)
        {
            if (FileExists(file))
            {
                FileInfo fileInfo = new FileInfo(file);
                fileInfo.IsReadOnly = readOnly;
            }
            else
            {
                GD.PrintErr(string.Format("Could not open file, File: {0} does not exist!", file));
            }
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (!FileExists(path))
                CreateFile(path);

            GD.Print($"Writing File : {path}");
            File.WriteAllBytes(path, bytes);
            GD.Print($"File Written: {path}");
        }

        public static void WriteObjectSmart(string directoryPath, object data)
        {
            string simpleName = data.GetType().Name;
            byte[] bytes = SerializeUtils.Serialize(data);

            string path;
            if (directoryPath.EndsWith("/"))
                path = directoryPath + simpleName;
            else
                path = directoryPath + "/" + simpleName;

            WriteAllBytes(path, bytes);
        }
        public static T ReadObjectSmart<T>(string directoryPath)
        {
            return (T)ReadObjectSmart<T>(typeof(T), directoryPath);
        }
        public static object ReadObjectSmart<T>(Type type,string directoryPath)
        {
            string simpleName = type.Name;
            string path;
            if (directoryPath.EndsWith("/"))
                path = directoryPath + simpleName;
            else
                path = directoryPath + "/" + simpleName;

            if (FileExists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                return SerializeUtils.Deserialize<T>(bytes);
            }
            return null;
        }
        public static byte[] ReadAllBytes(string path)
        {
            if (FileExists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        public static void WriteObject(string path, object data)
        {
            byte[] bytes = SerializeUtils.Serialize(data);
            WriteAllBytes(path, bytes);
        }


        public static void CreateFile(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Create(path).Close();
        }

        public static void DeleteFile(string path)
        {
            if(FileExists(path))
                File.Delete(path);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void WriteToFile(string path, string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (!FileExists(path))
            {
                CreateFile(path);
            }

            File.WriteAllText(path, text, encoding);
        }

        public static void AppendToFile(string path, string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (FileExists(path))
                File.AppendAllText(path, text, encoding);
            else
                GD.PrintErr(string.Format("Can't write to file, File: {0} does not exist", path));
        }

        public static string ReadTextFromFile(string path , bool isCritical = true, bool showLog = false)
        {
            if (FileExists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                if (showLog)
                {
                    if(isCritical)
                        GD.PrintErr(string.Format("Can't read from file, File: {0} does not exist", path));
                    else
                        GD.Print(string.Format("Can't read from file, File: {0} does not exist", path));
                }


                return "";
            }
        }

        public static string[] ReadLinesFromText(string path)
        {
            if (FileExists(path))
            {
                return File.ReadAllLines(path);
            }
            else
            {
                GD.PrintErr(string.Format("Can't read from file, File: {0} does not exist", path));
                return null;
            }
        }

        public static string FileSize(string path)
        {
            string[] sizes = {"B", "KB", "MB", "GB", "TB"};
            double len = new FileInfo(path).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
            ;
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string ConvertByteToKiloByte(double size)
        {
            string[] suffix = {"B", "KB", "MB", "GB"};
            int index = 0;
            do
            {
                size /= 1024;
                index++;
            } while (size >= 1024);

            return string.Format("{0:0.00} {1}", size, suffix[index]);
        }

        public static Dictionary<string, byte[]> GetFolderContents(string folder)
        {
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            DirectoryInfo info = new DirectoryInfo(folder);
            if (!info.Exists)
                Directory.CreateDirectory(folder);

            FileInfo[] fileInfos = info.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Exists)
                {
                    byte[] bytes = File.ReadAllBytes(folder + "/"+ fileInfo.Name);
                    result.Add(fileInfo.Name, bytes);
                }
            }

            return result;
        }

        public static bool FileExistCaseSensitive(string filepath)
        {
            string physicalPath = GetWindowsPhysicalPath(filepath);
            if (physicalPath == null) return false;
            if (filepath != physicalPath) return false;
            else return true;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetLongPathName(string ShortPath, StringBuilder sb, int buffer);

        [DllImport("kernel32.dll")]
        static extern uint GetShortPathName(string longpath, StringBuilder sb, int buffer);

        private static string GetWindowsPhysicalPath(string path)
        {
            StringBuilder builder = new StringBuilder(255);

            // names with long extension can cause the short name to be actually larger than
            // the long name.
            GetShortPathName(path, builder, builder.Capacity);

            path = builder.ToString();

            uint result = GetLongPathName(path, builder, builder.Capacity);

            if (result > 0 && result < builder.Capacity)
            {
                //Success retrieved long file name
                //builder[0] = char.ToLower(builder[0]);
                return builder.ToString(0, (int)result);
            }

            if (result > 0)
            {
                //Need more capacity in the buffer
                //specified in the result variable
                builder = new StringBuilder((int)result);
                result = GetLongPathName(path, builder, builder.Capacity);
                //builder[0] = char.ToLower(builder[0]);
                return builder.ToString(0, (int)result);
            }

            return null;
        }

        public static List<string> RecursiveDirSearch(string sDir,string searchPattern="*.*")
        {
            List<string> filesList = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, searchPattern))
                    {
                        filesList.Add(f);
                    }
                    filesList.AddRange(RecursiveDirSearch(d, searchPattern));
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return filesList;
        }
    }
}


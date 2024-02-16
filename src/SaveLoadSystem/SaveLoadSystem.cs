using System;
using System.IO;

namespace SimpleSaveLoadSystem
{
    public class SaveLoadSystem
    {
        private string _directory;

        public SaveLoadSystem()
        {
            _directory = FileUtils.SaveDataDirectory;
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void DeleteSavedData()
        {
            string target_dir = _directory;
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Directory.Delete(target_dir, false);
        }
        
        public void Init()
        {
            _directory = FileUtils.SaveDataDirectory;
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
        
        public T GetData<T>()
        {
            T data = FileUtils.ReadObjectSmart<T>(_directory);
            return data;
        }

        public void SaveData<T>(T data)
        {
            FileUtils.WriteObjectSmart(FileUtils.SaveDataDirectory, data);
        }
    }
}


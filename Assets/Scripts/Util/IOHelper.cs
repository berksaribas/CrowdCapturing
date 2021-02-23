using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Util
{
    public static class IOHelper
    {
        ///    File System API

        public static readonly string PathToAssets = Application.dataPath;
        
        public static string GetFullPath(string path, string name, string extension = "bytes")
        {
            return Path.Combine(
                PathToAssets,
                path,
                $"{name}.{extension}"
            );
        }

        public static FileStream OpenOrCreateFile(string path, string name, string extension = "bytes")
        {
            return File.Open(GetFullPath(path, name), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
        
        public static void SaveAsBinary<T>(T o, string path, string name)
        {
            using (var file = OpenOrCreateFile(path, name))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, o);
            }
        }

        public static T LoadFromBinary<T>(string path, string name) where T : class
        {
            using (var file = OpenOrCreateFile(path, name))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(file) as T;
            }
        }

        public static void DeleteFile(string path, string name)
        {
            File.Delete(GetFullPath(path, name));
        }

        public static bool DoesFileExist(string path, string name, string extension = "bytes")
        {
            return File.Exists(GetFullPath(path, name, extension));
        }

        ///    Unity Asset System API
        
        public static string GetAssetsRelativePath(string path, string name, string extension = "bytes")
        {
            return Path.Combine(
                "Assets",
                path,
                $"{name}.{extension}"
            );
        }

        public static T LoadFromBinaryAsset<T>(TextAsset asset) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                ms.Write(asset.bytes, 0, asset.bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                return formatter.Deserialize(ms) as T;
            }
        }
    }
}
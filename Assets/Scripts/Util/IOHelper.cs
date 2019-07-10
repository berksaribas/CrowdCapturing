using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Util
{
    public static class IOHelper
    {
        public static readonly string PathToAssets = Application.dataPath;

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }

        public static T[] FromJsonArrayOf<T>(TextAsset asset)
        {
            return JsonUtility.FromJson<Wrapper<T>>($"{{\"Items\": {asset.text}}}").Items;
        }

        public static void SaveAsBinary(object o, string path, string name)
        {
            var finalPath = Path.Combine(
                PathToAssets,
                path,
                $"{name}.bytes"
            );
        
            using (var file = File.Open(finalPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, o);
            }
        }

        public static T LoadFromBinary<T>(string path, string name) where T : class
        {
            var finalPath = Path.Combine(
                PathToAssets,
                path,
                $"{name}.bytes"
            );
        
            using (var file = File.Open(finalPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(file) as T;
            }
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

        public static void DeleteBinary(string path, string name)
        {
            var finalPath = Path.Combine(
                PathToAssets,
                path,
                $"{name}.bytes"
            );
        
            File.Delete(finalPath);
        }

        public static bool DoesBinaryExist(string path, string name)
        {
            var finalPath = Path.Combine(
                PathToAssets,
                path,
                $"{name}.bytes"
            );

            return File.Exists(finalPath);
        }
    }
}
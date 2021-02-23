using System;
using UnityEditor;
using UnityEngine;

namespace Util
{
    [Serializable]
    public class Baker
    {
        public string SavePath = "Baked";
        public string SaveName = "save";

        private readonly Func<MonoBehaviour, object> bakeOperation;
        public Action BakeAction;

        public bool IsBaking => BakeAction != null;
        public bool IsBaked => IOHelper.DoesFileExist(SavePath, SaveName);

        public Baker(Func<MonoBehaviour, object> operation)
        {
            bakeOperation = operation;
        }

        public void CreateBakeAction(MonoBehaviour component)
        {
            BakeAction = () =>
            {
                Debug.Log("@BakeAction: starting baking");

                var baked = bakeOperation(component);
                IOHelper.SaveAsBinary(baked, SavePath, SaveName);

                AssetDatabase.ImportAsset(
                    IOHelper.GetAssetsRelativePath(SavePath, SaveName)
                );
                
                BakeAction = null;
            };
        }

        public void CancelBakeAction()
        {
            BakeAction = null;
        }

        public virtual void Clear()
        {
            AssetDatabase.DeleteAsset(
                IOHelper.GetAssetsRelativePath(SavePath, SaveName)
            );
        }

        public T LoadBaked<T>() where T : class
        {
            return IOHelper.LoadFromBinary<T>(SavePath, SaveName);
        }
    }
}
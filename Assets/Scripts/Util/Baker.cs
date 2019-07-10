using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Util
{
    [Serializable]
    public class Baker
    {
        public float Progress => progress / progressMax;
        public bool Baking => Progress >= 0f;
        public bool Baked => IOHelper.DoesBinaryExist(SaveTo, FileName);

        public string SaveTo = "Baked";
        public string FileName = "save";

        protected readonly Func<MonoBehaviour, Task<object>> bake;
        protected float progress = -1f;
        protected float progressMax = 100f;
        protected string canceled = null;
        protected string failed = null;

        public Baker(Func<MonoBehaviour, Task<object>> bake)
        {
            this.bake = bake;
        }

        public async Task Bake(MonoBehaviour component)
        {
            if (Baking)
            {
                Debug.Log("Already baking :S");
                return;
            }

            Debug.Log("Starting bake coroutine :d");

            ResetProgress();

            progress = 0f;

            try
            {
                var baked = bake.Invoke(component).Result;
                IOHelper.SaveAsBinary(baked, SaveTo, FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ResetProgress();

                throw;
            }

            if (canceled != null)
            {
                Debug.Log("Canceled bake coroutine :|");
                Debug.Log($"Reason => {canceled}");
            }
            else if (failed != null)
            {
                Debug.Log("Failed bake coroutine :|");
                Debug.Log($"Reason => {failed}");
            }
            else
            {
                Debug.Log("Finished bake coroutine :P");
            }

            ResetProgress();
        }

        public void ResetProgress()
        {
            progress = -1f;
            canceled = null;
            failed = null;
        }

        public virtual void Clear()
        {
            IOHelper.DeleteBinary(SaveTo, FileName);
        }

        public void Cancel(string message = "Baking Canceled")
        {
            canceled = message;
        }

        protected void Fail(string message = "Baking Failed")
        {
            failed = message;
        }

        public T LoadBaked<T>() where T : class
        {
            return IOHelper.LoadFromBinary<T>(SaveTo, FileName);
        }
    }
}
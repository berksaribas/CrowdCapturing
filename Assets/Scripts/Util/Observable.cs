using System;
using JetBrains.Annotations;

namespace Util
{
    public class Observable<T> where T : class
    {
        [CanBeNull] private T value;

        public event Action<T> OnChange;

        public T Get()
        {
            return value;
        }
        
        public void Set(T newValue)
        {
            if (value == newValue) return;
            
            value = newValue;
            OnChange?.Invoke(newValue);
        }
    }
}
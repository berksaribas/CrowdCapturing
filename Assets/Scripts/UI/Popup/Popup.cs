using System;
using TMPro;
using UnityEngine;

namespace UI.Popup
{
    public class Popup : MonoBehaviour
    {
        public class Data
        {
            public string Title, Message;

            public Data(string title, string message)
            {
                Title = title;
                Message = message;
            }
        }
        
        [NonSerialized]
        private Data data;

        public TextMeshProUGUI Title, Message;

        public void SetData(Data newData)
        {
            data = newData;
            
            Title.text = data.Title;
            Message.text = data.Message;
        }
    }
}
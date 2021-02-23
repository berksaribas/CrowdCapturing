using UnityEngine;

namespace World
{
    public class DoorManager : MonoBehaviour
    {
        [HideInInspector] public Door[] Doors;

        private void Awake()
        {
            Doors = FindObjectsOfType<Door>();
        }
    }
}
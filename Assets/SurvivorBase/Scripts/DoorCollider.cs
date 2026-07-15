using UnityEngine;

namespace SurvivorBase.Scripts
{
    public class DoorCollider : MonoBehaviour
    {
        [SerializeField] private Door _door;
    
        public void Handle()
        {
            _door.Handle();
        }
    }
}

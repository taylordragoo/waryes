using UnityEngine;

namespace SurvivorBase.Scripts
{
    public class DoorHandler : MonoBehaviour
    {
        [SerializeField] private KeyCode _interactableKey = KeyCode.E;
   
        private DoorCollider _doorCollider;

        private void Update()
        {
            if (Input.GetKeyUp(_interactableKey))
                _doorCollider?.Handle();
        }

        public void TryHandle()
        {
            if (_doorCollider)
                _doorCollider?.Handle();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DoorCollider doorColider))
            {
                _doorCollider = doorColider;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_doorCollider != null && other.gameObject == _doorCollider.gameObject)
            {
                _doorCollider = null;
            }
        }

    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivorBase.Scripts
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private OpenClosedValue RotationAngles = new OpenClosedValue(90, 0);
        [SerializeField] private float RotationSpeed = 2f;
    
        private bool _isOpen = false;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;

        public UnityEvent DoorOpened;
        public UnityEvent DoorClosed;

        private void Start()
        {
            _closedRotation = Quaternion.Euler(0, RotationAngles.ClosedValue, 0) * transform.rotation;
            _openRotation = Quaternion.Euler(0, RotationAngles.OpenValue, 0) * transform.rotation;
        }

        [ContextMenu("Handle")]
        public void Handle()
        {
            StartCoroutine(_isOpen ? CloseDoor() : OpenDoor());
        }

        private IEnumerator OpenDoor()
        {
            _isOpen = true;
            while (Quaternion.Angle(transform.rotation, _openRotation) > 0.01f)
            {
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, _openRotation, RotationSpeed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = _openRotation;
            DoorOpened?.Invoke();
        }

        private IEnumerator CloseDoor()
        {
            _isOpen = false;
            while (Quaternion.Angle(transform.rotation, _closedRotation) > 0.01f)
            {
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, _closedRotation, RotationSpeed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = _closedRotation;
            DoorClosed?.Invoke();
        }
    }

    [Serializable]
    public struct OpenClosedValue
    {
        public float OpenValue;
        public float ClosedValue;

        public OpenClosedValue(float openValue, float closedValue)
        {
            OpenValue = openValue;
            ClosedValue = closedValue;
        }
    }
}
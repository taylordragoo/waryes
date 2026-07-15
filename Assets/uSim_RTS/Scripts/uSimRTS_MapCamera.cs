using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_MapCamera : MonoBehaviour
    {
        public Transform playerCameraRoot;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = playerCameraRoot.position;
            Vector3 eulers = transform.eulerAngles;
            eulers.y = playerCameraRoot.eulerAngles.y;
            transform.rotation = Quaternion.Euler(eulers);
        }
    }
}

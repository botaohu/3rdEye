using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloPlaySDK
{
    public class onOff : MonoBehaviour
    {

        public depthPlugin toggle;
        public KeyCode key;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(key))
                toggle.enabled = !toggle.enabled;

        }
    }
}

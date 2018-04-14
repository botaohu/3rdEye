using UnityEngine;

namespace HoloPlaySDK_Tests
{
    public class SimpleReturnToPos : MonoBehaviour
    {
        Rigidbody rb;
        Vector3 origin;

        public float intensity;

        void Awake()
        {
            origin = transform.position;
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            rb.AddForce((origin - rb.position) * Time.deltaTime * intensity, ForceMode.Acceleration);
        }
    }
}
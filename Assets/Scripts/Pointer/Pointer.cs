using UnityEngine;

namespace Pointer
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private float thickness;
        [SerializeField] private Material pointerMaterial;

        private GameObject pointer;

        private const float DefaultDistance = 100f;
        private const string PointerObjectName = "Laser";

        public void Start()
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pointer.name = PointerObjectName;
            pointer.transform.localScale = new Vector3(thickness, thickness, DefaultDistance);
            pointer.transform.localPosition = new Vector3(0f, 0f, DefaultDistance / 2f);
            pointer.transform.localRotation = Quaternion.Euler(90, 0, 0);
            pointer.transform.SetParent(transform);
            pointer.GetComponent<MeshRenderer>().material = pointerMaterial;
            var pointerCollider = pointer.GetComponent<CapsuleCollider>();
            Destroy(pointerCollider);
        }

        public void Update()
        {
            var controllerTransform = transform;
            var raycast = new Ray(controllerTransform.position, controllerTransform.forward);
            var hasHit = Physics.Raycast(raycast, out var hit);
            var laserDistance = DefaultDistance;

            if (hasHit)
                laserDistance = hit.distance;

            pointer.transform.localScale = new Vector3(thickness, laserDistance / 2f, thickness);
            pointer.transform.localPosition = new Vector3(0f, 0f, laserDistance / 2f);
        }
    }
}

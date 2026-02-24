using UnityEngine;

namespace Player
{
    public class MouseLookY : MonoBehaviour
    {
        [SerializeField] private float _sensVertical;
        [SerializeField] private float _minVertical;
        [SerializeField] private float _maxVertical;

        private float rotationX = 0f;

        private Protagonist _protagonist;


        private void Awake() => _protagonist = GetComponentInParent<Protagonist>();

        private void Update()
        {
            if (_protagonist.IsDead == false)
            {
                rotationX -= Input.GetAxis("Mouse Y") * _sensVertical;
                rotationX = Mathf.Clamp(rotationX, _minVertical, _maxVertical);
                float rotationY = transform.localEulerAngles.y;
                transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
            }   
        }
    }
}
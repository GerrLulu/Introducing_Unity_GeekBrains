using System;
using UnityEngine;

namespace Doors
{
    public class BlueCard : MonoBehaviour
    {
        [SerializeField] private float _rotationalSpeed;

        public static Action GiveBlueCard;


        private void Update() => transform.Rotate(Vector3.up, _rotationalSpeed * Time.deltaTime);

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                gameObject.SetActive(false);

                if (GiveBlueCard != null)
                    GiveBlueCard.Invoke();
            }
        }
    }
}
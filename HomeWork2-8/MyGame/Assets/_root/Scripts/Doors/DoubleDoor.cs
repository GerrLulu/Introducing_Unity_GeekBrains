using Player;
using UnityEngine;

namespace Doors
{
    public class DoubleDoor : MonoBehaviour
    {
        [SerializeField] private Protagonist _protagonist;

        private Animation _anim;


        private void Awake() => _anim = GetComponent<Animation>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && _protagonist.IsHaveBlueCard == true)
                _anim.Play("DoubleDoorOpen");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player" && _protagonist.IsHaveBlueCard == true)
                _anim.Play("DoubleDoorClose");
        }
    }
}
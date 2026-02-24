using System.Collections;
using UnityEngine;

namespace MineItem
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private int _damage;

        [SerializeField] private float _lifeTime;
        [SerializeField] private float _radiusExplosion;
        [SerializeField] private float _force;

        [SerializeField] private AudioClip[] _audioClips;


        private AudioSource _audioSours;

        private ParticleSystem _particleSystem;


        private void Awake()
        {
            _audioSours = GetComponent<AudioSource>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Start () => StartCoroutine("TimeToDie");

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player" || other.tag == "Enemy")
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusExplosion);
                foreach (Collider hit in colliders)
                {
                    IMineExplosion obj;

                    if (hit.TryGetComponent<IMineExplosion>(out obj))
                    {
                        if (obj != null)
                            obj.MineHit(_damage, _force, transform.position);
                    }
                }

                AudioClip clip = _audioClips[Random.Range(0, _audioClips.Length)];
                _audioSours.clip = clip;
                _audioSours.Play();

                _particleSystem.Play(true);

                Destroy(gameObject);
            }
        }


        private IEnumerator TimeToDie()
        {
            yield return new WaitForSeconds(_lifeTime);
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace Bullet
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int _damage;

        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;

        [SerializeField] private AudioClip[] _audioClipsImpact;


        private AudioSource _audioSource;


        private void Awake() => _audioSource = GetComponent<AudioSource>();

        void FixedUpdate()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
            Destroy(gameObject, _lifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            AudioClip clip = _audioClipsImpact[Random.Range(0, _audioClipsImpact.Length)];
            _audioSource.clip = clip;
            _audioSource.Play();

            IBulletDamage obj;

            if (other.TryGetComponent<IBulletDamage>(out obj))
            {
                if (obj != null)
                {
                    obj.Hit(_damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
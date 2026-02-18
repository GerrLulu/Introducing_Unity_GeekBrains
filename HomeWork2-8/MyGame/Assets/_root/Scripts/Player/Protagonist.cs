using Bullet;
using Doors;
using MineItem;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    public class Protagonist : MonoBehaviour, IMineExplosion, IBulletDamage
    {
        [SerializeField] private int _hp = 100;

        [SerializeField] private float _speed = 0.1f;
        [SerializeField] private float _boost = 1.5f;
        [SerializeField] private float _sensHorizontal = 7f;
        [SerializeField] private float _forceJump = 300f;
        [SerializeField] private float _accelerationAnim = 0.1f;
        [SerializeField] private float _decelerationAnim = 0.5f;

        [SerializeField] private GameObject _bulletPrefub;
        [SerializeField] private GameObject _minePrefab;
        [SerializeField] private GameObject _blueCardImg;
        [SerializeField] private GameObject _panelHP;
        [SerializeField] private GameObject _panelMenuPause;

        [SerializeField] private Transform _spawnBullet;
        [SerializeField] private Transform _spawnPointMine;

        [SerializeField] private Slider _sliderHP;

        [SerializeField] private AudioSource _audioShoot;
        [SerializeField] private AudioSource _audioWalk;
        [SerializeField] private AudioSource _audioRun;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _mixerGroup;

        private int _velocityHash;
        private float _velocity = 0.0f;
        private bool _isBoost;
        private bool _isGround;
        private bool _isHaveBlueCard;
        private bool _isGamePaused;
        private bool _isAudioMovePlay;
        private Vector3 _direction;
        private Rigidbody _rb;
        private Animator _animator;
        

        public int Hp
        {
            get { return _hp; }
            set { _hp = value; }
        }
        public bool IsHaveBlueCard { get { return _isHaveBlueCard; } }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _isGround = true;
            _isHaveBlueCard = false;
            _isGamePaused = false;

            _velocityHash = Animator.StringToHash("Velocity");

            BlueCard.GiveBlueCard += GetBlueCard;

            StartCoroutine(AudioMove());
        }

        private void Update()
        {
            _direction.z = Input.GetAxis("Vertical");
            _direction.x = Input.GetAxis("Horizontal");
            _isBoost = Input.GetButton("Boost");

            transform.Rotate(0, Input.GetAxis("Mouse X") * _sensHorizontal, 0);

            if (Input.GetButtonDown("Jump") && _isGround)
                Jump();

            if (Input.GetButtonDown("Put mine"))
                SpawnMine();

            if (Input.GetButtonDown("Fire1"))
                Shoot();

            _sliderHP.value = _hp;

            if (Input.GetButtonDown("Pause"))
            {
                if (!_isGamePaused)
                {
                    PausedGame();
                    _isGamePaused = true;
                }
            }
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Ground")
            {
                _animator.SetBool("IsJump", false);
                _isGround = true;
            }
        }


        private void Move()
        {
            float s;

            if (_direction == Vector3.zero)
            {
                _animator.SetBool("IsMove", false);
                _audioWalk.Stop();
                _audioRun.Stop();
            }
            else
            {
                _animator.SetBool("IsMove", true);
            }

            if (_isBoost)
                s = _boost * _speed;
            else
                s = _speed;

            if (_isBoost && _velocity <= 1.0f)
                _velocity += Time.deltaTime * _accelerationAnim;
            else if (!_isBoost && _velocity > 0.0f)
                _velocity -= Time.deltaTime * _decelerationAnim;
            else if (!_isBoost && _velocity < 0.0f)
                _velocity = 0.0f;

            _animator.SetFloat(_velocityHash, _velocity);

            transform.Translate(_direction.normalized * s);

            if (_isAudioMovePlay)
            {
                if (_isBoost)
                    _audioRun.Play();
                else
                    _audioWalk.Play();
            }

            _isAudioMovePlay = false;
        }

        private void Jump()
        {
            _rb.AddForce(new Vector3(0, _forceJump, 0), ForceMode.Impulse);
            _animator.SetBool("IsJump", true);
            _isGround = false;
        }

        private void Shoot()
        {
            Instantiate(_bulletPrefub, _spawnBullet.position, _spawnBullet.rotation);
            _audioShoot.Play();
            _animator.SetTrigger("Shoot");
        }

        private void SpawnMine()
        {
            Instantiate(_minePrefab, _spawnPointMine.position, _spawnPointMine.rotation);
        }

        public void Hit(int damage)
        {
            _hp = _hp - damage;
            DieProtagonist(_hp);
        }

        public void MineHit(int damage, float force, Vector3 positionMine)
        {
            _hp = _hp - damage;
            
            var positionImpulse = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Vector3 direction = positionImpulse - positionMine;
            _rb.AddForce(direction.normalized * force, ForceMode.Impulse);

            DieProtagonist(_hp);
        }

        private void GetBlueCard()
        {
            _isHaveBlueCard = true;
            _blueCardImg.SetActive(true);
        }

        private void DieProtagonist(int hp)
        {
            if(hp <= 0)
            {
                _animator.SetTrigger("Die");
                Application.Quit();
            }
        }

        public void PausedGame()
        {
            Time.timeScale = 0;
            _panelHP.SetActive(false);
            _blueCardImg.SetActive(false);
            _panelMenuPause.SetActive(true);
        }

        public void BackGame()
        {
            Time.timeScale = 1;
            _isGamePaused = false;
            _panelHP.SetActive(true);
            _panelMenuPause.SetActive(false);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            SceneManager.LoadScene(0);
        }

        private IEnumerator AudioMove()
        {
            while (true)
            {
                if (_audioWalk.isPlaying || _audioRun.isPlaying)
                {
                    yield return null;
                }
                else
                {
                    _isAudioMovePlay = true;
                    yield return null;
                }
            }
        }

        public void ToggleMusic(bool enabled)
        {
            if (enabled)
                _mixer.SetFloat(_mixerGroup.name, -80f);
            else
                _mixer.SetFloat(_mixerGroup.name, 0f);
        }

        public void ChangeVolume(float volume)
        {
            _mixer.SetFloat(_mixerGroup.name, Mathf.Lerp(-80f, 20f, volume));
        }


        private void OnDestroy()
        {
            BlueCard.GiveBlueCard -= GetBlueCard;
        }
    }
}
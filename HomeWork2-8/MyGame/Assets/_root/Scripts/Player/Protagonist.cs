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
        [SerializeField] private int _hp;
        [SerializeField] private int _timeAnimDie;

        [SerializeField] private float _speed;
        [SerializeField] private float _boost;
        [SerializeField] private float _sensHorizontal;
        [SerializeField] private float _forceJump;
        [SerializeField] private float _accelerationAnim;
        [SerializeField] private float _decelerationAnim;

        [SerializeField] private GameObject _bulletPrefub;
        [SerializeField] private GameObject _minePrefab;
        [SerializeField] private GameObject _blueCardImg;
        [SerializeField] private GameObject _panelHP;
        [SerializeField] private GameObject _panelMenuPause;
        [SerializeField] private GameObject _panelGameOver;

        [SerializeField] private Transform _spawnBullet;
        [SerializeField] private Transform _spawnPointMine;

        [SerializeField] private Toggle _muteToggle;

        [SerializeField] private Slider _sliderHP;
        [SerializeField] private Slider _sliderSoundVolume;

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
        private bool _isDead;

        private Vector3 _direction;

        private Rigidbody _rb;
        
        private Animator _animator;
        

        public int Hp
        {
            get { return _hp; }
            set { _hp = value; }
        }

        public bool IsHaveBlueCard { get { return _isHaveBlueCard; } }
        public bool IsDead { get { return _isDead; } }


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
            _isDead = false;

            _velocityHash = Animator.StringToHash("Velocity");

            BlueCard.GiveBlueCard += GetBlueCard;

            StartCoroutine(AudioMove());

            _mixer.GetFloat(_mixerGroup.name, out float valueVolume);

            if (valueVolume == -80f)
                _muteToggle.isOn = true;
            else
                _muteToggle.isOn = false;

            _sliderSoundVolume.value = (valueVolume - (-80f)) / (20f - (-80f));
        }

        private void Update()
        {
            if (_isDead == false || _isGamePaused == false)
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

                if (Input.GetButtonDown("Pause"))
                {
                    PausedGame();
                    _isGamePaused = true;
                }
            }
        }

        private void FixedUpdate() => Move();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Ground")
            {
                _animator.SetBool("IsJump", false);
                _isGround = true;
            }
        }

        private void OnDestroy() => BlueCard.GiveBlueCard -= GetBlueCard;


        private void Move()
        {
            float speed;

            if (_direction == Vector3.zero)
            {
                _animator.SetBool("IsMove", false);
                _audioWalk.Stop();
                _audioRun.Stop();
            }
            else
                _animator.SetBool("IsMove", true);

            if (_isBoost)
                speed = _boost * _speed;
            else
                speed = _speed;

            if (_isBoost && _velocity <= 1.0f)
                _velocity += Time.deltaTime * _accelerationAnim;
            else if (!_isBoost && _velocity > 0.0f)
                _velocity -= Time.deltaTime * _decelerationAnim;
            else if (!_isBoost && _velocity < 0.0f)
                _velocity = 0.0f;

            _animator.SetFloat(_velocityHash, _velocity);

            transform.Translate(_direction.normalized * speed);

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

        private void SpawnMine() => Instantiate(_minePrefab, _spawnPointMine.position, _spawnPointMine.rotation);

        private void Shoot()
        {
            Instantiate(_bulletPrefub, _spawnBullet.position, _spawnBullet.rotation);
            _audioShoot.Play();
            _animator.SetTrigger("Shoot");
        }


        public void Hit(int damage) => ChangeHealthPoint(damage);

        public void MineHit(int damage, float force, Vector3 positionMine)
        {
            ChangeHealthPoint(damage);

            var positionImpulse = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Vector3 direction = positionImpulse - positionMine;
            _rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }

        private void ChangeHealthPoint(int changeHP)
        {
           _hp = _hp + changeHP;
           _sliderHP.value = _hp;

            if (_hp <= 0)
            {
                _isDead = true;
                _animator.SetTrigger("Die");
                StartCoroutine(ProtogonistDies());
            }
        }

        private void GetBlueCard()
        {
            _isHaveBlueCard = true;
            _blueCardImg.SetActive(_isHaveBlueCard);
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
            _blueCardImg.SetActive(_isHaveBlueCard);
            _panelMenuPause.SetActive(false);
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }

        public void ToMainMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void ToggleMusic(bool enabled)
        {
            if (enabled)
                _mixer.SetFloat(_mixerGroup.name, -80f);
            else
                _mixer.SetFloat(_mixerGroup.name, 20f);
        }

        public void ChangeVolume(float volume) => _mixer.SetFloat(_mixerGroup.name, Mathf.Lerp(-80f, 20f, volume));


        private IEnumerator AudioMove()
        {
            while (true)
            {
                if (_audioWalk.isPlaying || _audioRun.isPlaying)
                    yield return null;
                else
                {
                    _isAudioMovePlay = true;
                    yield return null;
                }
            }
        }

        private IEnumerator ProtogonistDies()
        {
            yield return new WaitForSeconds(_timeAnimDie);
            _panelHP.SetActive(false);
            _blueCardImg.SetActive(false);
            _panelGameOver.SetActive(true);
        }
    }
}
using Bullet;
using MineItem;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IMineExplosion, IBulletDamage
    {
        [SerializeField] private int _hp;
        [SerializeField] private int _damage;
        [SerializeField] private int _timeToDestroy;

        [SerializeField] private float _huntingDistance;
        [SerializeField] private float _attackDistance;

        [SerializeField] private GameObject _attackZone;

        [SerializeField] private Transform[] _wayPoints;
        [SerializeField] private Transform _eyePosition;

        [SerializeField] private Protagonist _protagonist;

        private int m_CurrentWaypointIndex;

        private bool _isDead;

        private Ray _rayToPlayer;
        
        private Rigidbody _rb;
        
        private NavMeshAgent _agent;
        
        private Animator _animator;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _isDead = false;

            _agent.SetDestination(_wayPoints[0].position);
        } 
            

        private void FixedUpdate()
        {
            RaycastHit hit;

            Vector3 direction = _protagonist.transform.position - _eyePosition.position;
            direction = new Vector3(direction.x, direction.y + 1.7f, direction.z);
            _rayToPlayer = new Ray(_eyePosition.position, direction);
            Physics.Raycast(_rayToPlayer, out hit);

            if (hit.collider != null || _isDead == false)
            {
                if (hit.distance <= _huntingDistance)
                {
                    Debug.DrawRay(_eyePosition.position, direction, Color.red);

                    AttackRun(hit);
                }
                else
                {
                    Debug.DrawRay(_eyePosition.position, direction, Color.green);
                    
                    Patrol();
                }
            }
        }


        private void AttackRun(RaycastHit hit)
        {
            _animator.SetBool("IsRun", true);
            _agent.SetDestination(_protagonist.transform.position);

            if (hit.distance <= _attackDistance)
                _animator.SetBool("IsAttack", true);
            else
                _animator.SetBool("IsAttack", false);
        }

        private void Attack() => _protagonist.Hit(_damage);

        private void Patrol()
        {
            _animator.SetBool("IsRun", false);

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % _wayPoints.Length;
                _agent.SetDestination(_wayPoints[m_CurrentWaypointIndex].position);
            }
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

            if (_hp <= 0)
            {
                _isDead = true;
                _animator.SetTrigger("Die");
                StartCoroutine(DestroyEnemy());
            }
        }


        private IEnumerator DestroyEnemy()
        {
            yield return new WaitForSeconds(_timeToDestroy);
            Destroy(gameObject);
        }
    }
}
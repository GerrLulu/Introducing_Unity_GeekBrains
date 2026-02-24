using UnityEngine;

namespace Enemies
{
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;


        private void Start() => Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
    }
}
using Bullet;
using MineItem;
using UnityEngine;

namespace Turret
{
    public class TurretTriggerHP : MonoBehaviour, IBulletDamage, IMineExplosion
    {
        private Turret _turret;


        private void Start() => _turret = GetComponentInParent<Turret>();


        public void Hit(int damage) => ChangeHealthPoint(damage);

        public void MineHit(int damage, float force, Vector3 position) => ChangeHealthPoint(damage);

        private void ChangeHealthPoint(int changeHP)
        {
            _turret.Hp = _turret.Hp + changeHP;

            if (_turret.Hp <= 0)
                _turret.TurretDestruction();
        }
    }
}
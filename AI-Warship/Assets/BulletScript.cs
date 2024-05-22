using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Statistics;

namespace ShipGame.Ship.Weapons
{
    public class BulletScript : MonoBehaviour
    {
        private int bulletDamage;

        private void OnCollisionEnter(Collision other)
        {
            GameObject targetShip = other.gameObject;
            ShipStats targetShipStats = targetShip.GetComponent<ShipStats>();
            if (targetShipStats != null)
            {
                targetShipStats.LoseHealth(bulletDamage);
            }
            //PARTICLE EFFECT SPAWN AT POINT
            Destroy(this.gameObject);
        }

        public void SetBulletDamage(int damage)
        {
            bulletDamage = damage;
        }
    }
}
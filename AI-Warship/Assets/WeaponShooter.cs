using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Weapons
{
    public class WeaponShooter : MonoBehaviour
    {
        //AVFYRER FORSKJELLIGE VÅPEN SOM ER PÅ SKIPET VED Å TA KALKULASJONER I FRA WEAPONSYSTEM.SCRIPT



        public void FireTurret(GameObject bulletPrefab, Transform gunEnd, Vector3 totalVelocity, int damage)
        {
            GameObject bullet = Instantiate(bulletPrefab, gunEnd.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = totalVelocity;
            bullet.GetComponent<BulletScript>().SetBulletDamage(damage);
        }

    }
}
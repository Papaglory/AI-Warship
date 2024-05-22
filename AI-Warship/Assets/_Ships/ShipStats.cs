using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Statistics
{
    public class ShipStats : MonoBehaviour
    {
        [Header("Health Options")]
        [SerializeField] float defaultMaxHealth = 100;
        [SerializeField] float health;

        [Header("Fuel Options")]
        [SerializeField] float maxFuel = 100;
        [SerializeField] float fuel;
        [SerializeField] float fuelLossRate = 0.5f;

        [Header("Other Options")]
        [SerializeField] int level;
        [SerializeField] int weaponLevel;
        [SerializeField] bool isPlayer;
        [SerializeField] bool isShipAI = false;
        [SerializeField] bool disabled = false;
        [SerializeField] int parts = 0;

        bool refuling;

        private void Start()
        {
            health = defaultMaxHealth;
            fuel = maxFuel;
        }

        private void Update()
        {
            UsingFuel();

            if ((isShipAI == false || isPlayer == false) && health <= 0)
            {
                disabled = true;
            }
            else if (health > 0)
            {
                disabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject disabledTarget = collision.gameObject;
            ShipStats targetStats = disabledTarget.GetComponent<ShipStats>();
            if (targetStats && targetStats.GetDisabled() == true)
            {
                this.parts += 1;
                Destroy(disabledTarget);
            }
        }

        private void UsingFuel()
        {
            if (fuel > 0)
            {
                fuel -= fuelLossRate * Time.deltaTime;
            }
            else
            {
                //DEAKTIVER MOTOR
                print("OUT OF FUEL");
            }
        }

        public void Refuling(float refuelRate)
        {
            if (fuel < maxFuel)
            {
                refuling = true;
                fuel += refuelRate * Time.deltaTime;
            }
            else
            {
                refuling = false;
            }
        }

        public bool RefulingCheck()
        {
            return refuling;
        }

        #region Getters
        public float GetHealth()
        {
            return health;
        }

        public void LoseHealth(int damage)
        {
            if (health > 0)
            {
                health -= damage;
            }
            else
            {
                print("ALREADY DEAD");
            }
        }

        public float GetLevel()
        {
            return level;
        }

        public bool IsPlayer()
        {
            return isPlayer;
        }

        public float GetFuel()
        {
            return fuel;
        }

        public bool GetDisabled()
        {
            return disabled;
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Statistics
{
    public class ShipStats : MonoBehaviour
    {
        [Header("Health Attributes")]
        [SerializeField] float defaultMaxHealth = 100;
        [SerializeField] float health;

        [Header("Fuel Attributes")]
        [SerializeField] float maxFuel = 100;
        [SerializeField] float fuel;
        [SerializeField] float fuelLossRate = 0.5f;

        [Header("Other Statistics")]
        [SerializeField] int level;
        [SerializeField] int weaponLevel;
        [SerializeField] bool isPlayer;

        bool refuling;

        private void Start()
        {
            health = defaultMaxHealth;
            fuel = maxFuel;
        }

        private void Update()
        {
            UsingFuel();
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

        public float GetHealth()
        {
            return health;
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
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Statistics;

namespace ShipGame.Station
{
    public class ServiceStation : MonoBehaviour
    {
        [SerializeField] float refuelRate = 5;

        public static List<Transform> serviceStations = new List<Transform>();

        const int PLAYERLAYER = 9;
        const int SHIP = 11;

        private void Start()
        {
            serviceStations.Add(this.transform);
        }

        private void OnTriggerStay(Collider other)
        {
            GameObject customer = other.gameObject;
            if (customer.layer == PLAYERLAYER || customer.layer == SHIP)
            {
                customer.GetComponentInParent<ShipStats>().Refuling(refuelRate);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShipGame.Ship.Statistics;

namespace ShipGame.Ship.UI
{
    public class UIHandler : MonoBehaviour
    {
        [SerializeField] Text fuelText = null;

        ShipStats shipStats = null;

        private void Start()
        {
            shipStats = GetComponent<ShipStats>();
        }

        private void Update()
        {
            int fuelLeft = (int) shipStats.GetFuel();
            fuelText.text = "Fuel: " + fuelLeft + " Litre" + " / 100 Litre";
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Motor;
using ShipGame.Ship.Statistics;
using ShipGame.Ship.Weapons;
using System;

namespace ShipGame.Ship.Computer
{
    public class MovementCalculator : MonoBehaviour
    {
        //BRUKER INFORMASJON I FRA PID-ENE OG REKNER UT FART
        [Header("Motor Variables")]
        [SerializeField] float maxRotationSpeed = 20;
        [SerializeField] float maxThrusterSpeed = 10;
        [SerializeField] float rotationSpeed;
        [SerializeField] float thrusterSpeed;

        ShipStats myShipStats = null;
        RotationPid rotationPid = null;
        ThrusterPid thrusterPid = null;
        MovementMotor movementMotor = null;
        DecisionMaker decisionMaker = null;

        Transform closestStation = null;

        const int PLAYERLAYER = 9;
        const int ENEMY = 10;
        const int SERVICESTATION = 12;
        const int PATROLPOINT = 13;

        float fuel;

        int fuelingRange;
        int fireRange;

        bool refuling = false;
        bool inRange = false;
        bool gotTarget = false;


        private void Start()
        {
            myShipStats = GetComponent<ShipStats>();
            rotationPid = GetComponent<RotationPid>();
            thrusterPid = GetComponent<ThrusterPid>();
            movementMotor = GetComponent<MovementMotor>();
            decisionMaker = GetComponent<DecisionMaker>();

            fuelingRange = decisionMaker.GetFuelingRange();
            fireRange = decisionMaker.GetFireRange();
        }

        private void Update()
        {
            GameObject target = decisionMaker.GetTarget();
            if (target == null)
            {
                gotTarget = false;
                return;
            }

            bool isDisabled = false;
            if (target.layer == ENEMY || target.layer == PLAYERLAYER)
            {
                isDisabled = target.GetComponent<ShipStats>().GetDisabled();
            }

            if (isDisabled)
            {
                gotTarget = true;
                return;
            }
            else
            {
                InRangeToAttackCheck();
            }
            
            RefulingCheck();

            if (inRange || refuling)
            {
                gotTarget = false;
                return;
            }

            gotTarget = true;


        }

        private void FixedUpdate()
        {
            //Sjekker om me har target tilgjengelig, skal fungere for både rotationMotor og ThrusterMotor
            if (gotTarget == false)
            {
                return;
            }

            thrusterPid.PidCalculation();
            CalculateThrusterSpeed();

            rotationPid.PidCalculation();
            CalculateRotationSpeed();

            movementMotor.UseThrusterMotor(thrusterSpeed);
            movementMotor.UseRotationMotor(rotationSpeed);
        }

        private void IsDisabledCheck()
        {
            
        }

        #region Abort Movement Checkers
        private void RefulingCheck()
        {
            fuel = myShipStats.GetFuel();
            closestStation = decisionMaker.GetClosestStation();
            if ((closestStation.transform.position - this.transform.position).magnitude < fuelingRange && fuel < 85)
            {
                refuling = true;
            }
            else
            {
                refuling = false;
            }
        }

        private void InRangeToAttackCheck()
        {
            GameObject chosenTarget = decisionMaker.GetTarget();
            if ((chosenTarget.transform.position - this.transform.position).magnitude < fireRange && chosenTarget.gameObject.layer != SERVICESTATION && chosenTarget.gameObject.layer != PATROLPOINT)
            {
                inRange = true;
            }
            else
            {
                inRange = false;
            }

            if (inRange)
            {
                return;
            }
        }
        #endregion

        private void CalculateThrusterSpeed()
        {
            float output = thrusterPid.GetPercentageOutput();
            if (output == 0)
            {
                thrusterSpeed = 0;
            }
            else
            {
                float percentageAllowed = RestrictMaxThrust();
                thrusterSpeed = maxThrusterSpeed * output * percentageAllowed;
            }
        }

        private float RestrictMaxThrust()
        {
            Vector3 directionToTarget = rotationPid.DirectionToTarget();
            float degreesToTarget = Vector3.Angle(this.transform.forward, directionToTarget);
            //Gjør at skipet ikkje får gi full gass når det skal snu masse
            //0.00555f er det samme som å dele på 180
            float percentageAllowed = (180 - degreesToTarget) * 0.00555f;
            return percentageAllowed;
        }

        private void CalculateRotationSpeed()
        {
            float output = rotationPid.GetPercentageOutput();
            if (output == 0)
            {
                rotationSpeed = 0;
            }
            else
            {
                rotationSpeed = maxRotationSpeed * output;
            }
        }
    }
}

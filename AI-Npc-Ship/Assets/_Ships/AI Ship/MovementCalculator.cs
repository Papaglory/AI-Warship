using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Motor;

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

        RotationPid rotationPid = null;
        ThrusterPid thrusterPid = null;
        MovementMotor movementMotor = null;
        DecisionMaker decisionMaker = null;     

        private void Start()
        {
            rotationPid = GetComponent<RotationPid>();
            thrusterPid = GetComponent<ThrusterPid>();
            movementMotor = GetComponent<MovementMotor>();
            decisionMaker = GetComponent<DecisionMaker>();
        }

        private void FixedUpdate()
        {
            //Sjekker om me har target tilgjengelig, skal fungere for både rotationMotor og ThrusterMotor
            if (decisionMaker.GetTarget() == null)
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

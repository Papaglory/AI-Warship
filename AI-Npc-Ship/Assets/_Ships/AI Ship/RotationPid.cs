using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Computer
{
    [RequireComponent(typeof(DecisionMaker))]
    public class RotationPid : MonoBehaviour
    {
        [Header("PID-Constants")]
        [SerializeField] float kp;
        [SerializeField] float kd;
        [SerializeField] float ki; //INTEGRAL VERDI FRA RAKETT: 0.0679259

        [SerializeField] float rotationToActivateIntegral = 20;

        DecisionMaker decisionMaker = null;

        //BRUK THIS.TRANSFORM.FORWARD I RIGIDBODY OG IKKJE I EIN MEMBER VARIABLE
        bool targetAvailable = false;
        float errorInDegrees;
        float previousErrorInDegrees;
        float integralError;
        float errorSlope;
        float output;
        float percentageOutput;
        int min = -1;
        int max = 1;

        private void Start()
        {
            decisionMaker = GetComponent<DecisionMaker>();
        }

        public void PidCalculation()
        {
            CalculateErrorInDegrees();
            CalculateChangeInError();

            ResetIntegralCheck();
            integralError += errorInDegrees;

            output = (errorInDegrees * kp) + (errorSlope * kd) + (integralError * ki);
            previousErrorInDegrees = errorInDegrees;
        }

        private void CalculateErrorInDegrees()
        {
            Vector3 directionToTarget = DirectionToTarget();
            errorInDegrees = Vector3.Angle(this.transform.forward, directionToTarget);

            Vector3 crossVector = Vector3.Cross(directionToTarget, this.transform.forward);
            if (crossVector.y > 0)
            {
                errorInDegrees = -errorInDegrees;
            }
        }

        public Vector3 DirectionToTarget()
        {
            Vector3 targetPosition = decisionMaker.GetTarget().transform.position;
            Vector3 directionToTarget = (targetPosition - this.transform.position).normalized;
            return directionToTarget;
        }

        private void CalculateChangeInError()
        {
            float changeInError = errorInDegrees - previousErrorInDegrees;
            errorSlope = (changeInError) / Time.deltaTime;
        }

        private void ResetIntegralCheck()
        {
            if (Mathf.Abs(errorInDegrees) > rotationToActivateIntegral)
            {
                integralError = 0;
            }

            //NB! ENDRING 03.07.2018
            if (this.transform.forward == DirectionToTarget())
            {
                integralError = 0;
            }
        }

        public float GetPercentageOutput()
        {
            percentageOutput = Mathf.Clamp(output, min, max);
            percentageOutput = percentageOutput / max;
            return percentageOutput;
        }

        public float GetOutput()
        {
            return output;
        }

        public bool GetTargetAvailable()
        {
            return targetAvailable;
        }
    }
}

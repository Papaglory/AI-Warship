using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Computer
{
    public class ThrusterPid : MonoBehaviour
    {
        //THRUST BLIR EIN PROSENT AV THRUSTERPIDEN UT I FRA VINKELEN DEN HAR TIL MÅLET
        [Header("PID-Constants")]
        [SerializeField] float kp;
        [SerializeField] float kd;
        [SerializeField] float ki;

        [SerializeField] float distanceToActivateIntegral = 20;

        DecisionMaker decisionMaker = null;

        //BRUK THIS.TRANSFORM.FORWARD I RIGIDBODY OG IKKJE I EIN MEMBER VARIABLE
        float error;
        float previousError;
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
            CalculateError();
            CalculateChangeInError();

            ResetIntegralCheck();
            integralError += error;

            output = (error * kp) + (errorSlope * kd) + (integralError * ki);
            previousError = error;
        }

        private void CalculateError()
        {
            Vector3 targetPosition = decisionMaker.GetTarget().transform.position;
            Vector3 distanceToTarget = targetPosition - this.transform.position;
            error = (distanceToTarget).magnitude;
        }

        private void CalculateChangeInError()
        {
            float changeInError = error - previousError;
            errorSlope = (changeInError) / Time.fixedDeltaTime;
        }

        private void ResetIntegralCheck()
        {
            if (Mathf.Abs(error) > distanceToActivateIntegral)
            {
                integralError = 0;
            }

            if (integralError == error)
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
    }
}

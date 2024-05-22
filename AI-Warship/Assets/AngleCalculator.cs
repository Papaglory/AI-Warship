using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Weapons
{
    public class AngleCalculator : MonoBehaviour
    {
        const float GRAVITY = 9.81f;
        float heightDifference;
        float positiveAngleDegree;
        float negativeAngleDegree;

        public float MaxFireDistance(float _velocity)
        {
            float maxRange = (_velocity * _velocity) / GRAVITY;
            return maxRange;
        }

        public float CalculateFireAngle(bool _smallFireAngle, Transform _target, float _velocity, Transform _turret)
        {
            //Distanse i XZ-plan
            Vector3 distanceVectorXZ = _target.transform.position - _turret.transform.position;
            distanceVectorXZ.y = 0;
            float distanceXZ = distanceVectorXZ.magnitude;

            float distanceY = (_target.transform.position.y - _turret.transform.position.y);
            //distanceY = FlipSignCheck(distanceY);

            //ved +/- i formelen velger me her å plusse -b/2a +/- sqrt(b^2 - 4*ac)/2a
            float velocityPowerFour = _velocity * _velocity * _velocity * _velocity;
            float fourTimesAC = GRAVITY * ((GRAVITY * distanceXZ * distanceXZ) + (2 * _velocity * _velocity * distanceY));

            float returnAngle = 0;
            if (_smallFireAngle)
            {
                float positiveAngleRad = Mathf.Atan2((_velocity * _velocity) + Mathf.Sqrt(velocityPowerFour - fourTimesAC), GRAVITY * distanceXZ);
                positiveAngleDegree = (180 / Mathf.PI) * positiveAngleRad;
                positiveAngleDegree = 90 - positiveAngleDegree;
                returnAngle = positiveAngleDegree;
                //print("Positive Angle Degrees " + returnAngle);
            }
            else
            {
                float negativeAngleRad = Mathf.Atan2((_velocity * _velocity) - Mathf.Sqrt(velocityPowerFour - fourTimesAC), GRAVITY * distanceXZ);
                negativeAngleDegree = (180 / Mathf.PI) * negativeAngleRad;
                negativeAngleDegree = 90 - negativeAngleDegree;
                returnAngle = negativeAngleDegree;
                //print("Negative angle Degrees " + returnAngle);
            }
            return returnAngle;
        }

        public void CalculateTimeUntilImapct(Transform _target, float _velocity)
        {
            float angle = this.transform.rotation.eulerAngles.x;
            angle = (angle > 180) ? angle - 360 : angle;
            heightDifference = _target.transform.position.y - this.transform.position.y;
            heightDifference = FlipSignCheck(heightDifference);

            Vector3 toTargetXZ = _target.transform.position - this.transform.position;
            toTargetXZ.y = 0;
            float distanceXZ = toTargetXZ.magnitude;

            float angleDegrees = (Mathf.PI / 180) * angle;

            angleDegrees = FlipSignCheck(angleDegrees);
            if (angleDegrees > 90)
            {
                print("AngleDegrees: " + angleDegrees);
            }

            float timeUntilImpact = ((_velocity * Mathf.Sin(angleDegrees)) / (GRAVITY)) + (Mathf.Sqrt(_velocity * _velocity * Mathf.Sin(angleDegrees) + (2 * GRAVITY * heightDifference)) / (GRAVITY));
            if (float.IsNaN(timeUntilImpact))
            {
                print("TimeUntilImpact is NaN");
            }
            else
            {
                print("TimeUntilImpact: " + timeUntilImpact);
            }
        }

        //private void PrintVelocityComponentsXY()
        //{
        //    float angle = this.transform.eulerAngles.x;
        //    float degreeInRad = (Mathf.PI / 180) * angle;
        //    float velocityX = velocity * Mathf.Cos(degreeInRad);
        //    float velocityY = velocity * Mathf.Sin(degreeInRad);
        //    print("vX " + velocityX + "vY " + velocityY);
        //}

        private static float FlipSignCheck(float _numberToFlip)
        {
            if (_numberToFlip < 0)
            {
                _numberToFlip *= -1;
            }
            return _numberToFlip;
        }
    }
}

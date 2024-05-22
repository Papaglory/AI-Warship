using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Statistics;

namespace ShipGame.Ship.Computer
{
    public class ScoreGiver : MonoBehaviour {

        [Header("AI Brain Attributes")]

        [Range(0, 100)]
        [Tooltip("How important it is to gather resources instead hunting the player")]
        [SerializeField] float resourceGatherFactor;

        [Range(0, 100)]
        [Tooltip("How important it is to hunt for the player")]
        [SerializeField] float huntingPlayerFactor;

        [Range(0, 100)]
        [Tooltip("How important it is to compare target value with this.ship value")]
        [SerializeField] float survivalInstinctFactor;

        [Range(0, 100)]
        [Tooltip("How important it is to hunt closest target")]
        [SerializeField] float huntingClosestFactor;

        [Range(0, 100)]
        [Tooltip("How important it is to have fuel remaining")]
        [SerializeField] float fuelFactor;
        
        /// <summary>
        /// /////////////////////////////////////////////////////
        /// </summary>

        [Header("Graph Design: TARGET")]

        [Tooltip("Is the slope of the graph increasing or decreasing")]
        [SerializeField] bool target_positiveSlopeSign = false;

        [Tooltip("Is the slope changing quickly or slowly")]
        [SerializeField] bool target_aggressiveSlope = false;

        [Tooltip("Increases drastically in the end")]
        [SerializeField] bool target_eulerSlope = false;

        [Range(10, 25)]
        [Tooltip("Lower number for reaching max score faster, 19.6 gives a score of 90")]
        [SerializeField] float target_eulerStrength;

        [Tooltip("A slope that almost reaches 100 or 0")]
        [SerializeField] bool target_reachingLimitSlope = false;

        [Range(100, 2500)]
        [Tooltip("Higher number gives smoother LimitSlope, 2500 gives score of 100 to 80")]
        [SerializeField] int target_limitSlopeStrength;

        [Tooltip("A linear slope")]
        [SerializeField] bool target_linearSlope = false;

        /// <summary>
        /// ///////////////////////////////////////////////////////
        /// </summary>

        [Header("Graph Design: FUEL")]
        [Tooltip("Is the slope of the graph increasing or decreasing")]
        [SerializeField] bool fuel_positiveSlopeSign = false;

        [Tooltip("Is the slope changing quickly or slowly")]
        [SerializeField] bool fuel_aggressiveSlope = false;

        [Tooltip("Increases drastically in the end")]
        [SerializeField] bool fuel_eulerSlope = false;

        [Range(10, 25)]
        [Tooltip("Lower number for reaching max score faster, 19.6 gives a score of 90")]
        [SerializeField] float fuel_eulerStrength;

        [Tooltip("A slope that almost reaches 100 or 0")]
        [SerializeField] bool fuel_reachingLimitSlope = false;

        [Range(100, 2500)]
        [Tooltip("Higher number gives smoother LimitSlope, 2500 gives score of 100 to 80")]
        [SerializeField] int fuel_limitSlopeStrength;

        [Tooltip("A linear slope")]
        [SerializeField] bool fuel_linearSlope = false;


        Transform chosenTarget = null;

        GraphTemplate targetGraph = null;
        GraphTemplate fuelingGraph = null;

        const int PLAYERLAYER = 9;
        private float highestScoreNormalized;

        private void Start()
        {
            //Lager to eller tre graph templates for scoring, denne er den samme igjennom heile skipet sitt liv
            targetGraph = new GraphTemplate(target_positiveSlopeSign, target_aggressiveSlope, target_eulerSlope, target_eulerStrength, target_reachingLimitSlope, target_limitSlopeStrength);
            fuelingGraph = new GraphTemplate(fuel_positiveSlopeSign, fuel_aggressiveSlope, fuel_eulerSlope, fuel_eulerStrength, fuel_reachingLimitSlope, fuel_limitSlopeStrength);
        }

        public Transform ScoreEachTarget(List<Collider> targets, ShipStats myShipStats)
        {
            highestScoreNormalized = 0;
            chosenTarget = null;
            for (int i = 0; i < targets.Count; i++)
            {
                Transform potensialTarget = targets[i].transform;
                float distanceAway = (potensialTarget.position - this.transform.position).magnitude;
                Mathf.Clamp(distanceAway, 1, Mathf.Infinity);

                ShipStats targetShipStats = potensialTarget.GetComponent<ShipStats>();
                float targetHealth = targetShipStats.GetHealth();
                float targetLevel = targetShipStats.GetLevel();

                int targetIsPlayer;
                if (potensialTarget.gameObject.layer == PLAYERLAYER)
                {
                    targetIsPlayer = 1;
                }
                else
                {
                    targetIsPlayer = 0;
                }

                float levelAdvantage = myShipStats.GetLevel() - targetShipStats.GetLevel();
                //Gi ein score ut i fra det som er over               
                //float targetScore = (1 + (huntingClosestFactor / distanceAway)) + (huntingPlayerFactor * targetIsPlayer) + (resourceGatherFactor * targetIsPlayer) + (survivalInstinctFactor * levelAdvantage);
                float targetScore = 1 + resourceGatherFactor;
                Mathf.Clamp(targetScore, 0, 100);
                //Dersom score er linær så vil den ikkje endre seg i SlopeCalculation
                if (target_linearSlope == false)
                {
                    targetScore = targetGraph.SlopeCalculation(targetScore);
                }

                print(targets[i].name + " " + targetScore);
                if (highestScoreNormalized < targetScore)
                {
                    highestScoreNormalized = targetScore;
                    chosenTarget = targets[i].transform;
                }
            }
            return chosenTarget;
        }

        public Transform Tester(List<Collider> targets, ShipStats myShipStats)
        {
            return null;
        }

        public Transform ScoreClosestStation(Transform _closestStation, float fuel)
        {
            //REKN UT FUELSCORE HER
            if (_closestStation == null)
            {
                return chosenTarget;
            }
            Mathf.Clamp(fuel, 0, 100);
            float distance = (this.transform.position - _closestStation.transform.position).magnitude;
            float fuelScore = 0;
            float fuelMissing = 100 - fuel;
            fuelScore = fuelMissing;
            Mathf.Clamp(fuelScore, 0, 100);
            //Dersom score er linær så vil den ikkje endre seg i SlopeCalculation
            if (fuel_linearSlope == false)
            {
                fuelScore = fuelingGraph.SlopeCalculation(fuelScore);
            }

            print(_closestStation.name + " " + fuelScore);
            if (highestScoreNormalized < fuelScore)
            {
                //score systemet kan bare gi positive poeng. Dersom fuelScore blir større enn targetScore så vil me få target -1 som er ein konstant for fuelStation
                //highestScoreTarget = -1;
                chosenTarget = _closestStation;
            }
            return chosenTarget;
        }
    }
}

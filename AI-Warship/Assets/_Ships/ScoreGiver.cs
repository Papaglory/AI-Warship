using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Statistics;

namespace ShipGame.Ship.Computer
{
    public class ScoreGiver : MonoBehaviour {

        //Recommended total factors combined = 100, most aggressive is 100, rest under
        [Header("AI Brain Attributes")]

        [Range(-100, 100)]
        [Tooltip("How important it is to gather resources instead hunting the player")]
        [SerializeField] int resourceGatherFactor;

        [Range(-100, 100)]
        [Tooltip("How important it is to hunt for the player")]
        [SerializeField] int huntingPlayerFactor;

        [Range(-100, 100)]
        [Tooltip("How important it is to compare target value with this.ship value")]
        [SerializeField] int survivalInstinctFactor;

        [Range(-100, 100)]
        [Tooltip("How important it is to patrol, NORMAL: 0")]
        [SerializeField] int patrolFactor;

        [Range(-100, 100)]
        [Tooltip("How important it is to have fuel remaining")]
        [SerializeField] int fuelFactor;
        
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
        [Tooltip("Lower number for reaching max score faster, RECOMMENDED: 17.2")]
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
        [Tooltip("Lower number for reaching max score faster, RECOMMENDED: 17.2")]
        [SerializeField] float fuel_eulerStrength;

        [Tooltip("A slope that almost reaches 100 or 0")]
        [SerializeField] bool fuel_reachingLimitSlope = false;

        [Range(100, 2500)]
        [Tooltip("Higher number gives smoother LimitSlope, 2500 gives score of 100 to 80")]
        [SerializeField] int fuel_limitSlopeStrength;

        [Tooltip("A linear slope")]
        [SerializeField] bool fuel_linearSlope = false;

        /// <summary>
        /// ////////////////////////////////////////////////////////
        /// </summary>

        [Header("Graph Design: PATROL")]

        [Tooltip("Is the slope of the graph increasing or decreasing")]
        [SerializeField] bool patrol_positiveSlopeSign = false;

        [Tooltip("Is the slope changing quickly or slowly")]
        [SerializeField] bool patrol_aggressiveSlope = false;

        [Tooltip("Increases drastically in the end")]
        [SerializeField] bool patrol_eulerSlope = false;

        [Range(10, 25)]
        [Tooltip("Lower number for reaching max score faster, RECOMMENDED: 17.2")]
        [SerializeField] float patrol_eulerStrength;

        [Tooltip("A slope that almost reaches 100 or 0")]
        [SerializeField] bool patrol_reachingLimitSlope = false;

        [Range(100, 2500)]
        [Tooltip("Higher number gives smoother LimitSlope, 2500 gives score of 100 to 80")]
        [SerializeField] int patrol_limitSlopeStrength;

        [Tooltip("A linear slope")]
        [SerializeField] bool patrol_linearSlope = false;

        Transform chosenTarget = null;
        List<Transform> targetTransformOrganized = new List<Transform>();

        GraphTemplate targetGraph = null;
        GraphTemplate fuelingGraph = null;
        GraphTemplate patrolGraph = null;

        const int PLAYERLAYER = 9;
        float highestScore;
        int basicHighestScore;

        private void Start()
        {
            //Lager to eller tre graph templates for scoring, denne er den samme igjennom heile skipet sitt liv
            targetGraph = new GraphTemplate(target_positiveSlopeSign, target_aggressiveSlope, target_eulerSlope, target_eulerStrength, target_reachingLimitSlope, target_limitSlopeStrength);
            fuelingGraph = new GraphTemplate(fuel_positiveSlopeSign, fuel_aggressiveSlope, fuel_eulerSlope, fuel_eulerStrength, fuel_reachingLimitSlope, fuel_limitSlopeStrength);
            patrolGraph = new GraphTemplate(patrol_positiveSlopeSign, patrol_aggressiveSlope, patrol_eulerSlope, patrol_eulerStrength, patrol_reachingLimitSlope, patrol_limitSlopeStrength);
        }

        #region FiniteStateMachine
        public Transform FiniteStateMachineScoreEachTarget(List<Collider> targets, ShipStats myShipStats)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Transform potensialTarget;
                ShipStats targetShipStats;
                float distanceAway;
                GeneralTargetInformation(targets, i, out potensialTarget, out targetShipStats, out distanceAway);

                int targetIsPlayer = IsTargetPlayerCheck(potensialTarget);

                float levelAdvantage = myShipStats.GetLevel() - targetShipStats.GetLevel();
                //Gi ein score ut i fra det som er over               
                //float targetScore = (1 + (huntingClosestFactor / distanceAway)) + (huntingPlayerFactor * targetIsPlayer) + (resourceGatherFactor * targetIsPlayer) + (survivalInstinctFactor * levelAdvantage);
                int targetScore = 1 + resourceGatherFactor;
                Mathf.Clamp(targetScore, 0, 100);
                //Dersom score er linær så vil den ikkje endre seg i SlopeCalculation
                if (target_linearSlope == false)
                {
                    targetScore = targetGraph.SlopeCalculation(targetScore);
                }

                print(targets[i].name + " " + targetScore);
                if (highestScore < targetScore)
                {
                    highestScore = targetScore;
                    chosenTarget = targets[i].transform;
                }
            }
            return chosenTarget;
        }
        #endregion

        #region Basic Scores
        public List<int> BasicScoreEachTarget(List<Collider> targets, ShipStats myShipStats)
        {
            List<int> targetScoresList = new List<int>();
            for (int i = 0; i < targets.Count; i++)
            {
                Transform potensialTarget;                
                ShipStats targetShipStats;
                float distanceAway;
                GeneralTargetInformation(targets, i, out potensialTarget, out targetShipStats, out distanceAway);
                if (potensialTarget == null)
                {
                    break;
                }

                int targetIsPlayer = IsTargetPlayerCheck(potensialTarget);

                float levelAdvantage = myShipStats.GetLevel() - targetShipStats.GetLevel();

                //EQUATION HERE
                //TARGETSCORE = 100 GIR AT REFUEL BLIR VED 20 FUEL LEFT NÅR FUEL FACTOR = 0
                int targetScore = (1 + resourceGatherFactor) + (huntingPlayerFactor * targetIsPlayer);

                CheckIfGreaterThanBasicHighestScore(targetScore);

                targetScoresList.Add(targetScore);
                targetTransformOrganized.Add(potensialTarget);
            }

            return targetScoresList;
        }

        public int ScoreFuel(Transform _closestStation, float _fuel)
        {
            float distance = (this.transform.position - _closestStation.transform.position).magnitude;
            int fuelMissing = 100 - Mathf.RoundToInt(_fuel);

            int fuelScore;
            //EQUATION HERE
            fuelScore = fuelMissing + fuelFactor;

            CheckIfGreaterThanBasicHighestScore(fuelScore);

            return fuelScore;
        }

        public int ScorePatrol()
        {
            int patrolScore;
            patrolScore = patrolFactor;
            CheckIfGreaterThanBasicHighestScore(patrolScore);
            return patrolScore;
        }

        public int GetBasicHighestScore()
        {
            return basicHighestScore;
        }

        private void CheckIfGreaterThanBasicHighestScore(int _score)
        {
            if (basicHighestScore < _score)
            {
                basicHighestScore = _score;
            }
        }
        #endregion

        #region GraphCalculator
        public List<int> ScoreTargetGraphCalculator(List<int> _scoreList)
        {
            if (fuel_linearSlope == false)
            {
                for (int i = 0; i < _scoreList.Count; i++)
                {
                    targetGraph.SlopeCalculation(_scoreList[i]);
                }
            }
            return _scoreList;
        }

        public int ScoreFuelGraphCalculator(int _score)
        {
            if (fuel_linearSlope == false)
            {
                _score = fuelingGraph.SlopeCalculation(_score);
            }
            return _score;
        }

        public int ScorePatrolGraphCalculator(int _score)
        {
            if (patrol_linearSlope == false)
            {
                _score = patrolGraph.SlopeCalculation(_score);
            }
            return _score;
        }

        public void ResetVariablesForNextIteration()
        {
            highestScore = 0;
            basicHighestScore = 1;
            targetTransformOrganized.Clear();
            chosenTarget = null;
        }
        #endregion

        private void GeneralTargetInformation(List<Collider> targets, int i, out Transform potensialTarget, out ShipStats targetShipStats, out float distanceAway)
        {
            if (targets[i] == null)
            {
                distanceAway = Mathf.Infinity;
                targetShipStats = null;
                potensialTarget = null;
                return;
            }
            potensialTarget = targets[i].transform;
            distanceAway = (potensialTarget.position - this.transform.position).magnitude;
            Mathf.Clamp(distanceAway, 1, Mathf.Infinity);

            targetShipStats = potensialTarget.GetComponent<ShipStats>();
            float targetHealth = targetShipStats.GetHealth();
            float targetLevel = targetShipStats.GetLevel();
        }

        private static int IsTargetPlayerCheck(Transform potensialTarget)
        {
            int targetIsPlayer;
            if (potensialTarget.gameObject.layer == PLAYERLAYER)
            {
                targetIsPlayer = 1;
            }
            else
            {
                targetIsPlayer = 0;
            }

            return targetIsPlayer;
        }

        public List<Transform> GetTargetTransformOrganized()
        {
            return targetTransformOrganized;
        }
    }
}
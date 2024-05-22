using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.InformationGatherer;
using ShipGame.Ship.Statistics;
using ShipGame.Station;
using System;

namespace ShipGame.Ship.Computer
{
    [RequireComponent(typeof(TargetFinder))]
    public class DecisionMaker : MonoBehaviour
    {
        //LESER AV TARGET[] I FRA CurrentTarget OG TAR DET MED I UTREKNING PÅ KVA EIN SKAL GJØRE: FYLLE BENSIN, KVA TARGET Å KJØRE TIL UT I FRA HP, LOOT, DANGER
        //CURRENTTARGET MÅ ALTID VERE NOKE
        [Header("Using FiniteStateMachine or UtilityAI for fuel")]
        [SerializeField] bool usingStateMachine = true;
        [SerializeField] int idleMark = 60;
        [SerializeField] int chasingNPCMark = 40;
        [SerializeField] int chasingPlayerMark = 20;

        [SerializeField] GameObject currentTarget = null;

        [Header("Range Options")]
        [SerializeField] int fireRange = 100;
        [SerializeField] int fuelingRange = 20;

        ShipStats myShipStats = null;
        TargetFinder targetFinder = null;
        ScoreGiver scoreGiver = null;
        Patrol patrol = null;

        List<Collider> targets;
        Transform chosenTarget = null;
        Transform closestStation = null;

        const int PLAYERLAYER = 9;
        const int ENEMY = 10;
        const int SERVICESTATION = 12;
        const int PATROLPOINT = 13;

        public bool inRange;
        bool refuling;
        bool patroling;
        float fuel;
        int fuelScore;
        int patrolScore;

        private void Start()
        {
            myShipStats = GetComponent<ShipStats>();
            targetFinder = GetComponent<TargetFinder>();
            scoreGiver = GetComponent<ScoreGiver>();
            patrol = GetComponent<Patrol>();
        }

        private void Update()
        {
            ResetVariablesForNextIteration();
            scoreGiver.ResetVariablesForNextIteration();

            closestStation = FindClosestStation();
            UseChosenAIDesign();

            SetCurrentTarget(chosenTarget);
        }

        #region UtilityAI
        private void UtilityAI()
        {

            ScoreMinorTargets(closestStation);

            List<int> targetScores;
            List<Transform> targetTransformsOrganized;
            TargetsScoredAndOrganized(out targetScores, out targetTransformsOrganized);

            int basicHighestScore = scoreGiver.GetBasicHighestScore();


            //TODO FJERN DENNE MULIGENS. GJØR AT FUELFACTOR ALTID MINST ER 100 SLIK AT DET BLIR RETT NORMALIZING I FORHOLD TIL FUEL
            if (basicHighestScore < 100)
            {
                basicHighestScore = 100;
            }

            NormalizeAllBasicScores(targetScores, basicHighestScore);

            //TODO FJERN DENNE ER TEST FOR GRAPHS
            patrolScore = fuelScore;

            //Score using graphCalculator();
            fuelScore = scoreGiver.ScoreFuelGraphCalculator(fuelScore);
            patrolScore = scoreGiver.ScorePatrolGraphCalculator(patrolScore);
            targetScores = scoreGiver.ScoreTargetGraphCalculator(targetScores);

            DecideOnTarget(closestStation, targetScores, targetTransformsOrganized);
        }

        private void TargetsScoredAndOrganized(out List<int> targetScores, out List<Transform> targetTransformsOrganized)
        {
            targetScores = scoreGiver.BasicScoreEachTarget(targets, myShipStats);
            targetTransformsOrganized = scoreGiver.GetTargetTransformOrganized();
        }

        private void DecideOnTarget(Transform closestStation, List<int> targetScores, List<Transform> targetTransformsOrganized)
        {
            float finalHighestTargetScore = 0;
            int loopPosition = 0;

            if (targetTransformsOrganized.Count != 0)
            {
                for (int k = 0; k < targetScores.Count; k++)
                {
                    if (finalHighestTargetScore < targetScores[k])
                    {
                        loopPosition = k;
                        finalHighestTargetScore = targetScores[k];
                    }
                }
            }

            if (finalHighestTargetScore < fuelScore)
            {
                //FUEL
                chosenTarget = closestStation;
                finalHighestTargetScore = fuelScore;
            }

            if (finalHighestTargetScore < patrolScore)
            {
                patrol.HandlePatrol(out chosenTarget);
                finalHighestTargetScore = patrolScore;
            }

            if (targetTransformsOrganized.Count == 0)
            {
                return;
            }

            if (finalHighestTargetScore == targetScores[loopPosition])
            {
                //GO FOR TARGET
                chosenTarget = targetTransformsOrganized[loopPosition];

            }
                              
        }

        private void ScoreMinorTargets(Transform closestStation)
        {
            fuelScore = scoreGiver.ScoreFuel(closestStation, fuel);
            patrolScore = scoreGiver.ScorePatrol();
        }

        private void NormalizeAllBasicScores(List<int> targetScores, int basicHighestScore)
        {
            fuelScore = (100 * fuelScore) / basicHighestScore;
            patrolScore = (100 * patrolScore) / basicHighestScore;
            for (int i = 0; i < targetScores.Count; i++)
            {
                targetScores[i] = (100 * targetScores[i]) / basicHighestScore;

            }
        }
        #endregion

        #region FiniteStateMachine
        private void FiniteStateMachineFuel()
        {
            bool refuling = myShipStats.RefulingCheck();
            bool chasingTarget = ChasingTargetCheck();
            if (chasingTarget == true && refuling == false)
            {
                chosenTarget = scoreGiver.FiniteStateMachineScoreEachTarget(targets, myShipStats);
            }

            bool targetIsPlayer = IsTargetPlayerCheck();
            FuelLevelCheck(chasingTarget, targetIsPlayer);
        }

        private void FuelLevelCheck(bool chasingTarget, bool targetIsPlayer)
        {
            if (fuel <= idleMark && chasingTarget == false)
            {
                chosenTarget = FindClosestStation();
            }
            else if (fuel <= chasingNPCMark && chasingTarget == true && targetIsPlayer == false)
            {
                chosenTarget = FindClosestStation();
            }
            else if (fuel <= chasingPlayerMark && targetIsPlayer == true)
            {
                chosenTarget = FindClosestStation();
            }
        }

        private bool IsTargetPlayerCheck()
        {
            bool targetIsPlayer = false;
            if (chosenTarget != null)
            {
                if (chosenTarget.gameObject.layer == PLAYERLAYER)
                {
                    targetIsPlayer = true;
                }
                else
                {
                    targetIsPlayer = false;
                }
            }
            return targetIsPlayer;
        }

        private bool ChasingTargetCheck()
        {
            bool chasingTarget;
            if (targets.Count == 0)
            {
                chasingTarget = false;
            }
            else
            {
                chasingTarget = true;
            }

            return chasingTarget;
        }
        #endregion

        private void ResetVariablesForNextIteration()
        {
            fuel = myShipStats.GetFuel();
            targets = targetFinder.GetList();
            chosenTarget = null;
        }

        private Transform FindClosestStation()
        {
            Transform closestStation = null;
            float closestDistance = Mathf.Infinity;
            List<Transform> serviceStations = ServiceStation.serviceStations;
            for (int i = 0; i < serviceStations.Count; i++)
            {
                float distanceToStation = (this.transform.position - serviceStations[i].position).magnitude;
                if (distanceToStation < closestDistance)
                {
                    closestDistance = distanceToStation;
                    closestStation = serviceStations[i];
                }
            }
            return closestStation;
        }

        private void UseChosenAIDesign()
        {
            if (usingStateMachine)
            {
                FiniteStateMachineFuel();
            }
            else
            {
                UtilityAI();
            }
        }

        private void SetCurrentTarget(Transform newTarget)
        {
            if (newTarget == null)
            {
                currentTarget = null;
            }
            else
            {
                currentTarget = newTarget.gameObject;
            }
        }

        #region Getters
        public GameObject GetTarget()
        {
            return currentTarget;
        }

        public Transform GetClosestStation()
        {
            return closestStation;
        }
        
        public int GetFireRange()
        {
            return fireRange;
        }

        public int GetFuelingRange()
        {
            return fuelingRange;
        }
        #endregion
    }
}
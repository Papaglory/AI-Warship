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

        ShipStats myShipStats = null;
        TargetFinder targetFinder = null;
        ScoreGiver scoreGiver = null;

        List<Collider> targets;
        Transform chosenTarget = null;

        const int PLAYERLAYER = 9;

        float highestScore = 0;
        float fuel;        

        private void Start()
        {
            myShipStats = GetComponent<ShipStats>();
            targetFinder = GetComponent<TargetFinder>();
            scoreGiver = GetComponent<ScoreGiver>();
        }

        private void Update()
        {
            ResetVariablesForNextIteration();
            if (usingStateMachine)
            {
                //funke dårlig
                FiniteStateMachineFuel();
            }
            else
            {
                UtilityAIFuel();
            }

            SetCurrentTarget(chosenTarget);
        }

        private void UtilityAIFuel()
        {            
            chosenTarget  = scoreGiver.ScoreEachTarget(targets, myShipStats);
            Transform closestStation = FindClosestStation();
            chosenTarget = scoreGiver.ScoreClosestStation(closestStation, fuel);
        }

        private void FiniteStateMachineFuel()
        {
            bool refuling = myShipStats.RefulingCheck();
            bool chasingTarget = ChasingTargetCheck();
            if (chasingTarget == true && refuling == false)
            {
                chosenTarget = scoreGiver.ScoreEachTarget(targets, myShipStats);
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

        private void ResetVariablesForNextIteration()
        {
            fuel = myShipStats.GetFuel();
            targets = targetFinder.GetList();
            chosenTarget = null;
            highestScore = 0;
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

        public GameObject GetTarget()
        {
            return currentTarget;
        }
    }
}
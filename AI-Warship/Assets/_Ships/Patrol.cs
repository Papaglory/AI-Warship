using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Computer
{
    public class Patrol : MonoBehaviour
    {
        [Header("Patrol Options")]
        [SerializeField] int distanceBetweenPoints = 100;
        [Range(0, 5)]
        [Tooltip("How much previous Direction affects new direcction. Higher number makes new direction less random")]
        [SerializeField] float followDirectionFactor = 1;
        [SerializeField] float newPointTriggerRange = 300;

        [Header("Patrol World Restrictions")]
        [SerializeField] int xRestriction;
        [SerializeField] int zRestriction;

        GameObject myPatrolPoint = null;
        Vector3 previousPatrolPointDirection = Vector3.zero;
        Vector3 sameDirectionWeight;

        const int PATROLPOINT = 13;


        float usingFollowDirectionFactor;
        bool patroling = false;

        private void Start()
        {
            usingFollowDirectionFactor = followDirectionFactor;
            CreatePatrolObject();
        }

        private void CreatePatrolObject()
        {
            myPatrolPoint = new GameObject();
            myPatrolPoint.transform.position = Vector3.zero;
            myPatrolPoint.transform.rotation = Quaternion.identity;
            myPatrolPoint.name = this.gameObject.name + ": Patrol Point";
            myPatrolPoint.layer = PATROLPOINT;
        }

        public void HandlePatrol(out Transform chosenTarget)
        {
            if (patroling == false)
            {
                patroling = true;
                FindValidRoute();
            }

            chosenTarget = GetPatrolPoint();
            
            if ((chosenTarget.transform.position - this.transform.position).magnitude < newPointTriggerRange)
            {
                patroling = false;
            }
        }

        public void FindValidRoute()
        {
            Vector3 newPosition;
            CreateRoute(out newPosition);            

            while (newPosition.x < -xRestriction || newPosition.x > xRestriction || newPosition.z < -zRestriction || newPosition.z > zRestriction)
            {
                usingFollowDirectionFactor = 0;
                CreateRoute(out newPosition);

            }
            usingFollowDirectionFactor = followDirectionFactor;

            myPatrolPoint.transform.position = newPosition;

            previousPatrolPointDirection = (myPatrolPoint.transform.position - this.transform.position).normalized;
        }

        private void CreateRoute(out Vector3 newPosition)
        {
            sameDirectionWeight = previousPatrolPointDirection * usingFollowDirectionFactor;
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection = (randomDirection + sameDirectionWeight).normalized;
            randomDirection.y = 0;
            newPosition = this.transform.position + (randomDirection * distanceBetweenPoints);
        }

        public Transform GetPatrolPoint()
        {
            return myPatrolPoint.transform;
        }
    }
}
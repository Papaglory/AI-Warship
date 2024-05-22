using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.InformationGatherer
{
    public class TargetFinder : MonoBehaviour
    {
        //SAMLER INFORMASJON OM TARGETS OG GIR DET TIL SCRIPTS SOM TRENGER DET
        [Header("Search Attributes")]
        [SerializeField] float secondsBetweenPing = 5;
        [SerializeField] float searchRadius = 50;

        [Header("Tools")]
        [SerializeField] bool radarActivated = false;

        List<Collider> targets = new List<Collider>();
        IEnumerator currentCoroutine = null;

        const int PLAYERLAYER = 9;
        const int ENEMY = 10;
        const int SHIP = 11;

        bool radarDamaged = false;

        private void Start()
        {
            StartRadarCoroutine();
        }

        private void StartRadarCoroutine()
        {
            radarActivated = true;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = SearchForTargets();
            StartCoroutine(currentCoroutine);
        }

        IEnumerator SearchForTargets()
        {
            while(radarDamaged == false)
            {
                targets.Clear();
                Collider[] colliders = CollidersAroundShip();
                ResultIntoList(colliders);
                //PrintOutList();
                yield return new WaitForSeconds(secondsBetweenPing);    
            }
        }

        private Collider[] CollidersAroundShip()
        {
            return Physics.OverlapSphere(this.transform.position, searchRadius);
        }

        private void ResultIntoList(Collider[] colliders)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                FilterOutPlayerAndEnemy(colliders, i);
            }
        }

        private void FilterOutPlayerAndEnemy(Collider[] colliders, int i)
        {
            if (colliders[i].gameObject.layer == ENEMY || colliders[i].gameObject.layer == PLAYERLAYER)
            {
                targets.Add(colliders[i]);
            }
        }

        private void PrintOutList()
        {
            print("Length of List: " + targets.Count);
            for (int i = 0; i < targets.Count; i++)
            {
                print("ListElement: " + i + ", Name: " + targets[i].gameObject.name);
            }
        }

        public List<Collider> GetList()
        {
            return targets;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, searchRadius);
        }
    }
}

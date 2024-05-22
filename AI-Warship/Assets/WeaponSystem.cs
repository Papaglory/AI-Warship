using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Computer;
using ShipGame.Ship.Statistics;


namespace ShipGame.Ship.Weapons
{
    public class WeaponSystem : MonoBehaviour
    {
        //LAGE SLIK AT EIN KAN SETTE BULLETSPEED OG SÅ REKNER ME UT KVAR VINKEL KOR LANGT DEN VIL GÅ, VISS DET ER INNENFOR EIT OMRÅDE I FRA TARGET SÅ SKYTER ME. BRUKE IKKJE COROUTINE, KAN DERFOR DA HA EIN DELAY SKYETID
        [Header("Turrets Options")]
        [SerializeField] int turretDamage = 20;
        [SerializeField] float fireRate = 4;
        [SerializeField] int turretRange = 800;
        [SerializeField] AudioClip fireSound = null;
        [SerializeField] bool method2 = true;

        [Header("Method 1: Height and Unknown Speed Options")]
        [SerializeField] int bulletHeight = 200;
        [Tooltip("Change offset from gameobject center in y axis")]
        [SerializeField] int bulletHitOffset = 0;
        IEnumerator currentCoroutine = null;

        [Header("Method 2: Speed and Unknown Angle Options")]
        [SerializeField] float bulletVelocity = 200;
        [SerializeField] float maxBulletOffsetAngle = -0.5f;
        [SerializeField] float minBulletOffsetAngle = -2;
        [SerializeField] bool smallFireAngleBool = true;
        const float GRAVITY = 9.81f;
        float maxFireDistance;
        float heightDifference; //Bruker muligens ikkje denna her
        float fireAngle;
        float timer = 0;


        [SerializeField] GameObject[] turrets = null;
        [SerializeField] Transform[] turretEnd = null;
        [SerializeField] GameObject bulletPrefab = null;

        DecisionMaker decisionMaker = null;
        WeaponShooter weaponShooter = null;
        AngleCalculator angleCalculator = null;

        GameObject target = null;

        const int PLAYER = 9;
        const int ENEMY = 10;
        const int SERVICESTATION = 12;
        const int PATROLPOINT = 13;
        const float G = 9.81f;

        bool defaultTurretPosition = true;
        bool turretFireReady = false;

        private void Start()
        {
            decisionMaker = GetComponent<DecisionMaker>();
            weaponShooter = GetComponent<WeaponShooter>();
            angleCalculator = GetComponent<AngleCalculator>();

            maxFireDistance = angleCalculator.MaxFireDistance(bulletVelocity);
        }

        private void Update()
        {
            target = decisionMaker.GetTarget();
            InRangeToFireTurretCheck();
            //Passer på å bare skyte på target ships og ikkje andre targets
            if (target == null || target.layer == SERVICESTATION || target.layer == PATROLPOINT)
            {
                return;
            }
            //Sjekker om targetShip er disabled slik at det kan bli looted
            bool isDisabled = target.GetComponent<ShipStats>().GetDisabled();
            if (isDisabled)
            {
                print(isDisabled);
                return;
            }

            if (turretFireReady == false)
            {
                return;
            }

            if (method2)
            {
                if (timer < Time.timeSinceLevelLoad)
                {
                    //Bruker koden med AngleCalculator
                    if ((target.transform.position - this.transform.position).magnitude > maxFireDistance)
                    {
                        print("No target in range");
                        return;
                    }
                    GetComponent<AudioSource>().Play();
                    for (int i = 0; i < turrets.Length; i++)
                    {
                        float turretFireAngle = angleCalculator.CalculateFireAngle(smallFireAngleBool, target.transform, bulletVelocity, turrets[i].transform);
                        float bulletOffsetAngle = Random.Range(minBulletOffsetAngle, maxBulletOffsetAngle);
                        turretFireAngle += bulletOffsetAngle;
                        RotateTowardsTarget(turrets[i].transform, turretFireAngle);

                        FireProjectile(turrets[i].transform);                        

                    }
                    timer += fireRate;
                }
            }
            else
            {
                //Bruker den gamle koden med coroutine


                //Sjekker om targetShip er disabled slik at det kan bli looted
                //if (isDisabled)
                //{
                //    print(isDisabled);
                //    StopMyCoroutine();
                //    return;
                //}

                //InRangeToFIreTurretCheck();

                //CoroutineStarterStopperTurret();
            }
        }

        public void RotateTowardsTarget(Transform _turret, float _turretFireAngle)
        {
            Vector3 adjustedTarget = target.transform.position;
            adjustedTarget.y = _turret.transform.position.y;

            if (float.IsNaN(_turretFireAngle) == true)
            {
                print("OUT OF RANGE");
                return;
            }
            _turret.transform.LookAt(adjustedTarget);
            _turret.transform.Rotate(-fireAngle, 0, 0);
        }

        private void FireProjectile(Transform _turret)
        {
            GameObject bullet = Instantiate(bulletPrefab, _turret.transform.position, _turret.transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletVelocity;
            bullet.GetComponent<BulletScript>().SetBulletDamage(turretDamage);
        }

        private void CoroutineStarterStopperTurret()
        {
            if (turretFireReady && currentCoroutine == null)
            {
                currentCoroutine = ShootAtTarget();
                StartCoroutine(currentCoroutine);
            }
            else if (turretFireReady == false && currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        private void StopMyCoroutine()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        private void InRangeToFireTurretCheck()
        {
            if (target == null)
            {
                turretFireReady = false;
                return;
            }
            if ((target.transform.position - this.transform.position).magnitude < turretRange)
            {
                turretFireReady = true;
            }
            else
            {
                turretFireReady = false;
            }
        }

        private void ResetTurretRotation()
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].transform.Rotate(0, 0, 0);
            }
        }

        IEnumerator ShootAtTarget()
        {
            while(turretFireReady)
            {
                MoveTurretToFire();
                yield return new WaitForSeconds(fireRate);
            }
        }

        private void MoveTurretToFire()
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                GameObject turret = turrets[i];
                float timeUp, timeDown, totalTime;
                timeUp = Mathf.Sqrt(2 * bulletHeight / G);

                float distanceDown = (turret.transform.position.y + bulletHeight) - target.transform.position.y;

                //float distanceDown = bulletHeight - target.transform.position.y;
                timeDown = Mathf.Sqrt(2 * distanceDown / G);
                totalTime = timeUp + timeDown;

                Vector3 distanceInXZ = new Vector3(target.transform.position.x - turret.transform.position.x, 0, target.transform.position.z - turret.transform.position.z);
                Vector3 velocityXZ = distanceInXZ / totalTime;
                Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * G * bulletHeight);

                //Bulletoffset
                Vector3 bulletOffset = Vector3.up * bulletHitOffset;
                velocityY += bulletOffset;

                Vector3 totalVelocity = velocityY + velocityXZ;

                //float fireAngle = Mathf.Tan(velocityXZ.magnitude / velocityY.magnitude);
                float fireAngle = Mathf.Atan2(velocityY.magnitude, velocityXZ.magnitude);
                fireAngle = (180 / Mathf.PI) * fireAngle;


                Vector3 adjustedTarget = target.transform.position;
                adjustedTarget.y = turret.transform.position.y;

                turret.transform.LookAt(adjustedTarget);
                turret.transform.Rotate(-fireAngle, 0, 0);
             
                weaponShooter.FireTurret(bulletPrefab, turretEnd[i], totalVelocity, turretDamage);
            }
            
        }  
    }
}
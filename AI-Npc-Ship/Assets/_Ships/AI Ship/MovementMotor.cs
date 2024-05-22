using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipGame.Ship.Computer;

namespace ShipGame.Ship.Motor
{
    public class MovementMotor : MonoBehaviour
    {
        //BRUKER INFORMASJON DEN FÅR I FRA MOVEMENTCALCULATOR OG SETTER DEN I GANG I MOTOREN
        [SerializeField] Transform rotationMotor = null;

        Rigidbody rigidBody = null;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        public void UseRotationMotor(float speed)
        {
            rigidBody.AddForceAtPosition(this.transform.right * speed * Time.fixedDeltaTime, rotationMotor.position, ForceMode.Acceleration);
        }

        public void UseThrusterMotor(float speed)
        {
            rigidBody.AddForce(this.transform.forward * speed * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
}

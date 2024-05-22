using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] GameObject ship = null;
    [SerializeField] float speed = 1;
    [SerializeField] float rotateSpeed = 0.25f;


    private void LateUpdate()
    {
        Vector3 translateVector = (ship.transform.position - this.transform.position);

        if (translateVector.magnitude < 0.01f)
        {
            translateVector = Vector3.zero;
        }
        this.transform.Translate(translateVector * speed * Time.deltaTime);
        
    }
}

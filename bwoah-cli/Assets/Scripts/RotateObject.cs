using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationDeltaVector;
	
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation *= (Quaternion.Euler(_rotationDeltaVector * Time.deltaTime));
	}
}

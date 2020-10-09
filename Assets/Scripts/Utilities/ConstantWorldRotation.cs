using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantWorldRotation : MonoBehaviour
{
    public Vector3 euler, rotationSpeed;
    Vector3 addedRotation;

    // Update is called once per frame
    void Update()
    {
        addedRotation += rotationSpeed * Time.deltaTime;
        transform.eulerAngles = euler + addedRotation;
    }
}
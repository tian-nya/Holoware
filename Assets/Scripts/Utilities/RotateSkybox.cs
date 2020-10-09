using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float speed;
    float initialRotation;

    private void Start()
    {
        initialRotation = RenderSettings.skybox.GetFloat("_Rotation");
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", initialRotation + Time.time * speed);
    }
}
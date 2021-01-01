using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FllowTarget : MonoBehaviour
{
    public Transform character;
    public float smoothTime = 0.01f;
    private Vector3 cameraVelocity = Vector3.zero;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, character.position + new Vector3(0, 0, -5), ref cameraVelocity, smoothTime);
    }
}

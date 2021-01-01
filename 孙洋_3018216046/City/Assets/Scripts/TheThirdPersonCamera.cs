using UnityEngine;
using System.Collections;

/// <summary>
/// Third person camera.
/// </summary>
public class TheThirdPersonCamera : MonoBehaviour
{
    public Transform character;   //摄像机要跟随的人物
    public float smooth = 0.1f;  //摄像机平滑移动的时间
    public Vector3 v= new Vector3(0,2,-5);
    private Vector3 cameraVelocity = Vector3.zero;
    private Camera mainCamera;  //主摄像机（有时候会在工程中有多个摄像机，但是只能有一个主摄像机吧）

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, character.position + Vector3.up*2-character.forward*8, Time.deltaTime * smooth);
        transform.forward = character.forward;
    }
}
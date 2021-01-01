using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCtrl : MonoBehaviour
{
    public List<WheelCollider> wheels;
    public List<Transform> wheelModel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float s = 0;//控制前进后退
        if(Input.GetKey(KeyCode.W))
        {
            s = 1000;
        }
        if (Input.GetKey(KeyCode.S))
        {
            s = -1000;
        }
        float a = 0;//控制转向
        if (Input.GetKey(KeyCode.A))
        {
            a = -25;
        }
        if (Input.GetKey(KeyCode.D))
        {
            a = 25;
        }
        
        wheels[1].steerAngle = Mathf.Lerp(wheels[1].steerAngle,a,0.1f);
        wheels[2].steerAngle = Mathf.Lerp(wheels[2].steerAngle, a, 0.1f);

        float b = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            b=1000;
        }

        for (int i = 0; i < wheels.Count; i++)
        {
            wheels[i].motorTorque = s;//扭矩
            wheels[i].brakeTorque = b;//刹车
            //给轮子设置位置和角度
            Vector3 pos;
            Quaternion rot;
            wheels[i].GetWorldPose(out pos, out rot);
            wheelModel[i].position = pos;
            wheelModel[i].rotation = rot;
        }
    }
}

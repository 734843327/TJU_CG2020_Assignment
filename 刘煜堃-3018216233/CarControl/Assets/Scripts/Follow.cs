using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    private Transform target;
    private Vector3 fixedDistance;
    private Vector3 tempPos;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;//找到标签为player的目标物体
        fixedDistance = new Vector3(0, 3.35f, -7.33f);//相机处于赛车背后的初始位置，需要进行测试
    }

    void FixedUpdate()
    {
        tempPos = target.TransformDirection(fixedDistance) + target.position;
        transform.position = Vector3.Lerp(transform.position, tempPos, Time.fixedDeltaTime * 3);
        transform.LookAt(target);//使相机自身z坐标对齐物体的自身z坐标（注意不是世界坐标）
    }
}
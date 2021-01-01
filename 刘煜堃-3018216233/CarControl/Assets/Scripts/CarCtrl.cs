using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCtrl : MonoBehaviour
{
    float MoveSpeed = 5;//前进速度
    float RotateSpeed = 65;//转向速度

    void Update()
    {

        if (this.transform.up.y > 0 && this.transform.up.y <= 10)
            if (Input.GetKey(KeyCode.W))//当按下w键时
            {
                if (MoveSpeed <= 40)//当速度不大于60时，按下w键可以使车辆提速
                {
                    MoveSpeed = MoveSpeed + 10 * Time.deltaTime;
                }

                this.transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);

                if (Input.GetKey(KeyCode.A))//在按下w键的同时按下a键，车辆向左前方前进
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * -RotateSpeed);//
                }

                else if (Input.GetKey(KeyCode.D))//在按下w键的同时按下d键，车辆向右前方前进
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed);
                }
            }

            else if (Input.GetKey(KeyCode.S))//当按下s键时
            {
                MoveSpeed = 15;

                this.transform.Translate(Vector3.forward * Time.deltaTime * -MoveSpeed);
                if (Input.GetKey(KeyCode.A))//在按下s键的同时按下a键，车辆向左后方倒车
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * -RotateSpeed);
                }

                else if (Input.GetKey(KeyCode.D))//在按下s键的同时按下d键，车辆向右后方倒车
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed);
                }
            }
            //如果车辆并非在行驶过程中，那么只按下a键或d键不会有任何效果
    }
}
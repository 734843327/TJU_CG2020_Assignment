using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarCtrl : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] private float m_FullTorqueOverAllWheels; //最大力矩
        [Range(0, 1)] [SerializeField] private float m_TractionControl;
        private Rigidbody m_Rigidbody;
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 3.6f; } }
        private int m_GearNum;//档位
        [SerializeField] private static int NoOfGears = 5;
        private float m_GearFactor;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        public float Revs { get; private set; }
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private float m_MaximumSteerAngle;
        [SerializeField] private float m_Topspeed = 200;
        public float MaxSpeed { get { return m_Topspeed; } }
    
      
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private float m_SlipLimit = 0.3f;
       
        private float m_CurrentTorque; //现在的力矩
        private float m_SteerAngle;
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle { get { return m_SteerAngle; } }
        public float AccelInput { get; private set; }

        void Start()
        {
     
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

        }

        //齿轮改变，即挡位改变
        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed / MaxSpeed); //现在速度占全速度比例
            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit) //速度低了掉档
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        }

        //弯曲因子 1-（1-x）^2 = -x2+2x
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }

        //Lerp从from值到value值，但是允许超出from到to的范围，可以制作震荡效果
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }

        private void CalculateGearFactor()
        {
            float f = (1 / (float)NoOfGears);
            //计算当前速度周围挡位的比例，如果挡位1/5到2/5，速度为24%，返回值为0.2
            var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }

        //计算转速，可以做仪表盘
        private void CalculateRevs()
        {
            CalculateGearFactor();
            var gearNumFactor = m_GearNum / (float)NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }

        public void Move(float steering, float accel, float footbrake)
        {
  

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1); //限制steering范围 方向盘
            AccelInput = accel = Mathf.Clamp(accel, 0, 1); //加速
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0); //刹车
            //handbrake = Mathf.Clamp(handbrake, 0, 1); //手闸

        
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;
    

   
            ApplyDrive(accel, footbrake); //前进后退速度变化。
            CapSpeed(); //限制速度不能超过最大值



            CalculateRevs();//计算转速
            GearChanging(); //计算档位

            AddDownForce(); //添加抓地力
           
            TractionControl(); //控制牵引力
        }
    

        private void CapSpeed() //限制速度
        {
            float speed = m_Rigidbody.velocity.magnitude; //当前速度
        
            speed *= 3.6f;
            if (speed > m_Topspeed)
                m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;

        }

        //加速和减速
        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
        
            thrustTorque = accel * (m_CurrentTorque / 4f);
            
        if (accel == 1.0 && footbrake == 0)//前进
        {
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = thrustTorque;
            }

        }
        else if (footbrake == 1.0 && accel == 0 && CurrentSpeed <= 0.1f)
        {//倒车
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = -5000f/4f;
                
            }
        }
        else if (footbrake == 0 && accel == 0)
        {//停车
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = 0f;
                m_WheelColliders[i].brakeTorque = 0f;
            }
        }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = 20000f * footbrake;
                }
                //把刹车力去掉，方便再启动
                if (CurrentSpeed <= 0.1 && CurrentSpeed > -0.001f)
                {
                    m_WheelColliders[i].brakeTorque = 0;
                }
             
        }
        }


    //增加抓地力
    private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        private void TractionControl()
        {
            WheelHit wheelHit;
        
           
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].GetGroundHit(out wheelHit);

            //forwardSlip在滚动方向上的滑动，加速滑动为负，制动滑动为正。
                AdjustTorque(wheelHit.forwardSlip);//修改牵引力
            }
               
        }
    //当向前滑动距离超过阈值后，就说明轮胎过度滑转，则减少牵引力，以降低转速。上面函数调用
        private void AdjustTorque(float forwardSlip)
            {
                if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
                {
                    m_CurrentTorque -= 10 * m_TractionControl;
                }
                else
                {
                    m_CurrentTorque += 10 * m_TractionControl;
                    if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                    {
                        m_CurrentTorque = m_FullTorqueOverAllWheels;
                    }
                }
            }

      

    private void FixedUpdate()
    {
        //提供前后左右走的常数，传入Move
        float h=0;
        float v=0;
        if (Input.GetKey(KeyCode.W))
        {
            v = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            v = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            h = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            h = 1;
        }
        Move(h, v, v);
    }


}

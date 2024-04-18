//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VDDataRead
{

    public class DRModelPoseAdjust : MonoBehaviour
    {
        Camera m_CameraLook = null;
        VDDataReadClass m_VDDataReadClass = null;
        int m_ActorId = 0;
        float m_MoveSpeed = 1f;
        float m_RotateSpeed = 1f;
        public float MoveSpeed { get { return m_MoveSpeed; } set { if (value >= 0) { m_MoveSpeed = value; } } }
        public float RotateSpeed { get { return m_RotateSpeed; } set { if (value >= 0) { m_RotateSpeed = value; } } }

        // Use this for initialization
        void Awake()
        {
        }

        //
        void LateUpdate()
        {
            if (m_VDDataReadClass != null && m_ActorId == DRDataReadController.NOW_CHOOSED_ACTORID)
            {
                LateUpdate_RotationAdjust();
                LateUpdate_PositionKeySet();
                LateUpdate_PositionAdjust();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Initial(int actorId, VDDataReadClass dr, Camera cameraLook)
        {
            m_ActorId = actorId;
            m_VDDataReadClass = dr;
            m_CameraLook = cameraLook;
        }



        #region Rotation Adjust
        /// <summary>
        /// 模型旋转调节：shift + 鼠标右键按住拖动 调节模型方位角。
        /// 一定要在 LateUpdate() 中更新，否则会有抖的可能。
        /// </summary>
        void LateUpdate_RotationAdjust()
        {
            float moveSpeed = 100.0f * m_RotateSpeed;
            //Quaternion dq = new Quaternion(0, 0, 0, 1);
            float d_yaw = 0;

            //鼠标右键控制旋转，只平面旋转，即只改变方位角
            float h = 0;
            if (Input.GetMouseButton(1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                h = Input.GetAxis("Mouse X");
            }
            if (h != 0)
            {
                d_yaw = h * Time.deltaTime * moveSpeed;
            }

            //
            if (d_yaw != 0)
            {
                Vector3 adjust_euler_xyz = new Vector3(0, 0, 0);
                m_VDDataReadClass.PAGetRotationOffset(ref adjust_euler_xyz);
                adjust_euler_xyz.y = adjust_euler_xyz.y + d_yaw;
                if (adjust_euler_xyz.y > 180) { adjust_euler_xyz.y -= 360; }
                else if (adjust_euler_xyz.y <= -180) { adjust_euler_xyz.y += 360; }
                // apply
                m_VDDataReadClass.PASetRotationOffset(adjust_euler_xyz);
            }
        }
        #endregion


        #region Position Adjust
        /// <summary>
        /// 设置模型回到零点，原点；
        /// 设置模型原点；
        /// 锁定模型水平位移。
        /// 一定要在 LateUpdate() 中更新，否则会有抖的可能。
        /// </summary>
        void LateUpdate_PositionKeySet()
        {
            //Position to zero -- 回零点
            if (DRShortcutKey.JudgeKey(enum_ShortcutKeyEven.ModelPositionToZero))
            {
                m_VDDataReadClass.PASetPositionToZero();
                return;
            }

            //Position to origin -- 回原点
            if (DRShortcutKey.JudgeKey(enum_ShortcutKeyEven.ModelPositionToOrigin))
            {
                m_VDDataReadClass.PASetPositionToOrigin();
                return;
            }

            //设置原点
            if (DRShortcutKey.JudgeKey(enum_ShortcutKeyEven.SetModelCurrentPositionAsOrigin))
            {
                m_VDDataReadClass.PASetCurrentPositionAsOrigin();
                return;
            }

            //打开关闭位置限制
            if (DRShortcutKey.JudgeKey(enum_ShortcutKeyEven.LockModelPlanePosition))
            {
                m_VDDataReadClass.PALockPositionXY(!m_VDDataReadClass.PAIsLockPositionXY());
                return;
            }
        }

        /// <summary>
        /// 位移调节：（1）shift + 鼠标左键按住拖动 调节模型水平位移；
        /// （2）shift + 鼠标中键按住拖动 调节模型纵向位移。
        /// 一定要在 LateUpdate() 中更新，否则会有抖的可能。
        /// </summary>
        void LateUpdate_PositionAdjust()
        {
            float moveSpeed = 2.0f * m_MoveSpeed;
            Vector3 ds = new Vector3(0, 0, 0);

            if (true)
            {
                float h = 0, v = 0;

                // 按键控制移动
                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !Input.GetKey(KeyCode.Space))
                {
                    h = Input.GetAxis("Horizontal");
                    v = Input.GetAxis("Vertical");
                }

                // 鼠标左键控制移动，要求按住 Shift 键
                if (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    h = Input.GetAxis("Mouse X");
                    v = Input.GetAxis("Mouse Y");

                    //
                    if (Input.GetKey(KeyCode.X) && !Input.GetKey(KeyCode.Z)) { v = 0; }
                    else if (!Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.Z)) { h = 0; }
                }

                if (h != 0 || v != 0)
                {
                    float c_ey = 0;
                    if (m_CameraLook != null)
                    {
                        c_ey = m_CameraLook.transform.localEulerAngles.y;
                    }
                    c_ey = (c_ey - 180) * 3.14f / 180;
                    h = -h * Time.deltaTime * moveSpeed;
                    v = -v * Time.deltaTime * moveSpeed;
                    ds.x = h * Mathf.Cos(c_ey) + v * Mathf.Sin(c_ey);
                    ds.z = -h * Mathf.Sin(c_ey) + v * Mathf.Cos(c_ey);
                }
            }

            //鼠标中键控制高度，即 Y 轴位移。
            if (true)
            {
                float v = 0;

                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.Space))
                {
                    v = Input.GetAxis("Vertical");
                }

                if (Input.GetMouseButton(2) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    v = Input.GetAxis("Mouse Y");
                }

                if (v != 0)
                {
                    ds.y = v * Time.deltaTime * moveSpeed;
                }
            }


            //
            if (ds.x != 0 || ds.y != 0 || ds.z != 0)
            {
                Vector3 posi = new Vector3();
                m_VDDataReadClass.PAGetPositionOffset(ref posi);
                posi += ds;
                //限制不能把角色移到地面下。
                if (posi.y < 0) { posi.y = 0; }
                // apply
                m_VDDataReadClass.PASetPositionOffset(posi);
            }
        }
        #endregion


    }//end class
}//end namespace

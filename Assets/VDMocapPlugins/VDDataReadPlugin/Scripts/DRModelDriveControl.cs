//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;


namespace VDDataRead
{
    //设定所有 obj 初始时刻坐标系与Unity世界坐标系方向相同
    public class DRModelDriveControl : MonoBehaviour
    {
        public class ModelInfo
        {
            //
            public Pose[] m_BodyNodes_InitialPose = new Pose[DRDataType.NODES_BODY];
            public Pose[] m_rHandNodes_InitialPose = new Pose[DRDataType.NODES_HAND];
            public Pose[] m_lHandNodes_InitialPose = new Pose[DRDataType.NODES_HAND];
            //
            public Transform[] m_BodyNodes = null;  //NODES_BODY
            public Transform[] m_rHandNodes = null; //NODES_HAND
            public Transform[] m_lHandNodes = null; //NODES_HAND
            //
            public List<SkinnedMeshRenderer> m_FaceBlendShapesRenderer_ARKit = null;
            //key: SkinnedMeshRenerer.name:BlendShape.name, value: Used BlendShape indexes in DRDataType.enum_FaceBlendShape.
            public Dictionary<string, int[]> m_DicFaceBlendShapesIndex_ARKit = new Dictionary<string, int[]>();
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pose"></param>
            /// <param name="posi"></param>
            /// <param name="quat"></param>
            public static void ToPose(ref Pose[] pose, Vector3[] posi, Quaternion[] quat)
            {
                int count = quat.Length;
                for (int i = 0; i < count; i++)
                {
                    pose[i].position = posi[i];
                    pose[i].rotation = quat[i];
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pose"></param>
            /// <returns></returns>
            public static Vector3[] GetPosition(Pose[] pose)
            {
                int len = pose.Length;
                Vector3[] posi = new Vector3[len];
                for (int i = 0; i < len; i++)
                {
                    posi[i] = pose[i].position;
                }
                return posi;
            }
        }//end class ModelInfo

        //
        VDDataReadClass m_VDDataReadClass = null;
        ModelInfo m_ModelInfo = null;
        MocapData m_MocapDataBuffer = new MocapData();
        MocapData m_MotionData = new MocapData();
        object m_LockMotionData = new object();
        object m_LockModelInfo = new object();
        //
        public bool IsMocapDataBreak { get; set; }



        // Use this for initialization
        void Awake()
        {
            Initial();
        }

        // Update is called once per frame
        void Update()
        {
            //更新模型姿态
            UpdateModelAttitude();
        }

        //
        void OnDestroy()
        {
            Destory_m();
        }

        /// <summary>
        /// 
        /// </summary>
        void Initial()
        {
            IsMocapDataBreak = false;
        }

        /// <summary>
        /// 
        /// </summary>
        void Destory_m()
        {
            if (m_VDDataReadClass != null)
            {
                m_VDDataReadClass.MocapDataReceived_RemoveListener(OnRecvMocapData);
            }
        }


        #region RecvMocapData
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        public void AddListenerRecvMocapData(VDDataReadClass dr)
        {
            m_VDDataReadClass = dr;
            dr.MocapDataReceived_AddListener(OnRecvMocapData);
        }

        void OnRecvMocapData(bool isConnect, MocapData md, MocapData md_adjusted)
        {
            IsMocapDataBreak = !isConnect;
            //
            lock (m_LockMotionData) 
            {
                m_MocapDataBuffer = md_adjusted.Clone();
            }
        }
        #endregion



        //设置模型骨架
        #region 设置模型骨架
        /// <summary>
        /// 地址赋值，同地址。
        /// </summary>
        /// <param name="info"></param>
        public void SetModelInfo(ModelInfo info)
        {
            //锁
            System.Threading.Monitor.Enter(m_LockModelInfo);             
            //
            m_ModelInfo = info;
            //
            SetInitialNodesPositionInTpose();
            //解锁
            System.Threading.Monitor.Exit(m_LockModelInfo);
        }
        /// <summary>
        /// 每次更换骨架时，都需要调用
        /// </summary>
        void SetInitialNodesPositionInTpose()
        {
            m_VDDataReadClass.UdpSetPositionInInitialTpose(ModelInfo.GetPosition(m_ModelInfo.m_BodyNodes_InitialPose),
                ModelInfo.GetPosition(m_ModelInfo.m_rHandNodes_InitialPose), ModelInfo.GetPosition(m_ModelInfo.m_lHandNodes_InitialPose));
        }
        #endregion



        #region Get MotionData and Update Model Pose.
        /// <summary>
        /// 
        /// </summary>
        void UpdateModelAttitude()
        {
            //
            if (m_ModelInfo == null) { return; }

            //get new motion data
            //获取数据，必须放在第一位
            GetMotionData();

            if (m_MotionData.isUpdate)
            {
                //尝试锁
                if (System.Threading.Monitor.TryEnter(m_LockModelInfo))
                {
                    //更新模型节点姿态
                    SetModelPose();

                    //更新模型脸部表情
                    SetModelFaceBlendShapes();

                    //解锁
                    System.Threading.Monitor.Exit(m_LockModelInfo);
                }
            }
        }

        /// <summary>
        /// 注意：得到的动捕数据中 body 双手腕的 Pose, 必须与 rHand/lHand 的数据一致。 
        /// </summary>
        void GetMotionData()
        {
            lock (m_LockMotionData)
            {
                m_MotionData = m_MocapDataBuffer.Clone();
            }
        }

        /// <summary>
        /// 根据得到的动作数据，设置模型姿态
        /// </summary>
        void SetModelPose()
        {
            //
            if (m_MotionData == null || m_ModelInfo == null) { return; }
            //
            if (m_ModelInfo.m_BodyNodes != null)
            {
                for (byte i = 0; i < DRDataType.NODES_BODY; i++)
                {
                    if (m_ModelInfo.m_BodyNodes[i] == null) { continue; }
                    switch (i)
                    {
                        case (byte)(DRDataType._BodyNodes_.BN_Hips):    // Hips -- Root
                            {
                                if (m_MotionData.position_body != null)
                                {
                                    m_ModelInfo.m_BodyNodes[i].position = m_MotionData.position_body[i];
                                }
                                m_ModelInfo.m_BodyNodes[i].rotation = m_MotionData.quaternion_body[i] * m_ModelInfo.m_BodyNodes_InitialPose[i].rotation;
                            }
                            break;
                        case (byte)(DRDataType._BodyNodes_.BN_RightHand):
                        case (byte)(DRDataType._BodyNodes_.BN_LeftHand):  //right hand
                            { }
                            break;
                        default:
                            {
                                m_ModelInfo.m_BodyNodes[i].rotation = m_MotionData.quaternion_body[i] * m_ModelInfo.m_BodyNodes_InitialPose[i].rotation;
                            }
                            break;
                    }
                }
            }

            if (m_ModelInfo.m_rHandNodes != null)
            {
                for (byte i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    if (m_ModelInfo.m_rHandNodes[i] == null) { continue; }
                    m_ModelInfo.m_rHandNodes[i].rotation = m_MotionData.quaternion_rHand[i] * m_ModelInfo.m_rHandNodes_InitialPose[i].rotation;
                }
            }

            if (m_ModelInfo.m_lHandNodes != null)
            {
                for (byte i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    if (m_ModelInfo.m_lHandNodes[i] == null) { continue; }
                    m_ModelInfo.m_lHandNodes[i].rotation = m_MotionData.quaternion_lHand[i] * m_ModelInfo.m_lHandNodes_InitialPose[i].rotation;
                }
            }
        }

        /// <summary>
        /// 根据得到的脸部表情bs数据，设置模型脸部表情
        /// </summary>
        void SetModelFaceBlendShapes()
        {
            SetModelFaceBlendShapes_ARKit();
        }
        /// <summary>
        /// 根据得到的脸部表情bs数据，设置模型脸部表情。(ARKit表情规范)
        /// </summary>
        void SetModelFaceBlendShapes_ARKit()
        {
            //
            if (m_ModelInfo.m_FaceBlendShapesRenderer_ARKit == null || !m_MotionData.isUseFaceBlendShapesARKit) { return; }
            //
            int count_bs = m_MotionData.faceBlendShapesARKit.Length;
            foreach (var meshRenderer in m_ModelInfo.m_FaceBlendShapesRenderer_ARKit)
            {
                var mesh = meshRenderer.sharedMesh;
                var count = mesh.blendShapeCount;
                for (int i = 0; i < count; i++)
                {
                    if (m_ModelInfo.m_DicFaceBlendShapesIndex_ARKit.ContainsKey(meshRenderer.name + ":" + mesh.GetBlendShapeName(i)))
                    {
                        int[] indexs = m_ModelInfo.m_DicFaceBlendShapesIndex_ARKit[meshRenderer.name + ":" + mesh.GetBlendShapeName(i)];
                        float bs = 0f;
                        int nnn = 0;
                        foreach (int index in indexs)
                        {
                            if (index >= 0 && index < count_bs)
                            {
                                bs += m_MotionData.faceBlendShapesARKit[index];
                                nnn++;
                            }
                        }
                        if (nnn == 0) { continue; }
                        bs = bs / nnn;
                        if (bs >= 0) { meshRenderer.SetBlendShapeWeight(i, bs); }
                    }
                }
            }
        }
        #endregion


    }//end class
}//end namespace

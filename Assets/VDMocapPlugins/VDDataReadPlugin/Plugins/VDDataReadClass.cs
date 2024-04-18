//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;


namespace VDDataRead
{
    #region 枚举及结构体
    //body nodes name and its index
    public enum _BodyNodes_
    {
        BN_Hips = 0,                    ///< Hips
        BN_RightUpperLeg,               ///< Right Upper Leg
        BN_RightLowerLeg,               ///< Right Lower Leg
        BN_RightFoot,                   ///< Right Foot
        BN_RightToe,                    ///< Right Toe
        BN_LeftUpperLeg,                ///< Left Upper Leg
        BN_LeftLowerLeg,                ///< Left Lower Leg
        BN_LeftFoot,                    ///< Left Foot
        BN_LeftToe,                     ///< Left Toe
        BN_Spine,                       ///< Spine
        BN_Spine1,                      ///< Spine1
        BN_Spine2,                      ///< Spine2
        BN_Spine3,                      ///< Spine3 -- Back
        BN_Neck,                        ///< Neck
        BN_Head,                        ///< Head
        BN_RightShoulder,               ///< Right Shoulder
        BN_RightUpperArm,               ///< Right Upper Arm
        BN_RightLowerArm,               ///< Right Lower Arm
        BN_RightHand,                   ///< Right Hand
        BN_LeftShoulder,                ///< Left Shoulder
        BN_LeftUpperArm,                ///< Left Upper Arm
        BN_LeftLowerArm,                ///< Left Lower Arm
        BN_LeftHand,                    ///< Left Hand
    };


    public enum _HandNodes_
    {
        HN_Hand = 0,
        HN_ThumbFinger,
        HN_ThumbFinger1,
        HN_ThumbFinger2,
        HN_IndexFinger,
        HN_IndexFinger1,
        HN_IndexFinger2,
        HN_IndexFinger3,
        HN_MiddleFinger,
        HN_MiddleFinger1,
        HN_MiddleFinger2,
        HN_MiddleFinger3,
        HN_RingFinger,
        HN_RingFinger1,
        HN_RingFinger2,
        HN_RingFinger3,
        HN_PinkyFinger,
        HN_PinkyFinger1,
        HN_PinkyFinger2,
        HN_PinkyFinger3,
    };



    //Face BlendShape index
    public enum _FaceBlendShapeARKit_
    {
        BrowDownLeft = 0,
        BrowDownRight,
        BrowInnerUp,
        BrowOuterUpLeft,
        BrowOuterUpRight,
        CheekPuff,
        CheekSquintLeft,
        CheekSquintRight,
        EyeBlinkLeft,
        EyeBlinkRight,
        EyeLookDownLeft,
        EyeLookDownRight,
        EyeLookInLeft,
        EyeLookInRight,
        EyeLookOutLeft,
        EyeLookOutRight,
        EyeLookUpLeft,
        EyeLookUpRight,
        EyeSquintLeft,
        EyeSquintRight,
        EyeWideLeft,
        EyeWideRight,
        JawForward,
        JawLeft,
        JawOpen,
        JawRight,
        MouthClose,
        MouthDimpleLeft,
        MouthDimpleRight,
        MouthFrownLeft,
        MouthFrownRight,
        MouthFunnel,
        MouthLeft,
        MouthLowerDownLeft,
        MouthLowerDownRight,
        MouthPressLeft,
        MouthPressRight,
        MouthPucker,
        MouthRight,
        MouthRollLower,
        MouthRollUpper,
        MouthShrugLower,
        MouthShrugUpper,
        MouthSmileLeft,
        MouthSmileRight,
        MouthStretchLeft,
        MouthStretchRight,
        MouthUpperUpLeft,
        MouthUpperUpRight,
        NoseSneerLeft,
        NoseSneerRight,
        TongueOut,
    };

    //Face BlendShape index
    public enum _FaceBlendShapeAudio_
    {
        a = 0,
        b,
        c,
        d,
        e,
        f,
        g,
        h,
        i,
        j,
        k,
        l,
        m,
        n,
        o,
        p,
        q,
        r,
        s,
        t,
        u,
        v,
        w,
        x,
        y,
        z,
    };


    public enum _SensorState_
    {
        SS_NONE = 0,
        SS_Well,                        //正常
        SS_NoData,                      //无数据
        SS_UnReady,                     //初始化中
        SS_BadMag,                      //磁干扰
    };


    public enum _WorldSpace_
    {
        WS_Geo = 0,                        //表示世界坐标系为地理坐标系
        WS_Unity,                          //表示世界坐标系为Unity世界坐标系
        WS_UE4,                            //表示世界坐标系为UE4世界坐标系
    };


    public struct _Version_
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26, ArraySubType = UnmanagedType.I1)]
        public byte[] Project_Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
        public byte[] Author_Organization;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26, ArraySubType = UnmanagedType.I1)]
        public byte[] Author_Domain;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26, ArraySubType = UnmanagedType.I1)]
        public byte[] Author_Maintainer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26, ArraySubType = UnmanagedType.I1)]
        public byte[] Version;
        byte Version_Major;
        byte Version_Minor;
        byte Version_Patch;
    };


    //
    public struct _MocapData_
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool isUpdate;                              //true表示设备数据已更新
        [MarshalAs(UnmanagedType.U4)]
        public uint frameIndex;                            //帧序号
        [MarshalAs(UnmanagedType.I4)]
        public int frequency;                              //设备数据传输频率
        [MarshalAs(UnmanagedType.I4)]
        public int nsResult;                               //其它数据

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = UnmanagedType.I4)]   /*必须定义数组大小*/
        public _SensorState_[] sensorState_body;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] position_body;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23 * 4, ArraySubType = UnmanagedType.R4)]
        public float[] quaternion_body;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] gyr_body;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] acc_body;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] velocity_body;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.I4)]
        public _SensorState_[] sensorState_rHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] position_rHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 4, ArraySubType = UnmanagedType.R4)]
        public float[] quaternion_rHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] gyr_rHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] acc_rHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] velocity_rHand;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.I4)]
        public _SensorState_[] sensorState_lHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] position_lHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 4, ArraySubType = UnmanagedType.R4)]
        public float[] quaternion_lHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] gyr_lHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] acc_lHand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20 * 3, ArraySubType = UnmanagedType.R4)]
        public float[] velocity_lHand;

        [MarshalAs(UnmanagedType.I1)]
        public bool isUseFaceBlendShapesARKit;
        [MarshalAs(UnmanagedType.I1)]
        public bool isUseFaceBlendShapesAudio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52, ArraySubType = UnmanagedType.R4)]
        public float[] faceBlendShapesARKit;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26, ArraySubType = UnmanagedType.R4)]
        public float[] faceBlendShapesAudio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.R4)]
        public float[] localQuat_RightEyeball;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.R4)]
        public float[] localQuat_LeftEyeball;

        //
        public _MocapData_(int _null_)
        {
            isUpdate = false;
            frameIndex = 0;
            frequency = 60;
            nsResult = 0;
            sensorState_body = new _SensorState_[23];
            position_body = new float[23 * 3];
            quaternion_body = new float[23 * 4];
            for (int i = 0; i < 23; i++) { quaternion_body[i * 4] = 1; }
            gyr_body = new float[23 * 3];
            acc_body = new float[23 * 3];
            velocity_body = new float[23 * 3];
            sensorState_rHand = new _SensorState_[20];
            position_rHand = new float[20 * 3];
            quaternion_rHand = new float[20 * 4];
            for (int i = 0; i < 20; i++) { quaternion_rHand[i * 4] = 1; }
            gyr_rHand = new float[20 * 3];
            acc_rHand = new float[20 * 3];
            velocity_rHand = new float[20 * 3];
            sensorState_lHand = new _SensorState_[20];
            position_lHand = new float[20 * 3];
            quaternion_lHand = new float[20 * 4];
            for (int i = 0; i < 20; i++) { quaternion_lHand[i * 4] = 1; }
            gyr_lHand = new float[20 * 3];
            acc_lHand = new float[20 * 3];
            velocity_lHand = new float[20 * 3];
            isUseFaceBlendShapesARKit = false;
            isUseFaceBlendShapesAudio = false;
            faceBlendShapesARKit = new float[52];
            faceBlendShapesAudio = new float[26];
            localQuat_RightEyeball = new float[4];
            localQuat_LeftEyeball = new float[4];
        }
    };
    public class MocapData
    {
        public const int NODES_BODY = 23;
        public const int NODES_HAND = 20;
        public const int NODES_FACEBS_ARKIT = 52;
        //public const int NODES_FACEBS_AUDIO = 26;

        public bool isUpdate = false;                          //true表示设备数据已更新
        public uint frameIndex = 0;                            //帧序号
        public int frequency = 0;                              //设备数据传输频率
        //public int nsResult = 0;                             //其它数据

        //public _SensorState_[] sensorState_body = new _SensorState_[NODES_BODY];
        public Vector3[] position_body = new Vector3[NODES_BODY]/*xyz-m*/;
        public Quaternion[] quaternion_body = new Quaternion[NODES_BODY]/*wxyz*/;
        //public Vector3[] gyr_body = new Vector3[NODES_BODY];
        //public Vector3[] acc_body = new Vector3[NODES_BODY]; //去重力加速度
        public Vector3[] velocity_body = new Vector3[NODES_BODY];

        //public _SensorState_[] sensorState_rHand = new _SensorState_[NODES_HAND];
        public Vector3[] position_rHand = new Vector3[NODES_HAND]/*xyz-m*/;
        public Quaternion[] quaternion_rHand = new Quaternion[NODES_HAND]/*wxyz*/;
        //public Vector3[] gyr_rHand = new Vector3[NODES_HAND];
        //public Vector3[] acc_rHand = new Vector3[NODES_HAND];  //去重力加速度
        public Vector3[] velocity_rHand = new Vector3[NODES_HAND];

        //public _SensorState_[] sensorState_lHand = new _SensorState_[NODES_HAND];
        public Vector3[] position_lHand = new Vector3[NODES_HAND]/*xyz-m*/;
        public Quaternion[] quaternion_lHand = new Quaternion[NODES_HAND]/*wxyz*/;
        //public Vector3[] gyr_lHand = new Vector3[NODES_HAND];
        //public Vector3[] acc_lHand = new Vector3[NODES_HAND];  //去重力加速度
        public Vector3[] velocity_lHand = new Vector3[NODES_HAND];

        public bool isUseFaceBlendShapesARKit = false;
        //public bool isUseFaceBlendShapesAudio = false;
        public float[] faceBlendShapesARKit = new float[NODES_FACEBS_ARKIT];  //表情 bs 数据，图片方式识别的表情
        //public float[] faceBlendShapesAudio = new float[NODES_FACEBS_AUDIO];  //表情 bs 数据，语音方式识别的表情
        //public Quaternion localQuat_RightEyeball;  //右眼球数据
        //public Quaternion localQuat_LeftEyeball;   //左眼球数据

        public MocapData()
        {
            frequency = 60;
            for (int i = 0; i < NODES_BODY; i++) { quaternion_body[i] = new Quaternion(0, 0, 0, 1); }
            for (int i = 0; i < NODES_HAND; i++)
            {
                quaternion_rHand[i] = new Quaternion(0, 0, 0, 1);
                quaternion_lHand[i] = new Quaternion(0, 0, 0, 1);
            }
        }

        public MocapData Clone()
        {
            MocapData md_out = new MocapData();
            md_out.isUpdate = this.isUpdate;
            md_out.frameIndex = this.frameIndex;
            md_out.frequency = this.frequency;
            for (int i = 0; i < NODES_BODY; i++)
            {
                md_out.position_body[i] = this.position_body[i];
                md_out.quaternion_body[i] = this.quaternion_body[i];
                md_out.velocity_body[i] = this.velocity_body[i];
            }
            for (int i = 0; i < NODES_HAND; i++)
            {
                //rhand
                md_out.position_rHand[i] = this.position_rHand[i];
                md_out.quaternion_rHand[i] = this.quaternion_rHand[i];
                md_out.velocity_rHand[i] = this.velocity_rHand[i];
                //lhand
                md_out.position_lHand[i] = this.position_lHand[i];
                md_out.quaternion_lHand[i] = this.quaternion_lHand[i];
                md_out.velocity_lHand[i] = this.velocity_lHand[i];
            }
            md_out.isUseFaceBlendShapesARKit = this.isUseFaceBlendShapesARKit;
            for (int i = 0; i < NODES_FACEBS_ARKIT; i++)
            {
                md_out.faceBlendShapesARKit[i] = this.faceBlendShapesARKit[i];
            }
            return md_out;
        }
    }
    #endregion


    class VDDataReadPlugin
    {
        [DllImport("VDMocapSDK_DataRead")]
        public static extern void GetVersionInfo(ref _Version_ version);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpSetPositionInInitialTpose(int index, string dst_ip, ushort dst_port, _WorldSpace_ worldSpace,
            float[] initialPosition_body, float[] initialPosition_rHand, float[] initialPosition_lHand);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpOpen(int index, ushort localPort);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void UdpClose(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpIsOpen(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpRemove(int index, string dst_ip, ushort dst_port);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpSendRequestConnect(int index, string dst_ip, ushort dst_port);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpRecvMocapData(int index, string dst_ip, ushort dst_port, ref _MocapData_ mocapData);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool UdpGetRecvInitialTposePosition(int index, string dst_ip, ushort dst_port, _WorldSpace_ worldSpace,
			ref float initialPosition_body, ref float initialPosition_rHand, ref float initialPosition_lHand);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PAAdd(int index, _WorldSpace_ ws);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PARemove(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool PAIsExist(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PAOnAdjustMotionData(int index, ref float quat_b, ref float posi_b, 
            ref float quat_rh, ref float posi_rh, ref float quat_lh, ref float posi_lh);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PAGetRotationOffset(int index, ref float euler_xyz_out);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetRotationOffset(int index, float[] euler_xyz_RotationOrderInGeoIsZYX);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool PAIsLimitMotionInCircle(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PALimitMotionInCircle(int index, bool isLimit);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetMotionCircleRadius(int index, float radius);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern float PAGetMotionCircleRadius(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern bool PAIsLockPositionXY(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PALockPositionXY(int index, bool isLock);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetCurrentPositionAsOrigin(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetPositionToOrigin(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetPositionToZero(int index);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetPosition(int index, float[] actorPosition, int settedAxis);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PASetPositionOffset(int index, float[] positionOffset);

        [DllImport("VDMocapSDK_DataRead")]
        public static extern void PAGetPositionOffset(int index, ref float positionOffset);

    }//end class


    /// <summary>
    /// 
    /// </summary>
    public class VDDataReadClass : MonoBehaviour
    {
        #region some define
        public const string DATAREADDll_VERSION = "2.5.1";
        public const string SCRIPT_VERSION = "1.0.0";
        public const int NODES_BODY = 23;
        public const int NODES_HAND = 20;
        public const int NODES_FACEBS_ARKIT = 52;
        //
        public struct Addr
        {
            public int actorId;
            public string ip;
            public ushort port;
            public Addr(int actorId_, string ip_, ushort port_)
            {
                actorId = actorId_;
                ip = ip_;
                port = port_;
            }
        }
        //key: ip:port(如：192.168.1.234:9000)
        static Dictionary<string, Addr> m_DicDstAddr = new Dictionary<string, Addr>();
        static List<int> m_ListActorId = new List<int>();
        //
        public delegate void MocapDataReceivedEventHandler(bool isConnect, MocapData md, MocapData md_adjusted);
        private event MocapDataReceivedEventHandler MocapDataReceived = null;
        //
        bool m_IsRecvThreadRunning = false;
        bool m_IsConnect = false;
        _MocapData_ m_pq_initial = new _MocapData_(0);
        public Addr DstAddr { get; private set; }
        public int ActorId { get { return DstAddr.actorId; } }
        #endregion


        //若用构造函数，在Unity中可能会被多次执行
        #region 初始化
        //
        void Awake()
        {
            //
            int actorId = -1;
            for (int i = 0; i < m_ListActorId.Count; i++)
            {
                if (!m_ListActorId.Contains(i)) { actorId = i; break; }
            }
            if (actorId < 0) { actorId = m_ListActorId.Count; }
            m_ListActorId.Add(actorId);
            //
            DstAddr = new Addr(actorId, "", 0);
            //
            StartRecvMocapDataThread();
            //
            Debug.Log(GetVersionInfo());
        }
        //
        void OnDestroy()
        {
            if (m_ListActorId.Contains(ActorId)) { m_ListActorId.Remove(ActorId); }
            m_IsRecvThreadRunning = false;
        }
        #endregion


        #region public functions
        #region static
        /// <summary>
        /// 
        /// </summary>
        public static string GetVersionInfo()
        {
            _Version_ v = new _Version_();
            VDDataReadPlugin.GetVersionInfo(ref v);
            int count = v.Version.Length;
            for (int i = v.Version.Length - 1; i >= 0; i--) { if (v.Version[i] == 0) { count--; } else { break; } }
            string nowDllVersion = System.Text.Encoding.Default.GetString(v.Version, 0, count);
            //
            string version = "";
            version += "插件版本：" + SCRIPT_VERSION + "\r\n";
            version += "插件编写时 VDMocapSDK_DataRead.dll 版本：" + DATAREADDll_VERSION + "\r\n";
            version += "当前使用的 VDMocapSDK_DataRead.dll 版本：" + nowDllVersion + "\r\n";
            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool UdpIsOpen()
        {
            return VDDataReadPlugin.UdpIsOpen(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool UdpOpen()
        {
            if(!UdpIsOpen())
            {
                return VDDataReadPlugin.UdpOpen(0, 0);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UdpClose()
        {
            VDDataReadPlugin.UdpClose(0);
        }
        #endregion

        /// <summary>
        /// add mocap data received listener.
        /// </summary>
        /// <param name="func"></param>
        public void MocapDataReceived_AddListener(MocapDataReceivedEventHandler func)
        {
            MocapDataReceived += func;
        }

        /// <summary>
        /// remove mocap data received listener.
        /// </summary>
        /// <param name="func"></param>
        public void MocapDataReceived_RemoveListener(MocapDataReceivedEventHandler func)
        {
            MocapDataReceived -= func;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UdpRemove()
        {
            m_IsConnect = false;
            Thread.Sleep(50);
        }

        /// <summary>
        /// 请求连接。
        /// 返回 true: 发送请求成功, false: 失败。
        /// </summary>
        public bool UdpSendRequestConnect(string dst_ip, ushort dst_port)
        {
            if (m_IsConnect) { Debug.Log("请先断开连接！"); return false; }
            if (DicDstAddr_IsContains(dst_ip, dst_port)) { Debug.Log("目标地址被占用！"); return false; }
            //
            if (VDDataReadPlugin.UdpSendRequestConnect(0, dst_ip, dst_port))
            {
                DicDstAddr_Add(dst_ip, dst_port);
                DstAddr = DicDstAddr_Value(dst_ip, dst_port);
                m_IsConnect = true;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 执行前需保证已执行 UdpSendRequestConnect().
        /// 因为 DstAddr 是在 UdpSendRequestConnect() 中设置的，故必须先执行 UdpSendRequestConnect().
        /// </summary>
        public bool UdpSetPositionInInitialTpose(Vector3[] initialPosition_body, Vector3[] initialPosition_rHand, Vector3[] initialPosition_lHand)
        {
            if (!m_IsRecvThreadRunning) { return false; }
            //
            m_pq_initial.position_body = Convert_m(initialPosition_body);
            m_pq_initial.position_rHand = Convert_m(initialPosition_rHand);
            m_pq_initial.position_lHand = Convert_m(initialPosition_lHand);
            m_pq_initial.isUpdate = true;
            return VDDataReadPlugin.UdpSetPositionInInitialTpose(0, DstAddr.ip, DstAddr.port, _WorldSpace_.WS_Unity,
                m_pq_initial.position_body, m_pq_initial.position_rHand, m_pq_initial.position_lHand);
        }

        /// <summary>
        /// 执行前需保证已执行 UdpSendRequestConnect().
        /// 因为 DstAddr 是在 UdpSendRequestConnect() 中设置的，故必须先执行 UdpSendRequestConnect().
        /// </summary>
        public bool UdpGetRecvInitialTposePosition(ref Vector3[] initialPosition_body, ref Vector3[] initialPosition_rHand, ref Vector3[] initialPosition_lHand)
        {
            if (!m_IsRecvThreadRunning) { return false; }
            //
            float[] iniposi_b = new float[NODES_BODY];
            float[] iniposi_rh = new float[NODES_HAND];
            float[] iniposi_lh = new float[NODES_HAND];
            bool isOk = VDDataReadPlugin.UdpGetRecvInitialTposePosition(0, DstAddr.ip, DstAddr.port, _WorldSpace_.WS_Unity,
                ref iniposi_b[0], ref iniposi_rh[0], ref iniposi_lh[0]);
            Convert_m(iniposi_b, ref initialPosition_body);
            Convert_m(iniposi_rh, ref initialPosition_rHand);
            Convert_m(iniposi_lh, ref initialPosition_lHand);
            return isOk;
        }


        /// <summary>
        /// 
        /// </summary>
        public void PAGetRotationOffset(ref Vector3 euler_xyz_out)
        {
            float[] euler = new float[3] { 0, 0, 0 };
            VDDataReadPlugin.PAGetRotationOffset(DstAddr.actorId, ref euler[0]);
            Convert_m(euler, ref euler_xyz_out);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetRotationOffset(Vector3 euler_xyz_RotationOrderYZX)
        {
            VDDataReadPlugin.PASetRotationOffset(DstAddr.actorId, 
                new float[3] { euler_xyz_RotationOrderYZX.x, euler_xyz_RotationOrderYZX.y, euler_xyz_RotationOrderYZX.z });
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PAIsLimitMotionInCircle()
        {
            return VDDataReadPlugin.PAIsLimitMotionInCircle(DstAddr.actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PALimitMotionInCircle(bool isLimit)
        {
            VDDataReadPlugin.PALimitMotionInCircle(DstAddr.actorId, isLimit);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetMotionCircleRadius(float radius)
        {
            VDDataReadPlugin.PASetMotionCircleRadius(DstAddr.actorId, radius);
        }

        /// <summary>
        /// 
        /// </summary>
        public float PAGetMotionCircleRadius()
        {
            return VDDataReadPlugin.PAGetMotionCircleRadius(DstAddr.actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool PAIsLockPositionXY()
        {
            return VDDataReadPlugin.PAIsLockPositionXY(DstAddr.actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PALockPositionXY(bool isLock)
        {
            VDDataReadPlugin.PALockPositionXY(DstAddr.actorId, isLock);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetCurrentPositionAsOrigin()
        {
            VDDataReadPlugin.PASetCurrentPositionAsOrigin(DstAddr.actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetPositionToOrigin()
        {
            VDDataReadPlugin.PASetPositionToOrigin(DstAddr.actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetPositionToZero()
        {
            VDDataReadPlugin.PASetPositionToZero(DstAddr.actorId);
        }

        /// <summary>
        /// settedAxis 表示要设置的坐标轴，0-X, 1-Y, 2-Z, 3-XYZ.
        /// </summary>
        public void PASetPosition(Vector3 actorPosition, int settedAxis)
        {
            VDDataReadPlugin.PASetPosition(DstAddr.actorId, new float[3] { actorPosition.x, actorPosition.y, actorPosition.z }, settedAxis);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PASetPositionOffset(Vector3 positionOffset)
        {
            VDDataReadPlugin.PASetPositionOffset(DstAddr.actorId, new float[3] { positionOffset.x, positionOffset.y, positionOffset.z });
        }

        /// <summary>
        /// 
        /// </summary>
        public void PAGetPositionOffset(ref Vector3 positionOffset)
        {
            float[] posiOffset = new float[3] { 0, 0, 0 };
            VDDataReadPlugin.PAGetPositionOffset(DstAddr.actorId, ref posiOffset[0]);
            Convert_m(posiOffset, ref positionOffset);
        }
        #endregion


        #region private functions
        /// <summary>
        /// 
        /// </summary>
        void StartRecvMocapDataThread()
        {
            if (!m_IsRecvThreadRunning)
            {
                m_IsRecvThreadRunning = true;
                Thread recvThread = new Thread(RecvMocapDataOn);
                recvThread.IsBackground = true;
                recvThread.Start();
            }
        }

        /// <summary>
        /// 单个 index，最高限制尚未测试
        /// </summary>
        void RecvMocapDataOn()
        {
            int countBreak = 0;
            const int sleepMilliSeconds = 10;
            const int countBreakMax = 1000 / sleepMilliSeconds;
            bool m_IsHasRecv = false;
            bool m_IsInitial_md_ = false;
            _MocapData_ _md_ = new _MocapData_(0);
            _MocapData_ _md_pq_last = new _MocapData_(0);
            MocapData md = new MocapData();
            MocapData md_adjusted = new MocapData();
            VDDataReadPlugin.PAAdd(DstAddr.actorId, _WorldSpace_.WS_Unity);
            //
            while (m_IsRecvThreadRunning)
            {
                Thread.Sleep(sleepMilliSeconds);

                //
                if (m_IsConnect)
                {
                    m_IsHasRecv = true;
                    if (VDDataReadPlugin.UdpRecvMocapData(0, DstAddr.ip, DstAddr.port, ref _md_))
                    {
                        countBreak = 0;
                        if (_md_.isUpdate)
                        {
                            if (!m_IsInitial_md_) { m_IsInitial_md_ = true; }
                            Convert_m(_md_, ref md);
                            Clone(ref _md_pq_last.position_body, _md_.position_body);
                            Clone(ref _md_pq_last.position_rHand, _md_.position_rHand);
                            Clone(ref _md_pq_last.position_lHand, _md_.position_lHand);
                            Clone(ref _md_pq_last.quaternion_body, _md_.quaternion_body);
                            Clone(ref _md_pq_last.quaternion_rHand, _md_.quaternion_rHand);
                            Clone(ref _md_pq_last.quaternion_lHand, _md_.quaternion_lHand);
                        }
                    }
                    else
                    {
                        if (countBreak < countBreakMax) { countBreak++; }
                        else { m_IsConnect = false; }
                    }
                }
                else if (m_IsHasRecv)
                {
                    m_IsHasRecv = false;
                    VDDataReadPlugin.UdpRemove(0, DstAddr.ip, DstAddr.port);
                    DicDstAddr_Remove(DstAddr.ip, DstAddr.port);
                }
                
                //
                if (m_pq_initial.isUpdate || m_IsInitial_md_)
                {
                    if (!m_IsInitial_md_)
                    {
                        m_IsInitial_md_ = true;
                        for (int i = 0; i < NODES_BODY; i++)
                        {
                            for (int ii = 0; ii < 3; ii++)
                            {
                                _md_pq_last.position_body[i * 3 + ii] = m_pq_initial.position_body[i * 3 + ii];
                                md.position_body[i][ii] = m_pq_initial.position_body[i * 3 + ii];
                            }
                        }
                        for (int i = 0; i < NODES_HAND; i++)
                        {
                            for (int ii = 0; ii < 3; ii++)
                            {
                                _md_pq_last.position_rHand[i * 3 + ii] = m_pq_initial.position_rHand[i * 3 + ii];
                                md.position_rHand[i][ii] = m_pq_initial.position_rHand[i * 3 + ii];
                                _md_pq_last.position_lHand[i * 3 + ii] = m_pq_initial.position_lHand[i * 3 + ii];
                                md.position_lHand[i][ii] = m_pq_initial.position_lHand[i * 3 + ii];
                            }
                        }
                    }
                    if (!_md_.isUpdate)
                    {
                        Clone(ref _md_.position_body, _md_pq_last.position_body);
                        Clone(ref _md_.position_rHand, _md_pq_last.position_rHand);
                        Clone(ref _md_.position_lHand, _md_pq_last.position_lHand);
                        Clone(ref _md_.quaternion_body, _md_pq_last.quaternion_body);
                        Clone(ref _md_.quaternion_rHand, _md_pq_last.quaternion_rHand);
                        Clone(ref _md_.quaternion_lHand, _md_pq_last.quaternion_lHand);
                    }
                    //PoseAdjust
                    VDDataReadPlugin.PAOnAdjustMotionData(DstAddr.actorId, ref _md_.quaternion_body[0], ref _md_.position_body[0],
                             ref _md_.quaternion_rHand[0], ref _md_.position_rHand[0],
                             ref _md_.quaternion_lHand[0], ref _md_.position_lHand[0]);
                    //
                    Convert_m(_md_, ref md_adjusted);
                    _md_.isUpdate = false;
                    if (!md_adjusted.isUpdate) { md_adjusted.isUpdate = true; }
                    //
                    if (MocapDataReceived != null) { MocapDataReceived(m_IsConnect, md, md_adjusted); }
                }
            }
            VDDataReadPlugin.UdpRemove(0, DstAddr.ip, DstAddr.port);
            VDDataReadPlugin.PARemove(DstAddr.actorId);
            DicDstAddr_Remove(DstAddr.ip, DstAddr.port);
            m_IsRecvThreadRunning = false;
            return;
        }


        #region for DicDstAddr
        //
        string DicDstAddr_CreateKey(string dst_ip, ushort dst_port)
        {
            return dst_ip + ":" + dst_port.ToString();
        }
        //
        bool DicDstAddr_IsContains(string dst_ip, ushort dst_port)
        {
            return m_DicDstAddr.ContainsKey(DicDstAddr_CreateKey(dst_ip, dst_port));
        }
        //
        void DicDstAddr_Add(string dst_ip, ushort dst_port)
        {
            string key = DicDstAddr_CreateKey(dst_ip, dst_port);
            if (!m_DicDstAddr.ContainsKey(key))
            {
                m_DicDstAddr[key] = new Addr(ActorId, dst_ip, dst_port);
            }
        }
        //
        void DicDstAddr_Remove(string dst_ip, ushort dst_port)
        {
            string key = DicDstAddr_CreateKey(dst_ip, dst_port);
            if (m_DicDstAddr.ContainsKey(key))
            {
                m_DicDstAddr.Remove(key);
            }
        }
        //
        Addr DicDstAddr_Value(string dst_ip, ushort dst_port)
        {
            string key = DicDstAddr_CreateKey(dst_ip, dst_port);
            if (m_DicDstAddr.ContainsKey(key))
            {
                return m_DicDstAddr[key];
            }
            return new Addr(ActorId, "", 0);
        }
        #endregion
        #endregion


        #region sub function
        //
        static void Clone(ref float[] out_, float[] in_)
        {
            int count = in_.Length;
            for (int i = 0; i < count; i++) 
            {
                out_[i] = in_[i];
            }
        }
        //
        static void Convert_m(_MocapData_ _md_, ref MocapData md)
        {
            md.isUpdate = _md_.isUpdate;
            md.frameIndex = _md_.frameIndex;
            md.frequency = _md_.frequency;
            //body
            Convert_m(_md_.position_body, ref md.position_body);
            Convert_m(_md_.quaternion_body, ref md.quaternion_body);
            Convert_m(_md_.velocity_body, ref md.velocity_body);
            //rhand
            Convert_m(_md_.position_rHand, ref md.position_rHand);
            Convert_m(_md_.quaternion_rHand, ref md.quaternion_rHand);
            Convert_m(_md_.velocity_rHand, ref md.velocity_rHand);
            //lhand
            Convert_m(_md_.position_lHand, ref md.position_lHand);
            Convert_m(_md_.quaternion_lHand, ref md.quaternion_lHand);
            Convert_m(_md_.velocity_lHand, ref md.velocity_lHand);
            //face
            md.isUseFaceBlendShapesARKit = _md_.isUseFaceBlendShapesARKit;
            for (int i = 0; i < NODES_FACEBS_ARKIT; i++)
            {
                md.faceBlendShapesARKit[i] = _md_.faceBlendShapesARKit[i];
            }
        }
        //
        static float[] Convert_m(Vector3[] in_)
        {
            int count = in_.Length;
            float[] out_ = new float[count * 3];
            for (int i = 0; i < count; i++)
            {
                for (int ii = 0; ii < 3; ii++) { out_[i * 3 + ii] = in_[i][ii]; }
            }
            return out_;
        }
        //
        static void Convert_m(Vector3[] in_, ref float[] out_)
        {
            int count = in_.Length;
            for (int i = 0; i < count; i++)
            {
                for (int ii = 0; ii < 3; ii++) { out_[i * 3 + ii] = in_[i][ii]; }
            }
        }
        //
        static void Convert_m(float[] in_, ref Vector3[] out_)
        {
            int count = in_.Length / 3;
            for (int i = 0; i < count; i++)
            {
                for (int ii = 0; ii < 3; ii++)
                {
                    out_[i][ii] = in_[i * 3 + ii];
                }
            }
        }
        //
        static void Convert_m(float[] in_, ref Vector3 out_)
        {
            for (int i = 0; i < 3; i++)
            {
                out_[i] = in_[i];
            }
        }
        //
        static void Convert_m(Quaternion[] in_, ref float[] out_)
        {
            int count = in_.Length;
            for (int i = 0; i < count; i++)
            {
                out_[i * 4] = in_[i].w;
                out_[i * 4 + 1] = in_[i].x;
                out_[i * 4 + 2] = in_[i].y;
                out_[i * 4 + 3] = in_[i].z;
            }
        }
        //
        static void Convert_m(float[] in_, ref Quaternion[] out_)
        {
            int count = in_.Length / 4;
            for (int i = 0; i < count; i++)
            {
                out_[i].w = in_[i * 4];
                out_[i].x = in_[i * 4 + 1];
                out_[i].y = in_[i * 4 + 2];
                out_[i].z = in_[i * 4 + 3];
            }
        }
        #endregion

    }//end class

}//end namespace


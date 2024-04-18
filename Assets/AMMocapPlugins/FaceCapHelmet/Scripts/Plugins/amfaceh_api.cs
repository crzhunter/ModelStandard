using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace amfaceh
{
    public class amfaceh_api
    {
        public enum OpenResult
        {
            dongle_notConnect = -2,
            error = -1,
            success = 0,
            camera_occupied = 1,
            uninitialized = 2,
            camera_notExist = 3,
        };

        public enum AMFaceBS52
        {
            AMFBS_BrowDownLeft = 0,
            AMFBS_BrowDownRight,
            AMFBS_BrowInnerUp,
            AMFBS_BrowOuterUpLeft,
            AMFBS_BrowOuterUpRight,
            AMFBS_CheekPuff,
            AMFBS_CheekSquintLeft,
            AMFBS_CheekSquintRight,
            AMFBS_EyeBlinkLeft,
            AMFBS_EyeBlinkRight,
            AMFBS_EyeLookDownLeft,
            AMFBS_EyeLookDownRight,
            AMFBS_EyeLookInLeft,
            AMFBS_EyeLookInRight,
            AMFBS_EyeLookOutLeft,
            AMFBS_EyeLookOutRight,
            AMFBS_EyeLookUpLeft,
            AMFBS_EyeLookUpRight,
            AMFBS_EyeSquintLeft,
            AMFBS_EyeSquintRight,
            AMFBS_EyeWideLeft,
            AMFBS_EyeWideRight,
            AMFBS_JawForward,
            AMFBS_JawLeft,
            AMFBS_JawOpen,
            AMFBS_JawRight,
            AMFBS_MouthClose,
            AMFBS_MouthDimpleLeft,
            AMFBS_MouthDimpleRight,
            AMFBS_MouthFrownLeft,
            AMFBS_MouthFrownRight,
            AMFBS_MouthFunnel,
            AMFBS_MouthLeft,
            AMFBS_MouthLowerDownLeft,
            AMFBS_MouthLowerDownRight,
            AMFBS_MouthPressLeft,
            AMFBS_MouthPressRight,
            AMFBS_MouthPucker,
            AMFBS_MouthRight,
            AMFBS_MouthRollLower,
            AMFBS_MouthRollUpper,
            AMFBS_MouthShrugLower,
            AMFBS_MouthShrugUpper,
            AMFBS_MouthSmileLeft,
            AMFBS_MouthSmileRight,
            AMFBS_MouthStretchLeft,
            AMFBS_MouthStretchRight,
            AMFBS_MouthUpperUpLeft,
            AMFBS_MouthUpperUpRight,
            AMFBS_NoseSneerLeft,
            AMFBS_NoseSneerRight,
            AMFBS_TongueOut,

            AMFBS_COUNT,
        };

        public enum AMCamSettings
        {
            AMC_640_480_30 = 0,
            AMC_640_480_60,
            AMC_1280_720_30,
            AMC_1280_720_60,
            AMC_1920_1080_30,
            AMC_1920_1080_60,

            AMC_COUNT,
        };

        public struct AMFaceOpenInfo
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool detectedFace;
            //脸部表情bs
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52, ArraySubType = UnmanagedType.R4)]
            public float[] face_bs;
            //
            [MarshalAs(UnmanagedType.I1)]
            public bool is_update;


            public AMFaceOpenInfo(int null_)
            {
                detectedFace = false;
                face_bs = new float[52];
                for (int i = 0; i < 52; i++)
                {
                    face_bs[i] = 0;
                }
                is_update = false;
            }

            public AMFaceOpenInfo Clone()
            {
                AMFaceOpenInfo info = new AMFaceOpenInfo(0);
                info.detectedFace = this.detectedFace;
                for (int i = 0; i < 52; i++)
                {
                    info.face_bs[i] = this.face_bs[i];
                }
                info.is_update = this.is_update;
                return info;
            }
        };

        //
        //public delegate void FaceOpenDataReceivedCallBack([MarshalAs(UnmanagedType.LPArray, SizeConst = 52, ArraySubType = UnmanagedType.R4)] float[] face_bs, 
        //    IntPtr img, int img_width, int img_height, int img_channel);
        public delegate void FaceImgDataReceivedCallBack(IntPtr img, int img_width, int img_height, int img_channel);
        public delegate void FaceOpenDataReceivedCallBack(ref AMFaceOpenInfo faceInfo);


        const string DLL_NAME = "amfaceh.dll";
        public static bool m_isInitial = false;
        static bool m_isRecvImg = false;
        static bool m_isRecvData = false;
        static AMFaceOpenInfo m_faceInfo = new AMFaceOpenInfo();
        static Color32[] m_imgColor32 = null;
        static byte[] m_imgBytesRGB = null;
        static int m_imgWidth = 640;
        static int m_imgHeight = 480;
        static int m_imgChannel = 3;
        static object m_lockImg = new object();
        static object m_lockData = new object();
        public static bool IsInitial { get { return m_isInitial; } }
        public static bool IsOpenCapture { get; set; } = false;

        /*
        说明：初始化函数
        */
        [DllImport(DLL_NAME)]
        extern static void Init(string pwd);

        /**
         * @brief Get the Cameras Count object
         * 
         * @return int >=0:相机数，-1:失败 
         */
        [DllImport(DLL_NAME)]
        extern static int GetCamerasCount();

        /**
         * @brief Get the Cameras Count object
         * 
         * @return int >=0:相机数，-1:失败 
         */
        [DllImport(DLL_NAME)]
        extern static int GetCameras(ref byte cameras, ref int len);
        
        /**
        * @brief
        *   Set a function address as a callback handle to receive mocap data.
        */
        [DllImport(DLL_NAME)]
        extern static void SetFaceImgDataCallBackFunc(FaceImgDataReceivedCallBack function);

        [DllImport(DLL_NAME)]
        extern static void SetFaceOpenDataCallBackFunc(FaceOpenDataReceivedCallBack function);

        /**
         * @brief 
         * Start Motion Captrue.
         * 会开启线程，数据通过回调输出。
         * @param cameraIndex 相机id号
         * @param width 设置捕获相机图像宽
         * @param height 设置捕获相机图像高
         * @param fps 设置捕获相机帧率
         * @param width_outImage 在回调中输出图像宽
         * @param height_outImage 在回调中输出图像高
         * @return int -2:未连接加密狗; -1: failed_error; 0: success; 1: failed_busy. 2: 未初始化. 3: 相机不存在.
         * @remark
         */
        [DllImport(DLL_NAME)]
        extern static int OpenFaceCap(int cameraIndex, AMCamSettings camSettings, int width_outImage);

        /**
        * @brief
        * @remark
        */
        [DllImport(DLL_NAME)]
        extern static void Close();

        /// <summary>
        /// [10,100]
        /// </summary>
        /// <param name="sensitivity">[10,100]</param>
        [DllImport(DLL_NAME)]
        public extern static void SetSensitivity(float sensitivity);

        [DllImport(DLL_NAME)]
        public extern static float GetSensitivity();

        /**
	     * @brief 必须在主线程调用
         * 若不设置 winIcon, 则 winIconPath = "".
         * flipMode: 0-不翻转，1-上下翻转.
         * pollEvents: true: 生成exe（无其它事件轮询）; false:生成dll在unity等调用（unity等本身有事件轮询）。
         * 注：在unity IL2CPP 编译下，pollEvents 必须设置为 false.
	     */
        [DllImport(DLL_NAME)]
        extern static bool CreateImgWindow(string winName, int initWinWidth, int initWinHeight, int initPosX, int initPosY, int flipMode, bool pollEvents = false);
        public static bool CreateImgWindow()
        {
            return CreateImgWindow("AMFaceCapH", 640, 360, 20, 20, 1, false);
        }

        /**
         * @brief 必须在主线程调用
         * 
         * @param imgData 
         * @param imgWidth
         * @param imgHeight 
         * @param imgChannel 
         */
        [DllImport(DLL_NAME)]
        public extern static void ImshowImg();

        /**
         * @brief 必须在主线程调用
         * 
         */
        [DllImport(DLL_NAME)]
        public extern static void CloseImgWindow();






        // untiy 若要用 il2cpp 进行编译的话，需要加上 "[AOT.MonoPInvokeCallback(typeof(MocapDataReceivedCallBack))]", 且函数 OnRecvMocapData() 必须是静态的。
        // 若不用 il2cpp 编译，则不需要加声明，也不需要静态。
        [AOT.MonoPInvokeCallback(typeof(FaceOpenDataReceivedCallBack))]
        private static void OnRecvFaceImgData(IntPtr img, int img_width, int img_height, int img_channel)
        {
            #region for image bytes
            if (m_imgBytesRGB == null)
            {
                m_imgBytesRGB = new byte[img_width * img_height * img_channel];
                m_imgColor32 = new Color32[img_width * img_height];
            }
            else if (m_imgBytesRGB.Length != img_width * img_height * img_channel)
            {
                m_imgBytesRGB = new byte[img_width * img_height * img_channel];
                m_imgColor32 = new Color32[img_width * img_height];
            }

            //
            Marshal.Copy(img, m_imgBytesRGB, 0, img_width * img_height * img_channel);
            #endregion


            lock (m_lockImg)
            {
                #region for image
                m_imgWidth = img_width;
                m_imgHeight = img_height;
                m_imgChannel = img_channel;
                if (img_channel == 3)
                {
                    //for (int i = 0; i < m_imgColor32.Length; i++)
                    int nn = 0;
                    for (int i = m_imgColor32.Length - 1; i >= 0; i--)
                    {
                        m_imgColor32[nn++] = new Color32(m_imgBytesRGB[3 * i], m_imgBytesRGB[3 * i + 1], m_imgBytesRGB[3 * i + 2], 255);
                    }
                }
                else if (img_channel == 4)
                {
                    for (int i = 0; i < m_imgColor32.Length; i++)
                    {
                        m_imgColor32[i] = new Color32(m_imgBytesRGB[4 * i], m_imgBytesRGB[4 * i + 1], m_imgBytesRGB[4 * i + 2], m_imgBytesRGB[4 * i + 3]);
                    }
                }
                #endregion
            }//end lock

            //
            m_isRecvImg = true;
        }

        // untiy 若要用 il2cpp 进行编译的话，需要加上 "[AOT.MonoPInvokeCallback(typeof(MocapDataReceivedCallBack))]", 且函数 OnRecvMocapData() 必须是静态的。
        // 若不用 il2cpp 编译，则不需要加声明，也不需要静态。
        [AOT.MonoPInvokeCallback(typeof(FaceOpenDataReceivedCallBack))]
        private static void OnRecvFaceOpenData(ref AMFaceOpenInfo faceInfo)
        {            
            lock (m_lockData)
            {
                m_faceInfo = faceInfo.Clone();
            }//end lock

            //
            m_isRecvData = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool GetFaceImg(ref Texture2D tex2D)
        {
            if (m_isRecvImg && IsOpenCapture)
            {
                m_isRecvImg = false;
                //
                lock (m_lockImg)
                {
                    #region for image
                    if (tex2D == null || tex2D.width != m_imgWidth || tex2D.height != m_imgHeight)
                    {
                        tex2D = new Texture2D(m_imgWidth, m_imgHeight);
                    }
                    tex2D.SetPixels32(m_imgColor32);
                    tex2D.Apply();
                    #endregion
                }//end lock
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool GetFaceData(ref AMFaceOpenInfo faceInfo)
        {
            if (m_isRecvData && IsOpenCapture)
            {
                m_isRecvData = false;
                //
                lock (m_lockData)
                {
                    faceInfo = m_faceInfo.Clone();
                    faceInfo.is_update = true;
                }//end lock
                return true;
            }
            faceInfo.is_update = false;
            return false;
        }

        /// <summary>
        /// 加载时间较长，可在线程中执行。
        /// </summary>
        public static void Initial()
        {
            if (!m_isInitial)
            {
                Init("virdyn_am");
                SetFaceImgDataCallBackFunc(OnRecvFaceImgData);
                SetFaceOpenDataCallBackFunc(OnRecvFaceOpenData);

                m_isInitial = true;

                Debug.Log("AMFaceCapH Initial End.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameras"></param>
        /// <returns></returns>
        public static int GetCameras(out Dictionary<int, string> dic_cameras)
        {
            dic_cameras = new Dictionary<int, string>();
            byte[] cameras_bytes_ = new byte[10240];
            int bytes_len = 10240;
            int count = GetCameras(ref cameras_bytes_[0], ref bytes_len);
            byte[] cameras_bytes = new byte[bytes_len];
            for (int i = 0; i < bytes_len; i++)
            {
                cameras_bytes[i] = cameras_bytes_[i];
            }
            if (count > 0)
            {
                string cameras_string = Bytes2String_FromC(cameras_bytes);
                cameras_string = HexUnicodeToStr(cameras_string);
                //Debug.Log("cameras count = " + count + "；  cameras: " + cameras_string);
                LitJson.JsonData camerasList = LitJson.JsonMapper.ToObject(cameras_string);
                count = camerasList.Count;
                for (int i = 0; i < count; i++)
                {
                    dic_cameras[(int)(camerasList[i]["camera_index"])] = camerasList[i]["camera_name"].ToString();
                }
            }
            return count;
        }


        public static OpenResult OpenFaceCap(int cameraIndex, AMCamSettings camSettings)
        {
            if (!m_isInitial) { return OpenResult.uninitialized; }
            OpenResult result = (OpenResult)(OpenFaceCap(cameraIndex, camSettings, 180));
            Debug.Log("StartFaceCaptrue Result = " + result);
            if (OpenResult.success == result)
            {
                IsOpenCapture = true;
                //OpenWrite();
            }
            return result;
        }


        public static void CloseFaceCap()
        {
            if (IsOpenCapture)
            {
                Close();
                IsOpenCapture = false;
                //CloseWrite();
            }
        }





        #region sub functions
        /// <summary>
        /// string转bytes，用与C接口const char*，char* 字符输入。
        /// 注意项目Plugins中要包含"I18N.CJK.dll"和"I18N.dll"这两个C#动态链接库。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string Bytes2String_FromC(byte[] bytes)
        {
            return System.Text.Encoding.GetEncoding("gb2312").GetString(bytes);
        }
        /// <summary>
        /// 如："now\u661f\u671f\u4e09" -> "now星期三"
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        static string HexUnicodeToStr(string strDecode)
        {
            string[] str_split;
            str_split = strDecode.Split(new string[1] { "\\u" }, StringSplitOptions.None); //.Split('\\');
            if (str_split.Length > 0)
            {
                for (int i = 1; i < str_split.Length; i++)
                {
                    if (str_split[i].Length >= 4)
                    {
                        string str = str_split[i].Substring(0, 4);
                        try
                        {
                            string str_i = (char)int.Parse(str, System.Globalization.NumberStyles.HexNumber) + "";
                            strDecode = strDecode.Replace("\\u" + str, str_i);
                        }
                        catch { }
                    }
                }
            }
            return strDecode;
        }
        #endregion






        #region write data to test (can delete)
//#if UNITY_EDITOR
//        static System.IO.StreamWriter m_sw = null;
//#endif
//        const string m_bs_path = "C:/Users/Administrator/Desktop/FaceCapHelmetData/bs52.txt";

//        static void OpenWrite(string path = m_bs_path)
//        {
//#if UNITY_EDITOR
//            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(m_bs_path)))
//            {
//                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(m_bs_path));
//            }
//            if (m_sw != null)
//            {
//                CloseWrite();
//            }
//            m_sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
//#endif
//        }
//        static void Write(string line)
//        {
//#if UNITY_EDITOR
//            m_sw.WriteLine(line);
//#endif
//        }
//        static void CloseWrite()
//        {
//#if UNITY_EDITOR            
//            m_sw.Flush();
//            m_sw.Close();
//            m_sw = null;
//#endif
//        }
//        static void WriteFile(string filePath, string content)
//        {
//#if UNITY_EDITOR
//            // 也可以指定编码方式, true 是 Appendtext, false 为覆盖原文件 
//            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.Default);
//            sw.WriteLine(content);
//            sw.Flush();
//            sw.Close();
//#endif
//        }


        #endregion 

    }//end class

}//end namespace
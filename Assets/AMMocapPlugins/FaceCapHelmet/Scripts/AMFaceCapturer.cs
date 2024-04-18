using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace amfaceh
{
    public class AMFaceCapturer
    {
        static amfaceh_api.AMFaceOpenInfo m_faceData = new amfaceh_api.AMFaceOpenInfo(0);
        static Dictionary<int, string> m_DicCameras = new Dictionary<int, string>();
        static FaceBsSetter m_FaceBsSetter = new FaceBsSetter();
        static bool m_isOpenCameraImgWindow = false;

        /// <summary>
        /// 需要先设置
        /// </summary>
        public static amfaceh_api.AMFaceOpenInfo FaceData { get { return m_faceData; } }

        public static bool IsOpen { get { return amfaceh_api.IsOpenCapture; } }

        public static bool IsInitial { get { return amfaceh_api.IsInitial; } }

        public static bool CameraImgWindowOpenState { get; set; } = false;

        public static float Sensitivity { get { return amfaceh_api.GetSensitivity(); } set { amfaceh_api.SetSensitivity(value); } }


        



        #region public functions
        /// <summary>
        /// run in Start.
        /// </summary>
        public static void Initial(GameObject humanModel)
        {
            if (humanModel != null)
            {
                m_FaceBsSetter.Initial(humanModel);
            }            
            Initial_AMFaceH();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraName"></param>
        /// <param name="fps">30fps, 60fps</param>
        /// <returns></returns>
        public static amfaceh_api.OpenResult OpenFaceCap(int cameraID, amfaceh_api.AMCamSettings camSettings)
        {
            GetDCamerasName();
            if (m_DicCameras.ContainsKey(cameraID))
            {
                return amfaceh_api.OpenFaceCap(cameraID, camSettings);
            }
            return amfaceh_api.OpenResult.camera_notExist;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CloseFaceCap()
        {
            CloseCameraImgWindow();
            amfaceh_api.CloseFaceCap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDeviceCamerasName()
        {
            List<string> listCamerasName = new List<string>();
            GetDCamerasName();
            for (int i = 0; i < m_DicCameras.Count; i++)
            {
                listCamerasName.Add(m_DicCameras[i]);
            }
            return listCamerasName;
        }

        


        //
        #region run in Update()
        /// <summary>
        /// 更新面捕数据
        /// </summary>
        public static void Update_FaceData()
        {
            if (!amfaceh_api.IsOpenCapture) { return; }

            amfaceh_api.GetFaceData(ref m_faceData);

            //
            if (CameraImgWindowOpenState)
            {
                OpenCameraImgWindow();
                amfaceh_api.ImshowImg();
            }
            else
            {
                CloseCameraImgWindow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex2D"></param>
        /// <returns></returns>
        public static bool Update_DCameraImg(ref Texture2D tex2D)
        {
            return amfaceh_api.GetFaceImg(ref tex2D);
        }
        #endregion

        #region run in LateUpdate()
        /// <summary>
        /// 驱动模型表情
        /// </summary>
        public static void LateUpdate_FaceBS()
        {
            if (IsOpen && m_faceData.is_update)
            {
                m_FaceBsSetter.SetFaceBS(m_faceData.face_bs);
            }
        }
        #endregion

        #endregion







        #region private functions       
        /// <summary>
        /// 
        /// </summary>
        static void GetDCamerasName()
        {
            Dictionary<int, string> dicCameras;
            int len = amfaceh_api.GetCameras(out dicCameras);
            //
            m_DicCameras.Clear();
            if (len > 0)
            {
                //可以在这里进行一些相机过滤
                foreach (KeyValuePair<int, string> kv in dicCameras)
                {
                    m_DicCameras[kv.Key] = kv.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void Initial_AMFaceH()
        {
            if (!amfaceh_api.IsInitial)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    amfaceh_api.Initial();
                }).Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static void OpenCameraImgWindow()
        {
            if (IsOpen && !m_isOpenCameraImgWindow)
            {
                m_isOpenCameraImgWindow = true;
                amfaceh_api.CreateImgWindow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static void CloseCameraImgWindow()
        {
            if (m_isOpenCameraImgWindow)
            {
                m_isOpenCameraImgWindow = false;
                amfaceh_api.CloseImgWindow();
            }
        }
        #endregion


    }//end class

}//end namespace
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace amfaceh
{
    public class AMFaceCapController : MonoBehaviour
    {
        static AMFaceCapController _instance;
        public static AMFaceCapController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (AMFaceCapController)GameObject.FindObjectOfType(typeof(AMFaceCapController));
                }
                return _instance;
            }
        }

        #region define
        //================ GameObject ================//
        [SerializeField]
        GameObject m_HumanModel;
        [SerializeField]
        Dropdown m_DropdownDCameras;
        [SerializeField]
        Dropdown m_DropdownCamSettings;
        [SerializeField]
        Button m_BtnOpen;
        [SerializeField]
        Toggle m_ToggleShowCameraImgWindow;
        [SerializeField]
        Slider m_SliderSensitivity;
        [SerializeField]
        Text m_TextSensitivityValue;
        [SerializeField]
        RawImage m_RawImageCamera;
        [SerializeField]
        Text m_TextNote;
        //=================== 常量 ===================//
        //=================== 变量 ===================//
        //Vector2 m_RawImageCameraSize;
        Vector2 m_RawImageParentSize;
        List<string> m_CameraDevices;
        Texture2D m_dcameraTex2D = null;

        public bool IsOpen { get { return AMFaceCapturer.IsOpen; } }
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            Initial();
        }
        //
        void Update()
        {
            AMFaceCapturer.Update_FaceData();
            if (AMFaceCapturer.Update_DCameraImg(ref m_dcameraTex2D))
            {
                SetImageMaxInPanel(m_dcameraTex2D);
            }
        }
        void LateUpdate()
        {
            AMFaceCapturer.LateUpdate_FaceBS();
        }
        //
        void OnApplicationQuit()
        {
            AMFaceCapturer.CloseFaceCap();
        }
        private void OnDestroy()
        {
            if (m_dcameraTex2D != null) 
            {
                Destroy(m_dcameraTex2D);
            }
        }






        #region Initial
        //
        void Initial()
        {
            FindObj();
            InitialObj();
            AddComponent();
            AddEvent();
            InitialSome();
        }
        void FindObj()
        {
        }
        void InitialObj()
        {
            SetBtnImage(m_BtnOpen, false);
            ActiveBtnNotePanel(m_BtnOpen.gameObject, false);
            m_ToggleShowCameraImgWindow.isOn = false;
            AMFaceCapturer.CameraImgWindowOpenState = false;
            RefreshCameraDevices();
            SetDropdownShowText(m_DropdownDCameras, "VIRDYN AH");
            List<string> list_cam_fps = new List<string>();
            for (int i = 0; i < (int)amfaceh_api.AMCamSettings.AMC_COUNT; i++)
            {
                list_cam_fps.Add(((amfaceh_api.AMCamSettings)i).ToString().Replace("AMC_", "").Replace("_", "*").Replace("*30", " 30fps").Replace("*60", " 60fps"));
            }
            SetDropdownContent(m_DropdownCamSettings, list_cam_fps);
            m_DropdownCamSettings.value = 3;
            //Slider_Sensitivity
            m_SliderSensitivity.minValue = 10;
            m_SliderSensitivity.maxValue = 100;
            m_SliderSensitivity.wholeNumbers = true;           
        }
        void AddComponent()
        {     
        }
        void AddEvent()
        {
            m_BtnOpen.onClick.AddListener(OnOpenCloseCaptrue);
            m_ToggleShowCameraImgWindow.onValueChanged.AddListener(OnShowCameraImgWindow);
            m_SliderSensitivity.onValueChanged.AddListener(OnSensitivityValueChanged);
            //
            amfaceh.EventTriggerListener et = amfaceh.EventTriggerListener.Get(m_DropdownDCameras.gameObject);
            et.onDown = OnRefreshCameraDevices;
            m_DropdownDCameras.onValueChanged.AddListener(OnDCameraChanged);
        }
        void InitialSome()
        {
            AMFaceCapturer.Initial(m_HumanModel);
            if (!AMFaceCapturer.IsInitial) { m_TextNote.text = "初始化中..."; StartCoroutine(NoteInitialOn()); }
            //显示默认灵敏度值
            m_SliderSensitivity.value = AMFaceCapturer.Sensitivity;
            m_TextSensitivityValue.text = AMFaceCapturer.Sensitivity.ToString();
            //
            m_RawImageParentSize = m_RawImageCamera.transform.parent.GetComponent<RectTransform>().rect.size;
        }
        IEnumerator NoteInitialOn()
        {
            while (!AMFaceCapturer.IsInitial)
            {
                yield return new WaitForSeconds(0.1f);
            }
            m_TextNote.text = "初始化完成!";
            yield return new WaitForSeconds(1.5f);
            m_TextNote.text = "";
        }
        #endregion




        #region event functions
        //
        void OnRefreshCameraDevices(GameObject obj)
        {
            RefreshCameraDevices();
        }

        void OnDCameraChanged(int value)
        {
            if (IsOpen)
            {
                CloseCapture();
                StartCoroutine(SelectCameraOn(true));
            }
        }
        IEnumerator SelectCameraOn(bool needWait)
        {
            if (needWait) { yield return new WaitForSeconds(0.3f); }
            OpenCapture();
        }

        void OnOpenCloseCaptrue()
        {
            //开启
            if (!IsOpen)
            {
                OpenCapture();
            }
            //关闭
            else
            {
                CloseCapture();
            }
            //更新按钮状态
            SetBtnImage(m_BtnOpen, IsOpen);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnShowCameraImgWindow(bool value)
        {
            AMFaceCapturer.CameraImgWindowOpenState = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void OnSensitivityValueChanged(float value)
        {
            AMFaceCapturer.Sensitivity = value;
            m_TextSensitivityValue.text = value.ToString();
        }
        #endregion





        #region private functions
        /// <summary>
        /// 使图片在窗口内最大化显示。
        /// panel: PanelRectController.Transform.
        /// </summary>
        void SetImageMaxInPanel(Texture2D tex2D)
        {
            Vector2 scale = GetScale_InPanel(new Vector2(tex2D.width, tex2D.height), m_RawImageParentSize);
            m_RawImageCamera.transform.localScale = new Vector3(scale.x, scale.y, 1);
            m_RawImageCamera.texture = tex2D;
        }

        /// <summary>
        /// 保证picture不被拉伸压缩，只是改变显示区域。
        /// 得到的Scale可以使picture所有区域都在Panel内部。
        /// </summary>
        /// <returns></returns>
        Vector2 GetScale_InPanel(Vector2 pictureSize, Vector2 panelSize)
        {
            Vector2 lclScale = new Vector2(1, 1);
            if (pictureSize.x >= 1 && pictureSize.y >= 1
                && panelSize.x >= 1 && panelSize.y >= 1)
            {
                float w2h = pictureSize.x / ((float)pictureSize.y);
                float panel_w2h = panelSize.x / ((float)panelSize.y);
                if (w2h > panel_w2h)
                {
                    lclScale.y = panel_w2h / w2h;
                }
                else
                {
                    lclScale.x = w2h / panel_w2h;
                }
            }
            return lclScale;
        }

        /// <summary>
        /// 
        /// </summary>
        void SetBtnImage(Button btn, bool isClicked)
        {
            SetActive(btn.transform.Find("Img_After"), isClicked);
            SetActive(btn.transform.Find("Img_Before"), !isClicked);
        }
        bool IsClicked(Button btn)
        {
            return IsActive(btn.transform.Find("Img_After"));
        }
        void ActiveBtnNotePanel(GameObject btn, bool isActive)
        {
            SetActive(btn.transform.Find("Panel_Note"), isActive);
        }
        void SetBtnNotePanelText(GameObject btn, string text)
        {
            btn.transform.Find("Panel_Note/Text").GetComponent<Text>().text = text;
        }

        /// <summary>
        /// 刷新摄像头设备显示
        /// </summary>
        void RefreshCameraDevices()
        {
            if (m_DropdownDCameras != null)
            {
                List<string> devices = AMFaceCapturer.GetDeviceCamerasName();
                if (devices == null || devices.Count == 0)
                {
                    m_CameraDevices = null;
                    m_DropdownDCameras.ClearOptions();
                }
                else if (!IsTheSame(m_CameraDevices, devices))
                {
                    Copy(out m_CameraDevices, devices);
                    string lastDropdownText = m_DropdownDCameras.transform.Find("Label").GetComponent<Text>().text;
                    SetDropdownContent(m_DropdownDCameras, m_CameraDevices);
                    SetDropdownShowText(m_DropdownDCameras, lastDropdownText);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OpenCapture()
        {
            if (!IsOpen)
            {
                bool noRefreshCameraDeviceDropdown = false;
                List<string> cameraDevices = AMFaceCapturer.GetDeviceCamerasName();
                if (m_CameraDevices != null && cameraDevices != null && cameraDevices.Count > m_DropdownDCameras.value)
                {
                    if (m_CameraDevices[m_DropdownDCameras.value] == cameraDevices[m_DropdownDCameras.value])
                    {
                        noRefreshCameraDeviceDropdown = true;
                        amfaceh_api.OpenResult result = AMFaceCapturer.OpenFaceCap(m_DropdownDCameras.value, (amfaceh_api.AMCamSettings)(m_DropdownCamSettings.value));
                        StartCoroutine(ShowOpenCaptureResult(result));
                    }
                }
                //
                if (!noRefreshCameraDeviceDropdown)
                {
                    RefreshCameraDevices();
                }
            }
        }
        IEnumerator ShowOpenCaptureResult(amfaceh_api.OpenResult result)
        {
            switch(result)
            {
                case amfaceh_api.OpenResult.dongle_notConnect:
                    {
                        m_TextNote.text = "未连接加密狗!";
                    }
                    break;
                case amfaceh_api.OpenResult.error:
                    {
                        m_TextNote.text = "打开失败!";
                    }
                    break;
                case amfaceh_api.OpenResult.success:
                    {
                        m_TextNote.text = "打开成功!";
                    }
                    break;
                case amfaceh_api.OpenResult.camera_occupied:
                    {
                        m_TextNote.text = "相机被占用!";
                    }
                    break;
                case amfaceh_api.OpenResult.uninitialized:
                    {
                        m_TextNote.text = "未初始化!";
                    }
                    break;
                case amfaceh_api.OpenResult.camera_notExist:
                    {
                        m_TextNote.text = "相机不存在!";
                    }
                    break;
            }
            yield return new WaitForSeconds(1.5f);
            m_TextNote.text = "";
        }

        /// <summary>
        /// 
        /// </summary>
        void CloseCapture()
        {
            AMFaceCapturer.CloseFaceCap();
        }
        #endregion



        #region sub functions
        /// <summary>
        /// 设置 Dropdown 中的所有内容。
        /// </summary>
        void SetDropdownContent(Dropdown dropdown, List<string> texts)
        {
            int length = texts.Count;
            if (length != 0)
            {
                dropdown.ClearOptions();
                List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

                for (int i = 0; i < length; i++)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = texts[i];
                    list.Add(data);
                }
                dropdown.AddOptions(list);
            }
            else
            {
                dropdown.ClearOptions();
            }
        }

        /// <summary>
        /// 显示 Dropdown 中存在的某个内容。
        /// </summary>
        void SetDropdownShowText(Dropdown dropdown, string text)
        {
            for (int c = 0; c < dropdown.options.Count; c++)
            {
                if (dropdown.options[c].text == text)
                {
                    if (dropdown.value != c)
                        dropdown.value = c;
                    break;
                }
            }
        }
        //
        bool IsTheSame(List<string> a, List<string> b)
        {
            if (a == null || b == null) { return false; }
            int len_a = a.Count;
            int len_b = b.Count;
            if (len_a != len_b) { return false; }

            for (int i = 0; i < len_a; i++)
            {
                if (a[i] != b[i]) { return false; }
            }
            return true;
        }

        //
        void Copy(out List<string> str_dst, List<string> str_src)
        {
            int len = str_src.Count;
            str_dst = new List<string>();
            for (int i = 0; i < len; i++)
            {
                str_dst.Add(str_src[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void SetActive(Transform obj, bool isActive)
        {
            if (obj == null) { return; }
            if (obj.gameObject.activeSelf != isActive)
            {
                obj.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsActive(Transform obj)
        {
            if (obj == null) { return false; }
            return obj.gameObject.activeSelf;
        }
        #endregion

    }//end class
}//end namespace

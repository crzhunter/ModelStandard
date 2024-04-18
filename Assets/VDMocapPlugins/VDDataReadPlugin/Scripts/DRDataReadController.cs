//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace VDDataRead
{
    public class DRDataReadController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("VDDataReadController.")]
        Transform m_VDDataReadController;

        [SerializeField]
        [Tooltip("Camera")]
        Camera m_Camera;

        [SerializeField]
        [Tooltip("Humand Tpose Model.")]
        Transform m_TposeModel;

        [SerializeField]
        [Tooltip("Model Body Nodes Name.")]
        DRDataType.Name_BodyNodes m_BodyNodesName;

        [SerializeField]
        [Tooltip("Model RightHand Nodes Name.")]
        DRDataType.Name_RightHandNodes m_RightHandNodesName;

        [SerializeField]
        [Tooltip("Model LeftHand Nodes Name.")]
        DRDataType.Name_LeftHandNodes m_LeftHandNodesName;

        [SerializeField]
        [Tooltip("Face SkinnedMeshRenderer.")]
        SkinnedMeshRenderer[] m_FaceSkinnedMeshRenderer;

        [SerializeField]
        [Tooltip("Is Hide Model When Disconnected.")]
        bool m_IsHideModelWhenDisconnected = true;

        Text m_TextTitle;
        InputField m_InputFieldTitle;
        InputField m_InputFieldDstIp;
        InputField m_InputFieldDstPort;
        Button m_ButtonConnect;

        #region 
        VDDataReadClass m_DataRead = null;
        DRModelDriveControl m_ModelDrive = null;
        DRModelSign m_ModelSign = null;
        DRModelInfo m_ModelInfo = null;
        DRModelPoseAdjust m_ModelPoseAdjust = null;
        //
        Dictionary<string, string[][]> m_DicFaceBlendShapesMatchNameARKit = new Dictionary<string, string[][]>();
        Color m_ControllerInitialColor;
        delegate void ActorChoosedEventHandler(BaseEventData pd);
        public bool IsConnect { get; private set; }
        public int ActorId { get { return m_DataRead.ActorId; } }

        //
        static Color m_COLOR_ACTOR_CHOOSED = new Color(200f / 255, 1f, 200f / 255);
        static bool m_IS_CHANGE_ACTOR = false;
        static int m_LAST_CHOOSED_ACTORID = 0;
        public static int NOW_CHOOSED_ACTORID { get; private set; }
        #endregion

        // Use this for initialization
        void Start()
        {
            Initial();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateControllerState();
        }

        //
        void OnDestroy()
        {
            Disconnect();
        }

        //
        void OnApplicationQuit()
        {
            VDDataReadClass.UdpClose();
        }


        #region Initial
        //
        void Initial()
        {
            FindObj();
            AddComponent();
            InitialSome();
            AddEvent();
        }
        //
        void FindObj()
        {
            if (m_VDDataReadController == null) { m_VDDataReadController = this.transform; }
            if (m_Camera == null) { m_Camera = Camera.main; }
            m_TextTitle = m_VDDataReadController.Find("Panel_Title/Text_Title").GetComponent<Text>();
            m_InputFieldTitle = m_VDDataReadController.Find("Panel_Title/InputField_Title").GetComponent<InputField>();
            m_InputFieldDstIp = m_VDDataReadController.Find("InputField_DstIp").GetComponent<InputField>();
            m_InputFieldDstPort = m_VDDataReadController.Find("InputField_DstPort").GetComponent<InputField>();
            m_InputFieldDstPort.contentType = InputField.ContentType.IntegerNumber;
            m_InputFieldDstPort.characterLimit = 5;
            m_ButtonConnect = m_VDDataReadController.Find("Button_Connect").GetComponent<Button>();
        }
        //
        void AddComponent()
        {
            m_DataRead = DRBaseFunc.AddScript<VDDataReadClass>(m_VDDataReadController.gameObject, true);
            m_ModelDrive = DRBaseFunc.AddScript<DRModelDriveControl>(m_VDDataReadController.gameObject, true);
            m_ModelDrive.AddListenerRecvMocapData(m_DataRead);
            m_ModelSign = DRBaseFunc.AddScript<DRModelSign>(m_VDDataReadController.gameObject, true);
            m_ModelInfo = DRBaseFunc.AddScript<DRModelInfo>(m_VDDataReadController.gameObject, true);
            m_ModelPoseAdjust = DRBaseFunc.AddScript<DRModelPoseAdjust>(m_VDDataReadController.gameObject, true);
        }
        //
        void InitialSome()
        {
            //
            IsConnect = false;
            VDDataReadClass.UdpOpen();
            InitialTposeModel();
            m_ModelPoseAdjust.Initial(ActorId, m_DataRead, m_Camera);
            m_ControllerInitialColor = m_VDDataReadController.GetComponent<Image>().color;
            if (ActorId == NOW_CHOOSED_ACTORID) { m_VDDataReadController.GetComponent<Image>().color = m_COLOR_ACTOR_CHOOSED; }
            m_TextTitle.text = "(" + (ActorId + 1).ToString() + ")";
            m_VDDataReadController.SetSiblingIndex(ActorId);
            m_ModelSign.SetModelDefaultSignString(m_TextTitle.text);
            //
            m_InputFieldDstPort.text = "7771";
            m_InputFieldDstIp.text = "192.168.1.234";
        }
        //
        void AddEvent()
        {
            m_InputFieldTitle.onEndEdit.AddListener(OnEndEditTitle);
            m_InputFieldDstIp.onEndEdit.AddListener(OnEndEditDstIp);
            m_InputFieldDstPort.onEndEdit.AddListener(OnEndEditDstPort);
            m_ButtonConnect.onClick.AddListener(OnConnect);
            AddTriggersListener(m_VDDataReadController.gameObject, EventTriggerType.PointerDown, OnActorChoosed);
        }
        //
        void InitialTposeModel()
        {
            if (m_TposeModel == null) { return; }
            //
            MatchName_FaceBlendShapesARKit();
            //
            SetTposeModel(m_TposeModel, m_BodyNodesName, m_RightHandNodesName, m_LeftHandNodesName, m_DicFaceBlendShapesMatchNameARKit);
        }

        //
        void AddTriggersListener(GameObject obj, EventTriggerType eventTriggerType, ActorChoosedEventHandler actorChoosed)
        {
            EventTrigger ET = GetComponent<EventTrigger>();
            if (ET == null)
            {
                ET = obj.AddComponent<EventTrigger>();
            }
            if (ET.triggers.Count == 0)
            {
                ET.triggers = new List<EventTrigger.Entry>();
            }

            UnityAction<BaseEventData> callBack = new UnityAction<BaseEventData>(actorChoosed);
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventTriggerType;
            entry.callback.AddListener(callBack);

            ET.triggers.Add(entry);
        }
        #endregion


        #region SetModel
        /// <summary>
        /// 用于运行时更换模型。
        /// </summary>
        public void SetTposeModel(Transform model, DRDataType.Name_BodyNodes nodesName_body, DRDataType.Name_RightHandNodes nodesName_rHand, 
            DRDataType.Name_LeftHandNodes nodesName_lHand, Dictionary<string, string[][]> dicFaceBlendShapesMatchNameARKit)
        {
            m_TposeModel = model;
            m_BodyNodesName = nodesName_body;
            m_RightHandNodesName = nodesName_rHand;
            m_LeftHandNodesName = nodesName_lHand;
            m_DicFaceBlendShapesMatchNameARKit = dicFaceBlendShapesMatchNameARKit;
            //
            m_ModelInfo.DescribeAvailableNodesName(m_BodyNodesName, m_RightHandNodesName, m_LeftHandNodesName, m_DicFaceBlendShapesMatchNameARKit);
            m_ModelInfo.ModelCreate(m_TposeModel);
            //
            m_ModelSign.SetModelSignPose(m_ModelInfo.BodyNodes[(int)_BodyNodes_.BN_Head], m_Camera);
            //Set Model To Drive
            SetModelToDrive();
        }

        /// <summary>
        /// 用于运行时更换模型。
        /// </summary>
        public void SetTposeModel(Transform model, DRDataType.Name_BodyNodes nodesName_body, DRDataType.Name_RightHandNodes nodesName_rHand,
            DRDataType.Name_LeftHandNodes nodesName_lHand, SkinnedMeshRenderer[] faceSkinnedMeshRenderer)
        {
            m_TposeModel = model;
            m_BodyNodesName = nodesName_body;
            m_RightHandNodesName = nodesName_rHand;
            m_LeftHandNodesName = nodesName_lHand;
            m_FaceSkinnedMeshRenderer = faceSkinnedMeshRenderer;
            //
            MatchName_FaceBlendShapesARKit();
            //
            m_ModelInfo.DescribeAvailableNodesName(m_BodyNodesName, m_RightHandNodesName, m_LeftHandNodesName, m_DicFaceBlendShapesMatchNameARKit);
            m_ModelInfo.ModelCreate(m_TposeModel);
            //
            m_ModelSign.SetModelSignPose(m_ModelInfo.BodyNodes[(int)_BodyNodes_.BN_Head], m_Camera);
            //Set Model To Drive
            SetModelToDrive();
        }

        /// <summary>
        /// 
        /// </summary>
        void SetModelToDrive()
        {
            DRModelDriveControl.ModelInfo info = new DRModelDriveControl.ModelInfo();
            info.m_BodyNodes = m_ModelInfo.BodyNodes;
            info.m_rHandNodes = m_ModelInfo.RightHandNodes;
            info.m_lHandNodes = m_ModelInfo.LeftHandNodes;
            DRModelDriveControl.ModelInfo.ToPose(ref info.m_BodyNodes_InitialPose, m_ModelInfo.BodyNodesInitialPosition, m_ModelInfo.BodyNodesInitialQuat);
            DRModelDriveControl.ModelInfo.ToPose(ref info.m_rHandNodes_InitialPose, m_ModelInfo.RightHandNodesInitialPosition, m_ModelInfo.RightHandNodesInitialQuat);
            DRModelDriveControl.ModelInfo.ToPose(ref info.m_lHandNodes_InitialPose, m_ModelInfo.LeftHandNodesInitialPosition, m_ModelInfo.LeftHandNodesInitialQuat);
            info.m_FaceBlendShapesRenderer_ARKit = m_ModelInfo.SkinnedMeshRenderers_ARKitFBS;
            info.m_DicFaceBlendShapesIndex_ARKit = m_ModelInfo.DicBlendShapeIndexARKit;
            //
            m_ModelDrive.SetModelInfo(info);
        }

        //faceBlendShapesARKit_NameMatch
        void MatchName_FaceBlendShapesARKit()
        {
            #region faceBlendShapesARKit_NameMatch
            if (m_FaceSkinnedMeshRenderer != null && m_FaceSkinnedMeshRenderer.Length > 0)
            {
                Mesh mesh;
                DRDataType.Name_FaceBlendShapesARKit fbsDefaultName = new DRDataType.Name_FaceBlendShapesARKit();
                string[][] fbsNames;
                foreach (SkinnedMeshRenderer smr in m_FaceSkinnedMeshRenderer)
                {
                    mesh = smr.sharedMesh;
                    fbsNames = new string[mesh.blendShapeCount][];
                    for (int i = 0; i < mesh.blendShapeCount; i++)
                    {
                        fbsNames[i] = new string[1] { "NULL" };
                        foreach (string str in fbsDefaultName.m_BlendShapeNames)
                        {
                            if (mesh.GetBlendShapeName(i).Contains(str, System.StringComparison.OrdinalIgnoreCase))
                            {
                                fbsNames[i][0] = str;
                                break;
                            }
                        }
                    }
                    m_DicFaceBlendShapesMatchNameARKit[smr.name] = fbsNames;
                }
            }
            #endregion
        }
        #endregion


        #region Event functions
        //
        void OnEndEditTitle(string str)
        {
            m_ModelSign.SetModelSignString(str);
        }
        //
        void OnEndEditDstIp(string str)
        {
            if (str == "") { return; }
            System.Net.IPAddress ip;
            if (!System.Net.IPAddress.TryParse(str.Trim(), out ip))
            {
                StartCoroutine(NoteInputError(m_InputFieldDstIp));
            }
        }
        //
        void OnEndEditDstPort(string str)
        {
            if (str == "") { return; }
            ushort port;
            if (!ushort.TryParse(str.Trim(), out port))
            {
                StartCoroutine(NoteInputError(m_InputFieldDstPort));
            }
        }
        IEnumerator NoteInputError(InputField inputField)
        {
            WaitForSeconds waitSec = new WaitForSeconds(0.2f);
            Color color0 = inputField.GetComponent<Image>().color;
            for (int i = 0; i < 5; i++)
            {
                yield return waitSec;
                inputField.GetComponent<Image>().color = Color.red;
                yield return waitSec;
                inputField.GetComponent<Image>().color = color0;
                inputField.text = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnConnect()
        {
            if (!IsConnect)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        //
        void OnActorChoosed(BaseEventData pd)
        {
            if (NOW_CHOOSED_ACTORID == ActorId) { return; }
            m_LAST_CHOOSED_ACTORID = NOW_CHOOSED_ACTORID;
            NOW_CHOOSED_ACTORID = ActorId;
            m_IS_CHANGE_ACTOR = true;
            //
            m_VDDataReadController.GetComponent<Image>().color = m_COLOR_ACTOR_CHOOSED;
        }
        #endregion


        #region private functions
        /// <summary>
        /// 
        /// </summary>
        void Connect()
        {
            if (!IsConnect)
            {
                //connect
                string dst_ip;
                ushort dst_port;
                if (GetIpPort(out dst_ip, out dst_port))
                {
                    m_ModelDrive.IsMocapDataBreak = false;
                    IsConnect = m_DataRead.UdpSendRequestConnect(dst_ip, dst_port);
                    //
                    if (IsConnect)
                    {
                        SetModelToDrive();
                        m_ModelInfo.IsActiveUsedModel = true;
                        m_InputFieldDstIp.interactable = false;
                        m_InputFieldDstPort.interactable = false;
                        //
                        m_ButtonConnect.transform.Find("Text").GetComponent<Text>().text = "Disconnect";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Disconnect()
        {
            if (IsConnect)
            {
                //disConnect
                IsConnect = false;
                m_DataRead.UdpRemove();
                m_InputFieldDstIp.interactable = true;
                m_InputFieldDstPort.interactable = true;
                if (m_IsHideModelWhenDisconnected)
                {
                    m_ModelInfo.IsActiveUsedModel = false;
                    m_ModelSign.ActiveModelSign = false;
                }
                //
                m_ButtonConnect.transform.Find("Text").GetComponent<Text>().text = "Connect";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst_ip"></param>
        /// <param name="dst_port"></param>
        bool GetIpPort(out string dst_ip, out ushort dst_port)
        {
            dst_ip = "";
            dst_port = 0;
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(m_InputFieldDstIp.text.Trim(), out ip))
            {
                //ip
                dst_ip = ip.ToString();
                //port
                if (ushort.TryParse(m_InputFieldDstPort.text.Trim(), out dst_port))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// run in Update().
        /// </summary>
        void UpdateControllerState()
        {
            if (m_IS_CHANGE_ACTOR && ActorId == m_LAST_CHOOSED_ACTORID)
            {
                m_IS_CHANGE_ACTOR = false;
                m_VDDataReadController.GetComponent<Image>().color = m_ControllerInitialColor;
            }
            //
            if (m_ModelDrive.IsMocapDataBreak && IsConnect)
            {
                Disconnect();
            }
        }
        #endregion



    }//end class DRDataReadController
}

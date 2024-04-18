//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace VDDataRead
{

    public class DRModelInfo : MonoBehaviour
    {
        #region Bind Model

        #region define
        GameObject m_Model = null;
        Dictionary<string, int> m_DicFaceBlendShapeARKitDefaultNameIndex = new Dictionary<string, int>();
        DRDataType.Name_FaceBlendShapesARKit m_FaceBlendShapesARKit = new DRDataType.Name_FaceBlendShapesARKit();
        DRDataType.Name_BodyNodes m_BodyNodesName = new DRDataType.Name_BodyNodes();
        DRDataType.Name_RightHandNodes m_RHandNodesName = new DRDataType.Name_RightHandNodes();
        DRDataType.Name_LeftHandNodes m_LHandNodesName = new DRDataType.Name_LeftHandNodes();
        Quaternion[] m_InitialBodyQuat = new Quaternion[DRDataType.NODES_BODY];
        Quaternion[] m_InitialRightHandQuat = new Quaternion[DRDataType.NODES_HAND];
        Quaternion[] m_InitialLeftHandQuat = new Quaternion[DRDataType.NODES_HAND];
        Vector3[] m_InitialBodyPosition = new Vector3[DRDataType.NODES_BODY];
        Vector3[] m_InitialRightHandPosition = new Vector3[DRDataType.NODES_HAND];
        Vector3[] m_InitialLeftHandPosition = new Vector3[DRDataType.NODES_HAND];

        bool m_LastIsActiveUsedModel = true;

        bool m_FindModelActived = false;

        public bool IsActiveUsedModel { get; set; }

        //
        #region get model info
        public Transform UsedModel { get { if (m_Model != null) { return m_Model.transform; } return null; } }

        /// <summary>
        /// 使用的模型名称，包含路径（在场景中的路径）
        /// </summary>
        public string UsedModelName { get; private set; }

        /// <summary>
        /// 所使用的模型在场景中的源模型名称，包含路径（在场景中的路径）
        /// </summary>
        public string SourceModelName { get; private set; }

        public Transform[] BodyNodes { get; private set; }

        public Transform[] RightHandNodes { get; private set; }

        public Transform[] LeftHandNodes { get; private set; }

        //若运行 UpdateBlendShape() 函数，SKinnedMeshRenderers 是需要的。
        public List<SkinnedMeshRenderer> SkinnedMeshRenderers_ARKitFBS { get; private set; }

        //key: renderer.name + ":" + blendShape.name.  若运行 UpdateBlendShape() 函数，DicBlendShapeIndex 是需要的。
        public Dictionary<string, int[]> DicBlendShapeIndexARKit { get; private set; }

        public SkinnedMeshRenderer JawOpenBlendShapeRenderer { get; private set; }

        public string JawOpenBlendShapeName { get; private set; }

        public Quaternion[] BodyNodesInitialQuat
        {
            get
            {
                Quaternion[] iniQuat = new Quaternion[DRDataType.NODES_BODY];
                for (int i = 0; i < DRDataType.NODES_BODY; i++)
                {
                    iniQuat[i] = m_InitialBodyQuat[i];
                }
                return iniQuat;
            }
        }

        //初始 Tpose 下模型节点坐标
        public Vector3[] BodyNodesInitialPosition
        {
            get
            {
                Vector3[] iniPosi = new Vector3[DRDataType.NODES_BODY];
                for (int i = 0; i < DRDataType.NODES_BODY; i++)
                {
                    iniPosi[i] = m_InitialBodyPosition[i];
                }
                return iniPosi;
            }
        }

        public Quaternion[] RightHandNodesInitialQuat
        {
            get
            {
                Quaternion[] iniQuat = new Quaternion[DRDataType.NODES_HAND];
                for (int i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    iniQuat[i] = m_InitialRightHandQuat[i];
                }
                return iniQuat;
            }
        }
        //初始 Tpose 下模型节点坐标
        public Vector3[] RightHandNodesInitialPosition
        {
            get
            {
                Vector3[] iniPosi = new Vector3[DRDataType.NODES_HAND];
                for (int i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    iniPosi[i] = m_InitialRightHandPosition[i];
                }
                return iniPosi;
            }
        }

        public Quaternion[] LeftHandNodesInitialQuat
        {
            get
            {
                Quaternion[] iniQuat = new Quaternion[DRDataType.NODES_HAND];
                for (int i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    iniQuat[i] = m_InitialLeftHandQuat[i];
                }
                return iniQuat;
            }
        }
        //初始 Tpose 下模型节点坐标
        public Vector3[] LeftHandNodesInitialPosition
        {
            get
            {
                Vector3[] iniPosi = new Vector3[DRDataType.NODES_HAND];
                for (int i = 0; i < DRDataType.NODES_HAND; i++)
                {
                    iniPosi[i] = m_InitialLeftHandPosition[i];
                }
                return iniPosi;
            }
        }
        #endregion

        //
        #region set model info
        public void SetBodyNodesName(DRDataType.Name_BodyNodes nodesName) { m_BodyNodesName = nodesName.Clone(); }
        public void SetRightHandNodesName(DRDataType.Name_RightHandNodes nodesName) { m_RHandNodesName = nodesName.Clone(); }
        public void SetLeftHandNodesName(DRDataType.Name_LeftHandNodes nodesName) { m_LHandNodesName = nodesName.Clone(); }
        //key: m_FaceBlendShapeRenderersName[i]; value: m_FaceBlendShapeRenderersName[i].BlendShapes 对应的默认名称，可对应一个也可多个。
        public Dictionary<string, string[][]> DicFaceBlendShapesMatchNameARKit { get; set; }
        #endregion
        #endregion


        //
        void Awake()
        {
            UsedModelName = "";
            SourceModelName = "";
            m_LastIsActiveUsedModel = true;
            IsActiveUsedModel = true;
            BodyNodes = new Transform[DRDataType.NODES_BODY];
            RightHandNodes = new Transform[DRDataType.NODES_HAND];
            LeftHandNodes = new Transform[DRDataType.NODES_HAND];
            SkinnedMeshRenderers_ARKitFBS = new List<SkinnedMeshRenderer>();
            DicBlendShapeIndexARKit = new Dictionary<string, int[]>();
            DicFaceBlendShapesMatchNameARKit = new Dictionary<string, string[][]>();

            //m_DicFaceBlendShapeNameIndex Initial
            DRDataType.Name_FaceBlendShapesARKit bs_arkit = new DRDataType.Name_FaceBlendShapesARKit();
            for (int i = 0; i < DRDataType.NODES_FACEBS_ARKIT; i++)
            {
                m_DicFaceBlendShapeARKitDefaultNameIndex[bs_arkit.m_BlendShapeNames[i]] = i;
            }
        }

        //
        void Update()
        {
            if (IsActiveUsedModel != m_LastIsActiveUsedModel)
            {
                m_LastIsActiveUsedModel = IsActiveUsedModel;
                if (m_Model != null && m_Model.activeSelf == !IsActiveUsedModel)
                {
                    m_Model.SetActive(IsActiveUsedModel);
                }
            }
        }

        void OnDestroy()
        {
            if (UsedModel != null) { Destroy(UsedModel.gameObject); }
        }


        /// <summary>
        /// 提供要创建（或找寻）的模型中会使用到的节点名称。
        /// 需最先执行。
        /// </summary>
        public void DescribeAvailableNodesName(DRDataType.Name_BodyNodes nodesName_body, DRDataType.Name_RightHandNodes nodesName_rHand,
            DRDataType.Name_LeftHandNodes nodesName_lHand, Dictionary<string, string[][]> dicFaceBlendShapesMatchNameARKit)
        {
            SetBodyNodesName(nodesName_body);
            SetRightHandNodesName(nodesName_rHand);
            SetLeftHandNodesName(nodesName_lHand);
            DicFaceBlendShapesMatchNameARKit = dicFaceBlendShapesMatchNameARKit;
        }


        #region model create and destroy
        /// <summary>
        /// 创建（复制）模型，用于驱动。
        /// </summary>
        public bool ModelCreate(Transform model, bool isHideSrcModel = true)
        {
            ModelDestroy();
            try
            {
                m_Model = Instantiate(model).gameObject; //复制
                m_Model.transform.localScale = model.transform.localScale;
                if (model.transform.parent != null) { m_Model.transform.SetParent(model.transform.parent); }
                if (isHideSrcModel) { if (model.gameObject.activeSelf) { model.gameObject.SetActive(false); } }
                if (m_Model.activeSelf == false && IsActiveUsedModel) { m_Model.SetActive(true); }
                else if (m_Model.activeSelf && !IsActiveUsedModel) { m_Model.SetActive(false); }
            }
            catch { return false; }
            GetNeedModelNodes(m_Model);
            GetAllModelNodes(m_Model);
            if (GetNodesInitialInfo() == false) { ModelDestroy(); return false; }
            UsedModelName = GetObjectPath(m_Model.transform);
            SourceModelName = GetObjectPath(model);
            return true;
        }

        /// <summary>
        /// 销毁创建（复制）的模型。
        /// </summary>
        public void ModelDestroy()
        {
            try
            {
                if (m_Model != null)
                {
                    GameObject.Destroy(m_Model);
                    m_Model = null;
                    BodyNodes = new Transform[DRDataType.NODES_BODY];
                    RightHandNodes = new Transform[DRDataType.NODES_HAND];
                    LeftHandNodes = new Transform[DRDataType.NODES_HAND];
                    SkinnedMeshRenderers_ARKitFBS.Clear();
                    DicBlendShapeIndexARKit.Clear();
                }
                UsedModelName = "";
                SourceModelName = "";
            }
            catch { }
        }
        #endregion


        #region sub function
        /// <summary>
        /// 递归查找匹配需要的节点名称
        /// </summary>
        /// <VDSuitMiniSettingsam name="obj"></VDSuitMiniSettingsam>
        void GetAllModelNodes(GameObject obj)
        {
            foreach (Transform o in obj.GetComponentInChildren<Transform>(true))
            {
                GetNeedModelNodes(o.gameObject);
                GetAllModelNodes(o.gameObject);
            }
        }
        void GetNeedModelNodes(GameObject obj)
        {
            if (Equals(obj.name, m_BodyNodesName.BN_Hips))
            {
                BodyNodes[(int)_BodyNodes_.BN_Hips] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightUpperLeg))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightUpperLeg)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightLowerLeg))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightLowerLeg)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightFoot))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightFoot)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightToe))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightToe)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftUpperLeg))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftUpperLeg)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftLowerLeg))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftLowerLeg)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftFoot))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftFoot)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftToe))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftToe)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Spine))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Spine)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Spine1))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Spine1)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Spine2))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Spine2)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Spine3))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Spine3)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Neck))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Neck)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_Head))
            {
                BodyNodes[(int)(_BodyNodes_.BN_Head)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightShoulder))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightShoulder)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightUpperArm))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightUpperArm)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightLowerArm))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightLowerArm)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_RightHand))
            {
                BodyNodes[(int)(_BodyNodes_.BN_RightHand)] = obj.transform;
                RightHandNodes[(int)(_HandNodes_.HN_Hand)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftShoulder))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftShoulder)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftUpperArm))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftUpperArm)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftLowerArm))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftLowerArm)] = obj.transform;
            }
            else if (Equals(obj.name, m_BodyNodesName.BN_LeftHand))
            {
                BodyNodes[(int)(_BodyNodes_.BN_LeftHand)] = obj.transform;
                LeftHandNodes[(int)(_HandNodes_.HN_Hand)] = obj.transform;
            }

            //right hand fingers
            else if (Equals(obj.name, m_RHandNodesName.HN_RightThumbFinger))
            {
                RightHandNodes[(int)(_HandNodes_.HN_ThumbFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightThumbFinger1))
            {
                RightHandNodes[(int)(_HandNodes_.HN_ThumbFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightThumbFinger2))
            {
                RightHandNodes[(int)(_HandNodes_.HN_ThumbFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightIndexFinger))
            {
                RightHandNodes[(int)(_HandNodes_.HN_IndexFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightIndexFinger1))
            {
                RightHandNodes[(int)(_HandNodes_.HN_IndexFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightIndexFinger2))
            {
                RightHandNodes[(int)(_HandNodes_.HN_IndexFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightIndexFinger3))
            {
                RightHandNodes[(int)(_HandNodes_.HN_IndexFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightMiddleFinger))
            {
                RightHandNodes[(int)(_HandNodes_.HN_MiddleFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightMiddleFinger1))
            {
                RightHandNodes[(int)(_HandNodes_.HN_MiddleFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightMiddleFinger2))
            {
                RightHandNodes[(int)(_HandNodes_.HN_MiddleFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightMiddleFinger3))
            {
                RightHandNodes[(int)(_HandNodes_.HN_MiddleFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightRingFinger))
            {
                RightHandNodes[(int)(_HandNodes_.HN_RingFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightRingFinger1))
            {
                RightHandNodes[(int)(_HandNodes_.HN_RingFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightRingFinger2))
            {
                RightHandNodes[(int)(_HandNodes_.HN_RingFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightRingFinger3))
            {
                RightHandNodes[(int)(_HandNodes_.HN_RingFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightPinkyFinger))
            {
                RightHandNodes[(int)(_HandNodes_.HN_PinkyFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightPinkyFinger1))
            {
                RightHandNodes[(int)(_HandNodes_.HN_PinkyFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightPinkyFinger2))
            {
                RightHandNodes[(int)(_HandNodes_.HN_PinkyFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_RHandNodesName.HN_RightPinkyFinger3))
            {
                RightHandNodes[(int)(_HandNodes_.HN_PinkyFinger3)] = obj.transform;
            }

            //left hand fingers
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftThumbFinger))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_ThumbFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftThumbFinger1))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_ThumbFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftThumbFinger2))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_ThumbFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftIndexFinger))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_IndexFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftIndexFinger1))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_IndexFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftIndexFinger2))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_IndexFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftIndexFinger3))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_IndexFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftMiddleFinger))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_MiddleFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftMiddleFinger1))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_MiddleFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftMiddleFinger2))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_MiddleFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftMiddleFinger3))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_MiddleFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftRingFinger))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_RingFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftRingFinger1))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_RingFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftRingFinger2))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_RingFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftRingFinger3))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_RingFinger3)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftPinkyFinger))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_PinkyFinger)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftPinkyFinger1))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_PinkyFinger1)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftPinkyFinger2))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_PinkyFinger2)] = obj.transform;
            }
            else if (Equals(obj.name, m_LHandNodesName.HN_LeftPinkyFinger3))
            {
                LeftHandNodes[(int)(_HandNodes_.HN_PinkyFinger3)] = obj.transform;
            }

            //BlendShapesARKit
            bool isOk = false;
            int num_faces = 0;
            if (DicFaceBlendShapesMatchNameARKit != null) { num_faces = DicFaceBlendShapesMatchNameARKit.Count; }
            for (int i = 0; i < num_faces; i++)
            {
                if (DicFaceBlendShapesMatchNameARKit.ContainsKey(obj.name)) { isOk = true; break; }
            }
            if (isOk)
            {
                try
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderers_ARKitFBS.Add(skinnedMeshRenderer);
                    var mesh = skinnedMeshRenderer.sharedMesh;
                    var count = System.Math.Min(mesh.blendShapeCount, DicFaceBlendShapesMatchNameARKit[obj.name].Length);


                    //string[][] a = DicFaceBlendShapesMatchNameARKit[obj.name];
                    //string str = "";
                    //for (int i = 0; i < a.Length; i++)
                    //{
                    //    for (int ii = 0; ii < a[i].Length; ii++)
                    //    {
                    //        str += a[i][ii] + " ";
                    //    }
                    //    str += "\r\n";
                    //}
                    //Debug.Log("|||||    " + str);


                    for (int i = 0; i < count; i++)
                    {
                        string name_bs = mesh.GetBlendShapeName(i);

                        int nn = DicFaceBlendShapesMatchNameARKit[obj.name][i].Length;
                        int[] indexs = new int[nn];
                        for (int ii = 0; ii < nn; ii++)
                        {
                            indexs[ii] = -1;
                            if (m_DicFaceBlendShapeARKitDefaultNameIndex.ContainsKey(DicFaceBlendShapesMatchNameARKit[obj.name][i][ii]))
                            {
                                indexs[ii] = m_DicFaceBlendShapeARKitDefaultNameIndex[DicFaceBlendShapesMatchNameARKit[obj.name][i][ii]];

                                if (DicFaceBlendShapesMatchNameARKit[obj.name][i][ii] == m_FaceBlendShapesARKit.m_BlendShapeNames[(int)DRDataType._FaceBlendShapeARKit_.JawOpen])
                                {
                                    JawOpenBlendShapeRenderer = skinnedMeshRenderer;
                                    JawOpenBlendShapeName = name_bs;
                                }
                            }
                        }
                        DicBlendShapeIndexARKit[skinnedMeshRenderer.name + ":" + name_bs] = indexs;
                    }
                }
                catch { return; }
            }
        }

        /// <summary>
        /// 获取模型初始信息：初始四元数，坐标。
        /// </summary>
        bool GetNodesInitialInfo()
        {
            //body
            int nodeIndex = 0;
            for (int i = 0; i < DRDataType.NODES_BODY; i++)
            {
                if (BodyNodes[0] == null) { return false; }
                nodeIndex = i;
                if (i > 0 && BodyNodes[i] == null)
                {
                    switch (i)
                    {
                        case (int)(_BodyNodes_.BN_LeftUpperLeg):
                            {
                                nodeIndex = (int)(_BodyNodes_.BN_Hips);
                            }
                            break;
                        case (int)(_BodyNodes_.BN_Spine):
                            {
                                nodeIndex = (int)(_BodyNodes_.BN_Hips);
                            }
                            break;
                        case (int)(_BodyNodes_.BN_RightShoulder):
                            {
                                nodeIndex = (int)(_BodyNodes_.BN_Spine3);
                            }
                            break;
                        case (int)(_BodyNodes_.BN_LeftShoulder):
                            {
                                nodeIndex = (int)(_BodyNodes_.BN_Spine3);
                            }
                            break;
                        default:
                            {
                                nodeIndex = i - 1;
                            }
                            break;
                    }//end switch
                }

                if (nodeIndex == i)
                {
                    m_InitialBodyQuat[i] = BodyNodes[i].transform.rotation;
                    m_InitialBodyPosition[i] = BodyNodes[i].transform.position;
                }
                else
                {
                    m_InitialBodyPosition[i] = m_InitialBodyPosition[nodeIndex];
                }
            }//end for

            //rhand and lhand
            int rNodeIndex = 0, lNodeIndex = 0;
            for (int i = 0; i < DRDataType.NODES_HAND; i++)
            {
                rNodeIndex = i;
                lNodeIndex = i;

                if (i == 0)
                {
                    if (RightHandNodes[0] == null) { rNodeIndex = -1; m_InitialRightHandPosition[0] = m_InitialBodyPosition[(int)(_BodyNodes_.BN_RightLowerArm)]; }
                    if (LeftHandNodes[0] == null) { lNodeIndex = -1; m_InitialLeftHandPosition[0] = m_InitialBodyPosition[(int)(_BodyNodes_.BN_LeftLowerArm)]; }
                }

                if (i > 0 && (RightHandNodes[i] == null || LeftHandNodes[i] == null))
                {
                    switch (i)
                    {
                        case (int)(_HandNodes_.HN_IndexFinger):
                        case (int)(_HandNodes_.HN_MiddleFinger):
                        case (int)(_HandNodes_.HN_RingFinger):
                        case (int)(_HandNodes_.HN_PinkyFinger):
                            {
                                if (RightHandNodes[i] == null) { rNodeIndex = (int)(_HandNodes_.HN_Hand); }
                                if (LeftHandNodes[i] == null) { lNodeIndex = (int)(_HandNodes_.HN_Hand); }
                            }
                            break;
                        default:
                            {
                                if (RightHandNodes[i] == null) { rNodeIndex = i - 1; }
                                if (LeftHandNodes[i] == null) { lNodeIndex = i - 1; }
                            }
                            break;
                    }//end switch
                }

                if (rNodeIndex == i)
                {
                    m_InitialRightHandQuat[i] = RightHandNodes[i].transform.rotation;
                    m_InitialRightHandPosition[i] = RightHandNodes[i].transform.position;
                }
                else if (rNodeIndex >= 0)
                {
                    m_InitialRightHandPosition[i] = m_InitialRightHandPosition[rNodeIndex];
                }

                if (lNodeIndex == i)
                {
                    m_InitialLeftHandQuat[i] = LeftHandNodes[i].transform.rotation;
                    m_InitialLeftHandPosition[i] = LeftHandNodes[i].transform.position;
                }
                else if (lNodeIndex >= 0)
                {
                    m_InitialLeftHandPosition[i] = m_InitialLeftHandPosition[lNodeIndex];
                }
            }//end for

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetObjectPath(Transform obj)
        {
            if (obj == null) { return null; }
            Transform obj1 = obj;
            string path = obj1.name;
            while (obj1 != null)
            {
                obj1 = obj1.parent;
                if (obj1 != null)
                {
                    path = obj1.name + "/" + path;
                }
            }
            return path;
        }

        /// <summary>
        /// 不考虑空格，且默认不区分大小写。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool Equals(string a, string b, System.StringComparison comparisonType = System.StringComparison.OrdinalIgnoreCase)
        {
            return string.Equals(a.Trim(), b.Trim(), comparisonType);
        }
        #endregion
        #endregion

    }//end class

}//end namespace

//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using UnityEngine;


namespace VDDataRead
{
    //在每个角色形象头上显示名称
    public class DRModelSign : MonoBehaviour
    {
        Camera m_CamerLook = null;
        Transform m_ObjParent = null;
        Text3dControl m_Text3D = new Text3dControl();
        string m_SignStr = "";
        string m_DefaultSignStr = "";
        bool m_IsSetSignPose = false;
        //
        public bool ActiveModelSign { get { return m_Text3D.ActiveSelf; } set { m_Text3D.ActiveSelf = value; } }

        void Awake()
        {
            m_Text3D.Text3dText_Creat(m_SignStr, new Vector3(0, 0, 0), Quaternion.identity, Color.red);
        }

        void LateUpdate()
        {
            if (m_IsSetSignPose)
            {
                if (m_SignStr == "" && m_DefaultSignStr != "") { m_SignStr = m_DefaultSignStr; }
                m_Text3D.Text3dText_Update(m_SignStr, m_ObjParent.position + new Vector3(0, 0.4f, 0), m_CamerLook.transform.rotation);
                KeyActiveModelSign();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void KeyActiveModelSign()
        {
            if (DRShortcutKey.JudgeKey(enum_ShortcutKeyEven.ModelSignHideShow))
            {
                ActiveModelSign = !ActiveModelSign;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cameraLook"></param>
        public void SetModelSignPose(Transform parent, Camera cameraLook)
        {
            m_ObjParent = parent;
            m_CamerLook = cameraLook;
            m_IsSetSignPose = (m_ObjParent != null && m_CamerLook != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultSignStr"></param>
        public void SetModelDefaultSignString(string defaultSignStr)
        {
            m_DefaultSignStr = defaultSignStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="position"></param>
        /// <param name="quat"></param>
        public void SetModelSignString(string signStr)
        {
            m_SignStr = signStr;
        }

    }//end class


    /// <summary>
    /// 
    /// </summary>
    class Text3dControl
    {

        //for Text3dText display
        #region Text3dText
        static GameObject m_Text3dParent = null;
        GameObject m_Text3dText = null;
        string m_TextString = "";


        //创建出来的是隐藏的物体
        public void Text3dText_Creat(string text3dText, Vector3 position, Quaternion quat, Color color,
            TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignment textAlignment = TextAlignment.Center)
        {
            //
            if (m_Text3dParent == null)
            {
                m_Text3dParent = new GameObject();
                m_Text3dParent.name = "Text3D";
                m_Text3dParent.transform.localScale = new Vector3(1f, 1f, 1f);
                m_Text3dParent.transform.rotation = new Quaternion(0, 0, 0, 1);
                m_Text3dParent.transform.position = new Vector3(0, 0, 0);
            }
            //
            Text3dText_Destroy();
            m_Text3dText = new GameObject();
            m_Text3dText.SetActive(false);
            m_Text3dText.transform.parent = m_Text3dParent.transform;
            m_Text3dText.AddComponent<TextMesh>();  //添加TextMesh组件
            m_Text3dText.GetComponent<TextMesh>().anchor = textAnchor; //TextAnchor.MiddleCenter; //字体居中
            m_Text3dText.GetComponent<TextMesh>().alignment = textAlignment; //TextAlignment.Center; //每行字体居中对准
            m_Text3dText.GetComponent<TextMesh>().color = color;
            m_Text3dText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //
            Text3dText_Update(text3dText, position, quat);
        }

        public void Text3dText_Update(string text3dText, Vector3 position, Quaternion quat)
        {
            if (ActiveSelf)
            {
                m_Text3dText.transform.position = position;
                m_Text3dText.transform.rotation = quat;
                if (m_TextString != text3dText)
                {
                    m_Text3dText.GetComponent<TextMesh>().text = text3dText;
                    m_TextString = text3dText;
                }
            }
        }


        public bool ActiveSelf
        {
            get
            {
                if (m_Text3dText != null)
                {
                    return m_Text3dText.activeSelf;
                }
                return false;
            }
            set
            {
                if (m_Text3dText != null)
                {
                    if (m_Text3dText.activeSelf != value)
                    {
                        m_Text3dText.SetActive(value);
                    }
                }
            }
        }


        public void Text3dText_Destroy()
        {
            if (m_Text3dText != null)
            {
                GameObject.Destroy(m_Text3dText);
                m_Text3dText = null;
            }
        }
        #endregion
    }//end class 

}//namespace

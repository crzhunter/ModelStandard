using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace amfaceh
{
    /// <summary>
    /// 
    /// </summary>
    public class FaceBsSetter
    {
        #region define
        bool m_isInitial = false;
        VAB_FaceBS m_vab_facebs;
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public void Initial(GameObject humanModel)
        {
            m_vab_facebs = new VAB_FaceBS();
            m_vab_facebs.Initial(humanModel);
            m_isInitial = true;
        }


        /// <summary>
        /// 最好在 LateUpdate() 中执行，因为要等动画数据更新完（若存在动画），然后执行进行覆盖。
        /// 52个bs表情[0, 1],排序按照枚举 amfaceh_api.AMFaceBS52
        /// </summary>
        /// <param name="face_bs_52">52个bs表情,排序按照枚举 amfaceh_api.AMFaceBS52 </param>
        public void SetFaceBS(float[] face_bs_52)
        {
            if (m_isInitial)
            {
                m_vab_facebs.SetBlendShapeValue(face_bs_52);
            }            
        }


    }//end class



    public class VAB_FaceBS
    {
        #region define
        //================ GameObject ================//
        //=================== 枚举 ===================//
        //================ 类或结构体 ================//
        //================ 委托和事件 ================//
        //=================== 常量 ===================//
        //=================== 变量 ===================//
        int[] m_arkit_bs_index;
        VAB.VABInfo m_VABInfo;
        #endregion



        /// <summary>
        /// 
        /// </summary>
        public VAB_FaceBS()
        {
            InitialSome();
        }




        #region public functions
        /// <summary>
        /// 
        /// </summary>
        public void Initial(GameObject humanModel)
        {
            m_VABInfo = humanModel.GetComponent<VAB.VABInfo>();
        }

        /// <summary>
        /// index: (int)VAB.FaceBS_ARKit.
        /// value: [0, 1].
        /// </summary>
        public void SetBlendShapeValue_Standard(int index, float value)
        {
            if (index >= 0 && index < m_VABInfo.m_StandardFaceBS_ARKit.Length)
            {
                foreach (VAB.VABInfo.BSInfo fbs in m_VABInfo.m_StandardFaceBS_ARKit[index].bsInfos)
                {
                    fbs.renderer.SetBlendShapeWeight(fbs.index, 100 * value);
                }
            }
        }

        /// <summary>
        /// index: (int)VAB.FaceBS_ARKit.
        /// return value: [0, 1]. 
        /// </summary>
        public float GetBlendShapeValue_Standard(int index)
        {
            float value = 0;
            if (index >= 0 && index < m_VABInfo.m_StandardFaceBS_ARKit.Length)
            {
                foreach (VAB.VABInfo.BSInfo fbs in m_VABInfo.m_StandardFaceBS_ARKit[index].bsInfos)
                {
                    value = Mathf.Max(value, fbs.renderer.GetBlendShapeWeight(fbs.index));
                }
            }
            return 0.01f * value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetBlendShapeValue(float[] face_bs_arkit)
        {
            if (m_VABInfo == null) { return; }
            if (m_VABInfo.m_StandardFaceBS_ARKit != null)
            {
                int n = 0;
                foreach (VAB.VABInfo.StandardBSInfo s_bs in m_VABInfo.m_StandardFaceBS_ARKit)
                {
                    foreach (VAB.VABInfo.BSInfo bs in s_bs.bsInfos)
                    {
                        bs.renderer.SetBlendShapeWeight(bs.index, 100 * face_bs_arkit[m_arkit_bs_index[n]]);
                    }
                    n++;
                }
            }
        }
        #endregion



        #region private functions
        /// <summary>
        /// 
        /// </summary>
        void InitialSome()
        {
            m_arkit_bs_index = new int[(int)VAB.FaceBS_ARKit.COUNT];
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_BrowDownLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_BrowDownLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_BrowDownRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_BrowDownRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_BrowInnerUp] = (int)amfaceh_api.AMFaceBS52.AMFBS_BrowInnerUp;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_BrowOuterUpLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_BrowOuterUpLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_BrowOuterUpRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_BrowOuterUpRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeBlinkLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeBlinkLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeBlinkRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeBlinkRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeWideLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeWideLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeWideRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeWideRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookDownLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookDownLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookDownRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookDownRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookUpLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookUpLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookUpRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookUpRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookInLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookInLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookInRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookInRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookOutLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookOutLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_EyeLookOutRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeLookOutRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_JawOpen] = (int)amfaceh_api.AMFaceBS52.AMFBS_JawOpen;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_JawLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_JawLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_JawRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_JawRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthSmileLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthSmileLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthSmileRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthSmileRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthFrownLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthFrownLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthFrownRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthFrownRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthPucker] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthPucker;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthRollLower] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthRollLower;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Basic_MouthRollUpper] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthRollUpper;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Eye_SquintLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeSquintLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Eye_SquintRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_EyeSquintRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Jaw_Forward] = (int)amfaceh_api.AMFaceBS52.AMFBS_JawForward;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_Close] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthClose;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_DimpleLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthDimpleLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_DimpleRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthDimpleRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_Funnel] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthFunnel;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_Left] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_Right] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_LowerDownLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthLowerDownLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_LowerDownRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthLowerDownRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_PressLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthPressLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_PressRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthPressRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_ShrugLower] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthShrugLower;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_ShrugUpper] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthShrugUpper;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_StretchLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthStretchLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_StretchRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthStretchRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_UpperUpLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthUpperUpLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Mouth_UpperUpRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_MouthUpperUpRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Cheek_Puff] = (int)amfaceh_api.AMFaceBS52.AMFBS_CheekPuff;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Cheek_SquintLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_CheekSquintLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Cheek_SquintRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_CheekSquintRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Nose_SneerLeft] = (int)amfaceh_api.AMFaceBS52.AMFBS_NoseSneerLeft;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Nose_SneerRight] = (int)amfaceh_api.AMFaceBS52.AMFBS_NoseSneerRight;
            m_arkit_bs_index[(int)VAB.FaceBS_ARKit.Tongue_Out] = (int)amfaceh_api.AMFaceBS52.AMFBS_TongueOut;
        }

        #endregion

    }//end class
}//end namespace

//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System;

namespace VDDataRead
{
    public class DRDataType
    {
        public const int NODES_BODY = 23;
        public const int NODES_HAND = 20;
        public const int NODES_FACEBS_ARKIT = 52;

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


        public enum _WhichGlove_
        {
            GM_RightGlove = 0,
            GM_LeftGlove,
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



        [Serializable]
        public class Name_BodyNodes
        {
            public string BN_Hips = "Hips";
            public string BN_RightUpperLeg = "RightUpperLeg";
            public string BN_RightLowerLeg = "RightLowerLeg";
            public string BN_RightFoot = "RightFoot";
            public string BN_RightToe = "RightToe";
            public string BN_LeftUpperLeg = "LeftUpperLeg";
            public string BN_LeftLowerLeg = "LeftLowerLeg";
            public string BN_LeftFoot = "LeftFoot";
            public string BN_LeftToe = "LeftToe";
            public string BN_Spine = "Spine";
            public string BN_Spine1 = "Spine1";
            public string BN_Spine2 = "Spine2";
            public string BN_Spine3 = "Spine3";
            public string BN_Neck = "Neck";
            public string BN_Head = "Head";
            public string BN_RightShoulder = "RightShoulder";
            public string BN_RightUpperArm = "RightUpperArm";
            public string BN_RightLowerArm = "RightLowerArm";
            public string BN_RightHand = "RightHand";
            public string BN_LeftShoulder = "LeftShoulder";
            public string BN_LeftUpperArm = "LeftUpperArm";
            public string BN_LeftLowerArm = "LeftLowerArm";
            public string BN_LeftHand = "LeftHand";

            public Name_BodyNodes Clone()
            {
                return DRBaseFunc.Clone(this);
            }
        }


        [Serializable]
        public class Name_LeftHandNodes
        {
            public string HN_LeftHand = "LeftHand";
            public string HN_LeftThumbFinger = "LeftThumbFinger";
            public string HN_LeftThumbFinger1 = "LeftThumbFinger1";
            public string HN_LeftThumbFinger2 = "LeftThumbFinger2";
            public string HN_LeftIndexFinger = "LeftIndexFinger";
            public string HN_LeftIndexFinger1 = "LeftIndexFinger1";
            public string HN_LeftIndexFinger2 = "LeftIndexFinger2";
            public string HN_LeftIndexFinger3 = "LeftIndexFinger3";
            public string HN_LeftMiddleFinger = "LeftMiddleFinger";
            public string HN_LeftMiddleFinger1 = "LeftMiddleFinger1";
            public string HN_LeftMiddleFinger2 = "LeftMiddleFinger2";
            public string HN_LeftMiddleFinger3 = "LeftMiddleFinger3";
            public string HN_LeftRingFinger = "LeftRingFinger";
            public string HN_LeftRingFinger1 = "LeftRingFinger1";
            public string HN_LeftRingFinger2 = "LeftRingFinger2";
            public string HN_LeftRingFinger3 = "LeftRingFinger3";
            public string HN_LeftPinkyFinger = "LeftPinkyFinger";
            public string HN_LeftPinkyFinger1 = "LeftPinkyFinger1";
            public string HN_LeftPinkyFinger2 = "LeftPinkyFinger2";
            public string HN_LeftPinkyFinger3 = "LeftPinkyFinger3";

            public Name_LeftHandNodes Clone()
            {
                return DRBaseFunc.Clone(this);
            }
        }


        [Serializable]
        public class Name_RightHandNodes
        {
            public string HN_RightHand = "RightHand";
            public string HN_RightThumbFinger = "RightThumbFinger";
            public string HN_RightThumbFinger1 = "RightThumbFinger1";
            public string HN_RightThumbFinger2 = "RightThumbFinger2";
            public string HN_RightIndexFinger = "RightIndexFinger";
            public string HN_RightIndexFinger1 = "RightIndexFinger1";
            public string HN_RightIndexFinger2 = "RightIndexFinger2";
            public string HN_RightIndexFinger3 = "RightIndexFinger3";
            public string HN_RightMiddleFinger = "RightMiddleFinger";
            public string HN_RightMiddleFinger1 = "RightMiddleFinger1";
            public string HN_RightMiddleFinger2 = "RightMiddleFinger2";
            public string HN_RightMiddleFinger3 = "RightMiddleFinger3";
            public string HN_RightRingFinger = "RightRingFinger";
            public string HN_RightRingFinger1 = "RightRingFinger1";
            public string HN_RightRingFinger2 = "RightRingFinger2";
            public string HN_RightRingFinger3 = "RightRingFinger3";
            public string HN_RightPinkyFinger = "RightPinkyFinger";
            public string HN_RightPinkyFinger1 = "RightPinkyFinger1";
            public string HN_RightPinkyFinger2 = "RightPinkyFinger2";
            public string HN_RightPinkyFinger3 = "RightPinkyFinger3";

            public Name_RightHandNodes Clone()
            {
                return DRBaseFunc.Clone(this);
            }
        }


        [Serializable]
        public class Name_FaceBlendShapesARKit
        {
            public string[] m_BlendShapeNames = new string[NODES_FACEBS_ARKIT];

            public Name_FaceBlendShapesARKit()
            {
                m_BlendShapeNames[0] = "BrowDownLeft";
                m_BlendShapeNames[1] = "BrowDownRight";
                m_BlendShapeNames[2] = "BrowInnerUp";
                m_BlendShapeNames[3] = "BrowOuterUpLeft";
                m_BlendShapeNames[4] = "BrowOuterUpRight";
                m_BlendShapeNames[5] = "CheekPuff";
                m_BlendShapeNames[6] = "CheekSquintLeft";
                m_BlendShapeNames[7] = "CheekSquintRight";
                m_BlendShapeNames[8] = "EyeBlinkLeft";
                m_BlendShapeNames[9] = "EyeBlinkRight";
                m_BlendShapeNames[10] = "EyeLookDownLeft";
                m_BlendShapeNames[11] = "EyeLookDownRight";
                m_BlendShapeNames[12] = "EyeLookInLeft";
                m_BlendShapeNames[13] = "EyeLookInRight";
                m_BlendShapeNames[14] = "EyeLookOutLeft";
                m_BlendShapeNames[15] = "EyeLookOutRight";
                m_BlendShapeNames[16] = "EyeLookUpLeft";
                m_BlendShapeNames[17] = "EyeLookUpRight";
                m_BlendShapeNames[18] = "EyeSquintLeft";
                m_BlendShapeNames[19] = "EyeSquintRight";
                m_BlendShapeNames[20] = "EyeWideLeft";
                m_BlendShapeNames[21] = "EyeWideRight";
                m_BlendShapeNames[22] = "JawForward";
                m_BlendShapeNames[23] = "JawLeft";
                m_BlendShapeNames[24] = "JawOpen";
                m_BlendShapeNames[25] = "JawRight";
                m_BlendShapeNames[26] = "MouthClose";
                m_BlendShapeNames[27] = "MouthDimpleLeft";
                m_BlendShapeNames[28] = "MouthDimpleRight";
                m_BlendShapeNames[29] = "MouthFrownLeft";
                m_BlendShapeNames[30] = "MouthFrownRight";
                m_BlendShapeNames[31] = "MouthFunnel";
                m_BlendShapeNames[32] = "MouthLeft";
                m_BlendShapeNames[33] = "MouthLowerDownLeft";
                m_BlendShapeNames[34] = "MouthLowerDownRight";
                m_BlendShapeNames[35] = "MouthPressLeft";
                m_BlendShapeNames[36] = "MouthPressRight";
                m_BlendShapeNames[37] = "MouthPucker";
                m_BlendShapeNames[38] = "MouthRight";
                m_BlendShapeNames[39] = "MouthRollLower";
                m_BlendShapeNames[40] = "MouthRollUpper";
                m_BlendShapeNames[41] = "MouthShrugLower";
                m_BlendShapeNames[42] = "MouthShrugUpper";
                m_BlendShapeNames[43] = "MouthSmileLeft";
                m_BlendShapeNames[44] = "MouthSmileRight";
                m_BlendShapeNames[45] = "MouthStretchLeft";
                m_BlendShapeNames[46] = "MouthStretchRight";
                m_BlendShapeNames[47] = "MouthUpperUpLeft";
                m_BlendShapeNames[48] = "MouthUpperUpRight";
                m_BlendShapeNames[49] = "NoseSneerLeft";
                m_BlendShapeNames[50] = "NoseSneerRight";
                m_BlendShapeNames[51] = "TongueOut";
            }

            public Name_FaceBlendShapesARKit Clone()
            {
                return DRBaseFunc.Clone(this);
            }
        }


    }//end class

}

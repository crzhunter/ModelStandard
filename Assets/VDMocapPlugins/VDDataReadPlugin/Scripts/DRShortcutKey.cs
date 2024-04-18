//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;


namespace VDDataRead
{
    /// <summary>
    /// 
    /// </summary>
    public enum enum_ShortcutKeyEven
    {
        NULL = -1,

        // ActorChoose
        Actor0,
        Actor1,
        Actor2,
        Actor3,
        Actor4,
        Actor5,
        Actor6,
        Actor7,
        Actor8,
        Actor9,

        // Set Actor Hide or Show
        ActorHideShow,

        //
        ModelSignHideShow,

        //
        ModelPositionToZero,
        ModelPositionToOrigin,
        SetModelCurrentPositionAsOrigin,
        LockModelPlanePosition,

    }


    /// <summary>
    /// 
    /// </summary>
    public class DRShortcutKey
    {
        public const int m_ActorNumMax = 9;
        static bool m_IsInitial = false;
        // 快捷键字典
        static Dictionary<enum_ShortcutKeyEven, KeyCode[][]> m_DicShortcutKey = new Dictionary<enum_ShortcutKeyEven, KeyCode[][]>();


        //
        static void Initial()
        {
            if (!m_IsInitial)
            {
                m_IsInitial = true;
                // 快捷键字典赋默认值
                SetShortcutKeyDefault();
            }
        }


        // 快捷键字典赋默认值
        static void SetShortcutKeyDefault()
        {
            // ActorChoose
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor0] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F1 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor1] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F2 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor2] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F3 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor3] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F4 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor4] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F5 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor5] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F6 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor6] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F7 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor7] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F8 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor8] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F9 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.Actor9] = new KeyCode[1][] { new KeyCode[1] { KeyCode.F10 } };

            // ActorChoosedHide
            m_DicShortcutKey[enum_ShortcutKeyEven.ActorHideShow] = new KeyCode[m_ActorNumMax][];
            int nn = 0;
            for (int i = 0; i < m_ActorNumMax; i++)
            {
                nn = m_DicShortcutKey[(enum_ShortcutKeyEven)((int)enum_ShortcutKeyEven.Actor0 + i)][0].Length;
                m_DicShortcutKey[enum_ShortcutKeyEven.ActorHideShow][i] = new KeyCode[nn + 1];
                for (int j = 0; j < nn; j++)
                {
                    m_DicShortcutKey[enum_ShortcutKeyEven.ActorHideShow][i][j] = m_DicShortcutKey[(enum_ShortcutKeyEven)((int)enum_ShortcutKeyEven.Actor0 + i)][0][j];
                }
                m_DicShortcutKey[enum_ShortcutKeyEven.ActorHideShow][i][nn] = KeyCode.Alpha0;
            }
            
            // Model Sign Hide or Show
            m_DicShortcutKey[enum_ShortcutKeyEven.ModelSignHideShow] = new KeyCode[2][] { new KeyCode[2] { KeyCode.LeftShift, KeyCode.M }, new KeyCode[2] { KeyCode.RightShift, KeyCode.M } };
            //
            m_DicShortcutKey[enum_ShortcutKeyEven.ModelPositionToZero] = new KeyCode[2][] { new KeyCode[2] { KeyCode.LeftShift, KeyCode.Alpha0 }, new KeyCode[2] { KeyCode.RightShift, KeyCode.Alpha0 } };
            m_DicShortcutKey[enum_ShortcutKeyEven.ModelPositionToOrigin] = new KeyCode[2][] { new KeyCode[2] { KeyCode.LeftShift, KeyCode.O }, new KeyCode[2] { KeyCode.RightShift, KeyCode.O } };
            m_DicShortcutKey[enum_ShortcutKeyEven.SetModelCurrentPositionAsOrigin] = new KeyCode[2][] { new KeyCode[2] { KeyCode.LeftShift, KeyCode.P }, new KeyCode[2] { KeyCode.RightShift, KeyCode.P } };
            m_DicShortcutKey[enum_ShortcutKeyEven.LockModelPlanePosition] = new KeyCode[2][] { new KeyCode[2] { KeyCode.LeftShift, KeyCode.L }, new KeyCode[2] { KeyCode.RightShift, KeyCode.L } };
        }

        //
        static bool JudgeKey(KeyCode[][] key)
        {
            bool isGetKey = false;
            int row = key.Length;
            for (int i = 0; i < row; i++)
            {
                int col = key[i].Length;
                if (col < 1) { continue; }

                bool isOk = true;
                for (int j = 0; j < col - 1; j++)
                {
                    isOk = isOk && Input.GetKey(key[i][j]);
                    if (!isOk) { break; }
                }
                if (isOk && Input.GetKeyDown(key[i][col - 1]))
                {
                    isGetKey = true;
                    break;
                }
            }
            return isGetKey;
        }

        //
        public static bool JudgeKey(enum_ShortcutKeyEven key)
        {
            Initial();
            if (!m_DicShortcutKey.ContainsKey(key)) { return false; }
            return JudgeKey(m_DicShortcutKey[key]);
        }

        //可同时掩藏或激活多个角色
        public static bool JudgeActorHideOrShow(out List<int> actors)
        {
            Initial();
            actors = new List<int>();

            bool isGetKey = false;
            KeyCode[][] key = m_DicShortcutKey[enum_ShortcutKeyEven.ActorHideShow];
            int row = key.Length;
            for (int i = 0; i < row; i++)
            {
                int col = key[i].Length;
                if (col < 1) { continue; }

                bool isOk = true;
                for (int j = 0; j < col - 1; j++)
                {
                    isOk = isOk && Input.GetKey(key[i][j]);
                    if (!isOk) { break; }
                }

                if (isOk)
                {
                    isGetKey = true;
                    actors.Add(i);
                }
            }

            //
            if (isGetKey) { return Input.GetKeyDown(key[0][key[0].Length - 1]); }
            return false;
        }

    }//end class

}//end namespace








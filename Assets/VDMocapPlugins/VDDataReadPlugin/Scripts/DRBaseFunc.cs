//-----------------------------------------------------------------------------
// Copyright 2019-2020 VIRDYN Ltd.  All rights reserved.
// Author: qx.
//-----------------------------------------------------------------------------

using System;
using UnityEngine;


namespace VDDataRead
{
    public class DRBaseFunc : MonoBehaviour
    {
        /// <summary>
        /// 在 obj 上挂载脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="script"></param>
        public static T AddScript<T>(GameObject obj, bool isOnlyOne = false) where T : Component
        {
            T script = obj.transform.GetComponent<T>();
            if (script == null || !isOnlyOne)
            {
                script = obj.AddComponent<T>();
            }
            return script;
        }

        /// <summary>
        /// 深度克隆。
        /// 结构体及类的克隆要在结构体或类的上面（即struct/class上面）加上 [Serializable] 。
        /// 若存在Unity定义的变量（Vector3，Quaternion 等），则不能使用，因为它们不是系列化的，且因为是Unity内部定义的结构体或类，也不能加[Serializable]进行系列化。 
        /// 关于系列化，只是需要系列化里面的数值，而里面的方法确是可以用到Unity定义的变量的。
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new System.ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (System.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream stream = new System.IO.MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

    }//end class DRBaseFunc


    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class MyExtensions
    {
        /// <summary>
        /// StringComparison.OrdinalIgnoreCase 指定查找过程忽略大小写。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comparisonType">StringComparison.OrdinalIgnoreCase 指定查找过程忽略大小写</param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
        {
            return (source.IndexOf(toCheck, comparisonType) >= 0);
        }
    }//end class MyExtensions

}//end namespace VDDataRead

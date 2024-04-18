using UnityEngine;
using UnityEngine.EventSystems;

namespace amfaceh
{
    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject obj);
        public VoidDelegate onClick;
        public VoidDelegate onDown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static EventTriggerListener Get(GameObject obj)
        {
            EventTriggerListener lister = obj.GetComponent<EventTriggerListener>();
            if (lister == null)
            {
                lister = obj.AddComponent<EventTriggerListener>();
            }
            return lister;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick(gameObject);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
            }
        }

    }//end class
}//end namespace
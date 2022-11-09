using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KittyHelpYouOut
{
    [AddComponentMenu("KittyHelpYouOut/UIDrag")]
    public class UIDrag : MonoBehaviour,IDragHandler,IPointerDownHandler
    {

        private RectTransform Rect;
        private Vector2 Offset;
        //================================================================================
        private void Start()
        {
            Rect = this.GetComponent<RectTransform>();
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            Rect.anchoredPosition = eventData.position+Offset;
        }

        //================================================================================
        public void OnPointerDown(PointerEventData eventData)
        {
            Offset = Rect.anchoredPosition-eventData.position;
        }
    }
}
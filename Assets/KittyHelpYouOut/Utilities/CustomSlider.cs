using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace KittyHelpYouOut
{
    [AddComponentMenu("KittyHelpYouOut/CustomSlider")]
	public class CustomSlider:Slider
	{
        public Action<float> OnDragAction;
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            OnDragAction?.Invoke(value);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnDragAction?.Invoke(value);
        }
    }
}
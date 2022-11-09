using UnityEngine;
using UnityEngine.UI;

namespace Defy.MonoBehavior
{
    public class AmmoBarController : MonoBehaviour
    {
        private Slider _AmmoBarSlider;
        private Slider AmmoBarSlider
        {
            get
            {
                _AmmoBarSlider ??= this.GetComponent<Slider>();
                return _AmmoBarSlider;
            }
        }
        private Image _FillImage;
        private Image FillImage
        {
            get
            {
                _FillImage ??= AmmoBarSlider.fillRect.GetComponent<Image>();
                return _FillImage;
            }
        }
                 

        public void SetMaxAmmo( int max)
        {
            AmmoBarSlider.maxValue = max;
            FillImage.material.SetTextureScale("_MainTex",new Vector2(max,1));
        }

        public void SetCurAmmo(int cur)
        {
            AmmoBarSlider.value = cur;
        }

    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class ResourceCounterUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public void DisplayIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public void DisplayValue(int value)
        {
            string suffix = "";
            
            if (value > 1000)
            {
                value /= 1000;
                suffix = "K";
            }
            
            if (value > 1000)
            {
                value /= 1000;
                suffix = "M";
            }
            
            _text.text = value + suffix;
        }
    }
}
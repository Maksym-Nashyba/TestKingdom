using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class ResourceCounterUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public void Display(ResourceCount resourceCount)
        {
            Sprite icon = SystemLocator.I.ContentLibrary.ResourceIcons[(int)resourceCount.Type];
            _icon.sprite = icon;
            
            string suffix = "";
            if (resourceCount.Count > 1000)
            {
                resourceCount.Count /= 1000;
                suffix = "K";
            }
            if (resourceCount.Count > 1000)
            {
                resourceCount.Count /= 1000;
                suffix = "M";
            }
            _text.text = resourceCount.Count + suffix;
        }
    }
}
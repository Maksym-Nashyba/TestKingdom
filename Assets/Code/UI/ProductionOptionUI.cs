using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class ProductionOptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        
        private Action _callback;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }
        
        private void OnClicked()
        {
            _callback?.Invoke();
            _callback = null;
        }
        
        public void Display(ProductionOrder order, Action onSelected)
        {
            _callback = onSelected;
            
            _nameText.text = order.DisplayName;
            _icon.sprite = order.DisplayIcon;
        }
    }
}
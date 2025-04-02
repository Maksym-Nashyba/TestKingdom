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
        [SerializeField] private GameObject _isSelectedPanel;
        [SerializeField] private GameObject _requiredLevelPanel;
        [SerializeField] private TextMeshProUGUI _requiredLevelText;
        
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
        
        public void Display(ProductionOrder order, bool isSelected, Action onSelected)
        {
            _callback = onSelected;
            
            _button.interactable = !isSelected;
            _isSelectedPanel.SetActive(isSelected);
                
            _nameText.text = order.DisplayName;
            _icon.sprite = order.DisplayIcon;
        }

        public void DisplayInsufficientLevel(int requiredLevel)
        {
            _requiredLevelPanel.SetActive(true);
            _button.interactable = false;
            _requiredLevelText.text = $"Level {requiredLevel} Required";
        }
    }
}
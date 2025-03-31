using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class BuildOptionUI : MonoBehaviour
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

        public void Display(BuildingLine building, Action onSelected)
        {
            _callback = onSelected;
            
            _nameText.text = building.DisplayName;
            _descriptionText.text = building.DisplayDescription;
            _icon.sprite = building.BuyMenuIcon;
        }

        private void OnClicked()
        {
            _callback?.Invoke();
            _callback = null;
        }
    }
}
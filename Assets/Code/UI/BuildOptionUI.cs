﻿using System;
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
        [SerializeField] private Transform _inputsPool;

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

        public void Display(BuildingLine building, Action onSelected)
        {
            _callback = onSelected;
            
            _nameText.text = building.DisplayName;
            _descriptionText.text = building.DisplayDescription;
            _icon.sprite = building.BuyMenuIcon;
            
            foreach (ResourceCount resourceCount in building.Levels[0].Cost)
            {
                ResourceCounterUI counterUI = Instantiate(SystemLocator.I.ContentLibrary.ResourceCounterUIPrefab, _inputsPool);
                counterUI.Display(resourceCount);
            }
        }
    }
}
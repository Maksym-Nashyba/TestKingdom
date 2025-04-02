using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class BuildPanel : MonoBehaviour
    {
        [SerializeField] private Transform _optionPool;
        [SerializeField] private Button _cancelButton;
        
        private readonly List<BuildOptionUI> _optionUIs = new();
        private Action<string> _onOptionSelected;
        private Action _onCanceled;

        private void Awake()
        {
            _cancelButton.onClick.AddListener(OnCancelButton);
        }

        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(OnCancelButton);
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
        }

        public void Display(string[] optionTypeIds, Action<string> onOptionSelected, Action onCanceled)
        {
            _onOptionSelected = onOptionSelected;
            _onCanceled = onCanceled;

            foreach (string optionId in optionTypeIds)
            {
                BuildingLine building = SystemLocator.I.ContentLibrary.GetBuilding(optionId);
                BuildOptionUI optionUI = Instantiate(SystemLocator.I.ContentLibrary.BuildOptionUIPrefab, _optionPool);
                optionUI.Display(building, () => OnOptionSelected(optionId));
                _optionUIs.Add(optionUI);
            }
            
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            gameObject.SetActive(true);
        }

        private void Reset()
        {
            foreach (BuildOptionUI optionUI in _optionUIs)
            {
                Destroy(optionUI.gameObject);
            }
            
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
            _onOptionSelected = null;
            _onCanceled = null;
            _optionUIs.Clear();
        }

        private void OnCancelButton()
        {
            if (_onCanceled == null) return;
            
            Action temp = _onCanceled;
            Reset();
            Hide();
            temp?.Invoke();
        }

        private void OnCellUnselected(Cell obj)
        {
            Reset();
            Hide();
        }

        private void OnOptionSelected(string optionId)
        {
            Action<string> temp = _onOptionSelected;
            Reset();
            Hide();
            temp?.Invoke(optionId);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
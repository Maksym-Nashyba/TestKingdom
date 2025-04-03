using System;
using Code.Visualization;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class LeftPanel : MonoBehaviour
    {
        [SerializeField] private Image _panel;
        
        private void Awake()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
        }

        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            SystemLocator.I.PlayerController.CellSelected -= OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
        }

        private void OnCellSelected(Cell cell) => Show();

        private void OnCellUnselected(Cell cell) => Hide();
        
        private void Show()
        {
            _panel.enabled = true;
        }        
        
        private void Hide()
        {
            _panel.enabled = false;
        }
    }
}
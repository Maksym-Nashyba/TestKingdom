using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    internal sealed class ProductionPanel : MonoBehaviour
    {
        [SerializeField] private Transform _optionHolder;
        
        private readonly List<ProductionOptionUI> _optionUIs = new();
        private Cell _currentCell;
        
        private void Awake()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            SystemLocator.I.Map.CellChanged += OnCellChanged;
        }
        
        private void Start() => Hide();

        private void OnDestroy()
        {
            SystemLocator.I.PlayerController.CellSelected -= OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
            SystemLocator.I.Map.CellChanged -= OnCellChanged;
        }

        private void OnCellSelected(Cell cell)
        {
            _currentCell = cell;
            if (!SystemLocator.I.Map.CanProduce(cell)) return;
            
            UpdatePanel(_currentCell);
            Show();
        }

        private void OnCellUnselected(Cell obj)
        {
            _currentCell = null;
            Hide();
        }

        private void OnCellChanged(Cell cell)
        {
            if (cell != _currentCell) return;
            if (!SystemLocator.I.Map.CanProduce(cell)) return;
            
            UpdatePanel(_currentCell);
            Show();
        }

        private void UpdatePanel(Cell cell)
        {
            Reset();
            
            SystemLocator.I.Map.GetBuildingData(cell, out BuildingData buildingData);
            IEnumerable<ProductionOrder> productionOrders = SystemLocator.I.ContentLibrary.EnumerateOrders(buildingData.TypeId);
            foreach (ProductionOrder order in productionOrders)
            {
                ProductionOptionUI orderUI = Instantiate(SystemLocator.I.ContentLibrary.ProductionOptionUIPrefab, _optionHolder);
                orderUI.Display(order, () => OnOptionSelected(order));
                _optionUIs.Add(orderUI);
            }
        }

        private void Reset()
        {
            foreach (ProductionOptionUI optionUI in _optionUIs)
            {
                Destroy(optionUI.gameObject);
            }
            _optionUIs.Clear();
        }

        private void OnOptionSelected(ProductionOrder order)
        {
            SystemLocator.I.Map.StartProductionOrder(_currentCell, order);
            UpdatePanel(_currentCell);
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
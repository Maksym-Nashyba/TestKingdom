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
            
            CellData cellData = SystemLocator.I.Map.GetCellData(cell);
            BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(cellData.Building.TypeId);
            for (var i = 0; i < buildingLine.Levels.Length; i++)
            {
                foreach (ProductionOrder order in buildingLine.Levels[i].ProductionOrders)
                {
                    ProductionOptionUI orderUI = Instantiate(SystemLocator.I.ContentLibrary.ProductionOptionUIPrefab, _optionHolder);

                    bool isSelected = cellData.HasOrder && cellData.Order.TypeId == order.Id;
                    orderUI.Display(order, isSelected, () => OnOptionSelected(order));
                    
                    if (i+1 > cellData.Building.Level) orderUI.DisplayInsufficientLevel(i+1);
                    
                    _optionUIs.Add(orderUI);
                }
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
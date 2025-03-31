using System;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    internal sealed class CellInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;

        private void Start()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            Hide();
        }

        private void OnDestroy()
        {
            SystemLocator.I.PlayerController.CellSelected -= OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
        }

        private void OnCellSelected(Cell cell) => Display(cell);

        private void OnCellUnselected(Cell cell) => Hide();

        private void Display(Cell cell)
        {
            Vector2Int position = SystemLocator.I.Map.GetPosition(cell);
            bool cellHasBuilding = SystemLocator.I.PlayerData.GetBuildingData(position, out BuildingData buildingData);
            
            _nameText.text = cellHasBuilding 
                ? SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId).DisplayName
                : "Empty Cell"; 
            
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
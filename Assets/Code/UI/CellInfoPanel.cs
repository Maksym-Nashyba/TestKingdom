using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class CellInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _buildButton;
        [SerializeField] private Button _demolishButton;
        [SerializeField] private Button _upgradeButton;
        
        [SerializeField] private BuildPanel _buildPanel;
        
        private Cell _currentCell;

        private void Awake()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            _buildButton.onClick.AddListener(OnBuildButton);
            _demolishButton.onClick.AddListener(OnDemolishButton);
        }

        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            SystemLocator.I.PlayerController.CellSelected -= OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected -= OnCellUnselected;
            _buildButton.onClick.RemoveListener(OnBuildButton);
            _demolishButton.onClick.RemoveListener(OnDemolishButton);
        }

        private void OnBuildButton()
        {
            _buildPanel.Display(
                new []{"Farm"},
                selectedOptionId =>
                {
                    SystemLocator.I.Map.BuildBuilding(_currentCell, selectedOptionId);
                    UpdateDisplayValues(_currentCell);
                    Show();
                });
            Hide();
        }

        private void OnDemolishButton()
        {
            SystemLocator.I.Map.DemolishBuilding(_currentCell);
            UpdateDisplayValues(_currentCell);
        }
        
        private void OnCellSelected(Cell cell) => Display(cell);

        private void OnCellUnselected(Cell cell) => Hide();
        
        private void Display(Cell cell)
        {
            _currentCell = cell;
            UpdateDisplayValues(cell);
            Show();
        }

        private void UpdateDisplayValues(Cell cell)
        {
            Vector2Int position = SystemLocator.I.Map.GetPosition(cell);
            bool cellHasBuilding = SystemLocator.I.PlayerData.GetBuildingData(position, out BuildingData buildingData);
            
            _nameText.text = cellHasBuilding 
                ? SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId).DisplayName
                : "Empty Cell";

            _levelText.text = cellHasBuilding
                ? $"Level: {buildingData.Level}"
                : "";

            _buildButton.interactable = SystemLocator.I.Map.CanBuild(cell);
            _upgradeButton.interactable = SystemLocator.I.Map.CanBeUpgraded(cell);
            _demolishButton.interactable = SystemLocator.I.Map.CanDemolish(cell);
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
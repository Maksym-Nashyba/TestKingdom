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
        [SerializeField] private ResourceCounterUI[] _upradeResourceCounters;
        [SerializeField] private ResourceCounterUI[] _demolishResourceCounters;
        
        [SerializeField] private BuildPanel _buildPanel;
        
        private Cell _currentCell;

        private void Awake()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            SystemLocator.I.Map.CellChanged += OnCellChanged;
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
            SystemLocator.I.Map.CellChanged -= OnCellChanged;
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
                    Show();
                }, Show);
            Hide();
        }

        private void OnDemolishButton()
        {
            SystemLocator.I.Map.DemolishBuilding(_currentCell);
            UpdatePanel(_currentCell);
        }

        private void OnCellSelected(Cell cell)
        {
            _currentCell = cell;
            UpdatePanel(cell);
            Show();
        }

        private void OnCellUnselected(Cell cell) => Hide();

        private void OnCellChanged(Cell cell)
        {
            if (_currentCell != cell) return;
            
            UpdatePanel(cell);
        }

        private void UpdatePanel(Cell cell)
        {
            bool cellHasBuilding = SystemLocator.I.Map.GetBuildingData(cell, out BuildingData buildingData);
            
            _nameText.text = cellHasBuilding 
                ? SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId).DisplayName
                : "Empty Cell";

            _levelText.text = cellHasBuilding
                ? $"Level: {buildingData.Level+1}"
                : "";

            _buildButton.interactable = SystemLocator.I.Map.CanBuild(cell);
            _upgradeButton.interactable = SystemLocator.I.Map.CanBeUpgraded(cell);
            _demolishButton.interactable = SystemLocator.I.Map.CanDemolish(cell);

            foreach (ResourceCounterUI counter in _demolishResourceCounters) counter.gameObject.SetActive(false);
            foreach (ResourceCounterUI counter in _upradeResourceCounters) counter.gameObject.SetActive(false);
            
            if (!cellHasBuilding) return;
            
            BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId);

            for (var i = 0; i < Mathf.Min(_demolishResourceCounters.Length, buildingLine.Levels[buildingData.Level].Cost.Length); i++)
            {
                _demolishResourceCounters[i].gameObject.SetActive(true);
                _demolishResourceCounters[i].Display(buildingLine.Levels[buildingData.Level].Cost[i]);
            }

            if (buildingLine.Levels.Length >= buildingData.Level + 2)
            {
                for (var i = 0; i < Mathf.Min(_upradeResourceCounters.Length, buildingLine.Levels[buildingData.Level+1].Cost.Length); i++)
                {
                    _upradeResourceCounters[i].gameObject.SetActive(true);
                    _upradeResourceCounters[i].Display(buildingLine.Levels[buildingData.Level+1].Cost[i]);
                }
            }
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
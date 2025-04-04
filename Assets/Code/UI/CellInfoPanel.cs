using System.Linq;
using Code.Visualization;
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
        [SerializeField] private GameObject _maxLevelPanel;
        [SerializeField] private GameObject _castlePanel;
        [SerializeField] private GameObject _castleLowPanel;
        
        [SerializeField] private BuildPanel _buildPanel;
        
        private Cell _currentCell;

        private void Awake()
        {
            SystemLocator.I.PlayerController.CellSelected += OnCellSelected;
            SystemLocator.I.PlayerController.CellUnselected += OnCellUnselected;
            SystemLocator.I.Game.CellChanged += OnCellChanged;
            _buildButton.onClick.AddListener(OnBuildButton);
            _upgradeButton.onClick.AddListener(OnUpgradeButton);
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
            SystemLocator.I.Game.CellChanged -= OnCellChanged;
            _buildButton.onClick.RemoveListener(OnBuildButton);
            _upgradeButton.onClick.RemoveListener(OnUpgradeButton);
            _demolishButton.onClick.RemoveListener(OnDemolishButton);
        }

        private void OnBuildButton()
        {
            _buildPanel.Display(
                SystemLocator.I.ContentLibrary.Buildings.Where(b => b.CanBeBuilt),
                selectedOptionId =>
                {
                    SystemLocator.I.Game.BuildBuilding(_currentCell, selectedOptionId);
                    Show();
                }, Show);
            Hide();
        }

        private void OnUpgradeButton()
        {
            SystemLocator.I.Game.UpgradeBuilding(_currentCell);
        }

        private void OnDemolishButton()
        {
            SystemLocator.I.Game.DemolishBuilding(_currentCell);
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
            bool cellHasBuilding = SystemLocator.I.Game.GetBuildingData(cell, out BuildingData buildingData);
            
            _nameText.text = cellHasBuilding 
                ? SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId).DisplayName
                : "Empty Cell";

            _levelText.text = cellHasBuilding
                ? $"Level: {buildingData.Level+1}"
                : "";

            _buildButton.interactable = SystemLocator.I.Game.CanBuild(cell);
            
            Game.CanUpgradeResult canUpgrade = SystemLocator.I.Game.CanUpgrade(cell);
            _upgradeButton.interactable = canUpgrade == Game.CanUpgradeResult.Ok;
            _maxLevelPanel.SetActive(canUpgrade == Game.CanUpgradeResult.MaxLevel);
            _castleLowPanel.SetActive(canUpgrade == Game.CanUpgradeResult.CastleTooLow);
            
            Game.CanDemolishResult canDemolish = SystemLocator.I.Game.CanDemolish(cell);
            _demolishButton.interactable = canDemolish == Game.CanDemolishResult.Ok;
            _castlePanel.SetActive(canDemolish == Game.CanDemolishResult.Castle);

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
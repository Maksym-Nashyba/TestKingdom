using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal sealed class RightPanel : MonoBehaviour
    {
        [SerializeField] private Image _panel;
        
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
            if (!SystemLocator.I.Map.CanProduce(cell)) return;
            Show();
        }

        private void OnCellUnselected(Cell obj)
        {
            Hide();
        }

        private void OnCellChanged(Cell cell)
        {
            if (!SystemLocator.I.Map.CanProduce(cell)) return;
            Show();
        }
        
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
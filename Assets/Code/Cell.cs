using System;
using UnityEngine;

namespace Code
{
    internal sealed class Cell : MonoBehaviour
    {
        [SerializeField] private float _hoverRiseHeight = 0.1f;
        [SerializeField] private float _hoverAnimationDuration = 0.2f;
        [SerializeField] private MeshRenderer _progressRenderer;
        
        private static readonly int ValueNormalizedProperty = Shader.PropertyToID("_ValueNormalized");
        private float _progress;
        
        private Transform _transform;
        private GameObject _buildingView;
        
        private Vector3 _originalPosition;
        private SelectionState _selectionState;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            _originalPosition = _transform.position;
        }

        private void Update()
        {
            Vector3 targetPosition = _selectionState switch
            {
                SelectionState.Default => _originalPosition,
                SelectionState.Hovered => _originalPosition + (Vector3.up*_hoverRiseHeight),
                SelectionState.Selected => _originalPosition + (Vector3.up*_hoverRiseHeight),
                _ => throw new ArgumentOutOfRangeException()
            };
            _transform.position = Vector3.Lerp(_transform.position, targetPosition, (1f/_hoverAnimationDuration) * Time.deltaTime);
            
            if (_progress > float.Epsilon) _progressRenderer.enabled = true;
            _progressRenderer.material.SetFloat(ValueNormalizedProperty, _progress);
        }

        public void SetProductionProgress(float progress)
        {
            _progress = progress;
        }
        
        public void SetSelectionState(SelectionState selectionState)
        {
            _selectionState = selectionState;
        }

        public void DisplayBuildingView(BuildingData buildingData)
        {
            if (_buildingView != null) Destroy(_buildingView.gameObject);
            
            BuildingLine buildingLine = SystemLocator.I.ContentLibrary.GetBuilding(buildingData.TypeId);
            GameObject prefab = buildingLine.Levels[buildingData.Level].ViewPrefab;
            _buildingView = Instantiate(prefab, _transform);
            _buildingView.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        
        public void DemolishBuildingView()
        {
            Destroy(_buildingView);
        }
    }
}
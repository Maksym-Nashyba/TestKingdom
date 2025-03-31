using System;
using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Camera))]
    internal sealed class PlayerController : MonoBehaviour
    {
        public event Action<Cell> CellSelected;
        public event Action<Cell> CellUnselected;
        
        [SerializeField] private float _height;
        [SerializeField] private float _angle;
        
        private readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        private Camera _camera;
        private Transform _transform;
        
        private Vector2 _dragStartMousePosition;
        private Pose _dragStartCameraPose;
        private bool _cellSelected;
        private Cell _targetCell;
        private Cell _lastHoveredCell;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _transform.position = new Vector3(0f, _height, -10f);
        }

        private void Update()
        {
            if (_cellSelected)
            {
                Vector3 targetPosition = _targetCell.transform.position + new Vector3(0f, _height*0.8f, -10f);
                _transform.position = Vector3.Lerp(_transform.position, targetPosition, 0.5f);
                
                if (Input.GetKeyDown(KeyCode.Escape)) ClearSelection();
            }
            else
            {
                bool hoversOverCell = TryPickCell(Input.mousePosition, out Cell hoveredCell);
                
                if (_lastHoveredCell != hoveredCell) //change selection if necessary
                {
                    _lastHoveredCell?.SetSelectionState(SelectionState.Default);
                    hoveredCell?.SetSelectionState(SelectionState.Hovered);
                    _lastHoveredCell = hoveredCell;
                }
                
                if (Input.GetMouseButtonDown(0) && hoversOverCell) SelectCell(hoveredCell);
                
                if (Input.GetMouseButtonDown(1)) RestartDrag();

                Vector3 targetPosition = Input.GetMouseButton(1) //move camera if dragged
                    ? TargetPositionAfterDrag()
                    : new Vector3(_transform.position.x, _height, _transform.position.z);
                _transform.position = Vector3.Lerp(_transform.position, targetPosition, 0.5f);
            }
        }

        private void SelectCell(Cell cell)
        {
            _cellSelected = true;
            _targetCell = cell;
            CellSelected?.Invoke(cell);
        }

        private void ClearSelection()
        {
            _cellSelected = false;
            Cell temp = _targetCell;
            _targetCell = null;
            CellUnselected?.Invoke(temp);
        }

        private void RestartDrag()
        {
            _dragStartMousePosition = Input.mousePosition;
            _dragStartCameraPose = new Pose(_transform.position, _transform.rotation);
        }

        private Vector3 TargetPositionAfterDrag()
        {
            Pose currentPose = new Pose(_transform.position, _transform.rotation);
            _transform.SetPositionAndRotation(_dragStartCameraPose.position, _dragStartCameraPose.rotation);
            Ray dragStartRay = _camera.ScreenPointToRay(_dragStartMousePosition);
            Ray currentCameraRay = _camera.ScreenPointToRay(Input.mousePosition);
            _transform.SetPositionAndRotation(currentPose.position, currentPose.rotation);
                    
            _groundPlane.Raycast(dragStartRay, out float dragStartDistance);
            _groundPlane.Raycast(currentCameraRay, out float currentDistance);
                    
            Vector3 dragStartIntersect = dragStartRay.origin + dragStartRay.direction * dragStartDistance;
            Vector3 currentIntersect = currentCameraRay.origin + currentCameraRay.direction * currentDistance;
                    
            Vector3 delta = currentIntersect - dragStartIntersect;
                    
            return dragStartRay.origin - delta;
        }
        
        private bool TryPickCell(Vector2 screenPosition, out Cell cell)
        {
            cell = null;
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            cell = hit.collider.GetComponent<Cell>();
            return cell != null;
        }
    }
}
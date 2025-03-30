using System;
using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Camera))]
    internal sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _height;
        [SerializeField] private float _angle;
        [SerializeField] private float _tilesTravelPerScreenSwipe;
        
        private readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        private Camera _camera;
        private Vector2 _dragStartMousePosition;
        private Pose _dragStartCameraPose;
        
        private bool _locked;
        private Cell _targetCell;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            transform.position = new Vector3(0f, _height, -10f);
        }

        private void Update()
        {
            Vector3 targetPosition = transform.position;
            
            if (_locked)
            {
                targetPosition = _targetCell.transform.position + new Vector3(0f, _height*0.8f, -10f);
                
                if (Input.GetKeyDown(KeyCode.Escape)) Unlock();
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && TryPickCell(Input.mousePosition, out Cell cell))
                {
                    LockOnCell(cell);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    _dragStartMousePosition = Input.mousePosition;
                    _dragStartCameraPose = new Pose(transform.position, transform.rotation);
                }
                
                if (Input.GetMouseButton(1))
                {
                    Pose currentPose = new Pose(transform.position, transform.rotation);
                    transform.SetPositionAndRotation(_dragStartCameraPose.position, _dragStartCameraPose.rotation);
                    Ray dragStartRay = _camera.ScreenPointToRay(_dragStartMousePosition);
                    Ray currentCameraRay = _camera.ScreenPointToRay(Input.mousePosition);
                    transform.SetPositionAndRotation(currentPose.position, currentPose.rotation);
                    
                    _groundPlane.Raycast(dragStartRay, out float dragStartDistance);
                    _groundPlane.Raycast(currentCameraRay, out float currentDistance);
                    
                    Vector3 dragStartIntersect = dragStartRay.origin + dragStartRay.direction * dragStartDistance;
                    Vector3 currentIntersect = currentCameraRay.origin + currentCameraRay.direction * currentDistance;
                    
                    Vector3 delta = currentIntersect - dragStartIntersect;
                    
                    targetPosition = dragStartRay.origin - delta;
                }
                targetPosition.y = _height;
            }
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
        }

        private void LockOnCell(Cell cell)
        {
            _locked = true;
            _targetCell = cell;
        }

        private void Unlock()
        {
            _locked = false;
            _targetCell = null;
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
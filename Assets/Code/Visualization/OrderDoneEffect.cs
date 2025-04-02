using TMPro;
using UnityEngine;

namespace Code.Visualization
{
    internal sealed class OrderDoneEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private float _effectDuration;
        [SerializeField] private float _travelDistance;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _startTime;
        
        public void SetUp(Vector3 startPosition, ResourceCount resourceCount)
        {
            _startPosition = startPosition;
            _targetPosition = _startPosition + Vector3.up * _travelDistance;
            _spriteRenderer.sprite = SystemLocator.I.ContentLibrary.ResourceIcons[(int)resourceCount.Type];
            _text.text = $"+{resourceCount.Count}";
            _startTime = Time.time;
        }
        
        private void Update()
        {
            float t = (Time.time - _startTime)/_effectDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 4f);
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, easedT);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f-t);
            _text.color = new Color(1f, 1f, 1f, 1f-t);
            
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
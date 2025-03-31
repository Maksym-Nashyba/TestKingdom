using System;
using UnityEngine;

namespace Code.UI
{
    internal sealed class ResourceDisplayUI : MonoBehaviour
    {
        [SerializeField] private float _updateFrequencySeconds = 1f;
        
        private ResourceCounterUI[] _resourceCounterUIs;
        private float _lastUpdateTime;

        private void Start()
        {
            _resourceCounterUIs = new ResourceCounterUI[Enum.GetValues(typeof(ResourceType)).Length];
            
            for (var i = 0; i < _resourceCounterUIs.Length; i++)
            {
                _resourceCounterUIs[i] = Instantiate(SystemLocator.I.ContentLibrary.ResourceCounterUIPrefab, transform);
                Sprite icon = SystemLocator.I.ContentLibrary.ResourceIcons[i];
                _resourceCounterUIs[i].DisplayIcon(icon);
            }
        }

        private void Update()
        {
            if (Time.time - _lastUpdateTime < _updateFrequencySeconds) return;

            for (var i = 0; i < _resourceCounterUIs.Length; i++)
            {
                int value = SystemLocator.I.PlayerData.GetResourceCount((ResourceType)i);
                _resourceCounterUIs[i].DisplayValue(value);
            }

            _lastUpdateTime = Time.time;
        }
    }
}
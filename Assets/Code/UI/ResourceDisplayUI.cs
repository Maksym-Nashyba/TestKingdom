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
            _resourceCounterUIs = new ResourceCounterUI[SystemLocator.I.GameData.Resources.Keys.Count];
            
            for (var i = 0; i < _resourceCounterUIs.Length; i++)
            {
                _resourceCounterUIs[i] = Instantiate(SystemLocator.I.ContentLibrary.ResourceCounterUIPrefab, transform);
            }
        }

        private void Update()
        {
            if (Time.time - _lastUpdateTime < _updateFrequencySeconds) return;

            foreach (ResourceType resourceType in SystemLocator.I.GameData.Resources.Keys)
            {
                _resourceCounterUIs[(int)resourceType].Display(new ResourceCount()
                {
                    Count = SystemLocator.I.GameData.Resources[resourceType],
                    Type = resourceType
                });
            }

            _lastUpdateTime = Time.time;
        }
    }
}
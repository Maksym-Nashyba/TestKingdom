using UnityEngine;

namespace Code.Visualization
{
    internal sealed class BuildingView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        public void PlayBuiltAnimation()
        {
            _animator.Play("Built");
        }
    }
}
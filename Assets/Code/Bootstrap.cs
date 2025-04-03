using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    internal sealed class Bootstrap : MonoBehaviour
    {
        /// This is here for a very dumb reason.
        /// Basically, if you load the main scene directly on engine startup, the UI breaks.
        /// I've never seen anything like this before and didn't have the time to debug.
        /// If I had to guess, it's a bug in the engine...
        
        private void Start()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
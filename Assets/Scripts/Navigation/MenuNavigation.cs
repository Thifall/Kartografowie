using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kartografowie
{
    public class MenuNavigation : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            // Logic to load a specific scene
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        public void ExitGame()
        {
            // Logic to exit the game
            Debug.Log("Exiting the game...");
            Application.Quit();
        }
    }
}

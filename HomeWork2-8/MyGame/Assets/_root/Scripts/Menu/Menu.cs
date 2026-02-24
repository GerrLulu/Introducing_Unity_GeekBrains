using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] Button _buttonStartGame;


        private void Start() => _buttonStartGame.onClick.AddListener(StartGame);

        private void OnDestroy() => _buttonStartGame.onClick.RemoveListener(StartGame);


        void StartGame() => SceneManager.LoadScene(1);

        void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
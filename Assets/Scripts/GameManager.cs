using UnityEngine;
using UnityEngine.SceneManagement;

// Gestiona la carga de escenas

namespace MazesAndMore
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        [SerializeField]
        LevelGroup levelGroup;
        int levelIndex = 0;

        // Booleano para controlar que el menu de NoHints solo se muestre una vez por ejecucion
        [HideInInspector]
        public bool noHintsShown = false;

        public static GameManager Instance
        {
            get { return _instance; }
        }

        // GameManager es un singleton
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Carga la escena sceneName
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        // Carga la escena "Level" con el nivel levelIndex del levelGroup indicado
        public void LoadLevel(int levelIndex, LevelGroup levelGroup)
        {
            this.levelIndex = levelIndex;
            this.levelGroup = levelGroup;
            LoadScene("Level");
        }

        public LevelGroup GetCurrentLevelGroup()
        {
            return levelGroup;
        }

        public int GetCurrentLevelIndex()
        {
            return levelIndex;
        }
    }
}

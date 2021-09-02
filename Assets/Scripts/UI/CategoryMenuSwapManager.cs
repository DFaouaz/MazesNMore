using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Cambia la interfaz para alternar entre el menu de "Seleccion de Categoria" y el de "Seleccion de Nivel"

namespace MazesAndMore
{
    public class CategoryMenuSwapManager : MonoBehaviour
    {
        public Text title;
        public GameObject categoriesPanel;
        public GameObject levelsPanel;
        public GameObject levelsGrid;
        public LevelButton levelButtonPrefab;

        private UserData userData;

        void Start()
        {
            if (categoriesPanel == null)
            {
                Debug.LogError("CategoriesPanel not asigned");
                return;
            }

            if (levelsPanel == null)
            {
                Debug.LogError("LevelsPanel not asigned");
                return;
            }

            ShowCategories();

            userData = DataManager.Instance.GetUserData();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (levelsPanel.activeSelf)
                    ShowCategories();
                else
                    GameManager.Instance.LoadScene("MainMenu");
            }
        }

        // Muestra el menu de Seleccion de Categorias
        public void ShowCategories()
        {
            // Texto
            title.text = "CATEGORIES";
            // Paneles
            levelsPanel.SetActive(false);
            categoriesPanel.SetActive(true);

            // Vaciamos el grid
            while (levelsGrid.transform.childCount >= 1)
                DestroyImmediate(levelsGrid.transform.GetChild(0).gameObject);
        }

        // Muestra el menu de Seleccion de Nivel, instancia los niveles de esa categoria
        // y actualiza su estado segun el UserData
        public void ShowLevels(LevelGroup group)
        {
            if (group == null) return;

            // Texto
            title.text = group.levelGroupName;
            // Paneles
            levelsPanel.SetActive(true);
            categoriesPanel.SetActive(false);
            // Instancia los LevelButtons
            int levelsCompleted = userData.levels_completed.ContainsKey(group.levelGroupName) ? userData.levels_completed[group.levelGroupName] : 0;
            for (int i = 0; i < group.levels.Length; i++)
            {
                LevelButton button = Instantiate(levelButtonPrefab, levelsGrid.transform);
                button.Configure(i, group);
                button.Unlock();

                if (i < levelsCompleted)
                    button.SetCompleted();
                else if (i > levelsCompleted)
                    button.Lock();
            }
        }
    }
}

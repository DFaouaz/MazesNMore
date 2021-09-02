using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gestiona la interfaz del CategoryButton, los textos del nombre y porcentaje de progreso
// de ese LevelGroup

namespace MazesAndMore
{
    [ExecuteInEditMode]
    public class Category : MonoBehaviour
    {
        private LevelGroup levelGroup;

        public Text nameText;
        public Text percentageText;
        public CustomButton customButton;

#if UNITY_EDITOR
        private void Update()
        {
            ConfigureCategory();
        }
#endif

        public void SetCategory(LevelGroup levelGroup)
        {
            this.levelGroup = levelGroup;
            ConfigureCategory();
        }

        // Actualiza el texto dependiendo del UserData
        private void ConfigureCategory()
        {
            if (levelGroup == null) return;
            nameText.text = levelGroup.levelGroupName;

            // actualiza el porcentaje de progreso de esa categoria
            if (DataManager.Instance.GetUserData().levels_completed.ContainsKey(levelGroup.levelGroupName))
            {
                percentageText.text = ((int)(100.0f * DataManager.Instance.GetUserData().levels_completed[levelGroup.levelGroupName]
                / levelGroup.levels.Length)).ToString() + "%";
            }
            else
                percentageText.text = 0.ToString() + "%";

            customButton.SetSprites(levelGroup.button, levelGroup.pressedButton);
        }
    }
}
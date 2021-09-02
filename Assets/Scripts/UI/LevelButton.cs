using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Boton del nivel que carga la escena Level cuando se le pulsa

namespace MazesAndMore
{
    [ExecuteInEditMode]
    public class LevelButton : MonoBehaviour
    {
        [Tooltip("Color que tiene el boton cuando no esta completado el nivel")]
        public Color defaultColor;
        [Tooltip("Color que tiene el candado cuando no esta completado el nivel")]
        public Color defaultLockColor;
        public Image lockImage;
        public Text text;
        public CustomButton button;

        private int levelIndex = -1;
        private LevelGroup levelGroup;

        public void Configure(int index, LevelGroup group)
        {
            levelIndex = index;
            levelGroup = group;
            Init();
        }

        private void Init()
        {
            button.target.color = defaultColor;
            text.color = defaultLockColor;
            text.text = (levelIndex + 1) > 0 ? (levelIndex + 1).ToString() : "";
        }

        // El nivel se ha completado
        public void SetCompleted()
        {
            text.color = defaultColor;
            button.target.color = levelGroup.color;
        }

        // Desbloquea el boton (si se pulsa, se hace el clickedEvent)
        public void Unlock()
        {
            text.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);
            button.clickedEvent.AddListener(LoadLevel);
        }

        // Bloquea el boton (si se pulsa, no hace nada)
        public void Lock()
        {
            text.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            button.clickedEvent.RemoveListener(LoadLevel);
        }

        private void LoadLevel()
        {
            GameManager.Instance.LoadLevel(levelIndex, levelGroup);
        }
    }
}

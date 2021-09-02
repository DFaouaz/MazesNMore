using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

// Componente que se encarga de actualizar el componente Button con los sprites
// y el evento clickedEvent

namespace MazesAndMore
{
    [RequireComponent(typeof(Button))]
    [ExecuteInEditMode]
    public class CustomButton : MonoBehaviour
    {
        public Button button = null;
        public Image target = null;

        public Sprite idleSprite;
        public Sprite pressedSprite;
        [Space]
        public ButtonClickedEvent clickedEvent;

#if !UNITY_EDITOR
    private void Awake()
    {
        ConfigurateButton();
        ConfigurateImage();
    }
#endif

#if UNITY_EDITOR
        void Update()
        {
            ConfigurateButton();
            ConfigurateImage();
        }
#endif

        private void ConfigurateImage()
        {
            if (target == null) return;

            target.sprite = idleSprite;
        }

        private void ConfigurateButton()
        {
            if (target != null)
                button.targetGraphic = target;

            button.transition = Selectable.Transition.SpriteSwap;
            SpriteState spriteState = button.spriteState;
            spriteState.pressedSprite = pressedSprite;
            spriteState.highlightedSprite = idleSprite;
            spriteState.selectedSprite = idleSprite;
            spriteState.disabledSprite = idleSprite;
            button.spriteState = spriteState;
            button.onClick = clickedEvent;

            //clickedEvent.RemoveAllListeners();
            clickedEvent.AddListener(() => { AudioManager.Instance.Play("Button"); });
        }

        public void PlayButtonSound()
        {
            AudioManager.Instance.Play("Button");
        }

        public void SetSprites(Sprite idleSprite, Sprite pressedSprite) // TODO: COPIAR CODIGO == KK
        {
            this.idleSprite = idleSprite;
            this.pressedSprite = pressedSprite;

            SpriteState spriteState = button.spriteState;
            spriteState.pressedSprite = pressedSprite;
            spriteState.highlightedSprite = idleSprite;
            spriteState.selectedSprite = idleSprite;
            spriteState.disabledSprite = idleSprite;
            button.spriteState = spriteState;

            ConfigurateImage();
        }

    }
}

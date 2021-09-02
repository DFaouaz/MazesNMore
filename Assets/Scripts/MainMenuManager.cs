using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Gestiona la escena del MainMenu

namespace MazesAndMore
{
    public class MainMenuManager : MonoBehaviour
    {
        public enum OptionsPanel { Configuration, Shop };
        public Text playerLevelText;

        public CustomButton menuSoundButton;
        public CustomButton optionsSoundButton;
        public Sprite soundOn;
        public Sprite soundOff;

        public GameObject mainMenuPanel;
        public GameObject optionsPanel;
        public GameObject configPanel;
        public GameObject shopPanel;

        public PanelController exitPanel;
        public PanelController rewardedPanel;

        void Start()
        {
            // carga los datos del jugador del UserData
            AudioListener.volume = DataManager.Instance.GetUserData().volume;
            menuSoundButton.SetSprites(AudioListener.volume == 1 ? soundOn : soundOff, AudioListener.volume == 1 ? soundOn : soundOff);
            optionsSoundButton.SetSprites(AudioListener.volume == 1 ? soundOn : soundOff, AudioListener.volume == 1 ? soundOn : soundOff);

            playerLevelText.text = XPManager.GetLevel(XPManager.Instance.GetExperience()).ToString();

            // TODO: MOSTRAR ANUNCIO AL ABRIR LA APP

            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }

        private void Update()
        {
            // si se pulsa el Back del movil
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsPanel.activeSelf)
                    CloseOptionsPanel();
                else
                    exitPanel.OpenPanel();
            }
        }

        public void OpenConfigPanel()
        {
            OpenOptionsPanel(OptionsPanel.Configuration);
        }

        public void OpenShopPanel()
        {
            OpenOptionsPanel(OptionsPanel.Shop);
        }

        public void OpenOptionsPanel(OptionsPanel panel)
        {
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(true);

            bool isConfig = panel == OptionsPanel.Configuration;

            configPanel.SetActive(isConfig);
            shopPanel.SetActive(!isConfig);
        }

        private void CloseOptionsPanel()
        {
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }

        public void PlayButton()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            GameManager.Instance.LoadScene("SelectionMenu");
        }

        public void ReturnButton()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            GameManager.Instance.LoadScene("MainMenu");
        }

        public void SoundToggle()
        {
            if (AudioListener.volume == 1) // Mute
                AudioListener.volume = 0;
            else // Desmute
                AudioListener.volume = 1;

            menuSoundButton.SetSprites(AudioListener.volume == 1 ? soundOn : soundOff, AudioListener.volume == 1 ? soundOn : soundOff);
            optionsSoundButton.SetSprites(AudioListener.volume == 1 ? soundOn : soundOff, AudioListener.volume == 1 ? soundOn : soundOff);

            DataManager.Instance.GetUserData().volume = (int)AudioListener.volume;
        }

        // Muestra el anuncio RewardedAd y comienza una corrutina para esperar a que termine
        public void RewardedAdButton()
        {
            AdsManager.Instance.ShowRewardedAd();

            StartCoroutine(WaitForRewardedAd());
        }

        // Se queda esperando a que el usuario termine de ver el RewardedAd.
        // Cuando termina, se le da una pista si lo ha visto entero
        IEnumerator WaitForRewardedAd()
        {
            AdsManager adsManager = AdsManager.Instance;

            while (!adsManager.adFinished)
                yield return null;

            if (adsManager.adCompleted)
            {
                DataManager.Instance.GetUserData().hints++;
                rewardedPanel.OpenPanel();
            }
        }

        public void ExitApplication()
        {
            Application.Quit();
        }
    }
}

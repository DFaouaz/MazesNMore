using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

// Gestiona la escena de Level

namespace MazesAndMore
{
    public class LevelManager : MonoBehaviour
    {
        public PanelController pauseMenu;
        public PanelController restartMenu;
        public PanelController shopMenu;
        public PanelController noHintsMenu;
        public PanelController levelCompleteMenu;
        public PanelController viewLevelMenu;

        public GameObject blueBackgroundPanel;
        public Text levelTitle;

        public Text hintsText;

        public BoardManager boardManager;
        public LevelProgressBar progressBar;

        private string[] congrats;
        public Text congratsText;
        public Text viewCongratsText;

        private bool fromPause = false;

        void Start()
        {
            congrats = new string[] { "AMAZING!", "AWESOME!", "SUPREME!", "GOOD!", "NICE!", "WELL DONE!" };

            blueBackgroundPanel.SetActive(false);
            viewLevelMenu.gameObject.SetActive(false);

            Init();
        }

        private void Init()
        {
            LevelGroup levelGroup = GameManager.Instance.GetCurrentLevelGroup();
            int levelIndex = GameManager.Instance.GetCurrentLevelIndex();

            int hints = DataManager.Instance.GetUserData().hints;
            hintsText.text = hints.ToString();

            if (hints == 0 && !GameManager.Instance.noHintsShown)
                EnableNoHintsMenu(true);

            boardManager.SetBoardColor(levelGroup.color);
            boardManager.LoadLevel(levelGroup.levels[levelIndex]);
            boardManager.Init();
            boardManager.SetOnBoardCompleted(() =>
            {
                boardManager.boardCompleted = true;

                if (DataManager.Instance.GetUserData().no_ads == 1)
                    LevelComplete();
                else
                    StartCoroutine(ShowLevelCompleteAd());
            });

            levelTitle.text = "  " + levelGroup.name + " - " + (levelIndex + 1).ToString();
        }

        IEnumerator ShowLevelCompleteAd()
        {
            AdsManager adsManager = AdsManager.Instance;

            while (!adsManager.adReady)
                yield return null;

            Advertisement.Show("LevelComplete");

            while (!adsManager.adFinished)
                yield return null;

            LevelComplete();

            adsManager.adCompleted = false;
            adsManager.adFinished = false;
        }

        private void LevelComplete()
        {
            AudioManager.Instance.Play("LevelComplete");

            congratsText.text = congrats[Random.Range(0, congrats.Length)];
            viewCongratsText.text = congratsText.text;

            EnableLevelCompleteMenu(true);

            LevelGroup levelGroup = GameManager.Instance.GetCurrentLevelGroup();
            int levelIndex = GameManager.Instance.GetCurrentLevelIndex();

            UserData userData = DataManager.Instance.GetUserData();
            int boardLevel = userData.levels_completed.ContainsKey(levelGroup.levelGroupName) ? userData.levels_completed[levelGroup.levelGroupName] : 0;
            if (boardLevel == levelIndex)
            {
                DataManager.Instance.GetUserData().levels_completed[levelGroup.levelGroupName] = levelIndex + 1;
                progressBar.Add(XPManager.Instance.GetExperiencePerLevelCompleted());
            }
        }

        public void SetPause(bool pause)
        {
            if (pauseMenu == null) return;

            if (pause)
                pauseMenu.OpenPanel();
            else
                pauseMenu.ClosePanel();
        }

        public void OnPause(bool paused)
        {
            if (paused)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
        }

        public void EnableRestartMenu(bool enable)
        {
            if (restartMenu == null) return;

            if (enable)
                restartMenu.OpenPanel();
            else
                restartMenu.ClosePanel();
        }

        public void EnableShopMenuFromPause(bool enable)
        {
            EnableShopMenu(enable, true);
        }

        public void EnableShopMenuNotFromPause(bool enable)
        {
            EnableShopMenu(enable, false);
        }

        public void EnableShopMenu(bool enable, bool fromPause)
        {
            if (shopMenu == null) return;

            if (enable)
            {
                this.fromPause = fromPause;

                if (!fromPause)
                    boardManager.SetPlayerInputFrozen(true);

                shopMenu.OpenPanel();
            }
            else
            {
                if (!this.fromPause)
                    boardManager.SetPlayerInputFrozen(false);

                shopMenu.ClosePanel();
                if (!this.fromPause)
                    shopMenu.OnCloseFinished.AddListener(()=> { OnPause(false); shopMenu.OnCloseFinished.RemoveAllListeners(); } );
            }
        }

        public void EnableNoHintsMenu(bool enable)
        {
            if (noHintsMenu == null) return;

            if (enable)
            {
                noHintsMenu.OpenPanel();
                GameManager.Instance.noHintsShown = true;
            }
            else
                noHintsMenu.ClosePanel();
        }


        public void EnableLevelCompleteMenu(bool enable)
        {
            if (levelCompleteMenu == null) return;

            if (enable)
            {
                blueBackgroundPanel.SetActive(true);
                levelCompleteMenu.OpenPanel();
            }
            else
                CompressLevelComplete();
        }

        public void ExpandLevelComplete()
        {
            levelCompleteMenu.gameObject.SetActive(true);
            levelCompleteMenu.GetAnimator().SetTrigger("Move");
            viewLevelMenu.gameObject.SetActive(false);
            blueBackgroundPanel.SetActive(true);
        }

        public void CompressLevelComplete() // TODO: NOMBRE KK
        {
            viewLevelMenu.gameObject.SetActive(true);
            levelCompleteMenu.GetAnimator().SetTrigger("Move");
            levelCompleteMenu.gameObject.SetActive(false);
            blueBackgroundPanel.SetActive(false);
        }

        // TODO: SE PODRIA UNIFICAR TODAS LAS FUNCIONES DE MENUS EN UNA
        /*public void EnableMenu(bool enable, Animator anim)
        {
            if (anim == null || backgroundPanel == null) return;

            if (enable)
            {
                backgroundPanel.SetActive(true);
                anim.SetTrigger("Enter");
            }
            else
                ;
                // play pauseMenu's exit animation
                //StartCoroutine(PauseExitAnimation());
        }*/

        public void UseHint()
        {
            if (DataManager.Instance.GetUserData().hints == 0)
            {
                EnableShopMenu(true, false);
                OnPause(true);
            }
            else
            {
                if (boardManager.UseHint())
                    hintsText.text = (--DataManager.Instance.GetUserData().hints).ToString();
            }
        }

        public void HomeButton()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            Time.timeScale = 1.0f;
            GameManager.Instance.LoadScene("MainMenu");
        }

        public void LoadNextLevel()
        {
            LevelGroup levelGroup = GameManager.Instance.GetCurrentLevelGroup();
            int levelIndex = GameManager.Instance.GetCurrentLevelIndex();
            if ((levelIndex + 1) >= levelGroup.levels.Length)
            {
                GameManager.Instance.LoadScene("SelectionMenu");
                return;
            }
            GameManager.Instance.LoadLevel(levelIndex + 1, levelGroup);
        }

        public void UpdateHintText()
        {
            int hints = DataManager.Instance.GetUserData().hints;
            hintsText.text = hints.ToString();
        }
    }
}
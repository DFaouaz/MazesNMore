using System.Collections;
using UnityEngine.UI;
using UnityEngine;

// Gestiona la escena SelectionMenu.
// Contiene los lotes de niveles e instancia los CategoryButtons

namespace MazesAndMore
{
    public class SelectionMenuManager : MonoBehaviour
    {
        public CategoryMenuSwapManager categoryMenuManager;
        public GameObject categoriesPanel;
        public Category categoryButtonPrefab;
        public LevelGroup[] levelGroups;

        public Text playerLevelText;

        public PanelController rewardedPanel;

        private void Start()
        {
            playerLevelText.text = XPManager.GetLevel(XPManager.Instance.GetExperience()).ToString();
            Init();
        }

        private void Init()
        {
            if (categoriesPanel == null) return;
            if (categoryButtonPrefab == null) return;

            // Vaciar panel
            while (categoriesPanel.transform.childCount >= 1)
                DestroyImmediate(categoriesPanel.transform.GetChild(0).gameObject);

            // Instancia los CategoryButtons
            for (int i = 0; i < levelGroups.Length; i++)
            {
                Category category = Instantiate(categoryButtonPrefab, categoriesPanel.transform);
                if (category == null) continue;
                category.SetCategory(levelGroups[i]);
                int indexCpy = i;
                category.customButton.clickedEvent.AddListener(() => ButtonEventCallBack(indexCpy));
            }
        }

        private void ButtonEventCallBack(int index)
        {
            if (categoryMenuManager == null) return;

            categoryMenuManager.ShowLevels(levelGroups[index]);
        }

        public void LevelButton()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            GameManager.Instance.LoadScene("Level");
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
    }
}
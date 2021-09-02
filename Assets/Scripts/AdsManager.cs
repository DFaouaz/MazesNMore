using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

// Gestiona los anuncios del juego, mostrandolos o no dependiendo si el NoAds esta activo.

namespace MazesAndMore
{
    public class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        private static AdsManager _instance;

#if UNITY_ANDROID || UNITY_EDITOR
        string gameId = "3951249";
#elif UNITY_IOS
        string gameId = "3951248";
#endif

        bool testMode = true;

        [HideInInspector] public bool adStarted = false;
        [HideInInspector] public bool adCompleted = false;
        [HideInInspector] public bool adFinished = false;
        [HideInInspector] public bool adReady = false;

        [HideInInspector] public string adStatus = "";

        string bannerId = "Banner";

        public static AdsManager Instance
        {
            get { return _instance; }
        }

        // Es un singleton
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Init advertisement
            Advertisement.AddListener(this);
            Advertisement.Initialize(gameId, testMode);

            // Show banner ad
            StartCoroutine(ShowBannerAd());
        }

        // Muestra el anuncio banner abajo en el centro
        IEnumerator ShowBannerAd()
        {
            if (DataManager.Instance.GetUserData().no_ads == 0)
            {
                while (!Advertisement.IsReady(bannerId))
                    yield return null;

                Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
                Advertisement.Banner.Show(bannerId);
            }
        }

        // Muestra el anuncio de recompensa y resetea booleanos para la comprobacion 
        public void ShowRewardedAd()
        {
            if (adReady)
            {
                adReady = false;
                adStarted = false;
                adCompleted = false;
                adFinished = false;

                ShowOptions options = new ShowOptions();
                Advertisement.Show("rewardedVideo", options);
            }
        }

        public void OnUnityAdsReady(string placementId)
        {
            if (!adStarted)
                adReady = true;
        }

        public void OnUnityAdsDidError(string message)
        {
            adStatus = message;
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            adStarted = true;

            adCompleted = false;
            adFinished = false;
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            adStarted = false;
            adFinished = true;
            adCompleted = showResult == ShowResult.Finished;
        }

        public void EnableAds(bool enable)
        {
            if (enable)
                StartCoroutine(ShowBannerAd());
            else
                Advertisement.Banner.Hide();
        }
    }
}

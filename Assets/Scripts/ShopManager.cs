using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gestiona el menu de la tienda y los botones que aparecen en ella

namespace MazesAndMore
{
    public class ShopManager : MonoBehaviour
    {
        public void BuyHints(int quantity)
        {
            UserData userData = DataManager.Instance.GetUserData();
            long current = userData.hints;

            if (current + quantity > int.MaxValue)
                userData.hints = int.MaxValue;
            else
                userData.hints += quantity;
        }

        public void BuyNoAds() // Para las pruebas es un toggle
        {
            UserData userData = DataManager.Instance.GetUserData();

            if (userData.no_ads != 0)
            {
                userData.no_ads = 0;
                AdsManager.Instance.EnableAds(true);
            }
            else
            {
                userData.no_ads = 1; // Orden importa
                AdsManager.Instance.EnableAds(false);
            }
        }

        public void RestorePurchases()
        {
            UserData userData = DataManager.Instance.GetUserData();
            userData.hints = 0;
            if (userData.no_ads == 0) return;
            userData.no_ads = 0;
            AdsManager.Instance.EnableAds(true);
        }

    }
}

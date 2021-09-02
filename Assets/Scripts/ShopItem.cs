using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Boton de tienda

namespace MazesAndMore
{
    [ExecuteInEditMode]
    public class ShopItem : MonoBehaviour
    {
        public Text priceText;
        [Min(0.0f)]
        public float price;

        private void Start()
        {
            if (priceText == null) return;
            priceText.text = "EUR " + price.ToString("0.00");
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (priceText == null) return;
            priceText.text = "EUR " + price.ToString("0.00");
        }
#endif
    }
}
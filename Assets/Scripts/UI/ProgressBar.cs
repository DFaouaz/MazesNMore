using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Componente que se encarga de actualizar el componente de ProgressBar de Unity

namespace MazesAndMore
{
    [ExecuteInEditMode()]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private float minimum;
        [SerializeField]
        private float maximum;
        [SerializeField]
        private float current;
        public Color fillColor;
        public Image mask;
        public Image fill;

        private void Start()
        {
            RecalculateCurrentFill();
        }

#if UNITY_EDITOR
        private void Update()
        {
            RecalculateCurrentFill();
        }
#endif
        // Dado un maximo y un minimo, actualiza el valor del progressBar en proporcion
        private void RecalculateCurrentFill()
        {
            float currentOffset = current - minimum;
            float maximumOffset = maximum - minimum;
            float fillRatio = currentOffset / maximumOffset;
            mask.fillAmount = fillRatio;

            fill.color = fillColor;
        }

        public float GetMinimum()
        {
            return minimum;
        }

        public void SetMinimum(float minimum)
        {
            this.minimum = minimum;
            RecalculateCurrentFill();
        }

        public float GetMaximum()
        {
            return maximum;
        }

        public void SetMaximum(float maximum)
        {
            this.maximum = maximum;
            RecalculateCurrentFill();
        }

        public float GetCurrent()
        {
            return current;
        }

        public void SetCurrent(float current)
        {
            this.current = current;
            RecalculateCurrentFill();
        }
    }
}

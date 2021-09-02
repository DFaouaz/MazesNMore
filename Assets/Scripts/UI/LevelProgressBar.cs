using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gestiona la interfaz del panel de LevelComplete

namespace MazesAndMore
{
    public class LevelProgressBar : MonoBehaviour
    {
        public ProgressBar progressBar;
        public Text levelText;
        public float time;

        private int level;

        private void Start()
        {
            Configure();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Configure()
        {
            int totalExperience = XPManager.Instance.GetExperience();

            level = XPManager.GetLevel(totalExperience);
            levelText.text = level.ToString();

            int xp = XPManager.GetXPInLevel(level);
            progressBar.SetCurrent(xp - XPManager.GetRemainingXP(totalExperience));
            progressBar.SetMinimum(0.0f);
            progressBar.SetMaximum(XPManager.GetXPInLevel(level));
        }

        // Actualiza el valor del progressBar y del numero, 
        // empezando una corrutina para la animacion
        public void Add(float xp)
        {
            StopAllCoroutines();
            Configure();
            XPManager.Instance.AddExperience((int)xp);
            StartCoroutine(InternalAdd(xp));
        }

        // Devuelve el sobrante
        private float OnProgressBarCompleted()
        {
            float remaining = progressBar.GetCurrent() - progressBar.GetMaximum();
            return remaining;
        }

        // Corrutina para la animacion de la progressBar y actualizacion del numero
        // cuando la barra llega al final y se sube de nivel
        private IEnumerator InternalAdd(float xp)
        {
            AudioManager.Instance.Play("XPBar");

            float timer = 0.0f;
            float current = progressBar.GetCurrent();
            float diff = progressBar.GetMaximum() - current;

            while (timer < time)
            {
                float remaining = OnProgressBarCompleted();
                if (remaining >= 0.0f)
                {
                    current = -diff;
                    progressBar.SetMinimum(0.0f);
                    progressBar.SetMaximum(XPManager.GetXPInLevel(++level));
                    AudioManager.Instance.Play("LevelUp");
                }

                progressBar.SetCurrent(current + xp * timer / time);

                timer += Time.deltaTime;
                yield return null;
            }
            Configure();
        }
    }
}

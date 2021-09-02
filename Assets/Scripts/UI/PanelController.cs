using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Gestiona un panel de la interfaz, con sus animaciones y eventos

namespace MazesAndMore
{
    public class PanelController : MonoBehaviour
    {
        public GameObject backgroundPanel;
        public GameObject mainPanel;

        public Animator mainPanelAnimator;

        public bool startInOpenState = false;

        [Space]
        [Tooltip("Eventos que ocurre antes de que se llame a Open")]
        public UnityEvent OnOpenTriggered;
        [Tooltip("Eventos que ocurre cuendo se llame a Open")]
        public UnityEvent OnOpenFinished;
        [Tooltip("Eventos que ocurre antes de que se llame a Close")]
        public UnityEvent OnCloseTriggered;
        [Tooltip("Eventos que ocurre antes de que se llame a Close")]
        public UnityEvent OnCloseFinished;


        private void Start()
        {
            if (backgroundPanel != null)
                backgroundPanel.SetActive(startInOpenState);
            if (mainPanel != null)
                mainPanel.SetActive(startInOpenState);
        }

        public void OpenPanel()
        {
            if (OnOpenTriggered != null) OnOpenTriggered.Invoke();

            backgroundPanel.SetActive(true);
            mainPanel.SetActive(true);

            if (mainPanelAnimator != null)
            {
                mainPanelAnimator.SetTrigger("Enter");
                return;
            }

            if (OnOpenFinished != null) OnOpenFinished.Invoke();

        }

        public void ClosePanel()
        {
            if (OnCloseTriggered != null) OnCloseTriggered.Invoke();

            if (mainPanelAnimator != null)
            {
                mainPanelAnimator.SetTrigger("Exit");
                return;
            }

            backgroundPanel.SetActive(false);
            mainPanel.SetActive(false);

            if (OnCloseFinished != null) OnCloseFinished.Invoke();
        }

        public void SendAnimationEntered()
        {
            if (OnOpenFinished != null) OnOpenFinished.Invoke();
        }

        public void SendAnimationExited()
        {
            if (OnCloseTriggered != null) OnCloseTriggered.Invoke();

            backgroundPanel.SetActive(false);
            mainPanel.SetActive(false);

            if (OnCloseFinished != null) OnCloseFinished.Invoke();
        }

        public Animator GetAnimator()
        {
            return mainPanelAnimator;
        }
    }
}

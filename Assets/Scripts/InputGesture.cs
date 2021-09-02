using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detecta y controla deslizamientos en la pantalla en PC y ANDROID

namespace MazesAndMore
{
    public class InputGesture : MonoBehaviour
    {
        public enum Swipe { None, Left, Right, Up, Down };
        private Swipe swipe = Swipe.None;

        private Vector2 fingerStart;
        private Vector2 fingerEnd;

        private int minDistance = 10;
        private Vector2 fingerDistance = Vector2.zero;

        private bool canSwipe = true;

        void Update()
        {
            swipe = Swipe.None;
            fingerDistance = Vector2.zero;
#if UNITY_ANDROID
            foreach (Touch touch in Input.touches)
            {
                if (canSwipe)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        fingerStart = touch.position;
                        fingerEnd = touch.position;
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        fingerEnd = touch.position;
                        fingerDistance = fingerEnd - fingerStart;

                        if (Mathf.Abs(fingerDistance.x) >= Mathf.Abs(fingerDistance.y)) // Si el swipe es horizontal
                        {
                            if (fingerDistance.x > minDistance) swipe = Swipe.Right;
                            else if (fingerDistance.x < -minDistance) swipe = Swipe.Left;
                            else swipe = Swipe.None;
                        }
                        else // Si el swipe es vertical
                        {
                            if (fingerDistance.y > minDistance) swipe = Swipe.Up;
                            else if (fingerDistance.y < -minDistance) swipe = Swipe.Down;
                            else swipe = Swipe.None;
                        }

                        if (swipe != Swipe.None)
                            canSwipe = false;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    canSwipe = true;
            }
#endif
            // Solo para probar en el editor de unity
#if UNITY_EDITOR
            if (canSwipe)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    fingerStart = Input.mousePosition;
                    fingerEnd = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    fingerEnd = Input.mousePosition;
                    fingerDistance = fingerEnd - fingerStart;

                    if (Mathf.Abs(fingerDistance.x) >= Mathf.Abs(fingerDistance.y)) // Si el swipe es horizontal
                    {
                        if (fingerDistance.x > minDistance) swipe = Swipe.Right;
                        else if (fingerDistance.x < -minDistance) swipe = Swipe.Left;
                        else swipe = Swipe.None;
                    }
                    else // Si el swipe es vertical
                    {
                        if (fingerDistance.y > minDistance) swipe = Swipe.Up;
                        else if (fingerDistance.y < -minDistance) swipe = Swipe.Down;
                        else swipe = Swipe.None;
                    }

                    if (swipe != Swipe.None)
                        canSwipe = false;
                }
            }
            else if (Input.GetMouseButtonUp(0))
                canSwipe = true;
#endif
        }

        public bool GetSwipe(Swipe swipe)
        {
            return this.swipe == swipe;
        }
    }
}

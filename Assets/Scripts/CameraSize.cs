using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ajusta la camara al nivel

namespace MazesAndMore
{
    [RequireComponent(typeof(Camera))]
    public class CameraSize : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera = null;

        // bordes para dejar espacio al banner
        float offX = 0.1f;
        float offY = 4f;

        public void Configurate(int rows, int columns)
        {
            if (mainCamera == null)
            {
                Debug.LogError("MainCamera not assigned");
                return;
            }

            float rowsFixed = rows + offY;
            float columnsFixed = columns + offX;

            float aspectRatio = (float)Screen.width / Screen.height; // ratio de pantalla
            float targetRatio = columnsFixed / rowsFixed; // ratio del board

            if (aspectRatio >= targetRatio)
                mainCamera.orthographicSize = (float)rowsFixed / 2.0f;
            else
            {
                float differenceInSize = targetRatio / aspectRatio;
                mainCamera.orthographicSize = (float)(rowsFixed / 2.0f) * differenceInSize;
            }

            mainCamera.transform.position = new Vector3(columns / 2.0f, rows / 2.0f, -1);
        }
    }
}

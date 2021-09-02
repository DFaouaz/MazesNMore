using System.Collections.Generic;
using UnityEngine;

// Se encarga de instanciar
// Contiene listas para los traces del jugador y las pistas

namespace MazesAndMore
{
    public class TraceManager : MonoBehaviour
    {
        public Trace tracePrefab;
        public GameObject playerTracesParent;
        public GameObject hintsTracesParent;

        private List<Trace> playerTraces;
        private List<Trace> hintTraces;

        private void Start()
        {
            playerTraces = new List<Trace>();
            hintTraces = new List<Trace>();
        }

        // Limpia los traces del jugador
        public void Clear()
        {
            foreach (Trace trace in playerTraces)
                Destroy(trace.gameObject);

            playerTraces.Clear();
        }

        // Añade un trace o elimina el que estaba si se recorre hacia atras ese trace
        public void AddTrace(Vector3 from, Vector3 to, float time, Color color)
        {
            // Si ya tengo traces
            if (playerTraces.Count > 0)
            {
                Trace prev = playerTraces[playerTraces.Count - 1];
                // Si me dirijo hacia atras
                if ((prev.GetFrom() - to).normalized.magnitude < float.Epsilon)
                {
                    prev.Move(to - from, time);
                    prev.SetOnArrivedCallback(() =>
                    {
                        DestroyImmediate(prev.gameObject);
                    });
                    playerTraces.Remove(prev);
                    return;
                }
            }

            // Añado tenga o no
            Trace trace = Instantiate(tracePrefab, from, Quaternion.identity, playerTracesParent.transform);
            trace.Move(to - from, time);
            trace.SetColor(color);
            playerTraces.Add(trace);
        }

        // Añade un trace de pista
        public bool AddHintTrace(List<Vector3> hints, float time, Color color)
        {
            if (hints.Count < 2) return false;

            int size = hintTraces.Count;
            if (size >= hints.Count - 1) return false; // Ya no hay mas recorrido

            Vector3 from = new Vector3(hints[size].x, hints[size].y, 1.0f);
            Vector3 to = new Vector3(hints[size + 1].x, hints[size + 1].y, 1.0f);

            Trace trace = Instantiate(tracePrefab, from, Quaternion.identity, hintsTracesParent.transform);
            trace.Move(to - from, time);
            trace.SetColor(color);
            hintTraces.Add(trace);

            return true;
        }
    }
}

using System;
using UnityEngine;

// Actualiza la orientacion, posicion y size del trace

namespace MazesAndMore
{
    [ExecuteInEditMode]
    public class Trace : MonoBehaviour
    {
        public Transform pivot;
        public Transform from;
        public Transform to;
        public SpriteRenderer sprite;

        [SerializeField]
        private Movement movement = null;

        [SerializeField]
        private Vector2 initialSize;

        private void Awake()
        {
            movement.enabled = false;
        }

        void Start()
        {
            initialSize = Vector2.one * sprite.size;
        }

#if UNITY_EDITOR
        void Update()
        {
            Recalculate();
        }
#endif

        private void Recalculate()
        {
            pivot.position = from.position;
            Vector3 direction = to.localPosition - from.localPosition;
            float magnitude = direction.magnitude; // Candidad que se tiene que escalar extra

            // Normalizamos la direccion
            direction.Normalize();

            sprite.size = initialSize + Vector2.right * magnitude;
            sprite.gameObject.transform.localPosition = Vector2.right * magnitude / 2.0f;

            float angle = magnitude != 0.0f ? Mathf.Rad2Deg * Mathf.Acos(direction.x) : magnitude;
            angle *= Mathf.Sign(direction.y);
            pivot.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }

        public void Init(Vector3 from, Vector3 to)
        {
            this.from.localPosition = from;
            this.to.localPosition = to;
            Recalculate();
        }

        public void SetColor(Color color)
        {
            sprite.color = color;
        }

        // Establece la posicion en la que empieza el trace
        public void SetFrom(Vector3 position)
        {
            from.localPosition = position;
            Recalculate();
        }

        // Establece la posicion en la que termina el trace
        public void SetTo(Vector3 position)
        {
            to.localPosition = position;
            Recalculate();
        }

        public Vector3 GetFrom()
        {
            return from.localPosition + pivot.transform.position - pivot.transform.localPosition;
        }

        public Vector3 GetTo()
        {
            return to.localPosition + pivot.transform.position - pivot.transform.localPosition;
        }

        // Wrap de Movement que evita tener el Update del movement activo si no se está moviendo
        public void Move(Vector3 direction, float time)
        {
            movement.enabled = true;

            movement.Move(direction, time);

            movement.SetOnMovingCallback(() =>
            {
                Recalculate();
            });

            movement.SetOnArrivedCallback(() =>
            {
                Recalculate();
                movement.enabled = false; // Desactivamos el componente
            });
        }

        public void SetOnArrivedCallback(Action callback)
        {
            movement.SetOnArrivedCallback(() => 
            {
                Recalculate();
                callback.Invoke();
                if(movement != null)
                    movement.enabled = false; // Desactivamos el componente
            });
        }

    }
}

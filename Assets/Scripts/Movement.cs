using System;
using UnityEngine;

// Movimiento desde la posicion "from" con direccion "direction".
// Se usa para el player y para animar el trace

namespace MazesAndMore
{
    public class Movement : MonoBehaviour
    {
        private Vector3 from;
        private Vector3 direction;

        float timer;
        float movementTime;

        private bool arrived = false;
        private Action onArrived = null;
        private Action onMoving = null;
        private bool startedMoving = false;
        private Action onStartedMoving = null;

        void Update()
        {
            if (startedMoving) startedMoving = false;
            if (arrived) arrived = false;
            if (timer > movementTime) return;
            if (direction == Vector3.zero) return;

            // Si va a iniciar la marcha
            if (timer == 0.0f && !startedMoving)
            {
                startedMoving = true;
                if (onStartedMoving != null) onStartedMoving.Invoke();
            }

            timer += Time.deltaTime;

            float ratio = timer / movementTime;
            Vector3 position = Vector3.Lerp(from, from + direction, ratio);
            transform.position = position;

            if (onMoving != null) onMoving.Invoke();

            if (ratio >= 1.0f)
            {
                arrived = true;
                direction = Vector3.zero;
                if (onArrived != null) onArrived.Invoke();
            }
        }

        // Se mueve una unidad en la direccion y tiempo dado
        public void Move(Vector3 direction, float time)
        {
            if (direction == Vector3.zero) return;
            if (this.direction != Vector3.zero) return; // No admite llamadas si esta moviendose ya

            from = transform.position;
            this.direction = direction.normalized; // Direccion normalizada
            timer = 0.0f;
            movementTime = time;
        }

        public Vector3 GetDirection()
        {
            return direction;
        }

        public bool IsMoving()
        {
            return direction != Vector3.zero;
        }

        public bool HasStartedMoving()
        {
            return startedMoving;
        }

        public bool HasArrived()
        {
            return arrived;
        }

        public void SetOnStartedMovingCallback(Action callback)
        {
            onStartedMoving = callback;
        }

        public void SetOnMovingCallback(Action callback)
        {
            onMoving = callback;
        }

        public void SetOnArrivedCallback(Action callback)
        {
            onArrived = callback;
        }

        public void Stop()
        {
            timer = movementTime;
            startedMoving = false;
            direction = Vector3.zero;
            arrived = false;
        }
    }
}

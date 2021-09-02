using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Logica del player: movimiento, llegar a la meta, mostrar flechas
// 

namespace MazesAndMore
{
    // TODO : REQUIRE COMPONENT
    public class Player : MonoBehaviour
    {
        List<Vector2> DIRECTIONS = new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        [SerializeField]
        private Movement movement = null;
        [SerializeField]
        private InputGesture gesture = null;

        public SpriteRenderer playerSprite;
        public Animator playerAnimator;

        [SerializeField]
        private List<SpriteRenderer> arrows = null;

        Vector2 direction = Vector2.zero;
        List<Vector2> possibleDirections = null;

        private BoardManager board = null;

        private GameObject goal;
        // tiempo que tarda en moverse a una casilla
        float moveTime = 0.1f;
        private bool onIce = false;
        private bool inputFrozen = false;

        private void Start()
        {
            playerSprite.gameObject.SetActive(false);
        }

        public void Reset()
        {
            direction = Vector3.zero;
            movement.Stop();
            possibleDirections = CheckDirections();
            DeactivateArrows();
            ManageArrows();
            playerAnimator.Rebind();
        }

        public void Init()
        {
            playerSprite.gameObject.SetActive(true);
            possibleDirections = CheckDirections();
            ManageArrows();

            movement.SetOnArrivedCallback(() =>
            {
                possibleDirections = CheckDirections();
                if (transform.position == goal.transform.position)
                {
                    direction = Vector3.zero;
                    ManageArrows();
                    return;
                }
                // Si hay hielo y la direccion actual esta entre las posibles direcciones, seguir
                if (onIce && possibleDirections.Contains(direction))
                {
                    movement.Move(direction, moveTime);
                    return;
                }
                // Si solo hay una direccion posible (aparte de la de atras), seguir moviendose
                else if (possibleDirections.Count == 2)
                {
                    Vector2 iniDir = direction;

                    if (possibleDirections[0] == -direction)
                        direction = possibleDirections[1];
                    else if (possibleDirections[1] == -direction)
                        direction = possibleDirections[0];

                    if (direction != iniDir)
                        AudioManager.Instance.Play("Corner");

                    movement.Move(direction, moveTime);
                    return;
                }
                // Si no, pararse y mostrar flechas
                else
                {
                    AudioManager.Instance.Play("Corner");

                    direction = Vector3.zero;
                    ManageArrows();
                }
            });
        }

        void Update()
        {
            if (inputFrozen) return;

            if (HasReachedGoal()) return;

            // Si esta parado, se puede mover
            if (!movement.IsMoving() && direction == Vector2.zero)
            {
                if (possibleDirections == null) return;
                // Input de swipe en PC y ANDROID
                if (gesture.GetSwipe(InputGesture.Swipe.Left) && possibleDirections.Contains(Vector2.left))
                    direction.x = -1.0f;
                else if (gesture.GetSwipe(InputGesture.Swipe.Right) && possibleDirections.Contains(Vector2.right))
                    direction.x = 1.0f;
                else if (gesture.GetSwipe(InputGesture.Swipe.Up) && possibleDirections.Contains(Vector2.up))
                    direction.y = 1.0f;
                else if (gesture.GetSwipe(InputGesture.Swipe.Down) && possibleDirections.Contains(Vector2.down))
                    direction.y = -1.0f;


                if (direction != Vector2.zero)
                {
                    movement.Move(direction, moveTime);
                    // Desactivamos flechas
                    DeactivateArrows();
                }
            }
        }

        private void DeactivateArrows()
        {
            foreach (SpriteRenderer arrow in arrows)
                arrow.gameObject.SetActive(false);
        }

        private void ManageArrows()
        {
            for (int i = 0; i < DIRECTIONS.Count; i++)
            {
                if (possibleDirections.Contains(DIRECTIONS[i]))
                    arrows[i].gameObject.SetActive(true);
            }
        }

        private List<Vector2> CheckDirections()
        {
            if (board == null) return null;
            return board.GetPossibleDirections((int)transform.position.x, (int)transform.position.y);
        }

        public void SetBoard(BoardManager board)
        {
            this.board = board;
        }

        public Movement GetMovement()
        {
            return movement;
        }

        public void SetColor(Color color)
        {
            playerSprite.color = color;

            foreach (SpriteRenderer arrow in arrows)
                arrow.color = color;
        }

        public void SetGoal(GameObject goal)
        {
            this.goal = goal;
        }

        public bool HasReachedGoal()
        {
            return transform.position == goal.transform.position;
        }

        public float GetMovementTime()
        {
            return moveTime;
        }

        public void SetOnIce(bool ice)
        {
            onIce = ice;
        }

        public void SetInputFrozen(bool frozen)
        {
            inputFrozen = frozen;
        }
    }
}

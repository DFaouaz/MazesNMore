using UnityEngine;

// Clase de celda. Contiene referencia a los muros, hielo y goal

namespace MazesAndMore
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        private GameObject goalChild = null;
        [SerializeField]
        private GameObject iceChild = null;

        [SerializeField]
        private GameObject leftWall = null;
        [SerializeField]
        private GameObject downWall = null;

        [SerializeField]
        private SpriteRenderer goalSprite = null;
        [SerializeField]
        private Animator goalAnimator = null;
        [SerializeField]
        private Animator iceAnimator = null;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            EnableGoal(false);
            EnableIce(false);
            EnableLeftWall(false);
            EnableDownWall(false);
        }

        public bool IsIced()
        {
            if (iceChild == null) return false;
            return iceChild.activeSelf;
        }

        public bool IsGoal()
        {
            if (goalChild == null) return false;
            return goalChild.activeSelf;
        }

        public bool IsLeftWallEnabled()
        {
            if (leftWall == null) return false;
            return leftWall.activeSelf;
        }

        public bool IsDownWallEnabled()
        {
            if (downWall == null) return false;
            return downWall.activeSelf;
        }

        public void EnableLeftWall(bool enable)
        {
            if (leftWall == null) return;
            leftWall.SetActive(enable);
        }
        public void EnableDownWall(bool enable)
        {
            if (downWall == null) return;
            downWall.SetActive(enable);
        }

        public void EnableIce(bool enable)
        {
            if (iceChild == null) return;
            iceChild.SetActive(enable);
        }

        public void EnableGoal(bool enable)
        {
            if (goalChild == null) return;
            goalChild.SetActive(enable);
        }

        public void RebindIce()
        {
            if (iceAnimator == null) return;
            iceAnimator.Rebind();
        }

        public void RebindGoal()
        {
            if (goalAnimator == null) return;
            goalAnimator.Rebind();
        }

        public void SetColor(Color color)
        {
            if (goalSprite == null) return;
            goalSprite.color = color;
        }
    }
}

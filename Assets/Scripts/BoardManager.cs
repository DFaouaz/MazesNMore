using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazesAndMore
{
    public class BoardManager : MonoBehaviour
    {
        List<Vector2> DIRECTIONS = new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        public Cell cellPrefab;
        public Player playerPrefab;

        public GameObject cellsParent;

        public TraceManager traceManager;
        public Color hintTraceColor;

        public CameraSize cam;

        private Player player;
        private Vector3 start;

        // Celdas del grid
        private Cell[][] cells;
        private List<Cell> iceCells;
        private Cell goalCell;
        private List<Vector3> solution;

        int rows;
        int columns;

        Action onBoardCompleted;
        [HideInInspector]
        public bool boardCompleted = false;
        [HideInInspector]
        public bool boardCreated = false;

        private Color boardColor; // color del LevelGroup (player, traces y goal)

        private List<GameObject> gameObjects;

        private int hintsUsed;

        private void Start()
        {
            gameObjects = new List<GameObject>();
            iceCells = new List<Cell>();
            hintsUsed = 0;
        }

        private void Update()
        {
            if (boardCompleted || !boardCreated) return; // TODO: HACER QUE EL COMPONENTE SE DESACTIVE MEJORR

            if (player != null)
            {
                if (cells != null)
                    player.SetOnIce(cells[(int)Math.Round(player.transform.position.y)][(int)Math.Round(player.transform.position.x)].IsIced());

                if (player.HasReachedGoal())
                    if (onBoardCompleted != null) onBoardCompleted.Invoke();
            }

#if UNITY_EDITOR
            cam.Configurate(rows, columns);
#endif
        }

        public void SetBoardColor(Color color)
        {
            boardColor = color;
        }

        // Should be called when level is loaded
        public void Init()
        {
            // Configuramos al player
            player.SetBoard(this);
            player.GetMovement().SetOnStartedMovingCallback(() =>
            {
                Vector3 from = player.gameObject.transform.position;
                Vector3 to = from + player.GetMovement().GetDirection();
                AddTrace(from, to);
            });
            player.SetGoal(goalCell.gameObject);
        }

        public void LoadLevel(TextAsset levelTextAsset)
        {
            MazeData level = JsonUtility.FromJson<MazeData>(levelTextAsset.text);

            // Crear laberinto
            rows = level.r;
            columns = level.c;

            // Creamos las filas y las columnas Celdas[FILAS][COLUMNAS] (necesario para el hielo y el goal)
            GenerateBoardGrid(rows, columns);

            cam.Configurate(rows, columns);

            // Coloco muro
            foreach (WallData w in level.w)
            {
                bool rotatedWall = (w.o.y == w.d.y);
                int x = Mathf.Min(w.o.x, w.d.x);
                int y = Mathf.Min(w.o.y, w.d.y);
                if (rotatedWall)
                    cells[y][x].EnableDownWall(true);
                else
                    cells[y][x].EnableLeftWall(true);
            }

            // Crear player
            start = new Vector3(level.s.x, level.s.y, 0);

            if (player == null)
                player = Instantiate(playerPrefab, start, Quaternion.identity, null);
            else
                player.gameObject.transform.position = start;

            player.SetColor(boardColor);
            gameObjects.Add(player.gameObject);

            gameObjects.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
            StartCoroutine(AnimateBoard(level));

            // Poner goal en casilla destino
            goalCell = cells[level.f.y][level.f.x];
            goalCell.SetColor(boardColor);

            // Llenamos el recorrido solucion
            solution = new List<Vector3>(); // Pistas mas inicio y fin
            solution.Add(new Vector3(level.s.x, level.s.y, 0));

            for (int i = 0; i < level.h.Length; i++)
            {
                int solIndex = solution.Count - 1;

                if (GetPossibleDirections((int)solution[solIndex].x, (int)solution[solIndex].y).Contains(new Vector2(level.h[i].x - solution[solIndex].x, level.h[i].y - solution[solIndex].y)))
                {
                    solution.Add(new Vector3(level.h[i].x, level.h[i].y, 0.0f));
                }

            }
            solution.Add(new Vector3(level.f.x, level.f.y, 0.0f));
        }

        IEnumerator AnimateBoard(MazeData level) //TODO: NOMBRE KK
        {
            List<Vector3> finPositions = new List<Vector3>();

            // Guardar posiciones finales y colocar todos los elementos fuera de pantalla
            foreach (GameObject go in gameObjects)
            {
                finPositions.Add(go.transform.position);
                go.transform.position = new Vector3(go.transform.position.x, level.r * 2);
            }

            int i = 0;
            while (i < gameObjects.Count)
            {
                float y = finPositions[i].y;

                // Cada fila (con la misma Y)
                while (y == finPositions[i].y)
                {
                    StartCoroutine(AnimateObject(gameObjects[i], finPositions[i]));
                    i++;
                    if (i >= gameObjects.Count) break;
                }

                yield return new WaitForSeconds(0.1f);
                if (!AudioManager.Instance.IsPlaying("LevelCreation")) AudioManager.Instance.Play("LevelCreation");
            }

            // Cuando termine, hacer Init del jugador
            // Se espera a cuando las paredes ya hayan caido
            yield return new WaitForSeconds(0.2f);

            // Ice
            foreach (CellData ice in level.i)
            {
                // Si el hielo esta dentro (fix: hay un hielo fuera en el lv 38)
                if (ice.x >= 0 && ice.x < columns && ice.y >= 0 && ice.y < rows)
                {
                    Cell iceCell = cells[ice.y][ice.x];
                    iceCell.EnableIce(true);

                    iceCells.Add(iceCell);
                }
            }

            goalCell.EnableGoal(true);

            player.Init();
            boardCreated = true;
        }

        IEnumerator AnimateObject(GameObject go, Vector3 endPos)
        {
            Vector3 iniPos = go.transform.position;

            go.transform.position = iniPos;

            float elapsedTime = 0.0f;
            float waitTime = 0.1f;

            while (elapsedTime < waitTime)
            {
                go.transform.position = Vector3.Lerp(iniPos, endPos, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            go.transform.position = endPos;
            yield return null;
        }

        private void GenerateBoardGrid(int rows, int columns)
        {
            cells = new Cell[rows + 1][];
            for (int i = 0; i <= rows; i++)
            {
                cells[i] = new Cell[columns + 1];
                for (int j = 0; j <= columns; j++)
                {
                    Vector3 position = new Vector3(j, i, 0.0f);
                    cells[i][j] = Instantiate(cellPrefab, position, Quaternion.identity, cellsParent.transform);
                    cells[i][j].Init();
                    gameObjects.Add(cells[i][j].gameObject);
                }
            }
        }

        public List<Vector2> GetPossibleDirections(int x, int y)
        {
            List<Vector2> possibleDirections = new List<Vector2>();
            if (cells == null) return possibleDirections;

            for (int i = 0; i < DIRECTIONS.Count; i++)
            {
                Vector2 direction = DIRECTIONS[i];

                Cell cell = null;
                if (direction == Vector2.right || direction == Vector2.up)
                    cell = cells[y + Mathf.RoundToInt(direction.y)][x + Mathf.RoundToInt(direction.x)];
                else
                    cell = cells[y][x];

                if (i % 2 == 0 && !cell.IsDownWallEnabled())
                    possibleDirections.Add(direction);
                else if (i % 2 == 1 && !cell.IsLeftWallEnabled())
                    possibleDirections.Add(direction);
            }
            return possibleDirections;
        }

        private void AddTrace(Vector3 from, Vector3 to)
        {
            traceManager.AddTrace(from, to, player.GetMovementTime(), boardColor);
        }

        private bool AddHintTrace()
        {
            if (solution == null) return false;

            return traceManager.AddHintTrace(solution, 0.0f, hintTraceColor);
        }

        public void SetOnBoardCompleted(Action callback)
        {
            onBoardCompleted = callback;
        }

        public bool UseHint()
        {
            bool result = false;
            bool hintAdded = true;

            hintsUsed++;

            int i = 0;
            bool condition = hintsUsed >= 3 ? hintAdded : i < solution.Count / 3;

            while (condition)
            {
                hintAdded = AddHintTrace();
                result = hintAdded || result;
                i++;
                condition = hintsUsed >= 3 ? hintAdded : i < solution.Count / 3;
            }
            return result;
        }

        public void RestartLevel()
        {
            player.gameObject.transform.position = start;
            player.Reset();

            traceManager.Clear();

            goalCell.RebindGoal();
            foreach (Cell iceCell in iceCells)
                iceCell.RebindIce();
        }

        public void SetPlayerInputFrozen(bool frozen)
        {
            if (player == null) return;

            player.SetInputFrozen(frozen);
        }
    }
}

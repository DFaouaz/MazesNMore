using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazesAndMore
{
    [System.Serializable]
    public class MazeData
    {
        public int r; // rows
        public int c; // columns

        public CellData s; // start player cell
        public CellData f; // end of the maze

        public CellData[] h; // solution to goal
        public WallData[] w; // walls
        public CellData[] i; // cells with ice
        public CellData[] e; // enemies
        public CellData[] t; // turrets
    }
}
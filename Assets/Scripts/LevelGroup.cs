using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Datos del LevelGroup

namespace MazesAndMore
{
    [CreateAssetMenu(fileName = "LevelGroup", menuName = "ScriptableObjects/LevelGroup")]
    public class LevelGroup : ScriptableObject
    {
        public string levelGroupName; // nombre
        public TextAsset[] levels; // niveles que contiene
        public Color color; // color para el jugador, goal y traces

        public Sprite button;
        public Sprite pressedButton;
    }
}

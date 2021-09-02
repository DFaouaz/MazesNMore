using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gestiona la experiencia del jugador y la experiencia que debe conseguir por cada nivel

namespace MazesAndMore
{
    public class XPManager : MonoBehaviour
    {
        public static XPManager Instance;

        private int experience;
        public int XPPerLevelCompleted = 10;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            UserData userData = DataManager.Instance.GetUserData();
            int levelsCompleted = 0;

            // lee el numero de niveles completados
            for (int i = 0; i < userData.levels_completed.values.Count; i++)
                levelsCompleted += userData.levels_completed.values[i];

            // calcula la experiencia actual del jugador
            experience = levelsCompleted * XPPerLevelCompleted;
        }

        public void AddExperience(int experience)
        {
            this.experience += experience;
        }

        public void SetExperience(int experience)
        {
            this.experience = experience;
        }

        public int GetExperience()
        {
            return experience;
        }

        public int GetExperiencePerLevelCompleted()
        {
            return XPPerLevelCompleted;
        }

        // Metodo que devuelve la cantidad de experiencia que tiene que tener el nivel level
        public static int GetXPInLevel(int level)
        {
            if (level <= 0) return 0;
            if (level == 1) return 20;

            return (int)(GetXPInLevel(level - 1) * 1.1f); // El nivel anterior mas un poco mas
        }

        // Devuelve el nivel que queda antes de llegar al siguiente nivel
        public static int GetRemainingXP(int experience)
        {
            int level = 0;
            int sum = 0;

            while (sum <= experience)
                sum += GetXPInLevel(++level);

            return sum - experience;
        }

        // Devuelve el numero del nivel segun experience
        public static int GetLevel(int experience)
        {
            int level = 0;
            int sum = 0;

            while (sum <= experience)
                sum += GetXPInLevel(++level);

            return level;
        }
    }
}

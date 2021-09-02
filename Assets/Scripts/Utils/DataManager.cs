using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Se encarga de escribir/leer en un archivo para guardar los datos del
// UserData, que contiene el progreso del usuario

namespace MazesAndMore
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;
        private UserData userData;

        private string dataFolderPath;
        private string filename;
        private string SALT;
        private string DEFAULT_HASH_CODE;

        private void Awake()
        {
            dataFolderPath =
#if UNITY_EDITOR
     Application.dataPath; // Path donde guardar el progreso en el EDITOR DE UNITY
#else
    Application.persistentDataPath; // Path donde guardar el progreso en ANDROID
#endif

            filename = Path.Combine(dataFolderPath, "userdata.json");
            SALT = "No pongas mucha sal que se sala";
            DEFAULT_HASH_CODE = Hash.ToHash("", SALT);


            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
                Load();
            }
            else
                DestroyImmediate(gameObject);
        }

        // Carga el archivo filename, lee su contenido, lo deserializa y verifica el hash:
        // si no ha cambiado, se actualiza el UserData. si se ha cambiado, se borra el progreso.
        public void Load()
        {
            // Si no existe, se crea
            if (!File.Exists(filename))
            {
                FileStream file = new FileStream(filename, FileMode.Create);
                file.Close();
                UserData userData = new UserData();
                userData.hash = DEFAULT_HASH_CODE;
                this.userData = userData;
                Save();
                return;
            }

            StreamReader reader = new StreamReader(filename);
            string readerData = reader.ReadToEnd();
            reader.Close();

            // Leemos
            UserData data = JsonUtility.FromJson<UserData>(readerData);

            // Verificamos
            if (VerifyHash(data))
            {
                userData = data;
                return;
            }

            // Se ha modificado el archivo, empiezas de 0
            data = new UserData();
            data.hash = DEFAULT_HASH_CODE;
            this.userData = data;
            Save();
        }

        // Serializa el UserData y lo guarda en el archivo filename 
        public void Save()
        {
            userData.hash = DEFAULT_HASH_CODE;
            string json = JsonUtility.ToJson(userData);
            string hash = Hash.ToHash(json, SALT);

            userData.hash = hash;
            string finalJson = JsonUtility.ToJson(userData);
            // Se crea de nuevo
            FileStream file = new FileStream(filename, FileMode.Create);
            file.Close();

            StreamWriter writer = new StreamWriter(filename);
            writer.Write(finalJson);
            writer.Close();
        }

        // Compara el hash original con el actual
        private bool VerifyHash(UserData data)
        {
            string readedHash = data.hash;
            data.hash = DEFAULT_HASH_CODE;

            string json = JsonUtility.ToJson(data);

            bool correctHash = Hash.ToHash(json, SALT) == readedHash;

            data.hash = readedHash;

            return correctHash;
        }

        public UserData GetUserData()
        {
            return userData;
        }

        private void OnApplicationPause()
        {
            Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}

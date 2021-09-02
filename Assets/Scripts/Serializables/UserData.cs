using System;
using System.Collections.Generic;
using UnityEngine;

// Diccionario para guardar el progreso de los niveles completados por el jugador

// from: https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html?childToView=809221#answer-809221
namespace MazesAndMore
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }
    }

    [System.Serializable]
    public class LevelsCompletedDictionary : SerializableDictionary<string, int> { }

    // Clase que contiene los datos de progreso que van a guardarse en archivo
    [System.Serializable]
    public class UserData
    {
        public int hints = 0;
        public int no_ads = 0;
        public int volume = 1;
        public LevelsCompletedDictionary levels_completed = new LevelsCompletedDictionary();

        public string hash;
    }
}

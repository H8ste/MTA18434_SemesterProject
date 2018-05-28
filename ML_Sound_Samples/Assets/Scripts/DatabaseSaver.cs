using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseSaver : MonoBehaviour
{
    string baseSamplePath;
    string databaseName = "LetterDatabase.json";

    public void Start()
    {
        baseSamplePath = Application.dataPath + "/Resources/SampleDatabase/";
    }

    public void SaveDatabase()
    {
        Debug.Log(baseSamplePath + databaseName);
        Debug.Log(SampleTracker.samplesList.Count);

        if (File.Exists(baseSamplePath + databaseName))
        {
            Debug.Log("Exists");
            // Read and concatinate file
            string file = File.ReadAllText(baseSamplePath + databaseName);
            SampleDatabase tempDatabase = JsonUtility.FromJson<SampleDatabase>(file);

            Debug.Log(tempDatabase.database.Length);
            Debug.Log(tempDatabase.database[1].label);
            Debug.Log(tempDatabase.database[22].label);

            SampleDatabase newDatabase = new SampleDatabase(tempDatabase.database, SampleTracker.samplesList.ToArray());
            string newFile = JsonUtility.ToJson(newDatabase);
            File.WriteAllText(baseSamplePath + databaseName, newFile);
        }
        else
        {
            SampleDatabase tempDatabase = new SampleDatabase(SampleTracker.samplesList.ToArray());
            string newFile = JsonUtility.ToJson(tempDatabase);
            File.WriteAllText(baseSamplePath + databaseName, newFile);
        }
    }
}

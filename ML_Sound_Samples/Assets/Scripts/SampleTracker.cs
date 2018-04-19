using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SampleTracker
{
    public static List<DataSample> samplesList = new List<DataSample>();
}

[System.Serializable]
public class SampleDatabase
{
    public DataSample[] database;

    public SampleDatabase(DataSample[] data)
    {
        database = data;
    }

    public SampleDatabase(DataSample[] data1, DataSample[] data2)
    {
        database = new DataSample[data1.Length + data2.Length];
        data1.CopyTo(database, 0);
        data2.CopyTo(database, data1.Length);
    }
}

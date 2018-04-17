using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OnStartUpBehaviour : MonoBehaviour
{
    public Transform parentObj;
    public GameObject samplePopupPrefab;
    public TCPServer server;

    private WaveFileObject[] waveObjects;
    private string[] filePaths;
    private int soundSampleLength;
    private string baseSamplePath = ""; 


    void Start ()
    {
        baseSamplePath = Application.dataPath + "/Resources/SoundSamples";
        server.Initialize();

        //Load FFTWBuddy here
        //System.Diagnostics.Process.Start();

        filePaths = Directory.GetFiles(Application.dataPath + "/Resources/TrainingData");

        int wavCount = 0;

        for (int i = 0; i < filePaths.Length; i++)
        {
            if (filePaths[i].EndsWith(".wav"))
            {
                wavCount++;
            }
        }

        for (int i = 0; i < wavCount; i++)
        {    
            waveObjects = new WaveFileObject[filePaths.Length];

            if (filePaths[i].EndsWith(".wav"))
            {
                Debug.Log(filePaths[i]);
                waveObjects[i] = new WaveFileObject(filePaths[i]);
                Debug.Log(waveObjects[i].header.format);
            }
        }
        
         soundSampleLength = waveObjects.Length;

        InitSamplePopups(0, waveObjects);
	}

    //then we make class object for each of the original waveobjects where in we store:
    //all of the chunks (WaveFileObjects)
    //File paths to each of the new WaveFileChucks (they have to be written as .wav files)
    //A DataSample object which data will be set from FFTWBuddy and its label from the 
    //dropdown.

    private void InitSamplePopups(int index, WaveFileObject[] wavs)
    {
        WaveFileObject[] waves = wavs[index].ToWaveChunksWithOverlaps(100);

        string folderName = baseSamplePath + "/Sample_" + index;
        Directory.CreateDirectory(folderName);

        for (int i = 0; i < waves.Length; i++)
        {
            GameObject temp = Instantiate(samplePopupPrefab, parentObj) as GameObject;
            SamplePopupBehaviour spb = temp.AddComponent<SamplePopupBehaviour>();
            spb.wavObj = waves[i];

            WaveFileObject.WriteWaveFile(waves[i], folderName + "/soundSample" + i + ".wav");
        }
    }
}

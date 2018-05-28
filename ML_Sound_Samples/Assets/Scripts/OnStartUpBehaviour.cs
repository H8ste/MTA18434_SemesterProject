using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using static WaveFileObject;

public class OnStartUpBehaviour : MonoBehaviour
{
    public Transform parentObj;
    public GameObject samplePopupPrefab;
    public TCPServer server;
    [HideInInspector] public bool isLoaded = false;

    private WaveFileObject[] waveObjects;
    private List<SamplePopData> samplePopList = new List<SamplePopData>();
    private string[] filePaths;
    private int soundSampleCount;
    private string baseSamplePath = ""; 

    void Start ()
    {
        baseSamplePath = Application.dataPath + "/Resources/SoundSamples";
        server.Initialize();

        // Clean up from last time
        DirectoryInfo di = new DirectoryInfo(baseSamplePath);
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }

        //Load FFTWBuddy here
        //System.Diagnostics.Process.Start();

        filePaths = Directory.GetFiles(Application.dataPath + "/Resources/TrainingData");
        List<string> correctFilePaths = new List<string>();

        int wavCount = 0;

        for (int i = 0; i < filePaths.Length; i++)
        {
            if (filePaths[i].EndsWith(".wav"))
            {
                correctFilePaths.Add(filePaths[i]);
                wavCount++;
            }
        }

        soundSampleCount = wavCount;
        waveObjects = new WaveFileObject[wavCount];

        for (int i = 0; i < wavCount; i++)
        {    
            waveObjects[i] = new WaveFileObject(correctFilePaths[i]);
        }

        Debug.Log("Number of samples: " + wavCount);

        Thread t = new Thread(new ThreadStart(Process));
        t.Start();

        //double[] arr = new double[waveObjects[0].soundData.Count];
        //waveObjects[1].soundData.CopyTo(arr, 0);

        //WriteWaveFile(arr, baseSamplePath + "/" + "test.wav");
        //Debug.Log("wrote file");
    }

    private void Update()
    {
        if (isLoaded)
        {
            Debug.Log(samplePopList.Count);
            for (int i = 0; i < samplePopList.Count; i++)
            {
                GameObject tempObject = Instantiate(samplePopupPrefab, parentObj) as GameObject;
                SamplePopupBehaviour spb = tempObject.AddComponent<SamplePopupBehaviour>();
                spb.Init(samplePopList[i]);
            }
            isLoaded = false;
        }
    }

    private void Process()
    {
        for (int i = 0; i < waveObjects.Length; i++)
        {
            InitSamplePopups(i, waveObjects);
        }

        isLoaded = true;
        server.SendMsg("Quit");
    }

    private void InitSamplePopups(int index, WaveFileObject[] wavs)
    {
        WaveFileObject[] waves = wavs[index].ToWaveChunksWithOverlaps(40, wavs[index]);

        Debug.Log("Number of Chunks: " + waves.Length);

        string folderName = baseSamplePath + "/Sample_" + index;
        string resourcePath = "SoundSamples" + "/Sample_" + index;
        Directory.CreateDirectory(folderName);

        for (int i = 0; i < waves.Length; i++)
        {
            WriteWaveFile(waves[i], folderName + "/soundSample" + i + ".wav");
            string path = resourcePath + "/soundSample" + i;

            Debug.Log("Waiting for client to connect");
            while (server.clientReady != true) { }

            WFOTransporter transporter = new WFOTransporter(waves[i]);

            string json = JsonUtility.ToJson(transporter);
            server.SendMsg(json);

            Debug.Log("Waiting for client to Respond");
            while (server.jsonResult == "") { }
            Debug.Log("Response from client");

            DataSample sample = JsonUtility.FromJson<DataSample>(server.jsonResult);
            server.jsonResult = "";

            SamplePopData samplePop = new SamplePopData(sample, path, waves[i]);

            samplePopList.Add(samplePop);
            Debug.Log("Sample " + i + " Initialized");
        }
    }
}

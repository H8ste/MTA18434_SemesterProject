using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SamplePopupBehaviour : MonoBehaviour
{
    [SerializeField] public SamplePopData popData;

    private AudioSource source;
    private Dropdown dropdown;
    private Button playButton;
    private Button saveClose;

    public void Init(SamplePopData sample)
    {
        popData = sample;
    }

    public void Start()
    {
        source = GetComponent<AudioSource>();

        Object o = Resources.Load(popData.waveFilePath);

        Debug.Log(o.GetType());

        if (o is AudioClip)
        {
            Debug.Log("AudioClip");
        }

        source.clip = (AudioClip)o;

        dropdown = transform.Find("Label_Dropdown").GetComponent<Dropdown>();
        playButton = transform.Find("Play_Button").GetComponent<Button>();
        saveClose = transform.Find("Save_&_Close").GetComponent<Button>();

        playButton.onClick.AddListener(PlayPress);
        saveClose.onClick.AddListener(SaveClose);
    }

    public void PlayPress()
    {
        source.Play();
    }

    public void SaveClose()
    {
        popData.dataSample.label = dropdown.value;
        SampleTracker.samplesList.Add(popData.dataSample);
        Destroy(gameObject);
    }

}

[System.Serializable]
public class SamplePopData
{
    [SerializeField] public DataSample dataSample;
    [SerializeField] public string waveFilePath;
    [SerializeField] public WaveFileObject wavObj;

    public SamplePopData(DataSample sample, string path, WaveFileObject wav)
    {
        this.dataSample = sample;
        this.waveFilePath = path;
        this.wavObj = wav;
    }
}

using System.Numerics;
using UnityEngine;

public class WaveDisecter : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        short[] arr = new short[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Complex[] com = DSProcess.FFT(arr);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataSample
{
    public double[] data;
    public int label;
    public static int labelSize;

    public DataSample()
    {
        labelSize = Enum.GetNames(typeof(Label)).Length;
    }

    public DataSample(double[] data)
    {
        this.data = data;
        labelSize = Enum.GetNames(typeof(Label)).Length;
    }

    public DataSample(double[] data, int label)
    {
        this.data = data;
        this.label = label;
        labelSize = Enum.GetNames(typeof(Label)).Length;
    }
}

public enum Label
{
    Blank,
    Noise,
    Hej,
    Siri
};

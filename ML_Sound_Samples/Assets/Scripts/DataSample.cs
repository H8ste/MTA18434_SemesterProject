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

    public DataSample(double[] data, Label label)
    {
        this.data = data;
        this.label = (int)label;
        labelSize = Enum.GetNames(typeof(Label)).Length;
    }
}

public enum Label
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine
};

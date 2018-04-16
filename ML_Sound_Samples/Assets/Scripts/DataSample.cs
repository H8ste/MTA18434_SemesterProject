using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSample
{
    public float[] data;
    public int label;
    public static int labelSize;

    public DataSample(float[] data, Label label)
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

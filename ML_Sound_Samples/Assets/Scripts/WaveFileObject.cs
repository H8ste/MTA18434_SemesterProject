using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WaveFileObject
{

    public WavHeader header;
    public IList soundData;

    public WaveFileObject()
    {
    }

    public WaveFileObject(WavHeader header, IList soundData)
    {
        this.header = header;
        this.soundData = soundData;
    }

    public WaveFileObject(string path)
    {
        header = new WavHeader();

        using (FileStream fs = new FileStream(path, FileMode.Open))
        using (BinaryReader br = new BinaryReader(fs))
        {
            try
            {
                header.riff = br.ReadBytes(4);
                header.size = br.ReadUInt32();
                header.wavID = br.ReadBytes(4);
                byte[] temp = br.ReadBytes(4);
                string chunk = System.Text.Encoding.UTF8.GetString(temp);
                if (chunk != "fmt ")
                {
                    byte[] junk = br.ReadBytes(36);
                }
                else
                {
                    header.fmtID = temp;
                }
                header.fmtSize = br.ReadUInt32();
                header.format = br.ReadUInt16();
                header.channels = br.ReadUInt16();
                header.sampleRate = br.ReadUInt32();
                header.bytePerSec = br.ReadUInt32();
                header.blockSize = br.ReadUInt16();
                header.bit = br.ReadUInt16();
                header.dataID = br.ReadBytes(4);
                header.dataSize = br.ReadUInt32();

                if (header.bit == 16)
                {
                    soundData = new List<short>();

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        soundData.Add((short)br.ReadUInt16());
                    }
                }

                if (header.bit == 32)
                {
                    soundData = new List<int>();

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        soundData.Add((int)br.ReadUInt32());
                    }
                }

                if (header.bit == 64)
                {
                    soundData = new List<double>();

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        soundData.Add((double)br.ReadDouble());
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
    }

    public static WaveFileObject ReadWaveFile(string path)
    {
        WaveFileObject tempObj = new WaveFileObject();

        using (FileStream fs = new FileStream(path, FileMode.Open))
        using (BinaryReader br = new BinaryReader(fs))
        {
            try
            {
                tempObj.header.riff = br.ReadBytes(4);
                tempObj.header.size = br.ReadUInt32();
                tempObj.header.wavID = br.ReadBytes(4);
                byte[] temp = br.ReadBytes(4);
                string chunk = System.Text.Encoding.UTF8.GetString(temp);
                if (chunk != "fmt ")
                {
                    byte[] junk = br.ReadBytes(36);
                }
                else
                {
                    tempObj.header.fmtID = temp;
                }
                tempObj.header.fmtSize = br.ReadUInt32();
                tempObj.header.format = br.ReadUInt16();
                tempObj.header.channels = br.ReadUInt16();
                tempObj.header.sampleRate = br.ReadUInt32();
                tempObj.header.bytePerSec = br.ReadUInt32();
                tempObj.header.blockSize = br.ReadUInt16();
                tempObj.header.bit = br.ReadUInt16();
                tempObj.header.dataID = br.ReadBytes(4);
                tempObj.header.dataSize = br.ReadUInt32();

                if (tempObj.header.bit == 16)
                {
                    tempObj.soundData = new List<short>();

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add(br.ReadInt16());
                    }
                }

                if (tempObj.header.bit == 32)
                {
                    tempObj.soundData = new List<int>();

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add(br.ReadInt32());
                    }
                }

                if (tempObj.header.bit == 64)
                {
                    tempObj.soundData = new List<double>();

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add(br.ReadDouble());
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        return tempObj;
    }

    public static void WriteWaveFile(WaveFileObject obj, string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            try
            {
                bw.Write(obj.header.riff);
                bw.Write(obj.header.size);
                bw.Write(obj.header.wavID);
                bw.Write(obj.header.fmtID);
                bw.Write(obj.header.fmtSize);
                bw.Write(obj.header.format);
                bw.Write(obj.header.channels);
                bw.Write(obj.header.sampleRate);
                bw.Write(obj.header.bytePerSec);
                bw.Write(obj.header.blockSize);
                bw.Write(obj.header.bit);
                bw.Write(obj.header.dataID);
                bw.Write(obj.header.dataSize);

                if (obj.header.bit == 16)
                {
                    for (int i = 0; i < obj.header.dataSize / obj.header.blockSize; i++)
                    {
                        if (i < obj.soundData.Count)
                        {
                            bw.Write((short)obj.soundData[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                    }
                }

                if (obj.header.bit == 32)
                {
                    for (int i = 0; i < obj.header.dataSize / obj.header.blockSize; i++)
                    {
                        if (i < obj.soundData.Count)
                        {
                            bw.Write((int)obj.soundData[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                    }
                }

                if (obj.header.bit == 64)
                {
                    for (int i = 0; i < obj.header.dataSize / obj.header.blockSize; i++)
                    {
                        if (i < obj.soundData.Count)
                        {
                            bw.Write((double)obj.soundData[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                    }
                }
            }
            finally
            {
                if (bw != null)
                {
                    bw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        return;
    }

    /// <summary>
    /// Returns a two dimensional short array that represents the wave objects sound data in chunks, 
    /// how many chunks and their length is based on how many ms the chunks should represent and in 
    /// relation to the length of the original sound data
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public short[][] ToChunks(int ms)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(header.sampleRate * secs);
        long chunkCount = (header.dataSize / header.blockSize) / chunkSamples;
        short[][] chunks = new short[chunkCount][];

        int osIndex = 0;

        for (int i = 0; i < chunkCount; i++)
        {
            short[] temp = new short[chunkSamples];

            for (int j = 0; j < chunkSamples; j++)
            {
                temp[j] = (short)soundData[osIndex];
                osIndex++;
            }

            chunks[i] = temp;
        }

        return chunks;
    }

    public WaveFileObject[] ToWaveChunks(int ms)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(header.sampleRate * secs);
        long chunkCount = (header.dataSize / header.blockSize) / chunkSamples;
        WaveFileObject[] waves = new WaveFileObject[chunkCount];

        int osIndex = 0;

        for (int i = 0; i < chunkCount; i++)
        {
            for (int j = 0; j < chunkSamples; j++)
            {
                waves[i].soundData[j] = soundData[osIndex];
                osIndex++;
            }

            waves[i].header.dataSize = (uint)chunkSamples * 4;
        }

        return waves;
    }

    /// <summary>
    /// Returns a two dimensional short array that represents the wave objects sound data in chunks 
    /// with additional overlapping chunks to represent the sliced segments,
    /// how many chunks and their length is based on how many ms the chunks should represent and in 
    /// relation to the length of the original sound data
    /// </summary>
    /// <param name="ms">The amount of time in ms defines how long each chunk will be</param>
    /// <returns></returns>
    public short[][] ToChunksWithOverlaps(int ms)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(header.sampleRate * secs);
        long chunkCount = (((header.dataSize / header.blockSize) / chunkSamples) * 2) - 2;
        short[][] chunks = new short[chunkCount][];

        int osIndex = 0;
        // Even indexes are the original chunks
        // Odd indexes are the overlaps

        for (int i = 0; i < chunkCount / 2; i++)
        {
            short[] temp = new short[chunkSamples];

            for (int j = 0; j < chunkSamples; j++)
            {
                temp[j] = (short)soundData[osIndex];

                if (i == 1)
                {
                    temp[j] = (short)soundData[osIndex - (int)(chunkSamples * .5)];
                }

                if (i > 1 && i < chunkCount - 2)
                {
                    temp[j] = (short)soundData[osIndex - (int)(chunkSamples * .5)];
                }
                osIndex++;
            }

            chunks[i] = temp;
        }

        return chunks;
    }

    public WaveFileObject[] ToWaveChunksWithOverlaps(int ms)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(header.sampleRate * secs);
        long chunkCount = (((header.dataSize / header.blockSize) / chunkSamples) * 2) - 2;
        WaveFileObject[] waves = new WaveFileObject[chunkCount];

        int osIndex = 0;
        // Even indexes are the original chunks
        // Odd indexes are the overlaps

        for (int i = 0; i < chunkCount / 2; i++)
        {

            for (int j = 0; j < chunkSamples; j++)
            {
                waves[i].soundData[j] = soundData[osIndex];

                if (i == 1)
                {
                    waves[i].soundData[j] = soundData[osIndex - (int)(chunkSamples * .5)];
                }

                if (i > 1 && i < chunkCount - 2)
                {
                    waves[i].soundData[j] = soundData[osIndex - (int)(chunkSamples * .5)];
                }
                osIndex++;
            }

            waves[i].header.dataSize = (uint)chunkSamples * 4;
        }
        return waves;
    }

    public void PrintData()
    {
        for (int i = 0; i < soundData.Count; i++)
        {
            Debug.Log("Data: " + soundData[i]);
        }
    }

    public struct WavHeader
    {
        public byte[] riff;
        public uint size;
        public byte[] wavID;
        public byte[] fmtID;
        public uint fmtSize;
        public ushort format;
        public ushort channels;
        public uint sampleRate;
        public uint bytePerSec;
        public ushort blockSize;
        public ushort bit;
        public byte[] dataID;
        public uint dataSize;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class WaveFileObject
{

    public WavHeader header;
    public IList soundData;

    public WaveFileObject()
    {
    }

    public WaveFileObject(WavHeader header)
    {
        this.header = header;
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
                        soundData.Add(br.ReadInt16());
                    }
                }

                if (header.bit == 32)
                {
                    soundData = new List<uint>();

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        soundData.Add(br.ReadUInt32());
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
                    tempObj.soundData = new List<ushort>();

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add(br.ReadUInt16());
                    }
                }

                if (tempObj.header.bit == 32)
                {
                    tempObj.soundData = new List<uint>();

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add(br.ReadUInt32());
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

    public static void WriteWaveFile(short[] data, string path, int sampleRateArg)
    {
        WavHeader header = new WavHeader
        {
            riff = new byte[4] { 82, 73, 70, 70 },
            size = 36 + (uint)data.Length * 2,
            wavID = new byte[4] { 87, 65, 86, 69 },
            fmtID = new byte[4] { 102, 109, 116, 32 },
            fmtSize = 16,
            format = 1,
            channels = 1,
            sampleRate = (uint)sampleRateArg,
            bytePerSec = 16000,
            blockSize = 8,
            bit = 16,
            dataID = new byte[4] { 100, 97, 116, 97 },
            dataSize = (uint)data.Length * 2
        };

        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            try
            {
                bw.Write(header.riff);
                bw.Write(header.size);
                bw.Write(header.wavID);
                bw.Write(header.fmtID);
                bw.Write(header.fmtSize);
                bw.Write(header.format);
                bw.Write(header.channels);
                bw.Write(header.sampleRate);
                bw.Write(header.bytePerSec);
                bw.Write(header.blockSize);
                bw.Write(header.bit);
                bw.Write(header.dataID);
                bw.Write(header.dataSize);

                if (header.bit == 16)
                {
                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        if (i < data.Length)
                        {
                            bw.Write((short)data[i]);
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
        Console.WriteLine("File written");
        return;
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
                            bw.Write((ushort)obj.soundData[i]);
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
                            bw.Write((uint)obj.soundData[i]);
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

    public WaveFileObject[] ToWaveChunks(int ms, WaveFileObject obj)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(obj.header.sampleRate * secs);
        long chunkCount = (obj.header.dataSize / obj.header.blockSize) / chunkSamples;
        WaveFileObject[] waves = new WaveFileObject[chunkCount];
        int osIndex = 0;

        for (int i = 0; i < chunkCount; i++)
        {
            waves[i] = new WaveFileObject();
            WavHeader tempHeader = header;
            if (header.bit == 16)
            {
                tempHeader.dataSize = (uint)chunkSamples * 2;
                tempHeader.blockSize = 2;
                waves[i].soundData = new List<ushort>();
            }
            if (header.bit == 32)
            {
                tempHeader.dataSize = (uint)chunkSamples * 4;
                tempHeader.blockSize = 4;
                waves[i].soundData = new List<uint>();
            }
            if (header.bit == 64)
            {
                tempHeader.dataSize = (uint)chunkSamples * 8;
                tempHeader.blockSize = 8;
                waves[i].soundData = new List<double>();
            }

            for (int j = 0; j < chunkSamples; j++)
            {
                waves[i].soundData.Add(obj.soundData[osIndex]);
                osIndex++;
            }
            waves[i].header = tempHeader;
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

    public WaveFileObject[] ToWaveChunksWithOverlaps(int ms, WaveFileObject obj)
    {
        double secs = ms / 1000d;
        long chunkSamples = (long)(obj.header.sampleRate * secs);
        long chunkCount = (((obj.header.dataSize / obj.header.blockSize) / chunkSamples) * 2) - 2;
        WaveFileObject[] waves = new WaveFileObject[chunkCount];
        int osIndex = 0;

        for (int i = 0; i < chunkCount; i++)
        {
            waves[i] = new WaveFileObject();
            WavHeader tempHeader = header;

            if (header.bit == 16)
            {
                tempHeader.dataSize = (uint)chunkSamples * 2;
                tempHeader.blockSize = 2;
                waves[i].soundData = new List<ushort>();
            }
            if (header.bit == 32)
            {
                tempHeader.dataSize = (uint)chunkSamples * 4;
                tempHeader.blockSize = 4;
                waves[i].soundData = new List<uint>();
            }
            if (header.bit == 64)
            {
                tempHeader.dataSize = (uint)chunkSamples * 8;
                tempHeader.blockSize = 8;
                waves[i].soundData = new List<double>();
            }

            waves[i].header = tempHeader;
        }

        for (int i = 0; i < chunkCount; i++)
        {
            for (int j = 0; j < chunkSamples; j++)
            {
                if (!IsOdd(i))
                {
                    waves[i].soundData.Add(obj.soundData[osIndex]);
                    osIndex++;
                }
                else
                {
                    waves[i].soundData.Add(obj.soundData[osIndex + j - (int)(chunkSamples * .5)]);
                }
            }
        }
        return waves;
    }

    public void PrintData()
    {
        //for (int i = 0; i < soundData.Count; i++)
        //{
        //    Debug.Log("Data: " + soundData[i]);
        //}
    }

    public static bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class WFOTransporter
    {
        public WavHeader header;
        public ushort[] arrShort;
        public uint[] arrInt;
        public double[] arrDouble;

        public WFOTransporter(WaveFileObject obj)
        {
            this.header = obj.header;

            switch (obj.header.bit)
            {
                case 16:
                    arrShort = new ushort[obj.soundData.Count];
                    for (int i = 0; i < obj.soundData.Count; i++)
                    {
                        arrShort[i] = (ushort)obj.soundData[i];
                    }
                    break;

                case 32:
                    arrInt = new uint[obj.soundData.Count];
                    for (int i = 0; i < obj.soundData.Count; i++)
                    {
                        arrInt[i] = (uint)obj.soundData[i];
                    }
                    break;

                case 64:
                    arrDouble = new double[obj.soundData.Count];
                    for (int i = 0; i < obj.soundData.Count; i++)
                    {
                        arrDouble[i] = (double)obj.soundData[i];
                    }
                    break;

                default:
                    break;
            }
        }
    }

}

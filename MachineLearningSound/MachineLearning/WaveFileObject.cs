using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    /// <summary>
    /// Only supports mono channel audio.
    /// </summary>
    class WaveFileObject
    {

        public WavHeader header;
        public List<short> soundData = new List<short>();

        public WaveFileObject()
        {
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
                    Console.WriteLine(header.riff);

                    header.size = br.ReadUInt32();
                    Console.WriteLine(header.size);

                    header.wavID = br.ReadBytes(4);
                    Console.WriteLine(header.wavID.ToString());

                    byte[] temp = br.ReadBytes(4);

                    string chunk = System.Text.Encoding.UTF8.GetString(temp);

                    Console.WriteLine(chunk);

                    if (chunk == "JUNK")
                    {
                        byte[] junk = br.ReadBytes(36);
                    }
                    else
                    {
                        header.fmtID = temp;
                    }

                    header.fmtSize = br.ReadUInt32();
                    Console.WriteLine(header.fmtSize);

                    header.format = br.ReadUInt16();
                    Console.WriteLine(header.format);

                    header.channels = br.ReadUInt16();
                    Console.WriteLine(header.channels);

                    header.sampleRate = br.ReadUInt32();
                    Console.WriteLine(header.sampleRate);

                    header.bytePerSec = br.ReadUInt32();
                    Console.WriteLine(header.bytePerSec);

                    header.blockSize = br.ReadUInt16();
                    Console.WriteLine(header.blockSize);

                    header.bit = br.ReadUInt16();
                    Console.WriteLine(header.bit);

                    header.dataID = br.ReadBytes(4);
                    Console.WriteLine(header.dataID);

                    header.dataSize = br.ReadUInt32();
                    Console.WriteLine(header.dataSize);

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        soundData.Add((short)br.ReadUInt16());
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
                    Console.WriteLine(tempObj.header.riff);

                    tempObj.header.size = br.ReadUInt32();
                    Console.WriteLine(tempObj.header.size);

                    tempObj.header.wavID = br.ReadBytes(4);
                    Console.WriteLine(tempObj.header.wavID.ToString());

                    byte[] temp = br.ReadBytes(4);

                    string chunk = System.Text.Encoding.UTF8.GetString(temp);

                    Console.WriteLine(chunk);

                    if (chunk == "JUNK")
                    {
                        byte[] junk = br.ReadBytes(36);
                    }
                    else
                    {
                        tempObj.header.fmtID = temp;
                    }

                    tempObj.header.fmtSize = br.ReadUInt32();
                    Console.WriteLine(tempObj.header.fmtSize);

                    tempObj.header.format = br.ReadUInt16();
                    Console.WriteLine(tempObj.header.format);

                    tempObj.header.channels = br.ReadUInt16();
                    Console.WriteLine(tempObj.header.channels);

                    tempObj.header.sampleRate = br.ReadUInt32();
                    Console.WriteLine(tempObj.header.sampleRate);

                    tempObj.header.bytePerSec = br.ReadUInt32();
                    Console.WriteLine(tempObj.header.bytePerSec);

                    tempObj.header.blockSize = br.ReadUInt16();
                    Console.WriteLine(tempObj.header.blockSize);

                    tempObj.header.bit = br.ReadUInt16();
                    Console.WriteLine(tempObj.header.bit);

                    tempObj.header.dataID = br.ReadBytes(4);
                    Console.WriteLine(tempObj.header.dataID);

                    tempObj.header.dataSize = br.ReadUInt32();
                    Console.WriteLine(tempObj.header.dataSize);

                    for (int i = 0; i < tempObj.header.dataSize / tempObj.header.blockSize; i++)
                    {
                        tempObj.soundData.Add((short)br.ReadUInt16());
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
            long chunkCount =  (header.dataSize / header.blockSize) / chunkSamples;
            short[][] chunks = new short[chunkCount][];

            int osIndex = 0;

            for (int i = 0; i < chunkCount; i++)
            {
                short[] temp = new short[chunkSamples];

                for (int j = 0; j < chunkSamples; j++)
                {
                    temp[ j] = soundData[osIndex];
                    osIndex++;
                }

                chunks[i] = temp;
            }

            return chunks;
        }

        /// <summary>
        /// Returns a two dimensional short array that represents the wave objects sound data in chunks 
        /// with additional overlapping chunks to represent the sliced segments,
        /// how many chunks and their length is based on how many ms the chunks should represent and in 
        /// relation to the length of the original sound data
        /// </summary>
        /// <param name="ms"></param>
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
                    temp[j] = soundData[osIndex];

                    if (i == 1)
                    {
                        temp[j] = soundData[osIndex - (int)(chunkSamples * .5)];
                    }

                    if (i > 1 && i < chunkCount - 2)
                    {
                        temp[j] = soundData[osIndex - (int)(chunkSamples * .5)];
                    }
                    osIndex++;
                }

                chunks[i] = temp;
            }

            return chunks;
        }

        public void PrintData()
        {
            for (int i = 0; i < soundData.Count; i++)
            {
                Console.WriteLine("Data: " + soundData[i]);
            }
        }
    }

    struct WavHeader
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

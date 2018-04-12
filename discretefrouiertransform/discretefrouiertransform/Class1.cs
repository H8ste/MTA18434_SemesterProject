using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discretefrouiertransform
{
    class WaveFileObject
    {

        public WavHeader header;
        public List<short> soundData = new List<short>();

        public WaveFileObject(string path)
        {
            header = new WavHeader();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    header.riff = br.ReadBytes(4);
                    Console.WriteLine("riff " + header.riff);
                    header.size = br.ReadUInt32();
                    Console.WriteLine("size " + header.size);
                    header.wavID = br.ReadBytes(4);
                    Console.WriteLine("wavID " + header.wavID);
                    header.fmtID = br.ReadBytes(4);
                    Console.WriteLine("fmtID " + header.fmtID);
                    header.fmtSize = br.ReadUInt32();
                    Console.WriteLine("fmtSize " + header.fmtSize);
                    header.format = br.ReadUInt16();
                    Console.WriteLine("format " + header.format);
                    header.channels = br.ReadUInt16();
                    Console.WriteLine("channels " + header.channels);
                    header.sampleRate = br.ReadUInt32();
                    Console.WriteLine("sampleRate " + header.sampleRate);
                    header.bytePerSec = br.ReadUInt32();
                    Console.WriteLine("bytePerSec " + header.bytePerSec);
                    header.blockSize = br.ReadUInt16();
                    Console.WriteLine("blockSize " + header.blockSize);
                    header.bit = br.ReadUInt16();
                    Console.WriteLine("bit " + header.bit);
                    header.dataID = br.ReadBytes(4);
                    Console.WriteLine("dataID " + header.dataID);
                    header.dataSize = br.ReadUInt32();
                    Console.WriteLine("dataSize " + header.dataSize);

                    for (int i = 0; i < header.dataSize / header.blockSize; i++)
                    {
                        //Console.WriteLine("Max size: " + (header.dataSize / header.blockSize) + " Current size: " + soundData.Count);
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

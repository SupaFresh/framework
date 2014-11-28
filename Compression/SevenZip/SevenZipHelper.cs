namespace SevenZip
{
    using System;
    using System.IO;

    public static class SevenZipHelper
    {
        #region Fields

        static int dictionary = 1 << 23;

        // static Int32 posStateBits = 2;
        // static  Int32 litContextBits = 3; // for normal files
        // UInt32 litContextBits = 0; // for 32-bit data
        // static  Int32 litPosBits = 0;
        // UInt32 litPosBits = 2; // for 32-bit data
        // static   Int32 algorithm = 2;
        // static    Int32 numFastBytes = 128;
        static bool eos = false;

        // these are the default properties, keeping it simple for now:
        static object[] properties = 
                {
                    (Int32)(dictionary),
                    (Int32)(2),
                    (Int32)(3),
                    (Int32)(0),
                    (Int32)(2),
                    (Int32)(128),
                    "bt4",
                    eos
                };
        static CoderPropID[] propIDs = 
                {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.Algorithm,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker
                };

        #endregion Fields

        #region Methods

        public static void Compress(Stream inputStream, Stream outputStream) {
            Compression.LZMA.Encoder encoder = new Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outputStream);
            long fileSize = inputStream.Length;
            for (int i = 0; i < 8; i++)
                outputStream.WriteByte((Byte)(fileSize >> (8 * i)));

            encoder.Code(inputStream, outputStream, -1, -1, null);
        }

        public static byte[] Compress(byte[] inputBytes) {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            Compress(inStream, outStream);
            return outStream.ToArray();
        }

        public static void Decompress(Stream inputStream, Stream outputStream) {
            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();

            byte[] properties2 = new byte[5];
            if (inputStream.Read(properties2, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            long outSize = 0;
            for (int i = 0; i < 8; i++) {
                int v = inputStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            decoder.SetDecoderProperties(properties2);

            long compressedSize = inputStream.Length - inputStream.Position;
            decoder.Code(inputStream, outputStream, compressedSize, outSize, null);
        }

        public static byte[] Decompress(byte[] inputBytes) {
            MemoryStream newInStream = new MemoryStream(inputBytes);
            newInStream.Seek(0, 0);
            MemoryStream newOutStream = new MemoryStream();
            Decompress(newInStream, newOutStream);
            byte[] b = newOutStream.ToArray();
            return b;
        }

        #endregion Methods
    }
}
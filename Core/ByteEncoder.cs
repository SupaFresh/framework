// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;

namespace PMDCP.Core
{
    public class ByteEncoder
    {
        /// <summary>
        /// Creates a 16 bit byte array from a 16 bit character
        /// </summary>
        /// <param name="cChar">Character to convert</param>
        /// <returns></returns>
        public static byte[] CharToByteArray(char cChar) {
            return new byte[] {
                        (byte)((cChar >> 8) & 0xff),
                        (byte)((cChar >> 0) & 0xff),
                };
        }
        /// <summary>
        /// Creates an 8 bit byte array from a Boolean
        /// </summary>
        /// <param name="bBoolean">Boolean to convert</param>
        /// <returns></returns>
        public static byte[] BooleanToByteArray(bool bBoolean) {
            return new byte[] { (byte)(bBoolean ? 0x01 : 0x00) };
        }
        /// <summary>
        /// Creates a 32 bit byte array from an integer
        /// </summary>
        /// <param name="int32">Integer to convert</param>
        /// <returns></returns>
        public static byte[] IntToByteArray(int int32) {
            return new byte[] {
                        (byte)((int32 >> 24) & 0xff), 
                        (byte)((int32 >> 16) & 0xff), 
                        (byte)((int32 >> 8) & 0xff),
                        (byte)((int32 >> 0) & 0xff),
                };
        }

        /// <summary>
        /// Creates a 64 bit byte array from a long
        /// </summary>
        /// <param name="int64">Long to convert</param>
        /// <returns></returns>
        public static byte[] LongToByteArray(long int64) {
            return new byte[] {
                        (byte)((int64 >> 56) & 0xff),
                        (byte)((int64 >> 48) & 0xff),
                        (byte)((int64 >> 40) & 0xff),
                        (byte)((int64 >> 32) & 0xff),
                        (byte)((int64 >> 24) & 0xff),
                        (byte)((int64 >> 16) & 0xff),
                        (byte)((int64 >> 8)  & 0xff),
                        (byte)((int64 >> 0)  & 0xff)
                };
        }
        /// <summary>
        /// Creates a byte array from a String
        /// </summary>
        /// <param name="sString">String to convert</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string sString) {
            return (sString == null) ? null : StringEncoding().GetBytes(sString);
        }

        public static Encoding StringEncoding() {
            return Encoding.Unicode;
        }

        /// <summary>
        /// Creates a Char from a byte array
        /// </summary>
        /// <param name="byteArray">Array of bytes to convert</param>
        /// <returns></returns>
        public static char ByteArrayToChar(byte[] byteArray) {
            return (char)((0xff & byteArray[0]) << 8 | (0xff & byteArray[1]) << 0);
        }
        /// <summary>
        /// Creates a Char array from a byte array
        /// </summary>
        /// <param name="byteArray">Array of bytes to convert</param>
        /// <returns></returns>
        public static char[] ByteArrayToCharArray(byte[] byteArray) {
            int size = byteArray.Length / 2;
            char[] charArray = new char[size];
            byte[] tmpArray;
            for (int s = 0; s < size; s++) {
                tmpArray = GetSubByteArray(byteArray, s * 2, (s + 1) * 2);
                charArray[s] = ByteArrayToChar(tmpArray);
            }
            return charArray;
        }
        /// <summary>
        /// Creates a byte array from a Char array
        /// </summary>
        /// <param name="charArray">Array of characters to convert</param>
        /// <returns></returns>
        public static byte[] CharArrayToByteArray(char[] charArray) {
            byte[] byteArray = new byte[charArray.Length * 2];
            byte[] tmpArray;
            for (int s = 0; s < charArray.Length; s++) {
                tmpArray = CharToByteArray(charArray[s]);
                byteArray = AppendToByteArray(byteArray, tmpArray, s * 2);
            }
            return byteArray;
        }
        public static short ByteArrayToShort(byte[] byteArray, int offset) {
            return
                    (short)((0xff & byteArray[offset]) << 8 |
                                    (0xff & byteArray[offset + 1]) << 0);
        }
        /// <summary>
        /// Creates an Integer from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <param name="offset">Offset to start parsing</param>
        /// <returns></returns>
        public static int ByteArrayToInt(byte[] byteArray, int offset) {
            return
                    (0xff & byteArray[offset]) << 24 |
                    (0xff & byteArray[offset + 1]) << 16 |
                    (0xff & byteArray[offset + 2]) << 8 |
                    (0xff & byteArray[offset + 3]) << 0;
        }
        /// <summary>
        /// Creates a long from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <param name="offset">Offset to start parsing</param>
        /// <returns></returns>
        public static int ByteArrayToLong(byte[] byteArray, int offset) {
            return
                    (0xff & byteArray[offset]) << 56 |
                    (0xff & byteArray[offset + 1]) << 48 |
                    (0xff & byteArray[offset + 2]) << 40 |
                    (0xff & byteArray[offset + 3]) << 32 |
                    (0xff & byteArray[offset + 4]) << 24 |
                    (0xff & byteArray[offset + 5]) << 16 |
                    (0xff & byteArray[offset + 6]) << 8 |
                    (0xff & byteArray[offset + 7]) << 0;
        }

        /// <summary>
        /// Creates a String from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] byteArray) {
            return StringEncoding().GetString(byteArray);
        }

        public static string ByteArrayToString(byte[] byteArray, int offset, int count) {
            return StringEncoding().GetString(byteArray, offset, count);
        }
        /// <summary>
        /// Adds a byte array to another byte array
        /// </summary>
        /// <param name="dArray">Array to append to</param>
        /// <param name="sArray">Array to append</param>
        /// <param name="offset">Offset to start appending</param>
        /// <returns></returns>
        public static byte[] AppendToByteArray(byte[] dArray, byte[] sArray, int offset) {
            for (int i = 0; i < sArray.Length; i++)
                dArray[offset + i] = sArray[i];
            return dArray;
        }
        /// <summary>
        /// Adds a byte array to the end of a byte array
        /// </summary>
        /// <param name="dArray">Byte array to append to</param>
        /// <param name="sArray">Byte array to append</param>
        /// <returns></returns>
        public static byte[] AppendToByteArray(byte[] dArray, byte[] sArray) {
            int endOfArray = dArray.Length;
            dArray = endOfArray < endOfArray + sArray.Length ? ExpandByteArray(dArray, sArray.Length) : dArray;
            for (int i = 0; i < sArray.Length; i++)
                dArray[endOfArray + i] = sArray[i];
            return dArray;
        }
        //public static byte[] AppendToByteArray(byte[] dArray, params byte[] sArray) {
        //    foreach (byte bArray in sArray) {
        //        dArray = AppendToByteArray(dArray, bArray);
        //    }
        //    return dArray;
        //}
        /// <summary>
        /// Returns a byte array from another byte array
        /// </summary>
        /// <param name="byteArray">Byte array to parse</param>
        /// <param name="offset_START">Starting location</param>
        /// <param name="offset_END">Ending location</param>
        /// <returns></returns>
        public static byte[] GetSubByteArray(byte[] byteArray, int offset_START, int offset_END) {
            byte[] byteBuffer = new byte[offset_END - offset_START];
            for (int i = offset_START; i < offset_END; i++)
                byteBuffer[i - offset_START] = byteArray[i];
            return byteBuffer;
        }
        /// <summary>
        /// Extends the maximum size of a byte array
        /// </summary>
        /// <param name="byteArray">Byte array to expand</param>
        /// <param name="appendLength">Size to extend array</param>
        /// <returns></returns>
        public static byte[] ExpandByteArray(byte[] byteArray, int appendLength) {
            byte[] cArray = (byte[])byteArray.Clone();
            byteArray = new byte[cArray.Length + appendLength];
            for (int i = 0; i < cArray.Length; i++)
                byteArray[i] = cArray[i];
            return byteArray;
        }
        /// <summary>
        /// Compares two byte arrays
        /// </summary>
        /// <param name="array1">Array 1</param>
        /// <param name="array2">Array 2</param>
        /// <returns></returns>
        public static bool CompareByteArray(byte[] array1, byte[] array2) {
            return array1.Equals(array2);
        }
        /// <summary>
        /// Converts a String to an array of Char
        /// </summary>
        /// <param name="sVal">String to convert</param>
        /// <returns></returns>
        public static char[] StringToCharArray(string sVal) {
            return sVal.ToCharArray();
        }
        /// <summary>
        /// Converts an array of Char to a String
        /// </summary>
        /// <param name="charArray">Array of Chars to convert</param>
        /// <returns></returns>
        public static string CharArrayToString(char[] charArray) {
            return new string(charArray);
        }
        /// <summary>
        /// Converts an Integer to a 4 byte String
        /// </summary>
        /// <param name="val">Integer to convert</param>
        /// <returns></returns>
        public static string AppendIntegerToString(int val) {
            byte[] comCode = IntToByteArray(val);
            char[] chrCode = ByteArrayToCharArray(comCode);
            return CharArrayToString(chrCode);
        }

    }
}

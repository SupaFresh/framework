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
using System.Collections.Generic;
using System.Text;

namespace PMDCP.Core
{
	public class ByteArray : ByteEncoder
	{
		private byte[] byteArray;
		/// <summary>
		/// Returns the size of the byte array
		/// </summary>
		/// <returns></returns>
		public int Length() {
			return byteArray.Length;
		}
		/// <summary>
		/// Creates a new byte array
		/// </summary>
		public ByteArray() {
			byteArray = new byte[0];
		}

		/// <summary>
		/// Creates a new byte array from an array of bytes.
		/// </summary>
		/// <param name="newArray">Array of bytes to make the array from.</param>
		public ByteArray(byte[] newArray) {
			byteArray = newArray;
		}

		/// <summary>
		/// Creates a new byte array from an array of bytes.
		/// </summary>
		/// <param name="newArray">Array of bytes to make the array from.</param>
		/// <param name="length">Length of the array that will be made</param>
		public ByteArray(byte[] newArray, int length) {
			if (length > newArray.Length) {
				length = newArray.Length;
			}
			byteArray = new byte[length];
			for (int i = 0; i < length; i++) {
				byteArray[i] = newArray[i];
			}
		}

		public void AppendTo(ByteArray newByteArray) {
			AppendTo(newByteArray.ToArray());
		}

		/// <summary>
		/// Adds a Boolean to the end of the byte array
		/// </summary>
		/// <param name="newBool">Boolean to add</param>
		public void AppendTo(bool newBool) {
			byte[] newArray;
			newArray = BooleanToByteArray(newBool);
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds a byte to the end of the byte array
		/// </summary>
		/// <param name="newByte">Byte to add</param>
		public void AppendTo(byte newByte) {
			byte[] newArray = new byte[1];
			newArray[0] = newByte;
			byteArray = AppendToByteArray(byteArray, newArray);

		}
		/// <summary>
		/// Adds an integer to the end of the byte array
		/// </summary>
		/// <param name="newInt">Integer to add</param>
		public void AppendTo(int newInt) {
			byte[] newArray;
			newArray = IntToByteArray(newInt);
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds a long to the end of a byte array
		/// </summary>
		/// <param name="newLong">Long to add</param>
		public void AppendTo(long newLong) {
			byte[] newArray;
			newArray = LongToByteArray(newLong);
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds a String to the end of a byte array
		/// </summary>
		/// <param name="newString">String to add</param>
		public void AppendTo(String newString) {
			byte[] newArray;
			newArray = StringToByteArray(newString);
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds a character to the end of a byte array
		/// </summary>
		/// <param name="newChar">Char to add</param>
		public void AppendTo(char newChar) {
			byte[] newArray;
			newArray = CharToByteArray(newChar);
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds an array of bytes to the end of a byte array
		/// </summary>
		/// <param name="newArray">Bytes to add</param>
		public void AppendTo(byte[] newArray) {
			byteArray = AppendToByteArray(byteArray, newArray);
		}
		/// <summary>
		/// Adds an array of bytes to a specified location of the byte array
		/// </summary>
		/// <param name="newArray">Bytes to add</param>
		/// <param name="offset">Location to add to</param>
		public void AppendTo(byte[] newArray, int offset) {
			byteArray = AppendToByteArray(byteArray, newArray, offset);
		}
		/// <summary>
		/// Returns a new ByteArray from a section of the ByteArray
		/// </summary>
		/// <param name="start">Start location</param>
		/// <param name="end">End location</param>
		/// <returns></returns>
		public ByteArray SubArray(int start, int end) {
			return new ByteArray(GetSubByteArray(byteArray, start, end));
		}
		/// <summary>
		/// Returns an array of bytes representing the byte array
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray() {
			return byteArray;
		}
		/// <summary>
		/// Converts the ByteArray into a Char or 16 bit unsigned integer. This only will work if the array's size is 2
		/// </summary>
		/// <returns></returns>
		public char ToChar() {
			return ByteArrayToChar(ToArray());
		}
		/// <summary>
		/// Converts the ByteArray into a 32 bit signed integer. This only works if the array's size is 4.
		/// </summary>
		/// <returns>The ByteArray in integer form</returns>
		public int ToInt() {
			return ByteArrayToInt(ToArray(), 0);
		}
		/// <summary>
		/// Converts the ByteArray into a 64 bit signed integer. This only works if the array's size is 8
		/// </summary>
		/// <returns>The ByteArray in long form</returns>
		public long ToLong() {
			return ByteArrayToLong(ToArray(), 0);
		}
		public short ToShort() {
			return ByteArrayToShort(ToArray(), 0);
		}
		public override String ToString() {
			return ByteArrayToString(ToArray());
		}
		public byte ToByte() {
			return ToArray()[0];
		}

		/// <summary>
		/// Overwrites the byte array with an array of bytes
		/// </summary>
		/// <param name="newArray">Array of bytes to overwrite the byte array</param>
		public void SetArray(byte[] newArray) {
			byteArray = newArray;
		}
		
		public byte this[int index] {
			get { return byteArray[index]; }
		}
		
		public bool IsEmpty(int start, int end) {
			for (int i = start; i < end; i++) {
				if (byteArray[i] != 0) {
					return false;
				}
			}
			return true;
		}
		
		public bool IsEmpty() {
			return IsEmpty(0, byteArray.Length);
		}
		
		public int LengthWithoutTrailingNulls() {
			for (int i = byteArray.Length; i >= 0; i--) {
				if (byteArray[i] != 0) {
					return i;
				}
			}
			return 0;
		}
	}
}

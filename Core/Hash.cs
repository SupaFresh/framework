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
using System.IO;
using System.Security.Cryptography;

namespace PMDCP.Core
{
    public class Hash
    {
        public static string GetFileHash(string filePath, HashType type) {
            if (!File.Exists(filePath))
                return string.Empty;

            System.Security.Cryptography.HashAlgorithm hasher;
            switch (type) {
                case HashType.SHA1:
                default:
                    hasher = new SHA1CryptoServiceProvider();
                    break;
                case HashType.SHA256:
                    hasher = new SHA256Managed();
                    break;
                case HashType.SHA384:
                    hasher = new SHA384Managed();
                    break;
                case HashType.SHA512:
                    hasher = new SHA512Managed();
                    break;
                case HashType.MD5:
                    hasher = new MD5CryptoServiceProvider();
                    break;
            }
            StringBuilder buff = new StringBuilder();
            try {
                using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)) {
                    hasher.ComputeHash(f);
                    foreach (Byte hashByte in hasher.Hash) {
                        buff.Append(string.Format("{0:x2}", hashByte));
                    }
                }
            } catch {
                return "Error reading file." + new System.Random(DateTime.Now.Second * DateTime.Now.Millisecond).Next().ToString();
            }
            hasher.Dispose();
            return buff.ToString();
        }

        /// <summary>
        /// Generates a hash based on the source text.
        /// </summary>
        /// <param name="SourceText">The text to hash.</param>
        /// <returns>The hashed text as a Base64 string.</returns>
        public static string GenerateHash(string sourceText, string salt, HashType hashType) {
            sourceText = sourceText + salt;

            System.Security.Cryptography.HashAlgorithm hasher;
            switch (hashType) {
                case HashType.SHA1:
                default:
                    hasher = new SHA1CryptoServiceProvider();
                    break;
                case HashType.SHA256:
                    hasher = new SHA256Managed();
                    break;
                case HashType.SHA384:
                    hasher = new SHA384Managed();
                    break;
                case HashType.SHA512:
                    hasher = new SHA512Managed();
                    break;
                case HashType.MD5:
                    hasher = new MD5CryptoServiceProvider();
                    break;
            }

            StringBuilder buffer = new StringBuilder();
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            //Retrieve a byte array based on the source text
            byte[] byteSourceText = unicodeEncoding.GetBytes(sourceText);
            hasher.ComputeHash(byteSourceText);
            foreach (Byte hashByte in hasher.Hash) {
                buffer.Append(string.Format("{0:x2}", hashByte));
            }
            hasher.Dispose();
            return buffer.ToString();
        }
    }
}

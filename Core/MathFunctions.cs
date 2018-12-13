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

namespace PMDCP.Core
{
    public class MathFunctions
    {
        #region Fields

        static readonly Random random = new Random(System.Environment.TickCount);

        #endregion Fields

        #region Methods

        public static int CalculatePercent(int currentValue, int maxValue) {
            return currentValue * 100 / maxValue;
        }

        public static long CalculatePercent(long currentValue, long maxValue) {
            if (maxValue != 0) {
                return currentValue * 100 / maxValue;
            } else {
                return 0;
            }
        }

        public static ulong CalculatePercent(ulong currentValue, ulong maxValue) {
            return currentValue * 100 / maxValue;
        }

        public static int Rand(int low, int high) {
            return random.Next(low, high);
        }

        public static int RoundToMultiple(int number, int multiple) {
            double d = number / multiple;
            d = System.Math.Round(d, 0);
            return Convert.ToInt32(d * multiple);
        }

        #endregion Methods
    }
}

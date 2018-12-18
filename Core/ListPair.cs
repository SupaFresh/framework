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

namespace PMDCP.Core
{
    using System;
    using System.Collections.Generic;

    public class ListPair<TKey, TValue> : IEnumerable<TValue>
    {
        #region Constructors

        public ListPair()
        {
            Keys = new List<TKey>();
            Values = new List<TValue>();
        }

        #endregion Constructors

        #region Properties

        public int Count => Keys.Count;

        public List<TKey> Keys { get; }

        public List<TValue> Values { get; }

        #endregion Properties

        #region Indexers

        public TKey this[TValue val]
        {
            get
            {
                int index = Values.IndexOf(val);
                return Keys[index];
            }
            set
            {
                int index = Values.IndexOf(val);
                Keys[index] = value;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int index = Keys.IndexOf(key);
                return Values[index];
            }
            set
            {
                int index = Keys.IndexOf(key);
                Values[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(TKey key, TValue value)
        {
            if (key != null)
            {
                if (Keys.Contains(key) == false)
                {
                    Keys.Add(key);
                    Values.Add(value);
                }
                else
                {
                    throw (new ArgumentException("A key with the same value has already been added"));
                }
            }
            else
            {
                throw (new ArgumentNullException("key"));
            }
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return Keys.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return Values.Contains(value);
        }

        public ListPair<TKey, TValue> Copy()
        {
            lock (this)
            {
                ListPair<TKey, TValue> copy = new ListPair<TKey, TValue>();
                for (int i = 0; i < Keys.Count; i++)
                {
                    copy.Add(Keys[i], Values[i]);
                }
                return copy;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public TKey GetKey(TValue value)
        {
            int index = Values.IndexOf(value);
            if (index > -1)
            {
                return Keys[index];
            }
            else
            {
                return default(TKey);
            }
        }

        public TValue GetValue(TKey key)
        {
            int index = Keys.IndexOf(key);
            if (index > -1)
            {
                return Values[index];
            }
            else
            {
                return default(TValue);
            }
        }

        public int IndexOfKey(TKey key)
        {
            return Keys.IndexOf(key);
        }

        public int IndexOfValue(TValue value)
        {
            return Values.IndexOf(value);
        }

        public TKey KeyByIndex(int index)
        {
            return Keys[index];
        }

        public void RemoveAt(int index)
        {
            Keys.RemoveAt(index);
            Values.RemoveAt(index);
        }

        public void RemoveAtKey(TKey key)
        {
            if (key != null)
            {
                if (Keys.Contains(key))
                {
                    int index = Keys.IndexOf(key);
                    Keys.RemoveAt(index);
                    Values.RemoveAt(index);
                }
                else
                {
                    throw (new KeyNotFoundException());
                }
            }
            else
            {
                throw (new ArgumentNullException("key"));
            }
        }

        public void RemoveAtValue(TValue value)
        {
            if (value != null)
            {
                if (Values.Contains(value))
                {
                    int index = Values.IndexOf(value);
                    Keys.RemoveAt(index);
                    Values.RemoveAt(index);
                }
                else
                {
                    throw (new KeyNotFoundException());
                }
            }
            else
            {
                throw (new ArgumentNullException("value"));
            }
        }

        public void SetKey(TValue value, TKey newKey)
        {
            int index = Values.IndexOf(value);
            Keys[index] = newKey;
        }

        public void SetValue(TKey key, TValue value)
        {
            int index = Keys.IndexOf(key);
            Values[index] = value;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public TValue ValueByIndex(int index)
        {
            return Values[index];
        }

        #endregion Methods
    }
}
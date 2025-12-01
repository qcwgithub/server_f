using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Data
{
    // 与 CsvHelper 差不多，只是支持并行读取
    public class CsvHelperParallel
    {
        readonly List<string[]> lines;
        readonly Dictionary<string, int> name2ColumnIndex;
        readonly int startColumn;
        readonly int endColumn;
        public CsvHelperParallel(string[] headers, List<string[]> lines, int startColumn = -1, int endColumn = -1)
        {
            if (startColumn == -1 || endColumn == -1)
            {
                startColumn = 0;
                endColumn = headers.Length - 1;
            }
            this.startColumn = startColumn;
            this.endColumn = endColumn;

            this.name2ColumnIndex = new Dictionary<string, int>();
            for (int i = startColumn; i <= endColumn; i++)
            {
                this.name2ColumnIndex.Add(headers[i], i);
            }

            this.lines = lines;
        }

        public int rowCount
        {
            get
            {
                return this.lines.Count;
            }
        }

        public Dictionary<string, int> getName2Column()
        {
            return this.name2ColumnIndex;
        }

        string GetCell(int row, string name)
        {
            int columnIndex;
            if (!this.name2ColumnIndex.TryGetValue(name, out columnIndex))
            {
                return null;
            }
            return this.lines[row][columnIndex];
        }

        public string ReadString(int row, string name)
        {
            return this.GetCell(row, name);
        }

        public bool ReadBool(int row, string name, bool default_ = false)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return cell == "1" || cell == "true";
        }

        public int ReadInt(int row, string name, int default_ = 0)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return int.Parse(cell);
        }

        public long ReadLong(int row, string name, long default_ = 0)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return long.Parse(cell);
        }

        public fraction ReadFraction(int row, string name)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return fraction.zero;
            }
            return fraction.FromString(cell);
        }

        public BigInteger ReadBigInteger(int row, string name)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return BigInteger.Zero;
            }
            return BigInteger.Parse(cell);
        }

        public float ReadFloat(int row, string name, float default_ = 0f)
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return float.Parse(cell);
        }

        public T ReadObject<T>(int row, string name, T default_ = null) where T : class
        {
            var cell = this.GetCell(row, name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            if (cell.IndexOf(CsvUtils.COMMA_REPLACEMENT) >= 0)
            {
                cell = cell.Replace(CsvUtils.COMMA_REPLACEMENT, ',');
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cell);
        }

        public T ReadEnum<T>(int row, string name) where T : Enum
        {
            var cell = this.GetCell(row, name);
            return (T)Enum.Parse(typeof(T), cell);
        }

        public T ReadEnum<T>(int row, string name, T default_) where T : Enum
        {
            var cell = this.GetCell(row, name);

            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }

            return (T)Enum.Parse(typeof(T), cell);
        }
    }
}
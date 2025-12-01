using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Data
{
    public class CsvHelper
    {
        List<string[]> lines;
        int rowIndex;
        Dictionary<string, int> name2ColumnIndex;
        int startColumn;
        int endColumn;
        public CsvHelper(string[] headers, List<string[]> lines, int startColumn = -1, int endColumn = -1)
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
            this.rowIndex = -1;
        }

        public Dictionary<string, int> getName2Column()
        {
            return this.name2ColumnIndex;
        }

        public void ResetRowIndex()
        {
            this.rowIndex = -1;
        }

        string[] currentRow;
        public bool ReadRow()
        {
            this.rowIndex++;
            if (this.rowIndex >= this.lines.Count)
            {
                return false;
            }
            this.currentRow = this.lines[this.rowIndex];
            for (int i = startColumn; i <= endColumn; i++)
            {
                if (!string.IsNullOrEmpty(this.currentRow[i]))
                {
                    return true;
                }
            }
            return false;
        }

        string GetCell(string name)
        {
            int columnIndex;
            if (!this.name2ColumnIndex.TryGetValue(name, out columnIndex))
            {
                return null;
            }
            return this.currentRow[columnIndex];
        }

        public string ReadString(string name)
        {
            return this.GetCell(name);
        }

        public bool ReadBool(string name, bool default_ = false)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return cell == "1" || cell == "true";
        }

        public int ReadInt(string name, int default_ = 0)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return int.Parse(cell);
        }

        public long ReadLong(string name, long default_ = 0)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return long.Parse(cell);
        }

        public BigInteger ReadBigInteger(string name)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return BigInteger.Zero;
            }
            return BigInteger.Parse(cell);
        }

        public float ReadFloat(string name, float default_ = 0f)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return float.Parse(cell);
        }

        public T ReadObject<T>(string name, T default_ = null) where T : class
        {
            var cell = this.GetCell(name);
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

        public T ReadEnum<T>(string name) where T : Enum
        {
            var cell = this.GetCell(name);
            return (T)Enum.Parse(typeof(T), cell);
        }

        public T ReadEnum<T>(string name, T default_) where T : Enum
        {
            var cell = this.GetCell(name);

            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }

            return (T)Enum.Parse(typeof(T), cell);
        }
    }
}
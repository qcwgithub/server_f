using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Data
{
    public class CsvUtils
    {
        public const string IGNORE_LINE_FLAG = "#";
        public const string NULL_CELL_FLAG = "NULL";
        public const char CELL_SPLITER = ',';
        public const char COMMA_REPLACEMENT = '|';
        public const char QUOTATION_MARKS = '"';

        public static void SplitToLines(string text, Action<string[]> handleLine)
        {
            string[] lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith(IGNORE_LINE_FLAG))
                {
                    continue;
                }
                string[] cells = line.Split(CELL_SPLITER);
                handleLine(cells);
            }
        }

        public static void Parse(string text, Action<string[], List<string[]>> action)
        {
            string[] headers = null;
            List<string[]> lines = new List<string[]>();
            SplitToLines(text, line =>
            {
                if (headers == null)
                {
                    headers = line;
                }
                else
                {
                    string[] newLine = new string[headers.Length];
                    string newCell = string.Empty;
                    int newLineIndex = 0;
                    int qmsCount = 0;
                    foreach (var cell in line)
                    {
                        if (!string.IsNullOrEmpty(cell) && cell[0] == QUOTATION_MARKS && string.IsNullOrEmpty(newCell) || !string.IsNullOrEmpty(newCell))
                        {
                            if (!string.IsNullOrEmpty(newCell))
                            {
                                newCell += CELL_SPLITER; // 把被误当成分隔符的逗号补回去
                            }
                            newCell += cell;
                            qmsCount += GetQuotationMarksCount(cell);
                            if (cell[cell.Length - 1] == QUOTATION_MARKS && qmsCount % 2 == 0)
                            {
                                newLine[newLineIndex] = DelExtraQuotationMarks(newCell);
                                newLineIndex++;
                                newCell = string.Empty;
                                qmsCount = 0;
                            }
                        }
                        else
                        {
                            newLine[newLineIndex] = DelExtraQuotationMarks(cell);
                            newLineIndex++;
                        }
                    }
                    lines.Add(newLine);
                }
            });
            action(headers, lines);
        }

        public static CsvHelper Parse(string text)
        {
            CsvHelper helper = null;
            Parse(text, (headers, lines) => helper = new CsvHelper(headers, lines));
            return helper;
        }

        public static CsvHelperParallel ParseToParallel(string text)
        {
            CsvHelperParallel helper = null;
            Parse(text, (headers, lines) => helper = new CsvHelperParallel(headers, lines));
            return helper;
        }

        private static int GetQuotationMarksCount(string cell)
        {
            int count = 0;
            foreach (var item in cell)
            {
                if (item == QUOTATION_MARKS)
                {
                    count++;
                }
            }
            return count;
        }

        private static string DelExtraQuotationMarks(string cell)
        {
            int qmsIndex = 0;
            for (var i = cell.Length - 1; i >= 0; i--)
            {
                if (cell[i] == QUOTATION_MARKS)
                {
                    if (qmsIndex % 2 == 0 || i == 0)
                    {
                        cell = cell.Remove(i, 1);
                    }
                    qmsIndex++;
                }
            }
            return cell;
        }

        // parts 例：0,2,4,6,8,10
        // [0,2]列是一个表，[4,6]是一个表，[8,10]是一个表
        public static Dictionary<string, CsvHelper> ParseMultiple(string text, params int[] parts)
        {
            string[] tableNames = null;
            string[] headers = null;
            List<string[]> lines = new List<string[]>();
            SplitToLines(text, line =>
                {
                    if (tableNames == null)
                        tableNames = line;
                    else if (headers == null)
                        headers = line;
                    else
                        lines.Add(line);
                });

            var dict = new Dictionary<string, CsvHelper>();
            for (int i = 0; i < parts.Length; i += 2)
            {
                var tableName = tableNames[parts[i]];
                var helper = new CsvHelper(headers, lines, parts[i], parts[i + 1]);
                dict.Add(tableName, helper);
            }

            return dict;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SQLArrayFormat : Attribute
    {

        public ArrayType Type;
        public char SplitChar;
        public int ArrayMaxItems;
        public int ColumnStartIndex;
        public string[] CustomSuffixes;

        public SQLArrayFormat(char splitChar)
        {
            Type = ArrayType.Cascade;
            SplitChar = splitChar;
        }

        public SQLArrayFormat(int maxItems, int columnStartIndex)
        {
            Type = ArrayType.UniqueColumns;
            ArrayMaxItems = maxItems;
            ColumnStartIndex = columnStartIndex;
        }

        public SQLArrayFormat(string[] suffixes)
        {
            Type = ArrayType.UniqueColumns;
            ArrayMaxItems = suffixes.Length;
            CustomSuffixes = suffixes;
        }

    }

    public enum ArrayType
    {
        /// <summary>
        /// Cascade the given field into a single string column, with each item in the value of the field
        /// split by a certain character.
        /// </summary>
        Cascade,
        /// <summary>
        /// Separate the array into separate columns in the data table, up to a specific maximum.
        /// </summary>
        UniqueColumns
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO
{
    public class BulkModifier
    {


        private static string[] lines;
        private static int lastLine;

        public static int AppendQuery(string query)
        {
            Utils.PushArray(ref lines, query);
            var temp = lastLine;
            lastLine += 1;
            return temp;
        }

        public static string GetQueryByIndex(int index)
        {
            if (index < 0 || index > lines.Length - 1)
                return null;

            return lines[index];
        }

        public static void RemoveQueryByIndex(int index)
        {
            var temp = new string[lines.Length - 1];
            int offset = 0;
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (i == index)
                {
                    offset = 1;
                    continue;
                }
                temp[i] = lines[i + offset];
            }
            lines = temp;
        }

        public static string GetAllLines()
        {
            string queries = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if (i < lines.Length - 1)
                    queries += lines[i] + "\r\n";
                else
                    queries += lines[i];
            }

            return queries;
        }

        public static void Clear()
        {
            lines = new string[0];
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryDev.DBO
{
    public class Searching
    {

        public static int CurrentPage { get; private set; } = -1;
        public static int Limit { get; private set; } = -1;
        public static bool UsingOptions { get; private set; }
        public static bool UseSearchCount { get; private set; }

        public static string OrderBy { get; private set; }

        public static OrderMethod OrderAZ { get; private set; }

        public static void Clear()
        {
            CurrentPage = 0;
            Limit = 0;
            UsingOptions = false;
            UseSearchCount = false;
        }

        public static void Pages(int limit)
        {
            UsingOptions = true;
            CurrentPage = 0;
            Limit = limit;
        }

        public static void NextPage(int limit = -1)
        {
            UsingOptions = true;
            if (limit > -1)
            {
                Limit = limit;
            }
            else
            {
                Limit = 25;
            }

            CurrentPage += 1;
        }

        public static void CountNext()
        {
            UseSearchCount = true;
        }

        public static void Order(string byField, OrderMethod method)
        {
            OrderBy = byField;
            OrderAZ = method;
        }


    }

    public enum OrderMethod
    {
        Ascending,
        Descending
    }
}

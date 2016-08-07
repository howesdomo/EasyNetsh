using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Howe.Helper
{
    /// <summary>
    ///StringHelper 的摘要说明
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// String to Bool
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool StringToBool(string input)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(input))
            {
                bool.TryParse(input, out result);
            }
            return result;
        }

        /// <summary>
        /// String to int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int StringToInt(string input)
        {
            return StringHelper.StringToInt(input, 0);
        }

        /// <summary>
        /// String to int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int StringToInt(string input, int defaultVal)
        {
            int result = defaultVal;
            if (!string.IsNullOrEmpty(input))
            {
                int.TryParse(input, out result);
            }
            return result;
        }

        /// <summary>
        /// String to long
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long StringToLong(string input)
        {
            long result = 0;
            if (!string.IsNullOrEmpty(input))
            {
                long.TryParse(input, out result);
            }
            return result;
        }

        /// <summary>
        /// String to decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static decimal StringToDecimal(string input)
        {
            decimal result = 0;
            if (!string.IsNullOrEmpty(input))
            {
                decimal.TryParse(input, out result);
            }
            return result;
        }

        /// <summary>
        /// String to String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StringToString(string input)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Trim();
            }
            return result;
        }
    }
}
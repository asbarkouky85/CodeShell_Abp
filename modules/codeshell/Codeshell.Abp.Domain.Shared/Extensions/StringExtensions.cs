using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Codeshell.Abp.Extensions
{
    public static class StringExtensions
    {
        static readonly SHA256 ShaHash = SHA256.Create();

        public static byte[] ToSHA256Bytes(this string input)
        {
            return ShaHash.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public static string ToSHA256(this string input)
        {

            byte[] data = ShaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();

        }


        /// <summary>
        /// Serializes object to json string
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static string ToJson(this object ob, JsonSerializerSettings sett = null)
        {
            if (sett != null)
                return JsonConvert.SerializeObject(ob, sett);
            return JsonConvert.SerializeObject(ob);
        }

        /// <summary>
        /// Serializes object to json string using formatting option
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string ToJson(this object ob, Formatting formatting)
        {
            return JsonConvert.SerializeObject(ob, formatting);
        }
        /// <summary>
        /// Serializes object to json string
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static string ToJsonIndent(this object ob)
        {
            return JsonConvert.SerializeObject(ob, Formatting.Indented);
        }



        public static string ToEnglishNumber(this string input)
        {
            var EnglishNumbers = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    EnglishNumbers.Append(char.GetNumericValue(input, i));
                }
                else
                {
                    EnglishNumbers.Append(input[i].ToString());
                }
            }
            return EnglishNumbers.ToString();
        }


        public static T FromJson<T>(this string st)
        {
            return JsonConvert.DeserializeObject<T>(st);
        }

        public static object FromJson(this string st, Type t)
        {
            return JsonConvert.DeserializeObject(st, t);
        }

        public static bool TryRead<T>(this string st, out T data) where T : class
        {
            try
            {
                data = JsonConvert.DeserializeObject<T>(st);
                return data != null;
            }
            catch
            {
                data = null;
                return false;
            }

        }
        /// <summary>
        /// Returns the string with lower case first letter
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string LCFirst(this string st)
        {
            return st.Substring(0, 1).ToLower() + st.Substring(1, st.Length - 1);
        }

        /// <summary>
        /// Returns the string with upper case first letter
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string UCFirst(this string st)
        {
            return st.Substring(0, 1).ToUpper() + st.Substring(1, st.Length - 1);
        }

        /// <summary>
        /// Substracts the string after the last occurance of the given string
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="str">the given string</param>
        /// <returns></returns>
        public static string GetBeforeLast(this string subject, string str)
        {
            int ind = subject.LastIndexOf(str);
            if (ind != 0)
                return subject.Substring(0, ind);
            else
                return subject;
        }

        public static string GetBeforeFirst(this string subject, string str, int startIndex = 0)
        {
            int ind = subject.IndexOf(str, startIndex);
            if (ind > 0)
                return subject.Substring(0, ind);
            else
                return subject;
        }

        public static string GetAfterLast(this string subject, string str)
        {
            int ind = subject.LastIndexOf(str);
            if (ind != 0)
                return subject.Substring(ind + str.Length);
            else
                return subject;
        }

        public static string GetAfterFirst(this string subject, string str, int length = 0)
        {
            int ind = subject.IndexOf(str);
            if (ind != 0)
            {
                if (length == 0)
                    return subject.Substring(ind + str.Length);
                else
                    return subject.Substring(ind + str.Length, length);
            }

            else
                return subject;
        }

        public static T ConvertTo<T>(this string inputValue)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromString(inputValue);
        }

        public static object ConvertTo(this string inputValue, Type t)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(t);
            return typeConverter.ConvertFromString(inputValue);
        }

        public static bool GetPatternContents(this string subject, string pattern, out string[] data)
        {
            Regex reg = new Regex(pattern);
            var s = reg.Match(subject);
            if (!s.Success)
            {
                data = new string[0];
                return false;
            }
            var lst = new List<string>();
            for (var i = 0; i < s.Groups.Count; i++)
            {
                if (i > 0)
                {
                    lst.Add(s.Groups[i].Value);
                }
            }
            data = lst.ToArray();
            return true;
        }

        public static long ConvertToLong(this string str)
        {
            long.TryParse(str, out long value);
            return value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codeshell.Abp.Files
{
    public class MagicNumbersData
    {
        #region Magic Numbers
        static Dictionary<string, string> ExtensionToBytes = new Dictionary<string, string>
        {
            {".bmp","42-4D"},
            {".jpg","FF-D8"},
            {".gif","47-49-46-38"},
            {".tif","49-49"},
            {".png","89-50-4E-47"},
            {".psd","38-42-50-53"},
            {".wmf","D7-CD-C6-9A"},
            {".mid","4D-54-68-64"},
            {".ico","00-00-01-00"},
            {".mp3","49-44-33"},
            {".avi","52-49-46-46"},
            {".swf","46-57-53"},
            {".flv","46-4C-56"},
            {".mp4","00-00-00-18-66-74-79-70-6D-70-34-32"},
            {".mov","6D-6F-6F-76"},
            {".wmv","30-26-B2-75-8E-66-CF"},
            {".wma","30-26-B2-75-8E-66-CF"},
            {".zip","50-4B-03-04"},
            {".gz","1F-8B-08"},
            {".tar","75-73-74-61-72"},
            {".msi","D0-CF-11-E0-A1-B1-1A-E1"},
            {".obj","4C-01"},
            {".dll","4D-5A"},
            {".cab","4D-53-43-46"},
            {".exe","4D-5A"},
            {".rar","52-61-72-21-1A-07-00"},
            {".sys","4D-5A"},
            {".hlp","3F-5F-03-00"},
            {".vmdk","4B-44-4D-56"},
            {".pst","21-42-44-4E-42"},
            {".pdf","25-50-44-46"},
            {".doc","D0-CF-11-E0-A1-B1-1A-E1"},
            {".rtf","7B-5C-72-74-66-31"},
            {".xls","D0-CF-11-E0-A1-B1-1A-E1"},
            {".ppt","D0-CF-11-E0-A1-B1-1A-E1"},
            {".vsd","D0-CF-11-E0-A1-B1-1A-E1"},
            {".docx","50-4B-03-04"},
            {".xlsx","50-4B-03-04"},
            {".pptx","50-4B-03-04"},
            {".mdb","53-74-61-6E-64-61-72-64-20-4A-65-74"},
            {".ps","25-21"},
            {".msg","D0-CF-11-E0-A1-B1-1A-E1"},
            {".eps","25-21-50-53-2D-41-64-6F-62-65-2D-33-2E-30-20-45-50-53-46-2D-33-20-30"},
            {".jar","50-4B-03-04-14-00-08-00-08-00"},
            {".sln","4D-69-63-72-6F-73-6F-66-74-20-56-69-73-75-61-6C-20-53-74-75-64-69-6F-20-53-6F-6C-75-74-69-6F-6E-20-46-69-6C-65"},
            {".zlib","78-9C"},
            {".sdf","78-9C"}
        };
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension">with dot (ex: .jpg)</param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool ValidateMagic(string extension, byte[] bytes)
        {

            if (ExtensionToBytes.TryGetValue(extension, out string numbers))
            {
                int byteCount = numbers.Split("-").Length;
                var b = BitConverter.ToString(bytes, 0, byteCount);
                return numbers.ToUpper() == b;
            }


            return true;
        }

        public static bool ValidateMagic(string extension, Stream bytes)
        {

            //if (ExtensionToBytes.TryGetValue(extension, out string numbers))
            //{
            //    int byteCount = numbers.Split("-").Length;
            //    var b = BitConverter.ToString(bytes, 0, byteCount);
            //    return numbers.ToUpper() == b;
            //}
            return true;
        }
    }
}

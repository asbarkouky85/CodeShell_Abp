using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Files
{
    public class MagicNumbersData
    {
        #region Magic Numbers
        static Dictionary<string, string> ExtensionToBytes = new Dictionary<string, string>
        {
            {".avi","52-49-46-46"},
            {".bmp","42-4D"},
            {".cab","4D-53-43-46"},
            {".dll","4D-5A"},
            {".doc","D0-CF-11-E0-A1-B1-1A-E1"},
            {".docx","50-4B-03-04"},
            {".eps","25-21-50-53-2D-41-64-6F-62-65-2D-33-2E-30-20-45-50-53-46-2D-33-20-30"},
            {".exe","4D-5A"},
            {".flv","46-4C-56"},
            {".gif","47-49-46-38"},
            {".gz","1F-8B-08"},
            {".hlp","3F-5F-03-00"},
            {".ico","00-00-01-00"},
            {".jar","50-4B-03-04-14-00-08-00-08-00"},
            {".jpg","FF-D8"},
            {".jpeg","FF-D8"},
            {".mdb","53-74-61-6E-64-61-72-64-20-4A-65-74"},
            {".mid","4D-54-68-64"},
            {".mov","6D-6F-6F-76"},
            {".mp3","49-44-33"},
            {".mp4","00-00-00-18-66-74-79-70-6D-70-34-32"},
            {".msg","D0-CF-11-E0-A1-B1-1A-E1"},
            {".msi","D0-CF-11-E0-A1-B1-1A-E1"},
            {".obj","4C-01"},
            {".pdf","25-50-44-46"},
            {".png","89-50-4E-47,FF-D8"},
            {".ppt","D0-CF-11-E0-A1-B1-1A-E1"},
            {".pptx","50-4B-03-04"},
            {".ps","25-21"},
            {".psd","38-42-50-53"},
            {".pst","21-42-44-4E-42"},
            {".rar","52-61-72-21-1A-07-00"},
            {".rtf","7B-5C-72-74-66-31"},
            {".sdf","78-9C"},
            {".sln","4D-69-63-72-6F-73-6F-66-74-20-56-69-73-75-61-6C-20-53-74-75-64-69-6F-20-53-6F-6C-75-74-69-6F-6E-20-46-69-6C-65"},
            {".swf","46-57-53"},
            {".sys","4D-5A"},
            {".tar","75-73-74-61-72"},
            {".tif","49-49"},
            {".vmdk","4B-44-4D-56"},
            {".vsd","D0-CF-11-E0-A1-B1-1A-E1"},
            {".wma","30-26-B2-75-8E-66-CF"},
            {".wmf","D7-CD-C6-9A"},
            {".wmv","30-26-B2-75-8E-66-CF"},
            {".xls","D0-CF-11-E0-A1-B1-1A-E1"},
            {".xlsx","50-4B-03-04"},
            {".zip","50-4B-03-04"},
            {".zlib","78-9C"},
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
                var byteCheckers = numbers.Split(",");
                foreach (var checker in byteCheckers)
                {
                    int byteCount = checker.Split("-").Length;
                    var b = BitConverter.ToString(bytes, 0, byteCount);
                    if (checker.ToUpper() == b)
                        return true;
                }
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments
{
    public static class AttachmentConst
    {
        public const string DriveRootPath = "DriveRootPath";
        public const string ImageNoPdfExtensions = ".jpg.jpeg.png";
        public const string ImageExtensions = ".jpg.jpeg.png.pdf";
        public const string DocumentExtensions = ".doc.docx.pdf";
        public const string ImageDocumentExtensions = ".jpg.jpeg.png.pdf.doc.docx.xls.xlsx.pdf.txt";

        public const int MaxLogoWidth = 300;
        public const int MaxLogoHeight = 300;

        public const int MaxImageWidth = 1200;
        public const int MaxImageHeight = 1200;

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SASO.Attachments.Attachments
{
    public class UploadValidation_Tests : AttachmentsDomainTestBase
    {
        [Fact]
        public void SingleDotInFileName()
        {
            var fileName = "file.jpg";
            var regex = new Regex("\\.");

            var coll = regex.Matches(fileName);
            
        }
    }
}

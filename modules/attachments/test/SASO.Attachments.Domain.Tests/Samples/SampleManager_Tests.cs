using System.Threading.Tasks;
using Xunit;

namespace SASO.Attachments.Samples
{
    public class SampleManager_Tests : AttachmentsDomainTestBase
    {
        //private readonly SampleManager _sampleManager;

        public SampleManager_Tests()
        {
            //_sampleManager = GetRequiredService<SampleManager>();
        }

        [Fact]
        public Task Method1Async()
        {
            return Task.Run(() => { });
        }
    }
}

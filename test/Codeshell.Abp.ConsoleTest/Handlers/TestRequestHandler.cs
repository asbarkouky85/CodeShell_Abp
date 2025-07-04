using Codeshell.Abp.CliDispatch.Parsing;
using Codeshell.Abp.CliDispatch.Routing;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.ConsoleTest.Handlers
{
    public class TestRequestHandler : CliRequestHandler<TestRequest>
    {
        public override string FunctionDescription => "Test";

        protected override void Build(ICliRequestBuilder<TestRequest> builder)
        {
            builder.Property(e => e.Name, "name", "n", 1);
            builder.Property(e => e.Password, "password", "p", 2);
        }

        protected override Task<Result> HandleAsync(TestRequest request, CancellationToken token)
        {
            Console.WriteLine(request.ToJson());
            return Task.FromResult(new Result());
        }
    }
}

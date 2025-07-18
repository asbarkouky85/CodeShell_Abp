﻿using Codeshell.Abp.CliDispatch.Controllers;
using Codeshell.Abp.CliDispatch.Parsing;
using Codeshell.Abp.CliDispatch.Routing;
using Codeshell.Abp.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeshell.Abp.CliDispatch
{
    public class ConsoleControllerHandler<TController> : CliRequestHandler<object> where TController : ConsoleController
    {

        public override string FunctionDescription => "Routing using numbers";

        protected override void Build(ICliRequestBuilder<object> builder)
        {

        }

        protected override async Task<Result> HandleAsync(object request, CancellationToken token)
        {
            var cont = Activator.CreateInstance<TController>();
            cont.IsMain = true;
            await cont.Run();
            return new Result();
        }
    }
}

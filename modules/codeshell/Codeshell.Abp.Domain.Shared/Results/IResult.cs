using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Results
{
    public interface IResult
    {
        int Code { get; set; }
        string Message { get; set; }
        string ExceptionMessage { get; }
        bool IsSuccess { get; }

        void SetException(Exception e, bool recurse = false);
        Exception GetException();
    }
}

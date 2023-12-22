using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Contracts
{

    public interface ILocalizableEntity<TPrime> : IEntity<TPrime>
    {
        string NameAr { get; }
        string NameEn { get; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp.Linq
{
    public class CodeshellPagedRequestDto : PagedAndSortedResultRequestDto, ICodeshellPagedRequest
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
        public virtual string Filter { get; set; }
        public virtual SortDir? Direction { get; set; }
        public bool HasSearchTerm()
        {
            return !string.IsNullOrEmpty(Filter);
        }
    }
}

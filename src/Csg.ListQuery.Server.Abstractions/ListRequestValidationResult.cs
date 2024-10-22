using System.Collections.Generic;

namespace Csg.ListQuery.Server;

public class ListRequestValidationResult
{
    public ListRequestValidationResult(ICollection<ListRequestValidationError> errors, Csg.ListQuery.ListQueryDefinition query)
    {
        this.Errors = errors;
        this.ListQuery = query;
    }

    public virtual ICollection<ListRequestValidationError> Errors { get; set; }

    public virtual bool IsValid { get => this.Errors.Count <= 0; }

    public virtual Csg.ListQuery.ListQueryDefinition ListQuery { get; }
}
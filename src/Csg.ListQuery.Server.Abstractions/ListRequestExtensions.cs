using System.Collections.Generic;

namespace Csg.ListQuery.Server;

public static class ListRequestExtensions
{
    /// <summary>
    /// Adds a validation error message for the given field name.
    /// </summary>
    /// <param name="errors"></param>
    /// <param name="fieldName"></param>
    /// <param name="errorMessage"></param>
    public static void Add(this ICollection<ListRequestValidationError> errors, string fieldName, string errorMessage)
    {
        errors.Add(new ListRequestValidationError()
        {
            Field = fieldName,
            Error = errorMessage
        });
    }       
}
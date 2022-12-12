using System.Collections.Generic;
using Expressions.Task3.E3SQueryProvider.Models.Request.Enums;
using Newtonsoft.Json;

namespace Expressions.Task3.E3SQueryProvider.Models.Request
{
    [JsonDictionary]
    public class SortingCollection : Dictionary<string, SortOrder>
    {
    }
}

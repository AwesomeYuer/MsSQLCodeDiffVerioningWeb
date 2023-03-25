using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Microshaoft.WebApi.ModelBinders;

public class JsonNodeStrongTypeModelBinder<T> : JsonNodeModelBinder
{
    public override Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var jsonNode = GetJsonModelBindingResult(bindingContext);
        var json = jsonNode.ToJsonString();
        var result = JsonSerializer.Deserialize<T>(json);
        bindingContext
                   .Result = ModelBindingResult
                                           .Success
                                               (
                                                   result
                                               );
        return
               Task
                   .CompletedTask;
    }
}

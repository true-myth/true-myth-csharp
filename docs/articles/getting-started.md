# Getting Started

## Prerequisites & Dependencies
This assumes you have a working .NET environment, including access to nuget.org.  True
Myth targets the .NET Framework v4.6.1 (and higher) and .NET Standard 2.0 (and higher).
There are no other dependencies.

## Installation

Install the [TrueMyth](https://www.nuget.org/packages/TrueMyth) nuget package using your choice of package manager.  

## Usage
You'll want to add a `using` statement for the `TrueMyth` namespace.  This will make the
`Result<TValue,TError>`, `Maybe<TValue>`, and associated utility methods available in your
compilation unit.

## Simple Example
I've tried to make this as complete an example as reasonably possible without making it so
big that it takes more than a minute or two to read.  
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrueMyth;

namespace MyWebApp
{
    /*
    interface IModelRepository
    {
        Result<Maybe<Model>, string> GetModel(int id);
    }
    */

    public class AspNetController : Controller
    {
        private readonly IModelRepository _modelRepository;

        public AspNetController(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public IActionResult GetResource(int id)
        {
            return _modelRepository.GetModel(id)
                .Match(
                    err: err => StatusCode(500),
                    // val is a Maybe<Model>
                    ok: val => val.Match(
                        just: model => Json(model),
                        nothing: () => NotFound()));
        }
    }
}
```




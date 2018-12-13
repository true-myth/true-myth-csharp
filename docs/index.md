# Introduction

True Myth provides standard, type-safe wrappers and helper functions to help you with two
_extremely_ common cases in programming: 
 - not having a value — which it solves with a `Maybe` type and associated helper
   functions and methods 
 - having a result where you need to deal with either success or failure — which it solves
   with a `Result` type and associated helper functions and methods 
   
You could implement all of these yourself – it’s not hard! – but it’s much easier to just
have one extremely well-tested library you can use everywhere to solve this problem once
and for all. 

Even better to get one of these with no runtime overhead for using it other than the very
small cost of some little container objects — which we get by leaning hard on the type
system in C♯.

Aside: If you’re familiar with [LanguageExt](https://github.com/louthy/language-ext),
you'll see that this has a lot in common with it — its main differences are: 
 - True Myth has a much smaller API surface than LanguageExt 
 - True Myth aims to be much more approachable for people who aren’t already super
   familiar with functional programming concepts and jargon 

## Maybe
Sometimes you don’t have a value. In C♯ (and .NET generally), we usually represent that
with a `null` - either directly or by a `Nullable<T>` — and then trying to program
defensively in the places we think we might get `null` as arguments to our functions. For
example, imagine an endpoint which returns a JSON payload shaped like this:
```json
{
  "hopefullyAString": "Hello!"
}
```
But sometimes it might come over like this:
```json
{
  "hopefullyAString": null
}
```
Or even like this:
```json
{}
```
Assume we were doing something simple, like logging the length of whatever string was there or logging a default value if it was absent. In typical C♯ we’d write something like this:
```csharp
void LogValue(PayloadDto payload)
{
  var length = payload?.hopefullyAString?.Length();
  loger.Debug("Payload length: {length}", length);
}

async Task RequestFromApi()
{
  await client.FetchFromApi()
              .ContinueWith(payload => {
                LogValue(payload);
                // other stuff with payload ...
              });
}
```
This isn’t a big deal right here… but — and this is a big deal — we have to remember to do
this everywhere we interact with this payload. The property `hopefullyAString` can always
be `null` everywhere we interact with it, anywhere in our program. 😬

`Maybe` is our escape hatch. If, instead of just naively interacting with the payload, we do a very small amount of work
up front to normalize the data and use a `Maybe` instead of passing around `null` values, we can operate safely on the
data throughout our application. If we have something, we get `Maybe` called **Just** — as in, “What’s in this field?
Just a string” or “Just the string ‘hello’”. If there’s nothing there, we have a `Maybe` called **Nothing**. `Maybe` is
a wrapper type that holds the actual value in it, and **Just** and **Nothing** are the valid states for that type.
You’ll never get a `NullReferenceException` ("object reference not set to an instance of an object") when trying to use
it!

Importantly, you can do a bunch of neat things with a `Maybe` instance without checking whether it’s a **Nothing** or a
**Just**. For example, if you want to double a number if it’s present and do nothing if it isn’t, you can use the
`Maybe.Map` function:
```csharp
var hereIsANumber = Maybe.Of(42); // Maybe<int>
var hereIsNothing = Maybe<int>.Nothing;

int doubleFn = n => n * 2;
hereIsANumber.Map(doubleFn); // Just 84
hereIsNothing.Map(doubleFn); // Nothing
```
There are a lot of those [helper functions and methods]()! Just about any way you would
need to interact with a Maybe is there.

So now that we have a little idea what `Maybe` is for and how to use it, here’s that same
example, but rewritten to normalize the payload using a `Maybe` instance. We’re using C♯,
so we will get a compiler error if we don’t handle any of these cases right — or if we try
to use the value at `hopefullyAString` directly after we’ve normalized it!
```csharp
class PayloadDto
{
  public string HopefullyAString { get; set; }
}
class Payload
{
  public Maybe<string> HopefullyAString { get; set; }
}

async Task<Payload> Normalize(PayloadDto dto) => 
  return Task.FromResult(new Payload { 
    HopefullyAString = Maybe.Of(dto.HopefullyAString) 
  });

void LogValue(Payload payload)
{
  var length = payload.HopefullyAString.MapReturn(s => s.Length, 0);
  loger.Debug("Payload length: {length}", length);
}

async Task RequestFromApi()
{
  await client.FetchFromApi()
              .ContinueWith(Normalize)
              .ContinueWith(LogValue);
}

```
Now, you might be thinking, _Sure, but we could get the same effect by just supplying a default value when we
deserialize the data._ That’s true, you could! Here, for example, you could just normalize it to an empty string. And of
course, if just supplying a default value at the API boundary is the right move, you can still do that. `Maybe` is
another tool in your toolbox, not something you’re obligated to use everywhere you can.

However, sometimes there isn’t a single correct default value to use at the API boundary. You might need to handle that
missing data in a variety of ways throughout your application. For example, what if you need to treat “no value”
distinctly from “there’s a value present, and it’s an empty string”? That’s where `Maybe` comes in handy.

## Result

Another common scenario we find ourselves in is dealing with operations which might fail. The most common pattern in
.NET for dealing with this: _exceptions_. There are major problems with exception, especially around reusability and
composability.

Exceptions are unpredictable: you can’t know whether a given function invocation is going to throw an exception until
runtime as someone calling the function. No big deal if it’s a small application and one person wrote all the code, but
with even a few thousand lines of code or two developers, it’s very easy to miss that. And then this happens:

```csharp
// in one part of the codebase
object GetMeAValue(sring url) {
  if (IsMalformed(url)) {
    throw new Exception($"The url `{url}` is malformed!");
  }
  
  // do something else to load data from the URL
  return data;
}

string RenderHtml(object toRender) {
  // if toRender can't generate valid HTML, throw Error("invalid HTML");
  // if it can, theRenderedHTML;
}

void WriteOutput(string html)
{
  // I/O
}

// somewhere else in the codebase -- throws an exception
var badUrl = "http:/www.google.com";  // missing a slash
var response = GetMeAValue(badUrl);  // throws here

// we never get here, but it could throw too
var htmlForPage = RenderHtml(value);

// so we definitely can't get here safely
WriteOutput(htmlForPage);
```
Notice: there’s no way for the caller to know that the function will throw. Perhaps you’re
very disciplined and write good docstrings for every function – and moreover, perhaps
everyone’s editor shows it to them and they pay attention to that briefly-available
popover. More likely, though, this exception throws at runtime and probably as a result of
user-entered data – and then you’re chasing down the problem through error logs. More, if
you do want to account for the reality that any function anywhere in C♯ might
actually throw, you’re going to write something like this:
```csharp
try
{
  var badUrl = "http:/www.google.com";  // missing a slash
  var response = GetMeAValue(badUrl);  // throws here

  // we never get here, but it could throw too
  var htmlForPage = RenderHtml(value);

  // so we definitely can't get here safely
  WriteOutput(htmlForPage); 
}
catch(Exception exn)
{
  HandleErr(exn);
}
```
This kind of universal boilerplate works against the Don't Repeat Yourself principle, and C♯ can’t help you here!
There's no type signatures to say “This throws an exception!”

Instead, we can use a `Result` to get us a container type, much like `Maybe`, to let us deal with this scenario. A
`Result` is either an **Ok** wrapping around a value (like **Just** does) or an **Err** wrapping around some type
defining what went wrong (unlike **Nothing**, which has no contents).
```csharp
Result<Payload, string> GetMeAValue(string url)
{
  if (IsMalformed(url)) {
    return Result<Payload, string>.Err($"The url '{url}' is malformed");
  }
  
  // do something else to load data from the url
  return Result.Ok(data);
}

Result<string, string> RenderHtml(string toRender)
{
  // if toRender can't generate valid HTML, return Err("invalid HTML");
  // if it can, return Ok(theRenderedHTML);
}

void WriteOutput(string html)
{
  
}

// somewhere else in the codebase -- no exception this time!
var badUrl = "http:/www.google.com";  // missing a slash

// value = Err(The url '${http:/www.google.com}' is malformed)
var value = GetMeAValue(badUrl);

// htmlForPage = the same error! or, if it was Ok, could be a different
// `Err` (because of how `andThen` works).
var htmlForPage = value.AndThen(RenderHtml);

// we can't just invoke `setDom` because it doesn't take a `Result`.
value.Match(
  ok: html => WriteOutput(html);
  err: reason => Alert($"Something went seriously wrong here! {reason}");
)
```
When we have a `Result` instance, we can perform tons of operations on whether it’s **Ok** or **Err**, just as we could
with a **Just** and **Nothing**, until we need the value. Maybe that’s right away. Maybe we don’t need it until
somewhere else deep in our application! Either way, we can deal with it easily enough, and have type safety throughout!
LunarDoggo.Optionals
============
---
[![License](https://img.shields.io/github/license/LunarDoggo/Optionals)](https://github.com/lunardoggo/Optionals/blob/main/license) [![Nuget](https://img.shields.io/nuget/vpre/LunarDoggo.Optionals)](https://www.nuget.org/packages/LunarDoggo.Optionals/) ![Nuget](https://img.shields.io/nuget/vpre/LunarDoggo.Optionals)

This library was inspired by a project at university where we had to implement an extension to Java optionals. C# provides an alternative to optionals with the null-conditional (`?`) and null-coalescing (`??`) operators, but there is no easy way to return error messages to calling methods by using these operators without throwing Exceptions or returning custom objects that can contain a value or an error message.

This library's main goal is to provide an easy way for bubbling exceptions and messages up to calling methods without needing an uncountable amount of try-catch-blocks and if-statements. To achieve this goal, the library distinguishes between three kinds of optionals:
  * those which contain values
  * those which contain only a message
  * those which contain a message and an exception

Usage
=
---
**1. Creating optionals**
```csharp
using LunarDoggo.Optionals;
[...]
void SomeMethod()
{
    //Create an optional containing a value
    IOptional<int> value = Optional.OfValue<int>(12);
    
    //Create an optional containing a message
    IOptional<int> message = Optional.OfMessage<int>("Insert your message here");
    
    //Create an optional containing an exception
    IOptional<int> exception = Optional.OfException<int>(new ArgumentException("Some message"));
    
    //Create an optional containing an exception and a custom message
    IOptional<int> exception = Optional.OfException<int>(new ArgumentException("Some exception"), "Custom message");
    
    //Create an optional from a collection of optionals of the same generic type
    IOptional<IEnumerable<int>> values = Optional.OfOptionals<IEnumerable<int>>(new[] { Optional.OfValue<int>(1), [...] });
}
```

`Optional.OfMessage<T>(string)`, `Optional.OfException<T>(Exception)` and `Optional.OfException<T>(Exception, string)` are mainly used when mapping an optional using the methods `IOptional<T>.FlatMap<S>(Func<T, IOptional<S>>)` and `IOptional<T>.SafeFlatMap<S, V>(Func<T, IOptional<S>>)` in order to 

**2. Map between different types of optionals**
```csharp
using LunarDoggo.Optionals;
[...]
void SomeMethod()
{
    IOptional<string> value = Optional.Of<string>("123");
    
    //Map to int
    IOptional<int> map = value.Map(_str => Int32.Parse(_str)); //does not catch exceptions
    IOptional<int> safeMap = value.SafeMap<int, FormatException>(_str => Int32.Parse(_str)); //catches Exceptions of the provided type and returns an IOptional<T> containing the caught Exception if one was caught
    
    //FlatMap to int; useful for example for validation
    IOptional<int> flatMap = value.FlatMap(_str => {
        int i = Int32.Parse(_str);
        if(i > 100)
            return Optional.OfMessage<int>("Expected a value less or equal to 100");
        return Optional.OfValue<int>(i);
    }); //does not catch exceptions
    IOptional<int> safeFlatMap = value.FlatMap<int, FormatException>(_str => {
        int i = Int32.Parse(_str);
        if(i > 100)
            return Optional.OfMessage<int>("Expected a value less or equal to 100");
        return Optional.OfValue<int>(i);
    }); //catches Exceptions of the provided type and returns an IOptional<T> containing the caught Exception if one was caught
}
```
Please be aware that optionals that contain a message or an `Exception` will always return an optional containing the same message or `Exception`, the provided mapping functions will not be executed

**3. Apply changes to an optional:**
```csharp
using LunarDoggo.Optionals;
[...]
void SomeMethod()
{
    //Apply something to the contained value (only changes the contained value if the optional contains an object of a mutable reference type)
    IOptional<int[]> values = Optional.OfValue<int[]>(new int[] { 1, 2, 3 }).Apply(_values => _values[1] = 10); //sets the second value in the array contained in the optional to 10
}
```
Please be aware that optionals that contain a message or an `Exception` will not execute the provided action

**4. Extract values from an optional without using the Value property:**
```csharp
using LunarDoggo.Optionals;
[...]
void SomeMethod()
{
    IOptional<string> value = Optional.Of<string>("123");
    
    //Convert an optional to a string
    string intValue = value.ToString(_str => _str + "!"); //returns "123!" in this example
    string message = Optional.OfMessage<int>("Some message").ToString(_i => _i); //returns "Some message"
    
    //Get the contained value or an alternative
    string val1 = value.OrElse("Empty"); //Returns "123" in this example
    int val2 = Optional.OfMessage<int>("Message").OrElse(123); //returns 123
    int val3 = Optional.OfMessage<int>("Message").OrElse(() => 321); //returns 321
}
```


API
=
---

**Methods and properties provided by every `IOptional<T>` object:**
Method/Property | Description
---|---
`bool HasValue { get; }` | Returns whether the optional contains a value
`T Value { get; }` | Returns the contained value if it is present otherwise, a NotSupportedException is thrown
`bool HasMessage { get; }` | Returns whether the optional contains a message
`string Message { get; }` | Returns the contained message if it is present, otherwise a NotSupportedException is thrown
`bool HasException { get; }` | Returns whether the optional contains an `Exception`
`IOptional<S> SafeFlatMap(Func<T, IOptional<S>>)` | maps this optional to an optional of type `S`
`IOptional<S> FlatMap(Func<T, IOptional<S>>)` | maps this optional to an optional of type `S`
`IOptional<S> SafeMap(Func<T, S>)` | maps this optional to an optional of type `S`
`IOptional<S> Map(Func<T, S>)` | maps this optional to an optional of type `S`
`IOptional<T> Map(Action<T>)` | applies an `Action<T>` to the contained value
`string ToString(Func<T, string>)` | converts the contained value to a string if it is present, otherwise the contained message is returned
`T OrElse(Func<T>)` | Returnes the contained value if it is present, otherwise the result of provided function returning an object of type `T` is returned
`T OrElse(T)` | Returnes the contained value if it is present, otherwise the provided parameter of type `T` is returned

**Methods provided by the static class `Optional`:**
Method | Description
---|---
`IOptional<T> OfValue<T>(T)` | Creates a new `IOptional<T>` containing the provided value
`IOptional<T> OfMessage<T>(string)` | Creates a new `IOptional<T>` containing the provided message
`IOptional<T> OfException<T>(Exception)` | Creates a new `IOptional<T>` containing the provided `Exception`
`IOptional<T> OfException<T>(Exception, string)` | Creates a new `IOptional<T>` containing the provided `Exception` and custom message
`IOptional<IEnumerable<T>> OfOptionals<S, T>(IEnumerable<S>) where S : IOptional<T>` | Creates a new `IOptional<IEnumerable<T>>` containing the value of all provided `IOptional<T>` objects. If any of the provided objects contains an `Exception`, the resulting optional contains all `Exception`s, if any of the provided objects contains a message, all Messages will be concatenated to a single one and an optional containing this message will be returned instead

**Extension methods provided by the static class `Optional`:**
Extension method | Description
---|---
`IOptional<IEnumerable<T>> Filter<T>(Func<T, bool>)` | Filters all values contained in the optional and returns an optional that only contains the items that match the provided filter
`IOptional<S> ForEach<S, T>(Action<T>) where S : IEnumerable<T>` | Applies the provided action to every item of the optional's contained collection
`IOptional<S> Convert<T, S, V>(Func<T, S>)` | Returns an optional that contains the same values as the provided optional but with another kind of collection derived from `IEnumerable<V>`
`IOptional<S> Cast<T, S, V>()` | Returns an optional that contains the same collection as the calling optional, but with a type cast applied to it


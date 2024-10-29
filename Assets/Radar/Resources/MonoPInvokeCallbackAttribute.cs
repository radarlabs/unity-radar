using System;

/// <summary>
/// An attribute used to mark static methods as P/Invoke callbacks for IL2CPP.
/// Required to allow Unity's IL2CPP to marshal managed methods to native code in iOS builds.
/// Allows the method to be used as a delegate in native callbacks.
/// </summary>
public class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute(Type t) { }
}

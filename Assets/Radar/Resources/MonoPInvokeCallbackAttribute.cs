using System;
using AOT;  // Required for the MonoPInvokeCallback attribute

public class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute(Type t) { }
}

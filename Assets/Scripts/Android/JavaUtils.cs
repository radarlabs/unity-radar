using System;
using UnityEngine;

namespace RadarSDK.Android
{
    public static class JavaUtils
    {
        public static AndroidJavaObject ToJavaEnum<T>(T @enum, string className) where T : Enum =>
            new AndroidJavaClass(className).GetStatic<AndroidJavaObject>(@enum.ToString());
    }
}
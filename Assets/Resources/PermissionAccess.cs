using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PermissionAccess
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Input.location.Start();
    }
}

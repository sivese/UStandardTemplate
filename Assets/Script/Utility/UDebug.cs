using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Std.Utility
{
    public static class UDebug
    {
        public static void Log(string log)
        {
#if UNITY_EDITOR
            Debug.LogWarning(log);
#endif
        }

        public static void LogFormat(string log, params object[] param)
        {
#if UNITY_EDITOR
            Debug.LogWarning(log);
#endif
        }
    }
}

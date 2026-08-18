using UnityEngine;

namespace SmartVariables
{
    internal static class SmartObjectId
    {
        public static int Get(Object target)
        {
#if UNITY_6000_5_OR_NEWER
            return target.GetEntityId().GetHashCode();
#else
            return target.GetInstanceID();
#endif
        }
    }
}

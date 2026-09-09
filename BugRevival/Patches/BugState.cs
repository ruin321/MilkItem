using UnityEngine;

namespace BugRevival
{
    // Player state shared across patches, maintained by the HideableLocker patch.
    public static class BugState
    {
        public static bool PlayerInLocker;
    }
}
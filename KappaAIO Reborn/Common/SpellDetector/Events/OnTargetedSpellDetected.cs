using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;

namespace KappAIO_Reborn.Common.SpellDetector.Events
{
    public class OnTargetedSpellDetected
    {
        public delegate void TargetedSpellDetected(DetectedTargetedSpellData args);
        public static event TargetedSpellDetected OnDetect;
        internal static void Invoke(DetectedTargetedSpellData args)
        {
            var invocationList = OnDetect?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            OnTargetedSpellUpdate.Invoke(args);
        }

        static OnTargetedSpellDetected()
        {
            new TargetedSpellDetector();
        }
    }

    public class OnTargetedSpellUpdate
    {
        public delegate void TargetedSpellUpdate(DetectedTargetedSpellData args);
        public static event TargetedSpellUpdate OnUpdate;
        internal static void Invoke(DetectedTargetedSpellData args)
        {
            var invocationList = OnUpdate?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);
        }

        static OnTargetedSpellUpdate()
        {
            new TargetedSpellDetector();
        }
    }
}

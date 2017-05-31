using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;

namespace KappAIO_Reborn.Common.SpellDetector.Events
{
    public class OnSpecialSpellDetected
    {
        public delegate void SpecialSpellDetected(DetectedSpecialSpellData args);
        public static event SpecialSpellDetected OnDetect;
        internal static void Invoke(DetectedSpecialSpellData args)
        {
            var invocationList = OnDetect?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            OnSpecialSpellUpdate.Invoke(args);
        }

        static OnSpecialSpellDetected()
        {
            new SpecialSpellDetector();
        }
    }

    public class OnSpecialSpellUpdate
    {
        public delegate void SpecialSpellUpdate(DetectedSpecialSpellData args);
        public static event SpecialSpellUpdate OnUpdate;
        internal static bool Invoke(DetectedSpecialSpellData args)
        {
            var invocationList = OnUpdate?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            return true;
        }

        static OnSpecialSpellUpdate()
        {
            new SpecialSpellDetector();
        }
    }
}

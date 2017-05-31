using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;

namespace KappAIO_Reborn.Common.SpellDetector.Events
{
    public class OnDangerBuffDetected
    {
        public delegate void DetectedDangerBuff(DetectedDangerBuffData args);
        public static event DetectedDangerBuff OnDetect;
        internal static void Invoke(DetectedDangerBuffData args)
        {
            var invocationList = OnDetect?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            OnDangerBuffUpdate.Invoke(args);
        }

        static OnDangerBuffDetected()
        {
            new DangerBuffDetector();
        }
    }

    public class OnDangerBuffUpdate
    {
        public delegate void UpdateDangerBuff(DetectedDangerBuffData args);
        public static event UpdateDangerBuff OnUpdate;
        internal static void Invoke(DetectedDangerBuffData args)
        {
            var invocationList = OnUpdate?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);
        }

        static OnDangerBuffUpdate()
        {
            new DangerBuffDetector();
        }
    }
}

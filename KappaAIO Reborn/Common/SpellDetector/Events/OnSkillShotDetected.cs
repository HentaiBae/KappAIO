using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;

namespace KappAIO_Reborn.Common.SpellDetector.Events
{
    public class OnSkillShotDetected
    {
        public delegate void SkillShotDetected(DetectedSkillshotData args);
        public static event SkillShotDetected OnDetect;
        internal static void Invoke(DetectedSkillshotData args)
        {
            var invocationList = OnDetect?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);
        }

        static OnSkillShotDetected()
        {
            new SkillshotDetector();
        }
    }

    public class OnSkillShotDelete
    {
        public delegate void SkillShotDelete(DetectedSkillshotData args);
        public static event SkillShotDelete OnDelete;
        internal static bool Invoke(DetectedSkillshotData args)
        {
            args.Ended = true;
            var invocationList = OnDelete?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            OnSkillShotUpdate.Invoke(args);
            return true;
        }

        static OnSkillShotDelete()
        {
            new SkillshotDetector();
        }
    }

    public class OnSkillShotUpdate
    {
        public delegate void SkillShotUpdate(DetectedSkillshotData args);
        public static event SkillShotUpdate OnUpdate;
        internal static bool Invoke(DetectedSkillshotData args)
        {
            args.Ended = true;
            var invocationList = OnUpdate?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            return true;
        }

        static OnSkillShotUpdate()
        {
            new SkillshotDetector();
        }
    }
}

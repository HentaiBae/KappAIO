using KappAIO_Reborn.Common.SpellDetector.DetectedData;
using KappAIO_Reborn.Common.SpellDetector.Detectors;

namespace KappAIO_Reborn.Common.SpellDetector.Events
{
    public class OnEmpoweredAttackDetected
    {
        public delegate void EmpoweredAttackDetected(DetectedEmpoweredAttackData args);
        public static event EmpoweredAttackDetected OnDetect;
        internal static void Invoke(DetectedEmpoweredAttackData args)
        {
            var invocationList = OnDetect?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);

            OnEmpoweredAttackUpdate.Invoke(args);
        }

        static OnEmpoweredAttackDetected()
        {
            new EmpoweredAttackDetector();
        }
    }
    public class OnEmpoweredAttackUpdate
    {
        public delegate void EmpoweredAttackUpdate(DetectedEmpoweredAttackData args);
        public static event EmpoweredAttackUpdate OnUpdate;
        internal static void Invoke(DetectedEmpoweredAttackData args)
        {
            var invocationList = OnUpdate?.GetInvocationList();
            if (invocationList != null)
                foreach (var m in invocationList)
                    m?.DynamicInvoke(args);
        }

        static OnEmpoweredAttackUpdate()
        {
            new EmpoweredAttackDetector();
        }
    }
}

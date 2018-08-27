
namespace IceEvents
{
    namespace GameState
    {
        public class Ice_GameStart : IceEvent_BASE
        {
            
        }
    }

    namespace Player
    {

    }

    namespace Weapons
    {
        public class Ice_DownWeaponState : IceEvent_BASE
        {

        }
        public class Ice_AimingWeaponState : IceEvent_BASE
        {

        }
        public class Ice_FiringWeaponState : IceEvent_BASE
        {

        }
        public class Ice_ThrowingWeaponState : IceEvent_BASE
        {

        }

        public class Ice_ReloadWeapon : IceEvent_BASE
        {

        }

        public class Ice_SetFiringLock : IceEvent_BASE
        {
            public float LockTime;

            public Ice_SetFiringLock(float locktime)
            {
                LockTime = locktime;
            }
        }
    }

    namespace Enemy
    {

    }

    namespace UI
    {

    }

    namespace Enemy
    {

    }
}


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
        public class Ice_PlayerDamage : IceEvent_BASE
        {
            public float Damage;

            public Ice_PlayerDamage(float damage)
            {
                Damage = damage;
            }
        }

        public class Ice_PlayerHeal : IceEvent_BASE
        {
            public float Heal;

            public Ice_PlayerHeal(float heal)
            {
                Heal = heal;
            }
        }

        public class Ice_PlayerDEATH : IceEvent_BASE
        {
            public Ice_PlayerDEATH()
            {
            }
        }
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

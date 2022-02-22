// VERBOSE or Debugger goes here
//#define PROJECTSLOWMO_STATEMACHINE_VERBOSE

namespace ProjectSlowMo.StateMachine
{
    [System.Serializable]
    public abstract class JButler_State : StateInterface
    {
        public virtual void PreInitialize() { }
        public virtual void Execute() { }
        public virtual void PhysicsExecute() { }
        public virtual void PostExecute() { }

        public virtual void OnAnimatorIK(int layerIndex) { }

        public virtual void Initialize()
        {
#if (PROJECTSLOWMO_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // PROJECTSLOWMO_STATEMACHINE_VERBOSE
        }

        public virtual void Enter()
        {
#if (PROJECTSLOWMO_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // PROJECTSLOWMO_STATEMACHINE_VERBOSE
        }

        public virtual void Exit()
        {
#if (PROJECTSLOWMO_STATEMACHINE_VERBOSE)
            UnityEngine.Debug.Log(machine.name + "." + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "()");
#endif // PROJECTSLOWMO_STATEMACHINE_VERBOSE
        }

        public T GetMachine<T>() where T : MachineInterface
        {
            try
            {
                return (T)machine;
            }
            catch (System.InvalidCastException e)
            {
                if (typeof(T) == typeof(JButler_MachineState) || typeof(T).IsSubclassOf(typeof(JButler_MachineState)))
                {
                    throw new System.Exception(machine.name + ".GetMachine() cannot return the type you requested!\tYour machine is derived from JButler_MachineBehaviour not JButler_MachineState!\n" + e.Message);
                }
                else if (typeof(T) == typeof(JButler_MachineBehaviour) || typeof(T).IsSubclassOf(typeof(JButler_MachineBehaviour)))
                {
                    throw new System.Exception(machine.name + ".GetMachine() cannot return the type you requested?\tYour machine is derived from JButler_MachineState not JButler_MachineBehaviour!\n" + e.Message);
                }
                else
                {
                    throw new System.Exception(machine.name + ".GetMachine() cannot return the type you requested!\n" + e.Message);
                }
            }
        }

        internal MachineInterface machine { get; set; }

        public bool isActive { get { return machine.IsCurrentState(GetType()); } }
    }
}
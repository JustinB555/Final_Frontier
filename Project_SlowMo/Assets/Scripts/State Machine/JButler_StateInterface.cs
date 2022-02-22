namespace ProjectSlowMo.StateMachine
{
    public interface StateInterface
    {
        // Awake
        void PreInitialize();
        // Start
        void Initialize();
        // Start, when you enter state.
        void Enter();
        // Update
        void Execute();
        // FixedUpdate
        void PhysicsExecute();
        // LateUpdate
        void PostExecute();
        // When you leave the state.
        void Exit();
        // Animation with Rigidbodies.
        void OnAnimatorIK(int layerIndex);
        bool isActive { get; }
        T GetMachine<T>() where T : MachineInterface;
    }
}
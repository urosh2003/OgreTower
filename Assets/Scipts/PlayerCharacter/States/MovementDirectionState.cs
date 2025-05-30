using UnityEngine.InputSystem;

public interface MovementDirectionState
{
    public void Enter();
    public void Exit();
    public void Update();
    public MovementDirectionState GoRight(InputAction.CallbackContext context);
    public MovementDirectionState GoLeft(InputAction.CallbackContext context);
}
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : MovementDirectionState
{
    public void Enter()
    {
        return;
    }

    public void Exit()
    {
        return;
    }

    public MovementDirectionState GoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            return new WalkingRightState();
        else if (context.canceled)
            return new WalkingLeftState();
        else return null;
    }

    public MovementDirectionState GoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            return new WalkingLeftState();
        else if (context.canceled)
            return new WalkingRightState();
        else return null;
    }

    public void Update()
    {
        PlayerManager.Instance.playerRigidBody.velocity = new Vector2(0, PlayerManager.Instance.playerRigidBody.velocity.y);

    }
}
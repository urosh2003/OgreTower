using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingLeftState : MovementDirectionState
{
    public void Enter()
    {
        PlayerManager.Instance.animator.SetBool("walking", true);
        PlayerManager.Instance.animator.SetBool("left", true);
        PlayerManager.Instance.animator.SetBool("right", false);
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("walking", false);
    }

    public MovementDirectionState GoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            return new IdleState();
        else if (context.canceled)
            return new RunningLeftState();
        else return null;
    }

    public MovementDirectionState GoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            return new RunningLeftState();
        else if (context.canceled)
            return new IdleState();
        else return null;
    }

    public void Update()
    {
        PlayerManager.Instance.playerRigidBody.velocity = new Vector2(-PlayerManager.Instance.walkingSpeed, PlayerManager.Instance.playerRigidBody.velocity.y);
    }
}
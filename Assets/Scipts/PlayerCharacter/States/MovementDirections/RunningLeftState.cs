using UnityEngine;
using UnityEngine.InputSystem;

public class RunningLeftState : MovementDirectionState
{
    public void Enter()
    {
        PlayerManager.Instance.animator.SetBool("running", true);
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("running", false);
    }
    public MovementDirectionState GoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            return new WalkingLeftState();
        else return null;
    }
    public MovementDirectionState GoLeft(InputAction.CallbackContext context)
    {
        if (context.canceled)
            return new WalkingLeftState();
        else return null;
    }
    public void Update()
    {
        PlayerManager.Instance.playerRigidBody.velocity = new Vector2(-PlayerManager.Instance.runningSpeed, PlayerManager.Instance.playerRigidBody.velocity.y);
    }
}
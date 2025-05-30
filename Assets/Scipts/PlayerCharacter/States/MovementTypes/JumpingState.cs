using UnityEngine;
using UnityEngine.InputSystem;

public class JumpingState : MovementTypeState
{
    public override void Enter()
    {
        return;
    }

    public override void Exit()
    {
        return;
    }

    public override void Jump()
    {
        PlayerManager.Instance.playerRigidBody.velocity = Vector3.zero;
        PlayerManager.Instance.playerRigidBody.AddForce(Vector3.up * PlayerManager.Instance.jumpForce, ForceMode2D.Impulse);
    }

    public override MovementTypeState JumpPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player1Jumped)
        {
            PlayerManager.Instance.player1Jumped = true;
            Jump();
            return new JumpingState();
        }
        return null;
    }

    public override MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player2Jumped)
        {
            PlayerManager.Instance.player2Jumped = true;
            Jump();
            return new JumpingState();
        }
        return null;
    }

    public override void Slam()
    {
        throw new System.NotImplementedException();
    }

    public override MovementTypeState SlamPlayer1(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public override MovementTypeState SlamPlayer2(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public override MovementTypeState Update()
    {
        throw new System.NotImplementedException();
    }
}
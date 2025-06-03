using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleJumpingState : MovementTypeState
{
    public void Enter()
    {
        PlayerManager.Instance.animator.SetBool("isJumping", true);
        Jump();
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("isJumping", false);
        return;
    }

    public void Jump()
    {
        PlayerManager.Instance.playerRigidBody.velocity = new Vector2(PlayerManager.Instance.playerRigidBody.velocity.x, PlayerManager.Instance.jumpForce);
    }

    public MovementTypeState JumpPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player1Jumped)
        {
            PlayerManager.Instance.player1Jumped = true;
            return new DoubleJumpingState();
        }
        return null;
    }

    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player2Jumped)
        {
            PlayerManager.Instance.player2Jumped = true;
            return new DashingState();
        }
        return null;
    }
    public MovementTypeState Update()
    {
        Debug.Log(PlayerManager.Instance.playerRigidBody.velocity);
        PlayerManager.Instance.playerRigidBody.velocity += new Vector2(0,
            Time.deltaTime * PlayerManager.Instance.gravityScale);

        if (PlayerManager.Instance.playerRigidBody.velocity.y <= 0.001f)
        {
            return new SlammingState();
        }

        return null;
    }
    public MovementTypeState BounceOff()
    {
        PlayerManager.Instance.player1Jumped = false;
        PlayerManager.Instance.player2Jumped = false;
        return new JumpingState();
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpingState : MovementTypeState
{
    public void Enter()
    {
        Jump();
    }

    public void Exit()
    {
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
            return new JumpingState();
        }
        return null;
    }

    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player2Jumped)
        {
            PlayerManager.Instance.player2Jumped = true;
            return new JumpingState();
        }
        return null;
    }

    public MovementTypeState SlamPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerManager.Instance.player1Slamming = true;
            if(PlayerManager.Instance.player2Slamming)
                return new SlammingState();
        }
        if (context.canceled)
            PlayerManager.Instance.player1Slamming = false;
        return null;
    }

    public MovementTypeState SlamPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerManager.Instance.player2Slamming = true;
            if (PlayerManager.Instance.player1Slamming)
                return new SlammingState();
        }
        if (context.canceled)
            PlayerManager.Instance.player1Slamming = false;
        return null;
    }

    public MovementTypeState Update()
    {
        Debug.Log(PlayerManager.Instance.playerRigidBody.velocity);
        PlayerManager.Instance.playerRigidBody.velocity += new Vector2(0, 
            Time.deltaTime * PlayerManager.Instance.gravityScale);

        if (PlayerManager.Instance.playerRigidBody.velocity.y <= 0.001f)
            return new FallingState();

        return null;
    }
    public MovementTypeState BounceOff()
    {
        return new JumpingState();
    }
}
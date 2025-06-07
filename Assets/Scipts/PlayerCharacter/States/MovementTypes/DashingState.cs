using UnityEngine;
using UnityEngine.InputSystem;

public class DashingState : MovementTypeState
{
    private float timeDashing;
    private float direction;
    public MovementTypeState BounceOff()
    {
        PlayerManager.Instance.player1Jumped = false;
        PlayerManager.Instance.player2Jumped = false;
        return new JumpingState();
    }

    public void Enter()
    {
        PlayerManager.Instance.animator.SetBool("isDashing", true);
        PlayerManager.Instance.isDashing = true;
        direction = PlayerManager.Instance.lastMovementDirection;
        PlayerManager.Instance.Dashing(direction);

        timeDashing = 0f;
    }

    public void Exit()
    {
        PlayerManager.Instance.isDashing = false;
        PlayerManager.Instance.animator.SetBool("isDashing", false);
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
        PlayerManager.Instance.playerRigidBody.velocity = new Vector2(direction * PlayerManager.Instance.dashSpeed, 0);
        timeDashing += Time.deltaTime;
        if(timeDashing >= PlayerManager.Instance.DASH_DURATION_CONST)
        {
            return new FallingState();
        }
        return null;
    }
}
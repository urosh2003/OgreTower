using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GroundedState : MovementTypeState
{
    public void Enter()
    {
        Debug.Log("Landed");
        PlayerManager.Instance.player1Jumped = false;
        PlayerManager.Instance.player2Jumped = false;
        PlayerManager.Instance.player1Slamming = false;
        PlayerManager.Instance.player2Slamming = false;
        PlayerManager.Instance.isSlamming = false;
        PlayerManager.Instance.animator.SetBool("isGrounded", true);
        PlayerManager.Instance.Grounded();
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("isGrounded", true);
    }

    public MovementTypeState JumpPlayer1(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            PlayerManager.Instance.player1Jumped = true;
            return new JumpingState();
        }
        return null;
    }

    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerManager.Instance.player2Jumped = true;
            return new JumpingState();
        }
        return null;
    }

    public void Slam()
    {
        throw new System.NotImplementedException();
    }

    public MovementTypeState SlamPlayer1(InputAction.CallbackContext context)
    {
        return null;
    }

    public MovementTypeState SlamPlayer2(InputAction.CallbackContext context)
    {
        return null;
    }

    public MovementTypeState Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(PlayerManager.Instance.playerTransform.position,
            PlayerManager.Instance.groundedBoxCast, 0f, Vector2.down, 0.01f, PlayerManager.Instance.groundLayerMask);
        if (hit.collider != null && hit.normal.y > 0.99f)
        {
            return null;
        }
        return new FallingState();
    }
    public MovementTypeState BounceOff()
    {
        return new JumpingState();
    }
}

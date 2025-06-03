using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : MovementTypeState
{
    public void Enter()
    {
        
    }

    public void Exit()
    {
        
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
        PlayerManager.Instance.playerRigidBody.velocity += new Vector2(0,
            PlayerManager.Instance.fallingGravity * Time.deltaTime);
        RaycastHit2D hit = Physics2D.BoxCast(PlayerManager.Instance.playerTransform.position, 
            PlayerManager.Instance.groundedBoxCast, 0f, Vector2.down, 0.01f, PlayerManager.Instance.groundLayerMask);
        if (hit.collider != null && hit.normal.y > 0.99f)
        {
            return new GroundedState();
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

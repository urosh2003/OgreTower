using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : MovementTypeState
{
    public void Enter()
    {
        Debug.Log("Aaaaaaaaaaaaaa");
        PlayerManager.Instance.animator.SetBool("isGrounded", true);
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("isGrounded", true);
    }

    public void Jump()
    {
        PlayerManager.Instance.playerRigidBody.velocity = Vector3.zero;
        PlayerManager.Instance.playerRigidBody.AddForce(Vector3.up * PlayerManager.Instance.jumpForce, ForceMode2D.Impulse);
    }

    public MovementTypeState JumpPlayer1(InputAction.CallbackContext context)
    {
        if(context.performed && !PlayerManager.Instance.player1Jumped)
        {
            PlayerManager.Instance.player1Jumped = true;
            Jump();
            return new JumpingState();
        }
        return null;
    }

    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed && !PlayerManager.Instance.player2Jumped)
        {
            PlayerManager.Instance.player2Jumped = true;
            Jump();
            return new JumpingState();
        }
        return null;
    }

    public MovementTypeState SlamPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlayerManager.Instance.player1Slamming = true;
            if (PlayerManager.Instance.player2Slamming)
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
        RaycastHit2D hit = Physics2D.BoxCast(PlayerManager.Instance.playerTransform.position, 
            PlayerManager.Instance.groundedBoxCast, 0f, Vector2.down, 0.01f, PlayerManager.Instance.groundLayerMask);
        if (hit.collider != null && hit.normal.y > 0.99f)
        {
            return new GroundedState();
        }
        return null;
    }
}

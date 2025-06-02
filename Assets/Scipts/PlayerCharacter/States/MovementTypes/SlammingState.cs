using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlammingState : MovementTypeState
{
    public MovementTypeState BounceOff()
    {
        return new JumpingState();
    }

    public void Enter()
    {
        Slam();
    }

    public void Exit()
    {
        PlayerManager.Instance.animator.SetBool("isGrounded", true);
    }

    public MovementTypeState JumpPlayer1(InputAction.CallbackContext context)
    {
        return null;
    }

    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context)
    {
        return null;
    }

    public void Slam()
    {
        PlayerManager.Instance.isSlamming = true;
        PlayerManager.Instance.playerRigidBody.velocity = Vector3.zero;
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
        PlayerManager.Instance.playerRigidBody.velocity += new Vector2(0,
            PlayerManager.Instance.slamForce * Time.deltaTime);

        RaycastHit2D hit = Physics2D.BoxCast(PlayerManager.Instance.playerTransform.position,
            PlayerManager.Instance.groundedBoxCast, 0f, Vector2.down, 0.01f, PlayerManager.Instance.groundLayerMask);
        if (hit.collider != null && hit.normal.y > 0.99f)
        {
            return new GroundedState();
        }
        return null;
    }
}

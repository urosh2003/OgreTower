using UnityEngine.InputSystem;

public abstract class MovementTypeState
{
    public bool player1Jumped;
    public bool player2Jumped;
    public bool player1Slamming;
    public bool player2Slamming;
    public abstract void Enter() ;
    public abstract void Exit() ;
    public abstract MovementTypeState Update() ;
    public abstract MovementTypeState JumpPlayer1(InputAction.CallbackContext context) ;
    public abstract MovementTypeState JumpPlayer2(InputAction.CallbackContext context) ;
    public abstract void Jump() ;
    public abstract void Slam() ;
    public abstract MovementTypeState SlamPlayer1(InputAction.CallbackContext context) ;
    public abstract MovementTypeState SlamPlayer2(InputAction.CallbackContext context) ;
}
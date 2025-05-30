using UnityEngine.InputSystem;

public interface MovementTypeState
{
    public void Enter() ;
    public void Exit() ;
    public MovementTypeState Update() ;
    public MovementTypeState JumpPlayer1(InputAction.CallbackContext context) ;
    public MovementTypeState JumpPlayer2(InputAction.CallbackContext context) ;
    public MovementTypeState SlamPlayer1(InputAction.CallbackContext context) ;
    public MovementTypeState SlamPlayer2(InputAction.CallbackContext context) ;
}
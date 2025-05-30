using UnityEngine.InputSystem;

public interface MovementTypeState
{
    public void Enter() ;
    public void Exit() ;
    public void Update() ;
    public void Jump() ;
    public void Slam() ;
    public void Dash() ;
}
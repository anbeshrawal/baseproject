using UnityEngine;

public class Death : BaseStates
{
    public override void EnterState(StateManager stateManager)
    {
        Debug.Log("Player is dead");
        Object.Destroy(stateManager.gameObject);
    }

    public override void UpdateState(StateManager stateManager)
    {
        
    }

    public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        
    }
}

using UnityEngine;

public class Death : BaseStates
{
    private StateManager currentStateManager;
    public override void EnterState(StateManager stateManager)
    {
        currentStateManager = stateManager;
        stateManager.animator.Play("Death", 0, 0f);
        Debug.Log("Player is dead");
    }

    public override void UpdateState(StateManager stateManager)
    {
        
    }

    public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        
    }
}

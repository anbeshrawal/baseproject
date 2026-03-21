using UnityEngine;

public abstract class BaseStates
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void EnterState(StateManager stateManager);
    public abstract void UpdateState(StateManager stateManager);

    public abstract void OnTriggerEnter2D(StateManager stateManager, Collider2D collision);
}

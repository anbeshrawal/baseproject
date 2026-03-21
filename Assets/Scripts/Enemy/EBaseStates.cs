using UnityEngine;

public abstract class EBaseStates
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void EnterState(BossStateManager stateManager);
    public abstract void UpdateState(BossStateManager stateManager);
    public abstract void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision);
}

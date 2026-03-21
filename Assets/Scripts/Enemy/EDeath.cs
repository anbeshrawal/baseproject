using UnityEngine;

public class EDeath : EBaseStates
{
    public override void EnterState(BossStateManager stateManager)
    {
        
        stateManager.animator.Play("Edeath");
        if(stateManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        Debug.Log("Boss is dead");
        Object.Destroy(stateManager.gameObject, 2f);
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        
    }

    public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {
        
    }
}

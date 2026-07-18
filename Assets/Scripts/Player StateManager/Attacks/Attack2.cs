using System.Collections;
using UnityEngine;

public class Attack2 : BaseStates
{
    float timer;

    public override void EnterState(StateManager stateManager)
    {
        stateManager.animator.SetTrigger("Attack2");
        stateManager.canCombo = true;
        timer = 0.5f;
    }

    public override void UpdateState(StateManager stateManager)
    {
        timer =- Time.deltaTime;
        exitstate(stateManager);
        if (stateManager.input.attackPressed && timer != 0)
        {
            stateManager.animator.Play("Attack2"); //switch to Combo
            Debug.Log("Combo");
        }
        if (timer <= 0)
        {
            stateManager.canCombo = false;
        }
    }

    void exitstate(StateManager stateManager)
    {
        if(stateManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        {
            if(stateManager.stamina >= 10f) 
            Object.Instantiate(stateManager.sword, stateManager.Attackpoint.position, stateManager.Attackpoint.rotation);
            stateManager.SwitchState(stateManager.Idle);
        }
    }

    public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        Debug.Log("Attack2 Hitbox Triggered");
        if (collision.gameObject.CompareTag("Enemy"))
        {
        collision.gameObject.GetComponent<BossStateManager>().takeDamage(stateManager.damage);
        Debug.Log("Enemy Hit");
        }
    }
}

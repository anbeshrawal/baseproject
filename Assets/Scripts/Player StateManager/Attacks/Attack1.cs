using System.Threading;
using UnityEngine;

public class Attack1 : BaseStates
{
    float timer;
    public override void EnterState(StateManager stateManager)
    {
        stateManager.animator.SetTrigger("Attack1");
        stateManager.canCombo = true;
        timer = 0.5f;
    }

    public override void UpdateState(StateManager stateManager)
    {
        timer =- Time.deltaTime;
        exitstate(stateManager);
        if (stateManager.input.attackPressed && timer != 0)
        {
            stateManager.animator.SetTrigger("Attack1"); //switch to Combo
        }
        if (timer <= 0)
        { 
            stateManager.canCombo = false;
        }
    }

    void exitstate(StateManager stateManager)
    {
        if(stateManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f  && stateManager.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            stateManager.isattacking = false;
            stateManager.SwitchState(stateManager.Idle);
        }
        else if(stateManager.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
           stateManager.isattacking = false;
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

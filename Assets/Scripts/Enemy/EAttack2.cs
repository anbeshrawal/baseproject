using System.Collections;
using UnityEngine;

public class EAttack2 : EBaseStates
{
    float cdowntimer;

    public override void EnterState(BossStateManager stateManager)
    {
        Debug.Log("Enemy Attack 2");
        cdowntimer = 3f;
        stateManager.animator.Play("EAttack2");
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        cdowntimer -= Time.deltaTime;
        if(cdowntimer <= 0f)
        {
            stateManager.SwitchState(stateManager.EIdle);
        }
        exitstate(stateManager);
        FlipEnemy(stateManager);

    }

    void exitstate(BossStateManager stateManager)
    {
        if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) > 5f)
        {
            stateManager.SwitchState(stateManager.EWalk);
        }
    }

    public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {
if (collision.gameObject.CompareTag("Player"))
         {
             Debug.Log("Player Hit E2");
             cdowntimer = cdowntimer + 1f;
             int random = Random.Range(0, 5);
             collision.gameObject.GetComponent<StateManager>().knockback(stateManager.KBF);
             collision.gameObject.GetComponent<StateManager>().takeDamage(stateManager.s1Damage);
             if(random == 1 || random == 3)
             {
                stateManager.SwitchState(stateManager.EAttack1);
             }
             {
         }
    }
    }

        private void FlipEnemy(BossStateManager stateManager)
    {
        if (stateManager.player.transform.position.x > stateManager.transform.position.x && stateManager.facingleft)
        {
            stateManager.flip();
        }
        else if (stateManager.player.transform.position.x < stateManager.transform.position.x && !stateManager.facingleft)
        {
            stateManager.flip();
        }
    }

}

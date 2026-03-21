
using UnityEngine;

public class EIdle : EBaseStates
{
    public float Timer;
    public override void EnterState(BossStateManager stateManager)
    {
        Timer = 1f;
        stateManager.animator.Play("EIdle");
        Debug.Log("Enemy Idle");
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0f)
        {
            switchstates(stateManager);
        }
        if (stateManager.player.transform.position.x > stateManager.transform.position.x && stateManager.facingleft)
        {
            stateManager.flip();
        }
        else if (stateManager.player.transform.position.x < stateManager.transform.position.x && !stateManager.facingleft)
        {
            stateManager.flip();
        }
    }
        public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {
            if (collision.gameObject.CompareTag("Player"))
             {
                 stateManager.SwitchState(stateManager.EAttack1);
             }
            {
            }
    }

    void switchstates(BossStateManager stateManager)
    {
        int random = Random.Range(0, 2);
        Debug.Log("Random Number: " + random);
        if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) < 5f)
        {
            if (random == 0)
            {
            stateManager.SwitchState(stateManager.EAttack2);
            }
            else if(random == 1)
            {
            stateManager.SwitchState(stateManager.EAttack1);
            }
        }
        else if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) > 5f && Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) < 10f)   
        {
            stateManager.SwitchState(stateManager.EWalk);
        }    
    }


}

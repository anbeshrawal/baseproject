using UnityEngine;

public class EAttack1 : EBaseStates
{
    float cdowntimer;
    public override void EnterState(BossStateManager stateManager)
    {
        Debug.Log("Enemy Attack 1");
        cdowntimer = 5f;
        stateManager.animator.Play("EAttack");
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        cdowntimer -= Time.deltaTime;
        if(cdowntimer <= 0f)
        {
            stateManager.SwitchState(stateManager.EIdle);
        }
        else
        {
        stateManager.animator.Play("EAttack");
        }
        exitstate(stateManager);
        FlipEnemy(stateManager);
    }

    void exitstate(BossStateManager stateManager)
    {
        if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) > 2f)
        {
            stateManager.SwitchState(stateManager.EWalk);
        }
    }
        public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {}


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



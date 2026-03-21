using UnityEngine;

public class EWalk : EBaseStates
{
    public float Timer;
    public override void EnterState(BossStateManager stateManager)
    {
        stateManager.animator.Play("EWalk");
        Debug.Log("Enemy Walk");
        Walk(stateManager);
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        Walk(stateManager);

    }

      public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
        }
}

void Walk(BossStateManager stateManager)
    {
        int random = Random.Range(0, 2);
        
        if (stateManager.player.transform.position.x > stateManager.transform.position.x && stateManager.facingleft)
        {
            stateManager.flip();
        }
        else if (stateManager.player.transform.position.x < stateManager.transform.position.x && !stateManager.facingleft)
        {
            stateManager.flip();
        }
        if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) > 2f)
        {
            if(stateManager.Stage1 == true)
            {
                stateManager.transform.position = Vector2.MoveTowards(stateManager.transform.position, stateManager.player.transform.position, stateManager.s1Speed * Time.deltaTime);
            }
            else if(stateManager.Stage2 == true)
            {
                stateManager.transform.position = Vector2.MoveTowards(stateManager.transform.position, stateManager.player.transform.position, stateManager.s1Speed*stateManager.SMofifier * Time.deltaTime);
            }
        }
        else if(Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) < 5f)
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
        
        
        if (Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) > 10f)   
        {
            stateManager.SwitchState(stateManager.EWalkBack);
        }

    }

    
}

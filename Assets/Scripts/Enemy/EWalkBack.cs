using UnityEngine;

public class EWalkBack : EBaseStates
{    
    public GameObject baseposition;
    public override void EnterState(BossStateManager stateManager)
    {
        baseposition = stateManager.baseposition;
        walkback(stateManager);
    }

    public override void UpdateState(BossStateManager stateManager)
    {
        if (Mathf.Abs(stateManager.player.transform.position.x - stateManager.transform.position.x) < 10f)   
        {
            stateManager.SwitchState(stateManager.EWalk);
        }
        walkback(stateManager);
    }

    public override void OnTriggerEnter2D(BossStateManager stateManager, Collider2D collision)
    {
        
    }   
private void walkback(BossStateManager stateManager)
    {
       
        Debug.Log("Walking back");
        stateManager.animator.Play("EWalk");
        if(stateManager.facingleft && stateManager.baseposition.transform.position.x > stateManager.transform.position.x)
        {
            stateManager.flip();
        }
        else if(!stateManager.facingleft && stateManager.baseposition.transform.position.x < stateManager.transform.position.x)
        {
            stateManager.flip();
        }
        stateManager.transform.position = Vector2.MoveTowards(stateManager.transform.position, stateManager.baseposition.transform.position, stateManager.s1Speed * Time.deltaTime);
        if(Mathf.Abs(stateManager.transform.position.x - baseposition.transform.position.x) <= 0.5f)
        {
            stateManager.SwitchState(stateManager.EIdle);
        }
    }
}

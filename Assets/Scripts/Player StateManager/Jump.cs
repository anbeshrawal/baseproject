using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Jump : BaseStates
{
    float jumpCounter;
    float SJF = 0.7f;
    public override void EnterState(StateManager stateManager)
    {
        Jumps(stateManager);
        stateManager.isGrounded = false;
        jumpCounter = 0;
        Animationchecker(stateManager); 
    }

    public override void UpdateState(StateManager stateManager)
    {
        Animationchecker(stateManager); 
        
        if (Input.GetKeyDown(KeyCode.Space) && jumpCounter < 3)
        {
            Jumps(stateManager);
        }
        
     
        if (stateManager.isGrounded == true)
        {
            jumpCounter = 0;
            stateManager.SwitchState(stateManager.Idle);
        }

        if(stateManager.rb.linearVelocity.y == 0f)
        {
            stateManager.SwitchState(stateManager.Idle);
        }
    }

    private void Jumps(StateManager stateManager)
    {
        if (jumpCounter == 0)
        {

            stateManager.rb.linearVelocity = new Vector2(stateManager.rb.linearVelocity.x, stateManager.jumpspeed);
            jumpCounter++;
        }
        else if (jumpCounter == 1)
        {
            stateManager.rb.linearVelocity = new Vector2(stateManager.rb.linearVelocity.x, stateManager.jumpspeed*SJF);
            jumpCounter++;
        }
        else if (jumpCounter == 2 && stateManager.stamina >= 2f)
        {
            stateManager.rb.linearVelocity = new Vector2(stateManager.rb.linearVelocity.x, stateManager.jumpspeed*SJF);
            jumpCounter++;
        }
        else
        {
            Debug.Log("No more jumps");
            return;
        }
    }
 
    private void Animationchecker(StateManager stateManager)
    {
        if (stateManager.rb.linearVelocity.y > 0f && jumpCounter == 0)
        {
            stateManager.animator.Play("Jump");
        }
        if (stateManager.rb.linearVelocity.y > 0f && jumpCounter == 1)
        {
            stateManager.animator.Play("Jump",0,0.5f);
        }
        if (stateManager.rb.linearVelocity.y > 0f && jumpCounter == 2 && stateManager.stamina >= 5f)
        {
            stateManager.animator.Play("Triple_Jump");
        }
        else if (stateManager.rb.linearVelocity.y == 0f)
        {
            stateManager.SwitchState(stateManager.Idle);
        }
    }
    public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            stateManager.isGrounded = true;
        }
}
}

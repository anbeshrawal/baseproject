using UnityEngine;

public class Run : BaseStates
{
    public override void EnterState(StateManager stateManager)
    {
        
    }

    public override void UpdateState(StateManager stateManager)
    {
        HandleInput(stateManager);
        movement(stateManager);
    }
    private void movement(StateManager stateManager)
    {
       if(stateManager.canmove == true && stateManager.isGrounded == true)
        {
            stateManager.animator.Play("Run");
        stateManager.targetSpeed = stateManager.xInput * stateManager.runSpeed;
        float speeddiff = stateManager.targetSpeed - stateManager.rb.linearVelocity.x;
        
        if(stateManager.targetSpeed == 0)
        {
            float amount = Mathf.Min(Mathf.Abs(stateManager.rb.linearVelocity.x), Mathf.Abs(stateManager.frictionAmount));
            amount *= Mathf.Sign(stateManager.rb.linearVelocity.x);
            stateManager.rb.AddForce(-amount * Vector2.right, ForceMode2D.Impulse);
        }
        else
        {
            float move = Mathf.Pow(Mathf.Abs(speeddiff), stateManager.velPower) * Mathf.Sign(speeddiff);
            stateManager.rb.AddForce(move * Vector2.right);
        }
        }
}
private void HandleInput(StateManager stateManager)
    {
        stateManager.xInput = stateManager.input.moveInput;
        if (stateManager.xInput == 0)
        {
            stateManager.SwitchState(stateManager.Idle);
        }
        else if (stateManager.input.sprintReleased && stateManager.xInput != 0)
        {
            stateManager.SwitchState(stateManager.Walk);
        }

        if (stateManager.input.jumpPressed)
        {
        stateManager.isGrounded = false;
        stateManager.SwitchState(stateManager.Jump);
        }
        if (stateManager.input.attackPressed && stateManager.isGrounded == true)
        {
            
                int random = Random.Range(1, 2);
                Debug.Log("Random: " + random);
                if(random == 1)
                {
                    stateManager.canmove = false;
                    stateManager.SwitchState(stateManager.Attack1);
                }
                else if(random == 2)
                {
                    stateManager.canmove = false;
                    stateManager.SwitchState(stateManager.Attack2);
                }
        }
    }


    #region Flip and Animations
void HandleFlip(StateManager stateManager)
    {   
        if (stateManager.rb.linearVelocity.x > 0 && stateManager.isFacingRight==false && stateManager.targetSpeed !=0)
        {
            Flip(stateManager);
        }
        else if (stateManager.rb.linearVelocity.x < 0 && stateManager.isFacingRight==true && stateManager.targetSpeed !=0)
        {
            Flip(stateManager);
        }
    }
void Flip(StateManager stateManager)
    {
        stateManager.facingDirection = stateManager.facingDirection * -1;
        stateManager.transform.Rotate(0,180,0);
        stateManager.isFacingRight = !stateManager.isFacingRight;
    }
  #endregion  


      public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            stateManager.isGrounded = true;
        }
}
}

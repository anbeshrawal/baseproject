using System.Threading;
using UnityEngine;

public class Idle : BaseStates
{
    public float Timer;
    public override void EnterState(StateManager stateManager)
    {
        stateManager.canmove = true;
        stateManager.animator.Play("Idle");
        Timer = 15f;
    }

    public override void UpdateState(StateManager stateManager)
    {
        Timer -= Time.deltaTime;
        if (Timer <= 2 && Timer > 0 && stateManager.rb.linearVelocity.x == 0)
        {
            stateManager.animator.Play("Idle_2");
        }
        HandleInput(stateManager);
    }

    private void HandleInput(StateManager stateManager)
    {
        stateManager.xInput = stateManager.input.moveInput;
        if (stateManager.xInput != 0 && stateManager.isGrounded == true && stateManager.canmove == true)
        {
            stateManager.SwitchState(stateManager.Walk);
        }
        else if (stateManager.input.sprintPressed && stateManager.xInput != 0 && stateManager.isGrounded == true && stateManager.canmove == true)
        {
            stateManager.SwitchState(stateManager.Run);
        }

        if (stateManager.input.jumpHeld)
        {
            stateManager.SwitchState(stateManager.Jump);
        }

        if (stateManager.input.attackPressed && stateManager.isGrounded == true)
        {

                int random = Random.Range(1, 3);
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
        public override void OnTriggerEnter2D(StateManager stateManager, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            stateManager.isGrounded = true;
        }
}
}

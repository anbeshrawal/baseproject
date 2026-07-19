using UnityEngine;

public class Jump : BaseStates
{
    private int jumpCounter;

    private const float SecondJumpForce = 0.7f;
    private const float HeavyLandingSpeed = 7f;

    private float startinggrav;



    public override void EnterState(StateManager stateManager)
    {
        jumpCounter = 0;

        stateManager.isGrounded = false;
        stateManager.wasGrounded = false;
        stateManager.isfalling = false;
        stateManager.lastFallingVelocity = 0f;

        startinggrav = stateManager.rb.gravityScale;

        JumpPlayer(stateManager);
    }

    public override void UpdateState(StateManager stateManager)
    {
        CheckSurrounds(stateManager);
        if (
            Input.GetKeyDown(KeyCode.Space) &&
            jumpCounter < 3
        )
        {
            JumpPlayer(stateManager);
        }

        CheckAnimationAndLanding(stateManager);
        // Save the current grounded state
        // for the next frame.
        stateManager.wasGrounded =
            stateManager.isGrounded;
    }

void CheckSurrounds(StateManager stateManager)
    {
        if(stateManager.isTouchingWall && !stateManager.isTouchingLedge && !stateManager.ledgeDetected)
        {
            stateManager.ledgeDetected = true;
            stateManager.SwitchState(stateManager.EdgeGrab);

        }

    }










    private void JumpPlayer(StateManager stateManager)
    {
        // Prevent the previous fall velocity
        // from being reused.
        stateManager.lastFallingVelocity = 0f;
        stateManager.isfalling = false;


        if (jumpCounter == 0)
        {
            stateManager.rb.linearVelocity =
                new Vector2(
                    stateManager.rb.linearVelocity.x,
                    stateManager.jumpspeed
                );

            jumpCounter = 1;

            stateManager.animator.Play(
                "Jump",
                0,
                0f
            );
        }
        else if (jumpCounter == 1)
        {
            stateManager.rb.linearVelocity =
                new Vector2(
                    stateManager.rb.linearVelocity.x,
                    stateManager.jumpspeed *
                    SecondJumpForce
                );

            jumpCounter = 2;

            stateManager.animator.Play(
                "Jump",
                0,
                0.5f
            );
        }
        else if (
            jumpCounter == 2 &&
            stateManager.stamina >= 2f
        )
        {
            stateManager.rb.linearVelocity =
                new Vector2(
                    stateManager.rb.linearVelocity.x,
                    stateManager.jumpspeed *
                    SecondJumpForce
                );

            stateManager.stamina -= 2f;
            jumpCounter = 3;

            stateManager.animator.Play(
                "Triple_Jump",
                0,
                0f
            );
        }
        else
        {
            Debug.Log("No more jumps");
        }
    }

    private void CheckAnimationAndLanding(
        StateManager stateManager
    )
    {
        float verticalVelocity =
            stateManager.rb.linearVelocity.y;

        // Player is airborne and falling.
        if (
            !stateManager.isGrounded &&
            verticalVelocity < 0f
        )
        {
            stateManager.isfalling = true;

            // Continuously save the latest
            // downward velocity.
            stateManager.lastFallingVelocity =
                verticalVelocity;

            float fallAnimationTime =
                MapValue(
                    verticalVelocity,
                    -stateManager.jumpspeed,
                    stateManager.jumpspeed,
                    0f,
                    1f,
                    true
                );

            stateManager.animator.Play(
                "Fall",
                0,
                fallAnimationTime
            );
        }

        bool justLanded =
            !stateManager.wasGrounded &&
            stateManager.isGrounded &&
            stateManager.isfalling;

        if (!justLanded)
        {
            return;
        }

        float landingSpeed =
            Mathf.Abs(
                stateManager.lastFallingVelocity
            );

        jumpCounter = 0;

        if (landingSpeed > HeavyLandingSpeed)
        {
            stateManager.animator.Play(
                "Heavy Drop",
                0,
                0f
            );

            // Do not switch to Idle here.
            // Use an animation event at the
            // end of Heavy Drop.
        }
        else
        {
            stateManager.SwitchState(
                stateManager.Idle
            );
        }

        // Clear the old fall data so it
        // cannot affect the next jump.
        stateManager.isfalling = false;
        stateManager.lastFallingVelocity = 0f;
    }

    public override void OnTriggerEnter2D(
        StateManager stateManager,
        Collider2D collision
    )
    {
        if (collision.CompareTag("Ground"))
        {
            stateManager.isGrounded = true;
        }
    }

    private float MapValue(
        float value,
        float min,
        float max,
        float newMin,
        float newMax,
        bool clamp
    )
    {
        float mappedValue =
            (value - min) /
            (max - min) *
            (newMax - newMin) +
            newMin;

        if (clamp)
        {
            mappedValue =
                Mathf.Clamp(
                    mappedValue,
                    newMin,
                    newMax
                );
        }

        return mappedValue;
    }


}
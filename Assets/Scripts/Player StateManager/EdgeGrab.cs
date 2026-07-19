using UnityEngine;

public class EdgeGrab : BaseStates
{
    private Vector2 ledgePositionBottom;
    private Vector2 ledgePosition1;
    private Vector2 ledgePosition2;

    private StateManager stateManager;
    private RigidbodyConstraints2D previousConstraints;

    public override void EnterState(StateManager stateManager)
    {
        Debug.Log("Entered Edge Grab State");

        stateManager.ledgeDetected = true;
        stateManager.canGrabLedge = true;
        stateManager.canmove = false;

        // Stop any remaining jump/fall movement.
        stateManager.rb.linearVelocity = Vector2.zero;

        // Remember the original constraints.
        previousConstraints = stateManager.rb.constraints;

        // Freeze the player without changing gravityScale.
        stateManager.rb.constraints = RigidbodyConstraints2D.FreezeAll;

        ledgePositionBottom = stateManager.wallcheck.position;

        CalculateLedgePositions(stateManager);

        // Move the player into the hanging position.

        // Play the hanging/climbing animation.
        stateManager.animator.Play("Edge Grab", 0, 0f);
    }

    public override void UpdateState(StateManager stateManager)
    {
        // Keep the character exactly at the hanging position.

    }

    public override void OnTriggerEnter2D(
        StateManager stateManager,
        Collider2D collision
    )
    {
    }

    private void CalculateLedgePositions(StateManager stateManager)
    {
        float direction = stateManager.isFacingRight ? 1f : -1f;

        ledgePosition1 = new Vector2(
            ledgePositionBottom.x +
            stateManager.ledgeClimbXOffset1 * direction,

            ledgePositionBottom.y +
            stateManager.ledgeClimbYOffset1
        );

        ledgePosition2 = new Vector2(
            ledgePositionBottom.x +
            stateManager.ledgeClimbXOffset2 * direction,

            ledgePositionBottom.y +
            stateManager.ledgeClimbYOffset2
        );
    Debug.Log("Ledge Position 1: " + ledgePosition1);
    Debug.Log("Ledge Position 2: " + ledgePosition2);
    }

    // Call this using an animation event.
    public void FinishLedgeClimb(StateManager stateManager)
    {

        stateManager.rb.constraints = previousConstraints;
        stateManager.ledgeDetected = false;
        stateManager.canGrabLedge = false;  
        stateManager.canmove = true;
        stateManager.transform.position = ledgePosition2;
        stateManager.SwitchState(stateManager.Idle);

    }
}
using UnityEngine;


public class PlayerInputHandler : MonoBehaviour
{
    [Header("Controller Deadzone")]
    [Range(0f, 0.5f)]
    public float stickDeadzone = 0.15f;

    // Cached input values — read these from state scripts
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool jumpHeld;
    [HideInInspector] public bool sprintPressed;
    [HideInInspector] public bool sprintHeld;
    [HideInInspector] public bool sprintReleased;
    [HideInInspector] public bool attackPressed;

    void Update()
    {
        ReadMovement();
        ReadJump();
        ReadSprint();
        ReadAttack();
    }

    private void ReadMovement()
    {
        // Keyboard: Horizontal axis (A/D, Left/Right arrows)
        float keyboardInput = Input.GetAxisRaw("Horizontal");

        // Gamepad: Left stick or D-pad (both mapped to Horizontal by default in Unity)
        // Input.GetAxisRaw already picks up joystick if configured,
        // but we also explicitly read joystick axes for safety
        float gamepadStick = Input.GetAxisRaw("Horizontal"); // Unity maps joystick to this by default

        // Apply deadzone to stick input
        if (Mathf.Abs(gamepadStick) < stickDeadzone)
        {
            gamepadStick = 0f;
        }

        // Use whichever has a larger magnitude
        moveInput = Mathf.Abs(keyboardInput) >= Mathf.Abs(gamepadStick) ? keyboardInput : gamepadStick;
    }

    private void ReadJump()
    {
        // Keyboard: Space
        bool kbJumpDown = Input.GetKeyDown(KeyCode.Space);
        bool kbJumpHeld = Input.GetKey(KeyCode.Space);

        // Gamepad: South button (A on Xbox, X on PlayStation)
        bool gpJumpDown = Input.GetKeyDown(KeyCode.JoystickButton0);
        bool gpJumpHeld = Input.GetKey(KeyCode.JoystickButton0);

        jumpPressed = kbJumpDown || gpJumpDown;
        jumpHeld = kbJumpHeld || gpJumpHeld;
    }

    private void ReadSprint()
    {
        // Keyboard: Left Shift
        bool kbSprintDown = Input.GetKeyDown(KeyCode.LeftShift);
        bool kbSprintHeld = Input.GetKey(KeyCode.LeftShift);
        bool kbSprintUp = Input.GetKeyUp(KeyCode.LeftShift);

        // Gamepad: Left Trigger (Axis 9 on most controllers) or Left Bumper (Button 4)
        bool gpSprintDown = Input.GetKeyDown(KeyCode.JoystickButton4);
        bool gpSprintHeld = Input.GetKey(KeyCode.JoystickButton4);
        bool gpSprintUp = Input.GetKeyUp(KeyCode.JoystickButton4);

        sprintPressed = kbSprintDown || gpSprintDown;
        sprintHeld = kbSprintHeld || gpSprintHeld;
        sprintReleased = kbSprintUp || gpSprintUp;
    }

    private void ReadAttack()
    {
        // Keyboard/Mouse: Left mouse button
        bool kbAttack = Input.GetKeyDown(KeyCode.Mouse0);

        // Gamepad: West button (X on Xbox, Square on PlayStation)
        bool gpAttack = Input.GetKeyDown(KeyCode.JoystickButton2);

        attackPressed = kbAttack || gpAttack;
    }
}

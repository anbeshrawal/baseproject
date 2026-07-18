using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StateManager : MonoBehaviour
{
BaseStates currentState;
public BaseStates CurrentState => currentState;

public Attack1 Attack1 = new Attack1();
public Attack2 Attack2 = new Attack2();

public Attack3 Attack3 = new Attack3();
public Idle Idle = new Idle();
public Prayheal Prayheal = new Prayheal();
public Walk Walk = new Walk();
public Run Run = new Run();
public Jump Jump = new Jump();
public Death Death = new Death();

public Animator animator;
public Rigidbody2D rb;
public PlayerInputHandler input;

public bool isDead = false;

#region PlayerOrient and Movement
public float xInput;
public bool canmove = true;
public bool isFacingRight = true;
public int facingDirection = 1;

public bool isfalling = false;

public float jumpspeed = 5f;
public Transform JumpPoint;
public Transform GroundCheck;
public GameObject jsword;

#endregion


#region PlayerStats

public float walkSpeed = 2f;
public float runSpeed = 4f;
public float targetSpeed;
public float velPower = 1.5f;
public float frictionAmount = 0.1f;
float groundist;

public int damage = 10;

public float stamina = 20f;
public int Health = 100;

#endregion

#region Attacks and Combos
    public Transform Attackpoint;
    public GameObject sword;
    public bool isattacking;

#endregion

public bool canCombo = true;

[SerializeField]protected float groundDistance;
[SerializeField]public LayerMask whatisGround;
[SerializeField] public bool isGrounded;

public bool wasGrounded;
public float lastFallingVelocity;

void Awake()
{
    animator = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
    input = GetComponent<PlayerInputHandler>();
    if (input == null)
    {
        input = gameObject.AddComponent<PlayerInputHandler>();
    }
}

void Start()
{
    currentState = Idle;
    currentState.EnterState(this);
}

void Update()
{
    if (isDead) return;

    currentState.UpdateState(this); 
    CheckCollision();
    fallcheck();
}

public void SwitchState(BaseStates state)
{
    currentState = state;
    currentState.EnterState(this); 
}



#region GroundCheck
public void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(GroundCheck.position, Vector2.down, groundDistance, whatisGround);
        canmove = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatisGround);
    }

    void fallcheck()
    {
       if (!isGrounded)
    {
        if (rb.linearVelocity.y < 0f)
        {
            isfalling = true;

            lastFallingVelocity = rb.linearVelocity.y;

            float vely = maped(
                rb.linearVelocity.y,
                -jumpspeed,
                jumpspeed,
                0f,
                1f,
                true
            );

            animator.Play("Fall", 0, vely);
        }
    }

    // Runs only once: the frame the player lands
    if (!wasGrounded && isGrounded)
    {
        Debug.Log("Landing Velocity: " + lastFallingVelocity);

        if (Mathf.Abs(lastFallingVelocity) > 7f)
        {
            animator.Play("Heavy Drop", 0, 0f);
        }
        else
        {
            currentState = Idle;
            currentState.EnterState(this);
        }

        isfalling = false;
    }

    wasGrounded = isGrounded;
}


 /*isfalling = true;
        float vely = maped(rb.linearVelocity.y, -jumpspeed, jumpspeed, 0f, 1f, true);
       float landingvel = rb.linearVelocity.y;
       Debug.Log("Landing Velocity: " + landingvel);
        if (!isGrounded && rb.linearVelocity.y < 0f)
        {
        animator.Play("Fall", 0, vely);
        }
    if (isGrounded)
    {
    if (Mathf.Abs(landingvel) > 10f)
    {
        animator.Play("Heavy Drop", 0, vely);
        isfalling =false;
    }
    else if(!isfalling)
    {
    currentState = Idle;
    currentState.EnterState(this); 
    }
    }}

           //while(rb.linearVelocity.y == -0.1f)
           //{
          // RaycastHit2D hit = Physics2D.Raycast(GroundCheck.position, Vector2.down, 50f, whatisGround);  
           //}
            
           // if(groundist < 4f)
            {
                animator.Play("Fall", 0, vely);
                heavyfalling = false;
                Debug.Log("falling");
           // }
            else if(groundist > 4f)
          //  {
                animator.Play("Heavy Fall", 0, vely);
                heavyfalling = true;
                Debug.Log("Heavy falling");
                
          //  }
        }
        else if (isGrounded && rb.linearVelocity.x == 0f && !isattacking && !heavyfalling)
        {
            currentState = Idle;
            currentState.EnterState(this);
        }
        else if (isGrounded && rb.linearVelocity.x == 0f && !isattacking && heavyfalling)
        {
            animator.Play("Heavy Drop", 0, vely);
            heavyfalling = false;
        }*/

private void OnDrawGizmos()
    {
        Gizmos.DrawLine(GroundCheck.position, GroundCheck.position + new Vector3(0, -groundDistance));
    }
#endregion

void Triplejump()
    {
        
        if(stamina >= 5f)
        {
            Instantiate(jsword, JumpPoint.position, JumpPoint.rotation);
            useStamina(2f);
        }       
        else
        {
            Debug.Log("Not enough stamina for triple jump");
        }
    }

void useStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0)
        {
            stamina = 0;
            Debug.Log("Out of stamina");
        }
    }

public void takeDamage(int damage)
    {
        if (isDead) return;

        Health -= damage;

        if (Health <= 0)
        {
        isDead = true;
        SwitchState(Death);
        }
    }

public void knockback(int KBF)
    {
        canmove = false;
        rb.AddForce(new Vector2(-facingDirection * KBF, 1), ForceMode2D.Impulse);
        if(rb.linearVelocity.y <= 0.1f)
        {
            canmove = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Death"))
        {
            Debug.Log("death");
            Destroy(gameObject);
        }
    }

    public float maped(float value, float min, float max, float newMin, float newMax, bool clamp)
    {
        float new_value = (value -  min) / ( max -  min) * (newMax - newMin) + newMin;

        return new_value;

    }

public void DestroyPlayer()
{
    Debug.Log("Player is being destroyed");
    Destroy(gameObject);
}

public void HeavyDropFinished()
{
    SwitchState(Idle);
}

}
using UnityEngine;

public class StateManager : MonoBehaviour
{
BaseStates currentState;
public BaseStates CurrentState => currentState;

public Attack1 Attack1 = new Attack1();
public Attack2 Attack2 = new Attack2();
public Idle Idle = new Idle();
public Walk Walk = new Walk();
public Run Run = new Run();
public Jump Jump = new Jump();
public Death Death = new Death();

public Animator animator;
public Rigidbody2D rb;
public PlayerInputHandler input;

#region PlayerOrient and Movement
public float xInput;
public bool canmove = true;
public bool isFacingRight = true;
public int facingDirection = 1;

public float jumpspeed = 5f;
public Transform JumpPoint;
public GameObject jsword;

#endregion


#region PlayerStats

public float walkSpeed = 2f;
public float runSpeed = 4f;
public float targetSpeed;
public float velPower = 1.5f;
public float frictionAmount = 0.1f;

public int damage = 10;

public float stamina = 20f;
public int Health = 100;

#endregion

#region Attacks and Combos
    public Transform Attackpoint;
    public GameObject sword;

#endregion

public bool canCombo = true;

[SerializeField]protected float groundDistance;
[SerializeField]protected LayerMask whatisGround;
[SerializeField] public bool isGrounded;

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
    currentState.UpdateState(this); 
    CheckCollision();
}

public void SwitchState(BaseStates state)
{
    currentState = state;
    currentState.EnterState(this); 
}

#region GroundCheck
public void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatisGround);
        canmove = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, whatisGround);
    }

private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundDistance));
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
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("Player is dead");
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

}

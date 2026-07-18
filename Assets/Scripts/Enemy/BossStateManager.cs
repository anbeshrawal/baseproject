using UnityEngine;

public class BossStateManager : MonoBehaviour
{
EBaseStates currentState;

public EAttack1 EAttack1 = new EAttack1();
public EAttack2 EAttack2 = new EAttack2();
public EIdle EIdle = new EIdle();
public EWalk EWalk = new EWalk();
public EDeath EDeath = new EDeath();
public EWalkBack EWalkBack = new EWalkBack();


    #region BossStats
    public float Health = 100f;
    public float s1Speed = 2f;
    public float SMofifier = 2f;
    public int s1Damage = 1;
    public int KBF = 20;
    public bool facingleft = true;

    #endregion

    #region BossAssets
    public Animator animator;
    public Rigidbody2D rb;
    public bool Stage1 = true;
    public bool Stage2 = false;
    public GameObject player;
    #endregion

    #region Attack2Spawn
    public Transform JumpPoint;
    public GameObject Projectiles;

    public GameObject baseposition;
    
    #endregion
    
    
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

    }
    
    
    
    void Start()
    {
            currentState = EIdle;
            currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
       currentState.UpdateState(this);
       if(Health <= 50f && Stage1 == true)
       {
        Stage1 = false;
        Stage2 = true;
       }
    }


    public void SwitchState(EBaseStates state)
    {
    currentState = state;
    currentState.EnterState(this); 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
         {
             Debug.Log("Player Hit");
             collision.gameObject.GetComponent<StateManager>().knockback(KBF);
             collision.gameObject.GetComponent<StateManager>().takeDamage(s1Damage);
         }

    }

    public void takeDamage(int damage)
    {
        Health -= damage;
        
        if (Health <= 0)
        {
            SwitchState(EDeath);
        }
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void flip()
    {
       transform.Rotate(0,180,0);
       facingleft = !facingleft;
    }

    void Projectile()
    {
            Instantiate(Projectiles, JumpPoint.position, JumpPoint.rotation);
    }

}

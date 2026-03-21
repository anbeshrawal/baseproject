using UnityEngine;

public class JSword : MonoBehaviour
{
   public float speed = 25f;
   public Rigidbody2D rb;
   public Animator animator;
   public LayerMask whatisGround;
   public GameObject Player;
   public Transform SpawnPoint;
   public StateManager PlayerStateManager;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        animator.Play("SwordInstantiate");
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerStateManager = Player.GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);
        if(hitInfo.CompareTag("Ground"))
            rb.linearVelocity = Vector2.zero;
            Player.transform.position = Vector2.MoveTowards(transform.position, SpawnPoint.position, 10f);
            PlayerStateManager.isGrounded = true;
            PlayerStateManager.canmove = true;
            animator.Play("SwordHit wall");
            Destroy(gameObject, 3f);


        if(hitInfo.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            hitInfo.GetComponent<BossStateManager>().takeDamage(10);
        }
        else
        {
            Destroy(gameObject, 3f);
            Debug.Log("Player");
        }

    
    } 
}

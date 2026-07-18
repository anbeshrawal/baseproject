using System.Collections;
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
    public float destroyDelay = 2f;

    private bool isTeleporting = false;
    private Collider2D swordCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        swordCollider = GetComponent<Collider2D>();

        rb.linearVelocity = transform.right * speed;
        animator.Play("SwordInstantiate");

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerStateManager = Player.GetComponent<StateManager>();
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (isTeleporting) return;

        if (hitInfo.CompareTag("Ground"))
        {
            StartCoroutine(TeleportToSwordTopThenDestroy());
            return;
        }

        if (hitInfo.CompareTag("Enemy"))
        {
            hitInfo.GetComponent<BossStateManager>()?.takeDamage(10);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 2f);
    }

    private IEnumerator TeleportToSwordTopThenDestroy()
    {
        isTeleporting = true;

        rb.linearVelocity = Vector2.zero;
        if (swordCollider != null) swordCollider.enabled = false;

        animator.Play("SwordHit wall");

        // Let sword settle for one frame before placing player on top
        yield return null;

        if (Player != null)
        {
            Vector3 topPosition;
            Collider2D playerCollider = Player.GetComponent<Collider2D>();

            if (playerCollider != null && swordCollider != null)
            {
                float y = swordCollider.bounds.max.y + playerCollider.bounds.extents.y + 0.02f;
                topPosition = new Vector3(transform.position.x, y, Player.transform.position.z);
            }
            else
            {
                topPosition = new Vector3(transform.position.x, transform.position.y + 1f, Player.transform.position.z);
            }

            Player.transform.position = topPosition;
        }

        if (PlayerStateManager != null)
        {
            PlayerStateManager.isGrounded = true;
            PlayerStateManager.canmove = true;
        }

        yield return new WaitForSeconds(Mathf.Clamp(destroyDelay, 1f, 2f));
        Destroy(gameObject);
    }
}
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    StateManager playerStateManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GetComponentInParent<StateManager>() != null)
        playerStateManager = GetComponentInParent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(playerStateManager.stamina<=5 && playerStateManager.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            {
                collision.gameObject.GetComponent<BossStateManager>().takeDamage(10);
                Debug.Log("Enemy Hit");
            }
            else
            {
                collision.gameObject.GetComponent<BossStateManager>().takeDamage(10);
                Debug.Log("Enemy Hit");
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<StateManager>().takeDamage(10);
            collision.gameObject.GetComponent<StateManager>().knockback(2);
            Debug.Log("Boss Hit");
        }
    }
}

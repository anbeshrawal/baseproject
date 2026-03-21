using UnityEngine;

public class AttackSword : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public float speed = 10f;
   public Rigidbody2D rb;
   public int damage = 10;
    void Start()
    {
        rb.linearVelocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
       if (hitInfo.gameObject.CompareTag("Enemy"))
        {
        hitInfo.gameObject.GetComponent<BossStateManager>().takeDamage(damage);
        Debug.Log("Enemy Hit");
        }
        Destroy(gameObject);
    } 
}

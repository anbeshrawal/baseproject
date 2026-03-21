using UnityEngine;

public class Fires : MonoBehaviour
{
    public Transform target; 
    public float speed = 20f;
    public Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        HIT(); 
    }

    void HIT()
    {
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            rb.transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<StateManager>().takeDamage(5);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

}

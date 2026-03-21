using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Fires;
    public float timer;
    public float max;
    public float min;
    public float endtime;
    void Start()
    {
        Instantiate(Fires, transform.position, transform.rotation);
        max = 5f;
        min = 2f;
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        timer = Random.Range(min, max);
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            Instantiate(Fires, transform.position, transform.rotation);
        }
    }


    
}

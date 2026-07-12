using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public float startPosition, length;  
   public float parallaxEffect;
   [SerializeField] private GameObject cam;

    void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);
        float movement = cam.transform.position.x * (1 - parallaxEffect);


        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if (movement > startPosition +  length)
        {
            startPosition += length;
        }
        else if (movement < startPosition - length)
        {
            startPosition -= length;
        }
    }
}

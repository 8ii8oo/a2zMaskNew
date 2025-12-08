using UnityEngine;
using System.Collections; 

public class DestroyObject : MonoBehaviour
{
  
    public float destroyTime = 0.5f; 

    void Start()
    {
       
        StartCoroutine(TimeDestroy(destroyTime));
    }

    
    IEnumerator TimeDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

   
    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.gameObject.CompareTag("Player"))
        {
           
            Destroy(gameObject);
        }
    }
}
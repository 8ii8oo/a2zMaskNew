using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFind : MonoBehaviour
{
    string targetTag = "Enemy";
    public GameObject Potal;
    
    GameObject[] objectsWithTag;
   
   
    void Start()
    {
       
       
    }

    void Update()
    {
         objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);

        if(objectsWithTag.Length == 0)
        {
            
            Potal.SetActive(true);
        }
        
    }
}

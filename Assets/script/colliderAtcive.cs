using UnityEngine;

public class colliderAtcive : MonoBehaviour
{
    Collider2D collid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider2D collid = gameObject.GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            collid.enabled = false;
            Invoke("colliderAtcive", 1f);
        }
    }

    void ColliderActive()
    {
        collid.enabled = true;
    }
}

using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public string sceneToLoad;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.D))
        {
            PortalTransition.BeginTransition(other.gameObject, sceneToLoad);
        }
    }
}

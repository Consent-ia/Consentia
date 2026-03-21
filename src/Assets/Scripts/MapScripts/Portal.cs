using UnityEngine;

public class Portal : MonoBehaviour
{
    private ChangeScene changeScene;
    
    private void Start()
    {
        changeScene = GetComponent<ChangeScene>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            changeScene.Change();
        }
    }
}

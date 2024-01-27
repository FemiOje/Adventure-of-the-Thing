using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject inkManagerGameObject;
    [SerializeField] private InkManager inkManager;
    private void Start()
    {
        inkManagerGameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Instantiate(inkManager);
            inkManagerGameObject.SetActive(true);

            // Example: Start the scenario when the player enters a trigger zone
            inkManager.StartStory();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            // Instantiate(inkManager);
            inkManagerGameObject.SetActive(false);

        }
    }
}

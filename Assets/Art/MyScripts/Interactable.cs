using UnityEngine;
using TMPro;
using System.Collections;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;

    public string objectName = "Object Name";

    private TextMeshProUGUI hoverText;

    public void Start()
    {
        // Referenz auf das HoverText-UI-Element suchen
        GameObject hoverTextObject = GameObject.Find("HoverText");
        if (hoverTextObject != null)
        {
            hoverText = hoverTextObject.GetComponent<TextMeshProUGUI>();
            if (hoverText != null)
            {
                hoverText.text = "";
            }
            else
            {
                Debug.LogWarning("HoverText GameObject found, but it does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject named 'HoverText' found in the scene.");
        }
    }

    void OnMouseEnter()
    {
        // Setze den Text des HoverText-UI-Elements auf den Namen des Objekts
        if (hoverText != null)
        {
            hoverText.text = objectName;
            hoverText.gameObject.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        // Verberge das HoverText-UI-Element, wenn der Mauszeiger das Objekt verlässt
        if (hoverText != null)
        {
            hoverText.text = "";
            hoverText.gameObject.SetActive(false);
        }
    }

    public virtual void Interact()
    {
        // This method is meant to be overwritten
        Debug.Log("Interacting");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Coroutine to check the distance to the player
    public IEnumerator CheckDistanceToPlayer(PlayerController player, UnityEngine.AI.NavMeshAgent agent)
    {
        while (player.focus == this)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= radius)
            {
                agent.ResetPath();
                Interact();
                yield break; // Exit the coroutine once the object is destroyed
            }
            yield return null;
        }
    }
}

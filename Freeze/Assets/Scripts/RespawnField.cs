using UnityEngine;

public class RespawnField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Trigger"))
            return;
        Debug.Log("Triggered by " + other.transform);

        if (other.TryGetComponent(out IRespawn respawn))
        {
            respawn.Respawn();
        }
        /*MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            Debug.Log("Script: " + script.name);
            if (script is IRespawn respawnScript)
            {
                respawnScript.Respawn();
                break;
            }
        }*/
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour, IRespawn
{
    [SerializeField] private bool _physicsObject = false;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 1f;

    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;

    private void Start()
    {
        startPos = transform.position;
        endPos = new Vector3(startPos.x, startPos.y + bobHeight, startPos.z);
        if (_physicsObject)
            return;
        StartCoroutine(Bob());
    }

    void Update()
    {
        if(_physicsObject && transform.position.y < RespawnTarget.yLimit)
        {
            Respawn();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered " + other.transform);
        if (other.CompareTag("Player"))
        {
            SceneController.instance.GoToNextLevel();
        }
    }

    private IEnumerator Bob()
    {
        float duration = bobHeight / bobSpeed;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float factor = elapsedTime / duration;
            factor = Mathf.SmoothStep(0,1, factor);
            transform.position = Vector3.Lerp(startPos, endPos, factor);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        transform.position = endPos;

        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float factor = elapsedTime / duration;
            factor = Mathf.SmoothStep(0, 1, factor);
            transform.position = Vector3.Lerp(endPos, startPos, factor);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        transform.position = startPos;
        StartCoroutine(Bob());
    }

    public void Respawn()
    {
        
    }
}

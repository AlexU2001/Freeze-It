using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    [SerializeField] private AudioClip[] _music;
    public static MusicManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject controllerObj = new GameObject();
                controllerObj.name = "SceneController";
                _instance = controllerObj.AddComponent<MusicManager>();
                DontDestroyOnLoad(controllerObj);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            //Debug.Log("Instance was null");
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}

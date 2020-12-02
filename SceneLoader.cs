public class SceneLoader: UnityEngine.MonoBehaviour
{
    private void Awake()
    {
        if(FindObjectsOfType<SceneLoader>().Length > 1)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }
    public void LoadScene(int scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}

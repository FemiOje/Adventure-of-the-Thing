using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private LevelsData levelsData;
    [SerializeField] private GameObject lightBanditPrefab;
    [SerializeField] private GameObject heavyBanditPrefab;
    [SerializeField] private GameObject heartPrefab;

    private int _currentLevelIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentLevelIndex = scene.buildIndex;
        LoadLevel(_currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelsData.levels.Count)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        LevelData levelData = levelsData.levels[levelIndex];

        foreach (Vector2 position in levelData.lightBanditPositions)
        {
            Instantiate(lightBanditPrefab, position, Quaternion.identity);
        }

        foreach (Vector2 position in levelData.heavyBanditPositions)
        {
            Instantiate(heavyBanditPrefab, position, Quaternion.identity);
        }

        foreach (Vector2 position in levelData.heartPositions)
        {
            Instantiate(heartPrefab, position, Quaternion.identity);
        }
    }
}

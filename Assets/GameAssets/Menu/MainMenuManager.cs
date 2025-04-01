using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    string levelToLoadTEMP, gameplaySceneName, levelEditorSceneName;
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.currentLevelPath = levelToLoadTEMP;
    }

    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadLevelEditorScene()
    {
        SceneManager.LoadScene(levelEditorSceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour {

    public static ScenesManager instance;
    public bool lockScene = true;

    public const int titleSceneIndex = 0;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            Destroy(gameObject);
    }

	public void LoadScene(int index) {

        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void UnloadScene(int index) {

        SceneManager.UnloadSceneAsync(index);
    }

    public Scene GetCurrentScene() {

        //return SceneManager.GetActiveScene();
        return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    }

    public void SetCurrentScene(int index) {

        if (index < 0)
            return;

        //SceneManager.SetActiveScene(SceneManager.GetSceneAt(index));
    }
}

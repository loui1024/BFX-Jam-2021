using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public static void LoadLevel(int _level) {
        SceneManager.LoadScene(_level);
    }

    public static void Quit() {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}

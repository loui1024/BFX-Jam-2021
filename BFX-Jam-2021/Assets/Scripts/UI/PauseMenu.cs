using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    /* REFERENCES */
    public GameObject m_HUDItems;
    public Behaviour m_PausedImageEffect;

    /* PUBLIC */
    public bool m_Paused { get; private set; }

    public void TogglePause(bool _enabled) {

        m_Paused = _enabled;

        m_PausedImageEffect.enabled = m_Paused;

        Time.timeScale = m_Paused ? 0.0f : 1.0f;

        gameObject.SetActive( m_Paused);
        m_HUDItems.SetActive(!m_Paused);
    }

    public void LoadMenu() {
        Menu.LoadLevel(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAnimation : MonoBehaviour {

    [SerializeField] Animation m_Animation;

    void LateUpdate() {
        if (m_Animation.isPlaying == false) {
            m_Animation.Play();
        }
    }
}

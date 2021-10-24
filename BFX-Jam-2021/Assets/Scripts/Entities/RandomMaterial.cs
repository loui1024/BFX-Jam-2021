using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour {

    /* REFERENCES */
    [SerializeField] private Renderer m_Renderer;

    /* PRIVATE */
    [SerializeField] private Material[] m_Materials;

    void Start() {
        m_Renderer.material = m_Materials[Random.Range(0, m_Materials.Length)];
    }
}

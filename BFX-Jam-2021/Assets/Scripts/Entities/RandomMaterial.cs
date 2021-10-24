using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RandomMaterial : MonoBehaviour {

    /* REFERENCES */
    private Renderer m_Renderer;

    /* PRIVATE */
    [SerializeField] private Material[] m_Materials;

    void Start() {
        m_Renderer = GetComponent<Renderer>();

        m_Renderer.material = m_Materials[Random.Range(0, m_Materials.Length)];
    }
}

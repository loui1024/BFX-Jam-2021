using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    /* REFERENCES */
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private Camera m_Camera;

    [Header("Bike")]
    [SerializeField] private Transform m_Model;

    /* PRIVATE */
    private Vector2 m_MoveInput;
    private Vector3 m_Motion;

    private readonly Vector3 ZERO3 = new Vector3();
    private readonly Vector2 ZERO2 = new Vector2();

    void Start() {

        if (!m_Rigidbody) {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        if (!m_Camera) {
            m_Camera = Camera.main;
        }
        if (!m_Model) {
            m_Model = transform.Find("Model"); 
        }
    }

    private void Update() {
        PInput();
    }

    private void LateUpdate() {
        PModel();   
    }

    void FixedUpdate() {
        Vector3 motion = new Vector3(m_MoveInput.x, 0, m_MoveInput.y);

        Quaternion quat = Quaternion.Euler(0, m_Camera.transform.eulerAngles.y, 0);

        m_Motion = quat * motion * PlayerPreferences.Instance.m_Acceleration;

        m_Rigidbody.AddForce(m_Motion, ForceMode.Acceleration);
        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, PlayerPreferences.Instance.m_Speed);

    }

    void PInput() {
        m_MoveInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    void PModel() {

        if (m_Rigidbody.velocity != ZERO3) {
            m_Model.transform.forward = Vector3.Slerp(m_Model.transform.forward, m_Rigidbody.velocity, Time.deltaTime * 5.0f);
        }
    }
}

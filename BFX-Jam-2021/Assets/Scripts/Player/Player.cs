using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    /* REFERENCES */
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private Camera    m_Camera;

    [Header("Bike")]
    [SerializeField] private Transform m_Model;

    [Header("Throwing")]
    [SerializeField] private Transform m_Crosshair;

    /* PRIVATE */
    private Vector3 m_Motion;

    private Vector2 m_MoveInput;

    private bool m_LockThrowInput = false;
    private bool m_ThrowInput     = false;

    private int m_CurrentPoolItem = 0;
    private List<Rigidbody> m_ThrowPool;

    private readonly Vector3 ZERO3 = new Vector3();

    void Start() {

        m_ThrowPool = new List<Rigidbody>(PlayerPreferences.Instance.m_ThrowPoolSize);

        if (!m_Rigidbody) {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        if (!m_Camera) {
            m_Camera = Camera.main;
        }
        if (!m_Model) {
            m_Model = transform.Find("Model"); 
        }
        if (!m_Crosshair) {
            m_Crosshair = transform.Find("Crosshair");
        }
    }

    private void Update() {
        PInput();

        if (m_ThrowInput) {
            PThrow(1.0f);
        }
    }

    private void LateUpdate() {
        PModel();
        PCrosshair();
    }

    void FixedUpdate() {

        Vector3 motion = new Vector3(m_MoveInput.x, 0, m_MoveInput.y);

        Quaternion quat = Quaternion.Euler(0, m_Camera.transform.eulerAngles.y, 0);

        m_Motion = quat * motion * PlayerPreferences.Instance.m_Acceleration;

        m_Rigidbody.AddForce(m_Motion, ForceMode.Acceleration);
        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, PlayerPreferences.Instance.m_Speed);
    }

    void PThrow(float _power) {

        Rigidbody throwObject;

        if (m_ThrowPool.Count < PlayerPreferences.Instance.m_ThrowPoolSize) {
            throwObject = GameObject.Instantiate(
                PlayerPreferences.Instance.m_ThrowObject,
                transform.position + (transform.rotation * PlayerPreferences.Instance.m_ThrowOffset),
                m_Crosshair.rotation
            ).GetComponent<Rigidbody>();

            m_ThrowPool.Add(throwObject);
        }
        else {
            throwObject = m_ThrowPool[m_CurrentPoolItem];

            throwObject.velocity = ZERO3;
            throwObject.angularVelocity = ZERO3;

            throwObject.position = transform.position + (transform.rotation * PlayerPreferences.Instance.m_ThrowOffset);
            throwObject.rotation = m_Crosshair.rotation;

            m_CurrentPoolItem = (int)Mathf.Repeat(m_CurrentPoolItem + 1, PlayerPreferences.Instance.m_ThrowPoolSize);
        }

        throwObject.AddForce(m_Crosshair.forward * Mathf.LerpUnclamped(0, PlayerPreferences.Instance.m_ThrowPower, _power), ForceMode.VelocityChange);
        throwObject.AddTorque(Random.insideUnitSphere * PlayerPreferences.Instance.m_RandomSpin);
    }

    void PInput() {
        m_MoveInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (Input.GetAxis("Fire1") > 0.2f) {
            m_ThrowInput = !m_LockThrowInput;

            m_LockThrowInput = true;
        }
        else {
            m_LockThrowInput = false;
            m_ThrowInput     = false;
        }
    }

    void PModel() {
        if (m_Rigidbody.velocity != ZERO3) {
            m_Model.transform.forward = Vector3.Slerp(m_Model.transform.forward, m_Rigidbody.velocity, Time.deltaTime * 5.0f);
        }
    }

    void PCrosshair() {

        Vector2  bikePos = ((m_Camera.WorldToScreenPoint(transform.position) / new Vector2(Screen.width, Screen.height)) - new Vector2(0.5f, 0.5f)) * 2f;
        Vector2 mousePos = ((Input.mousePosition / new Vector2(Screen.width, Screen.height)) - new Vector2(0.5f, 0.5f)) * 2f;

        float angle = Vector2.SignedAngle(new Vector2(0f, 1f), mousePos + bikePos);

        m_Crosshair.transform.eulerAngles = new Vector3(0, 45f - angle, 0);
    }
}

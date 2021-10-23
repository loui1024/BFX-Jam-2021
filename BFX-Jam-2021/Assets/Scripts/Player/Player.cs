using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Player : MonoBehaviour {

    /* REFERENCES */
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private Camera    m_Camera;

    [Header("Bike")]
    [SerializeField] private Transform m_Model;

    [Header("Throwing")]
    [SerializeField] private Transform m_Crosshair;

    [Header("UI")]
    [SerializeField] private Image m_ThrowPowerImage;
    [SerializeField] private Gradient m_ThrowFillGradient;
    [SerializeField] private Animation m_JusticePopupImage;

    /* PRIVATE */
    private Vector3 m_Motion;

    private Vector2 m_MoveInput;

    private bool  m_ThrowInputHeld = false;
    private float m_ThrowInput     = -1.0f;

    private bool m_JusticePopupTrigger = false;
    private bool m_LockJumpInput  = false;
    private bool m_JumpInput      = false;

    private float m_ThrowPowerDir = 1.0f;

    private int m_CurrentPoolItem = 0;
    private List<Rigidbody> m_ThrowPool;

    private readonly Vector3 ZERO3 = new Vector3();
    private readonly Vector2 ZERO2 = new Vector2();

    private void Awake() {

        m_ThrowPool = new List<Rigidbody>(PlayerPreferences.Instance.m_ThrowPoolSize);
        m_JusticePopupImage.transform.localScale = ZERO3;

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

        if (m_ThrowInput > 0.0f) {
            m_JusticePopupTrigger = m_ThrowInput >= 0.8f;
            
            PThrow(m_ThrowInput);

            m_ThrowInput = -1.0f;
        }
    }

    private void LateUpdate() {
        PModel();
        PCrosshair();
        PUI();
    }

    private void FixedUpdate() {
        PPhysicsInput();
        PMove();
    }

    private void PUI() {

        m_ThrowPowerImage.gameObject.SetActive(m_ThrowInputHeld);

        if (m_ThrowPowerImage.gameObject.activeSelf) {
            float dir = m_Crosshair.transform.eulerAngles.y <= 45.0f || m_Crosshair.transform.eulerAngles.y > 225.0f ? 1.0f : -1.0f;

            m_ThrowPowerImage.rectTransform.position = Vector3.Lerp(
                m_ThrowPowerImage.rectTransform.position,
                m_Camera.WorldToScreenPoint(transform.position) + new Vector3(90 * dir, 0, 0),
                Time.deltaTime * 8.0f
            );

            m_ThrowPowerImage.fillAmount += ((m_ThrowPowerImage.fillAmount * Time.deltaTime * 4.0f) + (Time.deltaTime * 1.0f)) * m_ThrowPowerDir;

            if (m_ThrowPowerImage.fillAmount >= 1.0f || m_ThrowPowerImage.fillAmount <= 0.0f) {
                m_ThrowPowerDir *= -1f;
            }

            m_ThrowPowerImage.color = m_ThrowFillGradient.Evaluate(m_ThrowPowerImage.fillAmount);
        }

        if (m_JusticePopupTrigger) {
            m_JusticePopupImage.transform.position = m_ThrowPowerImage.transform.position;

            m_JusticePopupImage.Stop();
            m_JusticePopupImage.Play();

            m_JusticePopupTrigger = false;
        }
    }

    private void PMove() {

        Quaternion quat = Quaternion.Euler(0, m_Camera.transform.eulerAngles.y, 0);

        m_Motion = quat * new Vector3(m_MoveInput.x, 0, m_MoveInput.y) * PlayerPreferences.Instance.m_Acceleration;

        Vector2 lateralMotion = new Vector2(
            m_Rigidbody.velocity.x,
            m_Rigidbody.velocity.z
        );

        float velocity = lateralMotion.magnitude;

        float smoothStop = Mathf.Clamp01(PlayerPreferences.Instance.m_Speed - velocity);

        if (smoothStop != 0) {
            smoothStop = smoothStop * smoothStop;
        };

        m_Rigidbody.AddForce(m_Motion * smoothStop, ForceMode.Acceleration);

        if (m_JumpInput && Physics.Raycast(transform.position, new Vector3(0, -1, 0), 1.1f)) {
            m_Rigidbody.AddForce(new Vector3(0, PlayerPreferences.Instance.m_Jump, 0), ForceMode.VelocityChange);
        }
    }

    private void PThrow(float _power) {

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

        throwObject.velocity = m_Rigidbody.velocity;

        throwObject.AddForce(m_Crosshair.forward * Mathf.LerpUnclamped(0, PlayerPreferences.Instance.m_ThrowPower, _power), ForceMode.VelocityChange);
        throwObject.AddRelativeTorque(new Vector3(0, 0, Random.Range(-PlayerPreferences.Instance.m_RandomSpin / 2, PlayerPreferences.Instance.m_RandomSpin / 2)));
    }

    private void PInput() {

        if (Input.GetAxis("Fire1") > 0.2f) {

            if (!m_ThrowInputHeld) {
                m_ThrowPowerImage.fillAmount = Random.Range(0.0f, 0.75f);
            }

            m_ThrowInputHeld = true;
        }
        else {
            if (m_ThrowInputHeld == true) {
                m_ThrowInput = m_ThrowPowerImage.fillAmount;
            }

            m_ThrowInputHeld = false;
        }
    }

    private void PPhysicsInput() {

        m_MoveInput = Vector2.ClampMagnitude(
            new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            ), 
            1.0f
        );

        if (Input.GetAxis("Jump") > 0.2f) {
            m_JumpInput = !m_LockJumpInput;

            m_LockJumpInput = true;
        }
        else {
            m_LockJumpInput = false;
            m_JumpInput = false;
        }
    }

    private void PModel() {
        if (m_Rigidbody.velocity != ZERO3) {
            m_Model.transform.forward = Vector3.Slerp(m_Model.transform.forward, m_Rigidbody.velocity, Time.deltaTime * 5.0f);
        }
    }

    private void PCrosshair() {

        Vector2  bikePos = ((m_Camera.WorldToScreenPoint(transform.position) / new Vector2(Screen.width, Screen.height)) - new Vector2(0.5f, 0.5f)) * 2f;
        
        Vector2 controllerInput = new Vector2(Input.GetAxis("Controller Look X"), -Input.GetAxis("Controller Look Y"));

        float angle;

        if (controllerInput != ZERO2) {
            angle = Vector2.SignedAngle(new Vector2(0f, 1f), controllerInput);
        }
        else {
            Vector2 mousePos = ((Input.mousePosition / new Vector2(Screen.width, Screen.height)) - new Vector2(0.5f, 0.5f)) * 2f;

            angle = Vector2.SignedAngle(new Vector2(0f, 1f), mousePos - bikePos);
        }

        m_Crosshair.transform.eulerAngles = new Vector3(0, 45.0f - angle, 0);
    }
}

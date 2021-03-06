using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Cinemachine;

public class Player : MonoBehaviour {

    /* SINGLETON */
    public static Player Instance;

    /* REFERENCES */
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private CinemachineVirtualCamera m_CinemachineVirtualCamera;

    [Header("Bike")]
    [SerializeField] private Transform m_Model;

    [Header("Tank")]
    [SerializeField] private Transform m_Turret;

    [Header("Throwing")]
    [SerializeField] private Transform m_Crosshair;

    [Header("Sound")]
    public AudioClip[] m_JusticeSounds;
    public AudioClip   m_ThrowSound;

    [Header("UI")]
    [SerializeField] private GameObject m_ShopMenu;
    [SerializeField] private Image     m_ThrowPowerImage;
    [SerializeField] private Gradient  m_ThrowFillGradient;
    [SerializeField] private Animation m_JusticePopupImage;
    [SerializeField] private PauseMenu m_PauseMenu;
    [SerializeField] private Text      m_CurrencyText;

    /* PRIVATE */
    private Vector3 m_Motion;
    private Vector2 m_MoveInput;

    private float m_ThrowPowerDir = 1.0f;

    private float m_ThrowInput     = -1.0f;
    private bool  m_ThrowInputHeld = false;

    private bool m_JusticePopupTrigger = false;
    private bool m_LockJumpInput       = false;
    private bool m_JumpInput           = false;

    private int m_CurrentPoolItem = 0;

    private List<Rigidbody> m_ThrowPool;

    private readonly Vector3 ZERO3 = new Vector3();
    private readonly Vector2 ZERO2 = new Vector2();

    /* PROPERTIES */

    private int m_Money = 0;
    public int Money {
        get {
            return m_Money;    
        }
        set {
            if (value < 0) { value = 0; }

            m_Money = value;

            m_CurrencyText.text = m_Money.ToString().PadLeft(4, '0');
        }
    }

    private void Awake() {

        Instance = this;

        m_Camera.layerCullSpherical = true;

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

            float shakeFrequency;
            float shakeAmount;
            float shakeDuration;

            if (m_JusticePopupTrigger = m_ThrowInput >= 0.8f) {
                ShockWave.Get(m_Camera).StartIt(transform.position + (transform.rotation * PlayerPreferences.Instance.m_ThrowOffset), 8.0f, 0.05f, 0.03f, 0.1f);

                shakeFrequency = 0.08f;
                shakeAmount    = 20.0f;
                shakeDuration  = 1.00f;

                AudioSource.PlayClipAtPoint(m_JusticeSounds[Random.Range(0, m_JusticeSounds.Length)], m_Camera.transform.position, 0.3f);
            }
            else { 
                shakeFrequency = Mathf.Lerp(0.04f, 0.08f, m_ThrowInput);
                shakeAmount    = Mathf.Lerp(0.00f, 15.0f, m_ThrowInput);
                shakeDuration  = Mathf.Lerp(0.50f, 0.80f, m_ThrowInput);
            }
            
            PThrow(m_ThrowInput);

            StartCoroutine(PCameraShake(shakeFrequency, shakeAmount, shakeDuration));

            m_ThrowInput = -1.0f;
        }

        if (!m_PauseMenu.m_Paused) {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, Time.unscaledDeltaTime * 8.0f);
        }
    }

    private void LateUpdate() {
        if (!m_PauseMenu.m_Paused) {
            PModel();
            PCrosshair();
            PUI();
        }
    }

    private void FixedUpdate() {
        PPhysicsInput();
        PMove();
    }

    private void PUI() {

        m_ThrowPowerImage.gameObject.SetActive(m_ThrowInputHeld);

        if (m_ThrowPowerImage.gameObject.activeSelf) {

            float dir = m_Crosshair.eulerAngles.y <= 45.0f || m_Crosshair.eulerAngles.y > 225.0f ? 1.0f : -1.0f;

            m_ThrowPowerImage.rectTransform.position = Vector3.Lerp(
                m_ThrowPowerImage.rectTransform.position,
                m_Camera.WorldToScreenPoint(transform.position) + new Vector3(90 * dir, 0, 0),
                Time.unscaledDeltaTime * 8.0f
            );

            m_ThrowPowerImage.fillAmount += ((m_ThrowPowerImage.fillAmount * Time.unscaledDeltaTime * 4.0f) + Time.unscaledDeltaTime) * m_ThrowPowerDir;

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

        AudioSource.PlayClipAtPoint(m_ThrowSound, throwObject.transform.position, 1.0f);
    }

    private void PInput() {

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_ShopMenu.SetActive(true);
            m_PauseMenu.TogglePause(!m_PauseMenu.m_Paused);
        }
        else {
            if (Input.GetKeyDown(KeyCode.Escape)) { m_PauseMenu.TogglePause(!m_PauseMenu.m_Paused); }
        }

        if (!m_PauseMenu.m_Paused) { 
        
            if (Input.GetAxis("Fire1") > 0.2f) {
                if (!m_ThrowInputHeld) {
                    m_ThrowPowerImage.fillAmount = Random.Range(0.0f, 0.6f);
                }

                m_ThrowInputHeld = true;

                Time.timeScale = 0.2f;
            }
            else {
                if (m_ThrowInputHeld == true) {
                    m_ThrowInput = m_ThrowPowerImage.fillAmount;
                }

                m_ThrowInputHeld = false;
            }
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

        m_Crosshair.rotation = Quaternion.Slerp(
            m_Crosshair.rotation, 
            Quaternion.Euler(new Vector3(0, 45.0f - angle, 0)), 
            Time.unscaledDeltaTime * 25.0f
        );

        m_Turret.rotation = m_Crosshair.rotation;
    }

    private IEnumerator PCameraShake(float _frequency, float _amplitude, float _duration) {

        var noise = m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        float initialDuration = _duration;

        while (_duration > 0.0f) {

            float progress = 1.0f - (_duration / initialDuration);

            noise.m_AmplitudeGain = Mathf.SmoothStep(_amplitude, 0.0f, progress); 
            noise.m_FrequencyGain = Mathf.SmoothStep(_frequency, 0.0f, progress);

            _duration -= Time.deltaTime;

            yield return null;
        }
    }
}

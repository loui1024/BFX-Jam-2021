using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    /* REFERENCES */
    public static GameManager Instance;

    /* PUBLIC */
    public float m_TimeLeft                         { get; private set; }
    public List<DeliveryPoint> m_DeliveryPointsLeft { get; private set; }

    /* PRIVATE */
    private List<GameObject> m_UIDeliveryPointsIndicators;
    
    [SerializeField] private Image m_TimerHandImage;
    [SerializeField] private Text  m_DeliveriesRemainingText;
    [SerializeField] private RectTransform m_Minimap;
    [SerializeField] private GameObject m_MinimapDeliveryPointPrefab;
    [SerializeField] private float m_MinimapItemScaleAdjustment;
    [SerializeField] private Vector3 m_MinimapItemOffset;

    public enum GameCompletionState { 
        Victory,
        Failure
    }

    private void Awake() {

        Instance = this;

        m_TimeLeft = GamePreferences.Instance.m_InitialTime;

        m_DeliveryPointsLeft = new List<DeliveryPoint>(GameObject.FindObjectsOfType<DeliveryPoint>());
        m_UIDeliveryPointsIndicators = new List<GameObject>();

        foreach (var item in m_DeliveryPointsLeft) {

            var indicator = GameObject.Instantiate(m_MinimapDeliveryPointPrefab, m_Minimap);

            indicator.transform.localPosition = (new Vector3(item.transform.position.x, item.transform.position.z, 0) + m_MinimapItemOffset) * m_MinimapItemScaleAdjustment;

            m_UIDeliveryPointsIndicators.Add(indicator);
        }

        DeliveryPoint.ItemDelivered += OnItemDelivered;
    }

    private void Update() {

        UpdateTimerUI();

        if (m_TimeLeft <= 0.0f) {
            GameComplete(GameCompletionState.Failure);
        }
    }

    private void LateUpdate() {
        m_TimeLeft -= Time.deltaTime;
    }

    private void OnItemDelivered(DeliveryPoint.ItemDeliveredArgs _args) {

        Player.Instance.Money += GamePreferences.Instance.m_ItemDeliveredReward;

        m_TimeLeft += GamePreferences.Instance.m_TimePerItemDelivered;

        if (m_TimeLeft > GamePreferences.Instance.m_MaxTime) {
            m_TimeLeft = GamePreferences.Instance.m_MaxTime;
        }

        int index = m_DeliveryPointsLeft.IndexOf(_args.m_Instigator);

        m_DeliveryPointsLeft.RemoveAt(index);
        Destroy(m_UIDeliveryPointsIndicators[index]);
        m_UIDeliveryPointsIndicators.RemoveAt(index);

        if (m_DeliveryPointsLeft.Count == 0) {
            GameComplete(GameCompletionState.Victory);
        }

        UpdateDeliveriesUI();
    }

    private void UpdateDeliveriesUI() {
        m_DeliveriesRemainingText.text = m_DeliveryPointsLeft.Count.ToString();


    }

    private void UpdateTimerUI() {

        m_TimerHandImage.rectTransform.eulerAngles = new Vector3(
            0,
            0,
            Mathf.Lerp(-180.0f, 0.0f, (m_TimeLeft / GamePreferences.Instance.m_MaxTime))
        );
    }

    public void GameComplete(GameCompletionState _completionState) {
        enabled = false;

        switch (_completionState) {
            case GameCompletionState.Victory: {
                Debug.Log("Victory!");

                break;
            }
            case GameCompletionState.Failure: {
                Debug.Log("Failure!");

                break;
            }
            default: {
                throw new System.NotImplementedException("ERROR (GameManager[GameComplete(GameCompletionState)]): " + _completionState.ToString() + " has not yet been implemented!");
            }
        }
    }
}

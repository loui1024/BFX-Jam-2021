using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePreferences", menuName = "New GamePreferences")]
public class GamePreferences : ScriptableObject {

    /* SINGLETON */

    private static GamePreferences m_Instance;
    public static GamePreferences Instance {
        get {
            if (!m_Instance) {
                m_Instance = Resources.Load("GamePreferences") as GamePreferences;
            }

            return m_Instance;
        }
    }

    /* PUBLIC */

    [Header("Time")]
    public float m_InitialTime;
    public float m_MaxTime;
    public float m_TimePerItemDelivered;

    [Header("Money")]
    public int m_ItemDeliveredReward;
    public int m_CriminalKillReward;
}

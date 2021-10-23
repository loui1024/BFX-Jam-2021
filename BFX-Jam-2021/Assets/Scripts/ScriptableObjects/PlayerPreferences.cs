using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPreferences", menuName = "New PlayerPreferences")]
public class PlayerPreferences : ScriptableObject {

    /* SINGLETON */
    private static PlayerPreferences m_Instance;
    public static PlayerPreferences Instance {
        get {
            if (!m_Instance) {
                m_Instance = Resources.Load("PlayerPreferences") as PlayerPreferences;
            }

            return m_Instance;
        }
    }

    /* PUBLIC */

    [Header("Movement")]
    public float m_Speed;
    public float m_Acceleration;

    [Header("Throwing")]
    public float m_ThrowPower;
}

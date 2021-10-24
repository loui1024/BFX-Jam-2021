using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[RequireComponent(typeof(Rigidbody))]
public class Trolley : MonoBehaviour {
    
    /* REFERENCES */
    public Rigidbody   m_Rigidbody;
    public Transform[] m_Waypoints;

    /* PUBLIC */
    [Space]
    public bool isCriminal = false;

    [Header("Stats")]
    public float m_Speed = 2.0f;
    public float m_Smoothness = 5.0f;
    public int   m_CurrentWaypoint = 0;

    public MovementType m_MovementType;

    public enum MovementType {
        Once,
        PingPong,
        Continuous
    }

    /* PRIVATE */
    private int m_TargetWaypoint = 1;

    private void Start() {
        Init();
    }

    private void Init() {

        if (m_Rigidbody == null) {
            m_Rigidbody = GetComponent<Rigidbody>();

            if (m_Rigidbody == null) {
                Debug.LogError("Platform does not have a Rigidbody assigned!");
            }
        }

        m_Rigidbody.position = m_Waypoints[m_CurrentWaypoint].position;
    }

    private void FixedUpdate() {

        if (m_Waypoints != null && m_Waypoints.Length != 0) {

            if (m_Speed != 0) {

                int direction = (m_Speed > 0) ? 1 : -1;

                switch (m_MovementType) {
                    case MovementType.Once: {
                        m_TargetWaypoint = Mathf.Clamp(m_CurrentWaypoint + direction, 0, m_Waypoints.Length - 1);
                        break;
                    }
                    case MovementType.PingPong: {
                        m_TargetWaypoint = (int)Mathf.PingPong(m_CurrentWaypoint + direction, m_Waypoints.Length - 1);
                        break;
                    }
                    case MovementType.Continuous: {
                        m_TargetWaypoint = (int)Mathf.Repeat(m_CurrentWaypoint + direction, m_Waypoints.Length);
                        break;
                    }
                }

                var target = m_Waypoints[m_TargetWaypoint].position;

                if ((m_Rigidbody.position - target).sqrMagnitude < 0.1f) {
                    if (m_MovementType == MovementType.PingPong) {
                        m_CurrentWaypoint++;
                    }
                    else {
                        m_CurrentWaypoint = m_TargetWaypoint;
                    }
                }

                m_Rigidbody.MovePosition(Vector3.MoveTowards(m_Rigidbody.position, target, Mathf.Abs(Time.fixedDeltaTime * m_Speed)));
            }
        }

        m_Rigidbody.rotation = Quaternion.Slerp(
            m_Rigidbody.rotation,
            Quaternion.LookRotation(transform.position - m_Waypoints[m_TargetWaypoint].position),
            Time.deltaTime * m_Smoothness
        );
    }

    private void OnCollisionEnter(Collision collision) {

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;

        if (isCriminal) {
            Player.Instance.Money += GamePreferences.Instance.m_CriminalKillReward;
        }

        Destroy(this);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos() {

        var defaultColor = Gizmos.color;

        if (m_Waypoints != null && m_Waypoints.Length != 0) {

            const float handleSize = 0.5f;

            for (int i = 0; i < m_Waypoints.Length - 1; ++i) {

                var A = m_Waypoints[i].position;
                var B = m_Waypoints[i + 1].position;

                Gizmos.color = Color.white;
                Gizmos.DrawLine(A, B);

                if (m_MovementType == MovementType.PingPong) {
                    var adj = Vector3.Cross(A, B).normalized;

                    Handles.color = Color.blue;
                    Handles.ConeHandleCap(0, B - adj * handleSize, Quaternion.LookRotation(A - B), handleSize, EventType.Repaint);

                    A += adj * handleSize;
                    B += adj * handleSize;
                }

                Handles.color = Color.red;
                Handles.ConeHandleCap(0, A, Quaternion.LookRotation(B - A), handleSize, EventType.Repaint);
            }

            var lastPoint = m_Waypoints[m_Waypoints.Length - 1].position;

            if (m_MovementType == MovementType.Once) {

                Handles.color = Color.blue;

                Handles.SphereHandleCap(0, lastPoint, new Quaternion(0, 0, 0, 1), handleSize, EventType.Repaint);
            }
            else {
                var firstPoint = m_Waypoints[0].position;

                Handles.ConeHandleCap(0, lastPoint, Quaternion.LookRotation(firstPoint - lastPoint), handleSize, EventType.Repaint);

                if (m_MovementType == MovementType.Continuous) {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(lastPoint, m_Waypoints[0].position);
                }
            }

            Gizmos.color = defaultColor;
        }
    }


#endif

}

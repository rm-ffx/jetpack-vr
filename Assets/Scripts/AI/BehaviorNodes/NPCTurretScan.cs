using System.Collections.Generic;
using UnityEngine;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// NPC Turret scans an area for the target
/// </summary>
public class NPCTurretScan : Action
{
    public SharedObject npcInfo; // NPC Info that stores all information of the NPC
    private NPCInfo m_info;

    public float scanSpeed; // Speed with which the Turrent will scan the area
    public float scanAngle; // Maximum Scan-Angle of the Turrent
    public float waitAtMaxAngle; // Time the Turrent should wait when it reaches on max-angle

    private Quaternion startRotation; // Rotation when the game starts.

    private Vector3 minDirection;
    private Vector3 maxDirection;
    private Vector3 defaultDirection;

    private int currentDir = 1; // This is the switch between the two directions
    private bool m_init = true;

    public override void OnAwake()
    {
        m_info = npcInfo.GetValue() as NPCInfo;
        startRotation = m_info.turretHead.transform.rotation;

        m_info.turretHead.transform.RotateAround(m_info.turretHead.transform.position, Vector3.up, scanAngle);
        maxDirection = m_info.turretHead.transform.forward;

        m_info.turretHead.transform.rotation = startRotation;
        m_info.turretHead.transform.RotateAround(m_info.turretHead.transform.position, Vector3.up, -scanAngle);
        minDirection = m_info.turretHead.transform.forward;

        m_info.turretHead.transform.rotation = startRotation;
        defaultDirection = m_info.turretHead.transform.forward;
    }

    public override void OnStart()
    {
        // Rotate towards maxRotationPoint on start
        currentDir = 1;
        m_init = true;
    }

    public override TaskStatus OnUpdate()
    {
        // Make sure that the Turret is rotated properly on startup
        if (m_init)
        {
            if (Vector3.Angle(m_info.turretHead.transform.forward, defaultDirection) > 10)
            {
                // Rotate towards defaultAngle
                Quaternion currentRotation = m_info.turretHead.transform.rotation;
                m_info.turretHead.transform.LookAt(m_info.turretHead.transform.position + defaultDirection);
                Quaternion newRotation = m_info.turretHead.transform.rotation;
                m_info.turretHead.transform.rotation = Quaternion.Lerp(currentRotation, newRotation, scanSpeed * Time.deltaTime);
                return TaskStatus.Running;
            }
            else m_init = false;
        }

        if (Vector3.Angle(m_info.turretHead.transform.forward, maxDirection) > 10 && currentDir == 1)
        {
            // Rotate towards maxAngle
            Quaternion currentRotation = m_info.turretHead.transform.rotation;
            m_info.turretHead.transform.LookAt(m_info.turretHead.transform.position + maxDirection);
            Quaternion newRotation = m_info.turretHead.transform.rotation;
            m_info.turretHead.transform.rotation = Quaternion.Lerp(currentRotation, newRotation, scanSpeed * Time.deltaTime);
        }
        else
        {
            // Make sure this direction keeps getting executed
            currentDir = -1;

            // Rotate towards minAngle
            Quaternion currentRotation = m_info.turretHead.transform.rotation;
            m_info.turretHead.transform.LookAt(m_info.turretHead.transform.position + minDirection);
            Quaternion newRotation = m_info.turretHead.transform.rotation;
            m_info.turretHead.transform.rotation = Quaternion.Lerp(currentRotation, newRotation, scanSpeed * Time.deltaTime);

            if (Vector3.Angle(m_info.turretHead.transform.forward, minDirection) < 10) currentDir = 1;
        }

        // Pause


        return TaskStatus.Running;
    }
}

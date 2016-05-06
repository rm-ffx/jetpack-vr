using UnityEngine;
using Pathfinding.RVO;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// Makes the NPC Attack the Target
/// </summary>
public class NPCAttack : Action
{
    public SharedObject npcInfo;
    private NPCInfo m_info;

    public SharedObject rvoController;
    public SharedTransform target; // CurrentTarget
    public float rotationSpeed;

    private Transform m_currentTarget;
    public int minAttacks; // Minimum Number of Attacks
    public int maxAttacks; // Maximum Number of Attacks

    private int m_currentAttacks;
    private int m_currentMaxAttacks;

    public int aimTime;
    private int m_currentAimTime;

    public override void OnAwake()
    {
        m_info = npcInfo.GetValue() as NPCInfo;
    }

    public override void OnStart()
    {
        // Stop Movement
        (rvoController.GetValue() as RVOController).Move(Vector3.zero);

        // Set Variables
        m_currentAttacks = 0;
        m_currentMaxAttacks = Random.Range(minAttacks, maxAttacks + 1);
        m_currentAimTime = 0;
        m_currentTarget = target.GetValue() as Transform;
    }

    public override TaskStatus OnUpdate()
    {
        // SET ROTATION
        Quaternion currentRotation = transform.rotation;
        transform.LookAt(new Vector3(m_currentTarget.position.x, transform.position.y, m_currentTarget.position.z));
        Quaternion newRotation = transform.rotation;
        transform.rotation = Quaternion.Slerp(currentRotation, newRotation, rotationSpeed * Time.deltaTime);

        // Wait for AimTime to pass
        if (m_currentAimTime < aimTime) { m_currentAimTime++; return TaskStatus.Running; }
        else
        {
            // Perform Attack
            //Debug.DrawLine(transform.position, m_currentTarget.position, Color.red, 0.25f);
            for (int i = 0; i < m_info.gunPos.Length; i++)
            {
                GameObject.Instantiate(Resources.Load(m_info.projectile.name), m_info.gunPos[i].position, Quaternion.LookRotation(m_currentTarget.position - m_info.gunPos[i].position, Vector3.up));
            }
            m_currentAimTime = 0;
            m_currentAttacks++;

            // Check if Another Attack should be performed, or if success should be returned
            if (m_currentAttacks >= m_currentMaxAttacks || !withinSightSpherical(m_currentTarget)) return TaskStatus.Success;
            else return TaskStatus.Running;
        }
    }

    /// <summary>
    /// Returns true if targetTransform is within sight of current transform
    /// </summary>
    public bool withinSightSpherical(Transform targetTransform)
    {
        // Check if the object is obscured by something or visible
        RaycastHit hit;
        if (Physics.Raycast(m_info.eyePos.position, (targetTransform.position - m_info.eyePos.position)*1.25f, out hit) && hit.transform == targetTransform) return true;
        else return false;
    }
}

using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

public class NPCInfo : MonoBehaviour
{
    [Tooltip("Health of the NPC.")]
    public float health;

    [Tooltip("Field of View of this NPC.")]
    public float fov;

    [Tooltip("How far can this NPC see.")]
    public float viewDistance;

    public Transform eyePos;

    // Combat Variables
    public Transform[] gunPos;
    public GameObject projectile;

    [Tooltip("Head of the Turret. This transform will be rotated. Leave empty if NPC is not a turret.")]
    public Transform turretHead;

    public enum npcState
    {
        Idle = 0,
        Patrol = 1,
        Search = 2,
        Attack = 3
    }
    [Tooltip("State the NPC is currently in.")]
    public npcState currentState;

    public PuppetMaster puppetMaster { get; private set;}

    void Awake()
    {
        // Cache Variables
        puppetMaster = GetComponentInChildren<PuppetMaster>();
    }

    /// <summary>
    /// Debug Gizmos
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (eyePos != null)
        {
            Transform usedTransform;
            if (turretHead != null) usedTransform = turretHead;
            else usedTransform = transform;

            // Draw FieldOfView-Frustrum
            Matrix4x4 temp = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(eyePos.position, usedTransform.rotation, Vector3.one);

            if (currentState == npcState.Idle || currentState == npcState.Patrol) Gizmos.color = Color.green;
            else if (currentState == npcState.Attack) Gizmos.color = Color.red;
            else if (currentState == npcState.Search) Gizmos.color = Color.yellow;

            Gizmos.DrawFrustum(Vector3.zero, fov, viewDistance, 0, 1);
            Gizmos.matrix = temp;
        }
    }
}

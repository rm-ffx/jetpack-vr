using UnityEngine;
using System.Collections.Generic;
using RootMotion.Dynamics;

public class NPCInfo : MonoBehaviour
{
    [Tooltip("Health of the NPC.")]
    public float health;

    [Tooltip("Field of View of this NPC.")]
    public float fov;

    [Tooltip("How far can this NPC see.")]
    public float viewDistance;

    [Tooltip("RadarIconPrefab which can be choosed from")]
    public GameObject radarPrefab;

    [Tooltip("The currentIcon that will be displayed on the radar")]
    public Texture currentIconTexture;

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

    [Tooltip("Whether or not the death of this NPC should trigger a script.")]
    public bool triggerScriptOnDeath = false;
    [Tooltip("The triggered script. Will only be used if Trigger Script On Death is true.")]
    public TriggerScript triggerScript;

    public PuppetMaster puppetMaster { get; private set;} // Controls the Ragdoll

    [HideInInspector]
    public List<Transform> patrolWaypoints; // Waypoints this NPC will patrol between
    void Awake()
    {
        // Cache Variables
        puppetMaster = GetComponentInChildren<PuppetMaster>();
        if (currentIconTexture != null)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.mainTexture = currentIconTexture;
        }
    }

    /// <summary>
    /// Triggers the Ragdoll behaviour of the NPC
    /// </summary>
    /// <param name="col">Collider that was hit</param>
    public void TriggerPuppetMaster(Collider col, float impactForce, Vector3 impactPosition, float unpin)
    {
        // Check for Muscles
        MuscleCollisionBroadcaster broadcaster = col.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

        // Apply HIT-Force to the given Collider
        if (broadcaster)
        {
            // NPC Drops Dead
            puppetMaster.mode = PuppetMaster.Mode.Active;
            puppetMaster.state = PuppetMaster.State.Dead;
            puppetMaster.pinWeight = 0;

            // Process with Force
            broadcaster.Hit(unpin, (col.gameObject.transform.position - impactPosition) * impactForce, impactPosition);
        }
        
        // Disable Behavior and Controller
        GetComponent<BehaviorDesigner.Runtime.Behavior>().enabled = false;
        GetComponent<Pathfinding.RVO.RVOController>().enabled = false;
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

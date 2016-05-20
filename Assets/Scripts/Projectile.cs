using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 100;
    public float speed = 200;
    public float damage = 50;

    public float force = 10f; // Impact force of this Projectile on Ragdolls
    public float unpin = 10f; // Unpin-Force of this Projectile on Ragdolls

    private float m_currentLifeTime;

    void Update()
    {
        // Destroy if too old
        if (m_currentLifeTime >= lifeTime)
            Destroy(gameObject);
        m_currentLifeTime += Time.deltaTime;

        // Move Forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 9)
        {
            NPCInfo npcInfo = collider.transform.root.gameObject.GetComponent<NPCInfo>();
            npcInfo.health -= damage;
            if (npcInfo.health <= 0.0f)
            {
                if (npcInfo.puppetMaster) TriggerPuppetMaster(collider, npcInfo);
                npcInfo.gameObject.GetComponent<BehaviorDesigner.Runtime.Behavior>().enabled = false;
                npcInfo.gameObject.GetComponent<Pathfinding.RVO.RVOController>().enabled = false;

                //Destroy(collider.gameObject);
            }
        }
        else if(collider.gameObject.layer == 11)
        {
            PlayerInfo playerInfo = collider.gameObject.GetComponent<PlayerInfo>();
            playerInfo.health -= damage;
            if (playerInfo.health <= 0.0f)
                Debug.Log("Player died. Insert gamestate change here");
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Triggers the Ragdoll behaviour of the NPC
    /// </summary>
    /// <param name="col">Collider that was hit</param>
    void TriggerPuppetMaster(Collider col, NPCInfo info)
    {
        // Check for Muscles
        MuscleCollisionBroadcaster broadcaster = col.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

        // Apply HIT-Force to the given Collider
        if (broadcaster)
        {
            // NPC Drops Dead
            info.puppetMaster.mode = PuppetMaster.Mode.Active;
            info.puppetMaster.state = PuppetMaster.State.Dead;
            info.puppetMaster.pinWeight = 0;

            // Process with With Force
            broadcaster.Hit(unpin, (col.gameObject.transform.position - gameObject.transform.position) * force, transform.position);
        }
    }
}

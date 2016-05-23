using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

public class Projectile : MonoBehaviour
{
    [Tooltip("How long the projectile will fly before getting destroyed.")]
    public float lifeTime = 100;
    [Tooltip("How fast the projectile travels.")]
    public float speed = 200;
    [Tooltip("How much damage the projectile deals.")]
    public float damage = 50;
    [Tooltip("The impact force of this projectile on Ragdolls.")]
    public float force = 10f; // Impact force of this Projectile on Ragdolls

    //public LayerMask collisionTargets; // Collides with Objects in those layers

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
        //if (collisionTargets.value == collider.gameObject.layer)
        //{
            if (collider.gameObject.layer == 9)
            {
                // Collides with NPC
                NPCInfo npcInfo = collider.transform.root.gameObject.GetComponent<NPCInfo>();
                npcInfo.health -= damage;
                if (npcInfo.health <= 0.0f)
                {
                    // If the NPC has a Ragdoll - Activate Ragdoll. Else Destroy GameObject
                    if (npcInfo.puppetMaster) npcInfo.TriggerPuppetMaster(collider, force, transform.position, 0);
                    else Destroy(collider.gameObject);
                }
            }
            else if (collider.gameObject.layer == 11)
            {
                // Collides with Player
                Debug.Log("HIT PLAYER!");
                PlayerInfo playerInfo = collider.gameObject.transform.root.GetComponent<PlayerInfo>();
                playerInfo.health -= damage;
                if (playerInfo.health <= 0.0f)
                    Debug.Log("Player died. Insert gamestate change here");
            }
            else if (collider.gameObject.layer == 14)
            {
                Shield shield = collider.transform.parent.GetComponent<Shield>();
                if (shield.looseEnergyOnHit)
                    shield.GetHit(damage);
            }
            Destroy(gameObject);
        //}
    }
}

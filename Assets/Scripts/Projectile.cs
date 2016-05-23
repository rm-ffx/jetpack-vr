using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 100;
    public float speed = 200;
    public float damage = 50;

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
                Debug.Log("projectile hit shield");
                Shield shield = collider.transform.parent.GetComponent<Shield>();
                if (shield.looseEnergyOnHit)
                    shield.GetHit(damage);
            }
            Destroy(gameObject);
        //}
    }
}

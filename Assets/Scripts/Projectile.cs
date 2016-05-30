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

    private float m_currentLifeTime;
    private Vector3 m_lastPosition;
    private Vector3 m_direction;
    private RaycastHit m_hit;
    private LayerMask m_layerMask;

    void Start()
    {
        m_currentLifeTime = 0.0f;
        m_lastPosition = transform.position;
        m_direction = Vector3.zero;
        m_layerMask = 1 << 2 | 1 << 10 | 1 << gameObject.layer; // Ignore "Ignore Raycast", Controller and own Layer
        m_layerMask = ~m_layerMask;
    }

    void Update()
    {
        // Destroy if too old
        if (m_currentLifeTime >= lifeTime)
            Destroy(gameObject);
        m_currentLifeTime += Time.deltaTime;

        // Move Forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Manual Collision Detection
        m_direction = transform.position - m_lastPosition;
        if (Physics.Raycast(m_lastPosition, m_direction, out m_hit, layerMask: m_layerMask, maxDistance: m_direction.magnitude))
        {
            OnHit(m_hit);
        }
        m_lastPosition = transform.position;
    }

    //void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.gameObject.layer == 9)
    //    {
    //        // Collides with NPC
    //        NPCInfo npcInfo = collider.transform.root.gameObject.GetComponent<NPCInfo>();
    //        npcInfo.health -= damage;
    //        if (npcInfo.health <= 0.0f)
    //        {
    //            // If the NPC has a Ragdoll - Activate Ragdoll. Else Destroy GameObject
    //            if (npcInfo.puppetMaster) npcInfo.TriggerPuppetMaster(collider, force, transform.position, 0);
    //            else Destroy(collider.gameObject);
    //        }
    //    }
    //    else if (collider.gameObject.layer == 11)
    //    {
    //        // Collides with Player
    //        PlayerInfo playerInfo = collider.gameObject.transform.root.GetComponent<PlayerInfo>();
    //        playerInfo.health -= damage;
    //        if (playerInfo.health <= 0.0f)
    //            UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
    //    }
    //    else if (collider.gameObject.layer == 14)
    //    {
    //        Shield shield = collider.transform.parent.GetComponent<Shield>();
    //        if (shield.looseEnergyOnHit)
    //            shield.GetHit(damage);
    //    }
    //    Destroy(gameObject);
    //}

    void OnHit(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer == 9)
        {
            // Collides with NPC
            NPCInfo npcInfo = hit.collider.transform.root.gameObject.GetComponent<NPCInfo>();
            npcInfo.health -= damage;
            if (npcInfo.health <= 0.0f)
            {
                if (npcInfo.triggerScriptOnDeath)
                    if (npcInfo.triggerScript != null)
                        npcInfo.triggerScript.Activate();

                // If the NPC has a Ragdoll - Activate Ragdoll. Else Destroy GameObject
                if (npcInfo.puppetMaster)
                    npcInfo.TriggerPuppetMaster(hit.collider, force, transform.position, 0);
                else
                    Destroy(hit.collider.gameObject);
            }
        }
        else if (hit.collider.gameObject.layer == 11)
        {
            // Collides with Player
            PlayerInfo playerInfo = hit.collider.gameObject.transform.root.GetComponent<PlayerInfo>();
            playerInfo.health -= damage;
            if (playerInfo.health <= 0.0f)
                UnityEngine.SceneManagement.SceneManager.LoadScene(GameInfo.mainMenuIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else if (hit.collider.gameObject.layer == 14)
        {
            Shield shield = hit.collider.transform.parent.GetComponent<Shield>();
            if (shield.looseEnergyOnHit)
                shield.GetHit(damage);
        }
        Destroy(gameObject);
    }
}

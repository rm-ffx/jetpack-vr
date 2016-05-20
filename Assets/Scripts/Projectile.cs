using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 100;
    public float speed = 200;
    public float damage = 50;

    private float m_currentLifeTime;
    //private bool m_isPlayerProjectile;

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
            NPCInfo npcInfo = collider.gameObject.GetComponent<NPCInfo>();
            npcInfo.health -= damage;
            if (npcInfo.health <= 0.0f)
            {
                Destroy(collider.gameObject);
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
}

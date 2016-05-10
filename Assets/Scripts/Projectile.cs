using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float LifeTime = 10;
    public float Speed = 20;
    public float Damage = 50;

    private float m_currentLifeTime;
    //private bool m_isPlayerProjectile;

    void Update()
    {
        // Destroy if too old
        if (m_currentLifeTime >= LifeTime)
            Destroy(gameObject);
        m_currentLifeTime += Time.deltaTime;

        // Move Forward
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 9)
        {
            NPCInfo npcInfo = collider.gameObject.GetComponent<NPCInfo>();
            npcInfo.health -= Damage;
            if (npcInfo.health <= 0.0f)
            {
                Destroy(collider.gameObject);
            }
        }
        else if(collider.gameObject.layer == 11)
        {
            PlayerInfo playerInfo = collider.gameObject.GetComponent<PlayerInfo>();
            playerInfo.health -= Damage;
            if (playerInfo.health <= 0.0f)
                Debug.Log("Player died. Insert gamestate change here");
        }

        Destroy(gameObject);
    }
}

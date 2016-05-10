﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float LifeTime = 10;
    public float Speed = 20;
    public float Damage = 50;
    private float m_currentLifeTime;

    void Update()
    {
        // Destroy if too old
        if (m_currentLifeTime >= LifeTime)
            Destroy(gameObject);
        m_currentLifeTime += Time.deltaTime;

        // Move Forward
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9)
        {
            NPCInfo npcInfo = collision.gameObject.GetComponent<NPCInfo>();
            npcInfo.health -= Damage;
            if (npcInfo.health <= 0.0f)
                Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}

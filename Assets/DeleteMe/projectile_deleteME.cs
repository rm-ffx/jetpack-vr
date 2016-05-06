using UnityEngine;
using System.Collections;

public class projectile_deleteME : MonoBehaviour
{
    public float lifeTime;
    public float speed;
    private float m_currentLifeTime;

    void Update()
    {
        // Destroy if too old
        if (m_currentLifeTime >= lifeTime) Destroy(this.gameObject);
        m_currentLifeTime++;

        // Move Forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

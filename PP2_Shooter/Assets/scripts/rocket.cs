using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour, IDamagable
{
    [SerializeField] int speed;
    [SerializeField] int hp;
    [SerializeField] Rigidbody rb;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject explosion;



    void Start()
    {
        rb.velocity = (gamemanager.instance.player.transform.position - transform.position) + new Vector3(0, 0.5f, 0) * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {

        Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }
    public void takeDamage(int dmg)
    {
        hp -= dmg;


        if (hp <= 0)
        {
            foreach (Collider col in GetComponents<Collider>())
                col.enabled = false;
            Instantiate(explosion, transform.position, explosion.transform.rotation);
            rb.velocity = (gamemanager.instance.player.transform.position - transform.position) + new Vector3(0, 0.5f, 0) * speed;
            Destroy(gameObject);
        }
    }
}

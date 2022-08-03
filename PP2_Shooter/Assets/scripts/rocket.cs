using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocket : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] Rigidbody rb;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject explosion;

    void Start()
    {
        rb.velocity = (gamemanager.instance.player.transform.position - transform.position).normalized * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(explosion, transform.position, explosion.transform.rotation);
            Destroy(gameObject);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int explosionKnockBack;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.GetComponent<IDamagable>() != null)
            {
                if (other.CompareTag("Player"))
                {
                    gamemanager.instance.playerScript.pushback = (gamemanager.instance.player.transform.position - transform.position) * explosionKnockBack;
                    IDamagable isDamagable = other.GetComponent<IDamagable>();
                    isDamagable.takeDamage(damage);
                    //if (!other.CompareTag("player"))
                    //{
                    //    isDamagable = other.GetComponent<IDamagable>();
                    //    isDamagable.takeDamage(damage / 2);
                    //}
                }
               

            }
        }

    }
}

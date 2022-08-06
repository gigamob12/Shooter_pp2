using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI2 : MonoBehaviour, IDamagable
{
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;

    [Header("Enemy Attributes")]
    [Header("-----------------------------------")]
    [SerializeField] int hp;
    [SerializeField] int viewAngle;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int roamRadius;
    [SerializeField] float grenadeCooldownTime;


    [Header("Enemy Weapon Stats")]
    [Header("-----------------------------------")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shootPosition;
    [SerializeField] float grenadeTossRate;
    [SerializeField] GameObject grenade;

    bool canThrowGrenade;
    bool canShoot;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startingPos;
    float stoppingDisOrig;

    void Start()
    {
        startingPos = transform.position;
        stoppingDisOrig = agent.stoppingDistance;
        //agent.radius = Random.Range(agent.radius, agent.radius + 2);
        //agent.speed = Random.Range(agent.speed, agent.speed + 2);
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.magnitude, Time.deltaTime * 4));
            playerDir = gamemanager.instance.player.transform.position - transform.position;

            if (playerInRange)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                canSeePlayer();
                facePlayer();
            }
            else if (agent.remainingDistance < 0.1f)
            {
                roam();
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }

    void facePlayer()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //.LookAt(gamemanager.instance.player.transform.position);
            playerDir.y = 0;
            var rotation = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);


        }


    }
    void canSeePlayer()
    {
        float angle = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            Debug.DrawRay(transform.position, playerDir);

            if (Time.time > grenadeTossRate)
            {
                if (hit.collider.CompareTag("Player") && canShoot && angle <= viewAngle)
                {
                    StartCoroutine(shootGrenade());
                    grenadeTossRate = Time.time + grenadeCooldownTime;
                }

            }

            if (hit.collider.CompareTag("Player") && canShoot && angle <= viewAngle)
                StartCoroutine(shoot());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            canThrowGrenade = true;
            canShoot = true;
            agent.stoppingDistance = stoppingDisOrig;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            canThrowGrenade = false;
            canShoot = false;
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int dmg)
    {
        hp -= dmg;
        playerInRange = true;

        anim.SetTrigger("Damage");
        StartCoroutine(flashColor());

        if (hp <= 0)
        {
            gamemanager.instance.checkEnemyKills();
            agent.enabled = false;
            anim.SetBool("Dead", true);

            foreach (Collider col in GetComponents<Collider>())
                col.enabled = false;
        }
    }

    IEnumerator flashColor()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        canShoot = false;

        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPosition.transform.position, bullet.transform.rotation);

        yield return new WaitForSeconds(shootRate);

        canShoot = true;
    }

    IEnumerator shootGrenade()
    {
        canThrowGrenade = false;
        anim.SetTrigger("Grenade");
        Instantiate(grenade, shootPosition.transform.position, grenade.transform.rotation);
        yield return new WaitForSeconds(grenadeTossRate);
        canThrowGrenade = true;

    }
}

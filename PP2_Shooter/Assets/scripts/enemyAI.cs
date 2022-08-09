using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamagable
{
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;
    [Header("---------------------------------------------------------------")]
    [Header("Enemy Attributes")]
    [SerializeField] int HP;
    [SerializeField] int viewAngle;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int roamRadius;
    [Header("---------------------------------------------------------------")]
    [Header("Weapon Stats")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shootPos;
    // Grenades being called
    [Header("---------------------------------------------------------------")]
    [Header("Grenades Stats")]
    [SerializeField] bool grenadesActive;
    [SerializeField] float grenadeTossRate;
    [SerializeField] float grenadeFirstToss;
    [SerializeField] float grenadeCooldownTime;
    [SerializeField] GameObject grenade;
    bool canThrowGrenade = true;

    bool canShoot = true;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startignPos;
    float StoppingDisOrig;

    void Start()
    {
        startignPos = transform.position;
        StoppingDisOrig = agent.stoppingDistance;

    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));
            playerDir = gamemanager.instance.player.transform.position - transform.position;

            if (playerInRange)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                canSeePlayer();
                facePlayer();
            }
            else if (agent.remainingDistance < 0.1f)
                roam();
        }
    }

    public void roam()
    {
        agent.stoppingDistance = 0;
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startignPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }

    public void facePlayer()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            playerDir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
        }
    }

    void canSeePlayer()
    {

        float angle = Vector3.Angle(playerDir, transform.forward);
        //Debug.Log(angle);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            Debug.DrawRay(transform.position, gamemanager.instance.player.transform.position - transform.position);

            //timer for the Grenades, this sets a cooldown so he cant infinite throw grenades while shooting
            if (hit.collider.CompareTag("Player") && canShoot && angle <= viewAngle)
                StartCoroutine(shoot());
            if (grenade == true)
            {
                if (Time.time > grenadeTossRate)
                {
                    StartCoroutine(TossFirstGrenade());
                    if (hit.collider.CompareTag("Player") && canShoot && angle <= viewAngle)
                    {
                        StartCoroutine(shootGrenade());
                        grenadeTossRate = Time.time + grenadeCooldownTime;
                    }
                }
            }

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            agent.stoppingDistance = StoppingDisOrig;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        playerInRange = true;
        anim.SetTrigger("Damage");
        //StartCoroutine(flashColor());

        if (HP <= 0)
        {
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
        rend.material.color = Color.yellow;
    }

    IEnumerator shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            anim.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.transform.position, bullet.transform.rotation);
            yield return new WaitForSeconds(shootRate);
            canShoot = true;
        }
    }

    //throwing of the grenades
    IEnumerator shootGrenade()
    {
        if (canThrowGrenade)
        {
            canThrowGrenade = false;
            //anim.SetTrigger("Grenade");
            Instantiate(grenade, shootPos.transform.position, grenade.transform.rotation);
            yield return new WaitForSeconds(grenadeTossRate);
            canThrowGrenade = true;
        }
    }
    IEnumerator TossFirstGrenade()
    {
        yield return new WaitForSeconds(grenadeFirstToss);

    }
}

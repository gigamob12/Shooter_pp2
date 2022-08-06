using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamagable 
{
    //header helps with organizing it on Unity
    [Header("Components")]
    [SerializeField] CharacterController controller;

    [Header("-----------------------------------")]
    [Header("Player Attributes")]
    [Range(1, 25)][SerializeField] int hp;
    [Range(3, 50)][SerializeField] float playerSpeed;
    [Range(1.5f, 24)][SerializeField] float sprintMulti;
    [Range(1, 6)][SerializeField] int jump;
    [Range(5, 30)][SerializeField] float jumpHeight;
    [Range(1, 20)][SerializeField] float gravityValue;

    [Header("-----------------------------------")]
    [Header("Player Weapon stats")]
    [Range(0.1f, 3)][SerializeField] float shootRate;
    [Range(1, 10)][SerializeField] int weaponDamage;

    [Header("-----------------------------------")]
    [Header("Effects")]
    [SerializeField] GameObject hitEffectSpark;
    [SerializeField] GameObject muzzleFlash;

    [Header("-----------------------------------")]
    [Header("Physics")]
    public Vector3 pushback = Vector3.zero;
    [SerializeField] int pushResolve;


    bool isSprinting = false;
    float playerSpeedOrig;
    Vector3 playerVelocity;
    Vector3 move;
    int timesJumped;
    bool canShoot = true;
    int hpOrig;
    Vector3 playerSpawnPosition;

    private void Start()
    {
        playerSpeedOrig = playerSpeed;
        hpOrig = hp;
        playerSpawnPosition = transform.position;
    }

    void Update()
    {

        if (!gamemanager.instance.paused)
        {
            pushback = Vector3.Lerp(pushback, Vector3.zero, Time.deltaTime * pushResolve);

            movePlayer();
            sprint();
            StartCoroutine(shoot());
        }
    }
    private void movePlayer()
    {
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVelocity.y -= 2;
        }


        // if land reset the plaer velocity
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            timesJumped = 0;
            playerVelocity.y = 0f;
        }
        // get the input from unity input system
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        // add our move vector into the character controller move
        controller.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jump)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }
        //add gravity
        playerVelocity.y -= gravityValue * Time.deltaTime;

        // add gravity back into the character controller move
        controller.Move((playerVelocity + pushback) * Time.deltaTime);

    }
    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed = playerSpeed * sprintMulti;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed = playerSpeedOrig;

        }
    }

    IEnumerator shoot()
    {
        if (Input.GetButton("Shoot") && canShoot)
        {
            canShoot = false;

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit))
            {
                Instantiate(hitEffectSpark, hit.point, hitEffectSpark.transform.rotation);

                if (hit.collider.GetComponent<IDamagable>()!= null)
                {
                    IDamagable isDamagable = hit.collider.GetComponent<IDamagable>();

                    if (hit.collider is SphereCollider)
                    {
                        isDamagable.takeDamage(100);
                    }
                    else
                    {
                        isDamagable.takeDamage(weaponDamage);
                    }
                }
            }

            muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            muzzleFlash.SetActive(false);


            yield return new WaitForSeconds(shootRate);
            canShoot = true;
        }


    }

    public void takeDamage(int dmg)
    {
        hp -= dmg;
        updatePlayerHp();
        StartCoroutine(damageFlash());


        if (hp <= 0)
        {
            //kill player
            gamemanager.instance.playerDead();

        }
    }
    IEnumerator damageFlash()
    {
        gamemanager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageFlash.SetActive(false);
    }

    public void giveHP(int amount)
    {
        if (hp<hpOrig)
            hp += amount;
        if (hp > hpOrig)
            hp = hpOrig;
        updatePlayerHp();
    }
    public void updatePlayerHp()
    {
        gamemanager.instance.HPBar.fillAmount = (float)hp / (float)hpOrig;

    }
    public void respawn()
    {
        hp = hpOrig;
        updatePlayerHp();
        controller.enabled = false;
        transform.position = playerSpawnPosition;
        controller.enabled = true;
        pushback = Vector3.zero;


    }



}

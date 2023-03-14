using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float maxChAttavkDmg;

    [Header("cur values")]
    public int curHp;
    public int jumps;
    public int score;
    public float curMoveInput;
    public float playerIndex;
    public bool isSlowed;
    public float timeHit;
    public int faceingDir;

    [Header("mods")]
    public float movespeed;
    public float jumpforce;

    [Header("Audio")]
    // 0 jump
    // 1 hit ground
    // 2 taunt 1 
    // 3 playerjoin
    // 4 die
    // 5 iceball
    // 6 charge ball
    public AudioClip[] playerfx;


    [Header("Attacking")]
    [SerializeField]
    private PlayerController curAttacker;
    public float attackRate = 3;
    public float lastAttackTime;
    public float attackSpeed;
    public float attackDamage;
    public float slowTime;
    public bool isCharging;
    public float chAttackDamage;
    public float chAttackRate;
    public GameObject chargeBall;

    // 0 fireball
    public GameObject[] attackPrefabs;

    [Header("components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource Audio;
    public Transform muzzle;
    public Transform ChargeMuzzle;
    public PlayerContainerUI uiContainer;
    public GameManager gameManager;
    public GameObject deathEfectPrefab;


    [Header("debug")]
    public bool debug;

   




    // Start is called before the first frame update

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        Audio = GetComponent<AudioSource>();
        Audio.PlayOneShot(playerfx[3]);
        gameManager = GameObject.FindObjectOfType<GameManager>();
        muzzle =GetComponentInChildren<Muzzle>().transform;
        ChargeMuzzle = GetComponentInChildren<ChargeMuzzle>().transform;
        uiContainer.updateHealthBar(curHp, maxHp);
        uiContainer.updateChargeBar(chAttackDamage, maxChAttavkDmg);
    }
    void Start()
    {
        curHp = maxHp;
        jumps = maxJumps;
        score = 0;
        faceingDir = -1;
        
    }
    private void FixedUpdate()
    {
        move();   
        if(curMoveInput == 1 )
        {
            faceingDir = -1;
        }
        if(curMoveInput == -1)
        {
            faceingDir = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10 || curHp <= 0)
        {
            die();
        }
        if(isSlowed)
        {
            if(Time.time - timeHit > slowTime)
            {
                isSlowed = false;
            }
        }
        if(isCharging)
        {
            curMoveInput = 0;
            jumps = 0;
            chargeBall.transform.position = ChargeMuzzle.position;
            chAttackDamage += chAttackRate;
                if(chAttackDamage> maxChAttavkDmg)
                {
                chAttackDamage = maxChAttavkDmg;
                }
            uiContainer.updateChargeBar(chAttackDamage, maxChAttavkDmg);
        }




        if(score > gameManager.highScore)
        {
            gameManager.winningPlayers.Clear();
            gameManager.highScore = score;
        }
        if(score == gameManager.highScore && !gameManager.winningPlayers.Contains(this))
        {
            gameManager.winningPlayers.Add(this);
        }
    }
    private void LateUpdate()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D x in collision.contacts)
        {
            if(x.collider.CompareTag("Ground"))
            {
                if(x.point.y < transform.position.y)
                {
                    hitGround();
                }
            }
            if((x.point.x > transform.position.x || x.point.x < transform.position.x) && x.point.y <= transform.position.y )
            {
                if(jumps < maxJumps)
                {
                    jumps++;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    private void jump()
    {
        //set cur y vel to 0
        rig.velocity = new Vector2(rig.velocity.x, 0);
        // add jump force
        rig.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
        Audio.PlayOneShot(playerfx[0]);
    }
    private void move()
    {
        rig.velocity = new Vector2(curMoveInput * movespeed, rig.velocity.y);
        if(curMoveInput != 0.0f)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }
    public void die()
    {
        Audio.PlayOneShot(playerfx[4]);
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            if(score < 0)
            {
                score = 0;
            }
        }
        uiContainer.updateScoreText(score);


       GameObject effect = Instantiate(deathEfectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        respawn();





        if (debug)
        {
            print("player "+ playerIndex + " has died");
        }
    }
    private void taunt1()
    {
        Audio.PlayOneShot(playerfx[2]);
    }
    public void addScore()
    {
        score++;
        uiContainer.updateScoreText(score);
    }

    public void takeDamage(int ammount, PlayerController attacker)
    {
        curHp -= ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(curHp, maxHp);
    }

    //over load method to take float
    public void takeDamage(float ammount, PlayerController attacker)
    {
        curHp -= (int)ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(curHp, maxHp);
    }
    public void takeIceDammage(float ammount, PlayerController atttacker)
    {
        curHp -= (int)ammount;
        curAttacker = atttacker;
        uiContainer.updateHealthBar(curHp, maxHp);
    }

    private void respawn()
    {
        curHp = maxHp;
        jumps = maxJumps;
        curAttacker = null;
        transform.position = gameManager.holdpoint.position;
        transform.position = gameManager.spawnpoints[Random.Range(0, gameManager.spawnpoints.Length)].position;
        uiContainer.updateHealthBar(curHp, maxHp);
        if (debug)
        {
            print("player " + playerIndex + " has respwaned");
        }
    }
    
    
    public void spawnstdFireball()
    {
      GameObject fireball =  Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireball.GetComponent<Projectiles>().onSpawn(attackSpeed, attackDamage, this, faceingDir);
        
        
    }
    public void spawmIceAttack()
    {
        Audio.PlayOneShot(playerfx[5]);
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<Projectiles>().onSpawn(attackRate, attackDamage, this, faceingDir);
    }
    public void spawnChargeAttack()
    {
        Audio.PlayOneShot(playerfx[6]);
        chargeBall = Instantiate(attackPrefabs[2], ChargeMuzzle.position, Quaternion.identity);
        
       // chargeBall.GetComponent<Projectiles>().onSpawn(attackSpeed, chAttackDamage, this, faceingDir);
    }
    public void fireChargeAttack()
    {
        chargeBall.GetComponent<Projectiles>().onSpawn(attackSpeed, chAttackDamage, this, faceingDir);
    }


 private void hitGround()
    {
        jumps = maxJumps;
        Audio.PlayOneShot(playerfx[1]);
        rig.velocity = new Vector2(rig.velocity.x, 0);
    }


    //input sytem methods below 

    //move input methods 

    public void onMoveInput(InputAction.CallbackContext context)
    {
        if(debug)
        {
            print("player " + playerIndex + " pressed move button") ;
        }
        float x = context.ReadValue<float>();
        if (x > 0)
        {
            curMoveInput = 1;
        }
        else if(x < 0)
        {
            curMoveInput = - 1;
        }
        else
        {
            curMoveInput = 0;
        }
    }
    public void onJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            if(debug)
            {
                print("player " + playerIndex +" pressed jump button");
            }
            if(jumps > 0)
            {
                jumps--;
                jump();
            }
           
        }
    }
    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (debug)
            {
                print("player " + playerIndex + " pressed block button");
            }
        }
    }
    public void onStandardAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            spawnstdFireball();
            if(isCharging)
            {
                chAttackDamage = 0;
                isCharging = false;
            }

            if (debug)
            {
                print("player " + playerIndex + " pressed StdAttack button");
            }
        }
    }
    public void onIceAttactInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            spawmIceAttack();
            if (isCharging)
            {
                chAttackDamage = 0;
                isCharging = false;
            }
            if (debug)
            {
                print("player " + playerIndex + " pressed IceAttack button");
            }
        }
    }
    public void onChargeAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            
            isCharging = true;
            spawnChargeAttack();

            if (debug)
            {
                print("player " + playerIndex + " pressed ChargeAttack button"); ;
            }
        }
        if(context.phase == InputActionPhase.Canceled)

        {
            isCharging = false;
            fireChargeAttack();
        }
    }
    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (debug)
            {
                print("player " + playerIndex + " pressed pause button");
            }
        }
    }
    public void onTauntInput1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            taunt1();
            if (debug)
            {
                print("player " + playerIndex + " pressed taunt1 button");
            }
        }
    }
    public void onTauntInput2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (debug)
            {
                print("player " + playerIndex + " pressed taunt2 button");
            }
        }
    }
    public void onTauntInput3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (debug)
            {
                print("player " + playerIndex + " pressed taunt3 button");
            }
        }
    }
    public void onTauntInput4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (debug)
            {
                print("player " + playerIndex + " pressed taunt4 button");
            }
        }
    }
    public void setuicontainer(PlayerContainerUI containerUI)
    {
        this.uiContainer = containerUI;
    }


}

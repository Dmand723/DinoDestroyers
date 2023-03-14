using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{

    public int damageAmount;
    public PlayerController owner;
    public float lifetime = 5f;
    public float speed;

    public Rigidbody2D rig;

    // Start is called before the first frame update

    private void Awake()
    {
        
        rig = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("hit");
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeDamage(damageAmount, owner);
        }
        Destroy(gameObject);
        owner.chargeBall = null;
    }

    public void onSpawn(float speed, float damage, PlayerController owner, float dir)
    {
        setOwner(owner);
        setDamage(damage);
        setSpeed(speed);
        rig.velocity = new Vector2(-dir * speed, 0);
    }
    public void onSpawn(float speed, int damage, PlayerController owner, float dir)
    {
        setOwner(owner);
        setDamage(damage);
        setSpeed(speed);
        rig.velocity = new Vector2(dir * speed, 0);
    }

    public void setOwner(PlayerController owner)
    {
        this.owner = owner;
    }
    
    public void setDamage(float damage)
    {
        this.damageAmount = (int)damage;
    }
    public void setDamage(int damage)
    {
        this.damageAmount = damage;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
        
    }
}

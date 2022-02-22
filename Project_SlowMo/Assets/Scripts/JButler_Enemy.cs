using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Enemy : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("Health Stats")]
    [Tooltip("This is the enemy's max healt.\nDefault is 100.")]
    [Min(0f)]
    [SerializeField] int maxHealth = 100;
    [Tooltip("This is the enemy's current health.")]
    [Min(0f)]
    [SerializeField] int curHealth;

    [Space(5)]
    [Header("Vitals Status")]
    [Tooltip("This is a bool to determine if the enemy is alive or dead.\nDefault is false.")]
    public bool dead = false;

    [Space(5)]
    [Header("Enemy's Rigidbody")]
    [Tooltip("There should be a rigidbody attached to the Avatar.\nPut that rigidbody here.")]
    [SerializeField] Rigidbody rb = null;

    [Space(5)]
    [Header("Is Active")]
    [Tooltip("In order for the EnemyManager to keep track of this enemy, DO NOT TURN IT OFF!\nInstead set this bool to false.")]
    [SerializeField] bool isActive = true;

    [Space(5)]
    [Header("DEBUG")]
    [Tooltip("This will make the enemy unkillable, but damageable.")]
    [SerializeField] bool immortal = false;
    [Tooltip("This will make the enemy unmoveable.")]
    [SerializeField] bool unmoveable = false;

    [Space(5)]
    [Header("AudioClips")]
    [SerializeField] AudioClip Death = null;
    [SerializeField] AudioClip Hurt_1 = null;
    [SerializeField] AudioClip Hurt_2 = null;

    [Space(5)]
    [Header("Pickup")]
    public GameObject ammoObject = null;

    AudioSource EnemyAudio = null;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth();
        ClampMax();

        CheckFields();

        EnemyAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMove();
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Revive()
    {
        dead = false;
        MaxHealth();
    }

    public int CurHealth()
    {
        return curHealth;
    }

    public void MaxHealth()
    {
        curHealth = maxHealth;
    }

    public void Kill()
    {
        curHealth = 0;
        CheckVitals();
    }

    public void DontLoad()
    {
        dead = true;
    }

    public void TakeDamage(int damage)
    {
        if (!immortal)
        {
            curHealth -= damage;

            int rand = Random.Range(0, 6);

            if(rand >= 3)
            {
                EnemyAudio.clip = Hurt_1;
                EnemyAudio.Play();
            }
            else if(rand == 4)
            {
                EnemyAudio.clip = Hurt_2;
                EnemyAudio.Play();
            }
        }
        CheckVitals();
        try
        {
            //FindObjectOfType<JButler_CrosshairManager>().ShowDamage(damage);
        }
        catch (System.NullReferenceException e)
        {
            throw new System.Exception(name + " <b><color=red>could not find JButler_CrosshairManage</color></b>!\tMake sure that the Crosshair canvas is active inside the scene!\n" + e.Message);
        }
    }

    public void ToggleImmortal()
    {
        immortal = !immortal;
    }

    public void ToggleUnmoveable()
    {
        unmoveable = !unmoveable;
    }

    public void TurnBackOff()
    {
        if (!isActive)
            gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        if (!isActive)
        {
            isActive = true;
        }
    }

    public bool IsDead()
    {
        return dead;
    }

    public void TurnOffKinematic()
    {
        rb.isKinematic = false;
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CheckVitals()
    {
        if (curHealth <= 0 && !dead)
        {
            GameObject a = Instantiate(ammoObject) as GameObject;
            a.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);
            //a.transform.position = gameObject.transform.position;
            curHealth = 0;
            dead = true;
            rb.constraints = RigidbodyConstraints.None;
            ToggleCrosshair();
            Invoke("ToggleCrosshair", 0.2f);
            EnemyAudio.clip = Death;
            EnemyAudio.Play();
        }
        else if (curHealth <= 0)
            curHealth = 0;
    }

    void ToggleCrosshair()
    {
        try
        {
            FindObjectOfType<JButler_CrosshairManager>().ActiveDeactivate();
        }
        catch (System.NullReferenceException e)
        {
            throw new System.Exception(name + " <b><color=red>could not find JButler_CrosshairManage</color></b>!\tMake sure that the Crosshair canvas is active inside the scene!\n" + e.Message);
        }
    }

    void CheckMove()
    {
        //if (unmoveable)
        //    rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        if (!dead)
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void ClampMax()
    {
        if (maxHealth < 1)
        {
            maxHealth = 1;
            throw new System.Exception("<b><color=red>Max Health for " + name + " has been set too low</color></b>!\tDouble check if this is what you want.");
        }
    }

    void CheckFields()
    {
        if (rb == null)
            throw new System.Exception(name + " <b><color=red>does not have a Rigidbody reference</color></b>!\tMake sure to add the correct Rigidbody to Rb.");
    }
}

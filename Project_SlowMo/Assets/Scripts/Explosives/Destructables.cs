using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructables : MonoBehaviour
{
    //Invisible Section

    Sound_Manager sm;
    Destructables_Manager dsm = null;
    bool destroyed = false;

    Color originalColor;
    Color darkenedColor;
    Color currentColor;

    float interval = 0;
    bool rising = false;

    //Visible Section

    [Header("Instantiatable Info")]

    [Tooltip("The broken object to swap with this object")]
    [SerializeField] GameObject destroyedObject = null;

    [Tooltip("If this object spawns a pickup, place the EGO where the pickup will spawn here")]
    [SerializeField] Transform pickupLoc = null;

    [Tooltip("The pickup you want should be placed here. Find the correct one in the Prefabs > DestructableObjects > Pickups")]
    [SerializeField] GameObject pickupSpawn = null;

    [Tooltip("The remaining object(s) that appear and stay after the object is destroyed")]
    [SerializeField] GameObject objectResidual = null;

    [Header("Adjustable Values")]
    [SerializeField] int objMaxHealth = 0;

    [SerializeField] int objCurrHealth = 0;

    [Tooltip("Does this object have infinite health?")]
    [SerializeField] bool indestructable = false;

    [Tooltip("Can this object take damage from gunfire?")]
    [SerializeField] bool bulletDamage = false;

    [Tooltip("Does this object show a color tone change?")]
    [SerializeField] bool colorPulse = false;

    [Tooltip("Does this object spawn a pickup upon destruction?")]
    [SerializeField] bool pickup = false;

    [Tooltip("Will this object leave a portion of destroyed material behind after its destruction?")]
    [SerializeField] bool residual = true;

    [Tooltip("Must match sound listed in scene's SoundManager")]
    [SerializeField] string soundToPlayImpact = null;

    [Tooltip("Must match sound listed in scene's SoundManager")]
    [SerializeField] string soundToPlayDestroy = null;

    
    void Start()
    {
        objCurrHealth = objMaxHealth;

        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        dsm = FindObjectOfType<Destructables_Manager>();

        if (colorPulse)
        {
            originalColor = GetComponent<Renderer>().material.color;
            darkenedColor = new Color(originalColor.r - 0.15f, originalColor.g - 0.15f, originalColor.b - 0.15f);
        }

        
    }

    void Update()
    {
        if(objCurrHealth <= 0)
        {
            if (!string.IsNullOrEmpty(soundToPlayDestroy))
            {
                sm.Play(soundToPlayDestroy);
            }
            destroyed = true;
            dsm.Store_DES_Data();
            if(destroyedObject != null)
            {
                Instantiate(destroyedObject, transform.position, transform.rotation);
            }

            if (pickup)
            {
                Instantiate(pickupSpawn, pickupLoc.position, pickupLoc.rotation);
            }

            if (residual)
            {
                Instantiate(objectResidual, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }

        //Color Pulse
        if (colorPulse)
        {
            

            if (rising)
            {
                interval -= Time.deltaTime;

                GetComponent<Renderer>().material.color = Color.Lerp(originalColor, darkenedColor, interval);

                if (interval <= 0)
                {
                    rising = false;
                }
            }
            else
            {
                interval += Time.deltaTime;

                GetComponent<Renderer>().material.color = Color.Lerp(originalColor, darkenedColor, interval);

                if (interval >= 1)
                {
                    rising = true;
                }
            }

            currentColor = GetComponent<Renderer>().material.color;
        }
    }

    public bool Destroyed()
    {
        return destroyed;
    }

    public void NewDestroyed(bool value)
    {
        destroyed = value;
    }

    public void LoadDestoyed()
    {
        if (destroyed)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if(!string.IsNullOrEmpty(soundToPlayImpact))
        {
            sm.Play(soundToPlayImpact);
        }

        if (!indestructable)
        {
            objCurrHealth -= damage;
        }
    }

    public void TakeBulletDamage(int damage)
    {
        if (bulletDamage)
        {
            if (!string.IsNullOrEmpty(soundToPlayImpact))
            {
                sm.Play(soundToPlayImpact);
            }

            if (!indestructable)
            {
                objCurrHealth -= damage;
            }
        }
    }
}

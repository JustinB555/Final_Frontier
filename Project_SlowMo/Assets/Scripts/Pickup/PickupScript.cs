using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] bool isPainkiller = false;
    [SerializeField] bool isBT = false;
    [SerializeField] GameObject bottle = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] GameObject placeholder = null;
    [SerializeField] GameObject BTPill = null;
    [SerializeField] GameObject Grenade = null;


    bool isPlayerInTrigger = false;
    bool pickedUp = false;

    Sound_Manager sndmngr = null;
    
    PainKiller pk = null;
    BulletTimePicky btp = null;
    Player_Values pv = null;
    CheckpointManager cm = null;
    PlayerAnimatorScript plyrAnim;

    DebugScript debug = null;
    // Start is called before the first frame update
    void Start()
    {
        if (isPainkiller && !isBT && !pickedUp)
        {
            gameObject.GetComponent<Animator>().SetBool("isPainkiller", true);
            bottle.SetActive(true);
            bullet.SetActive(false);
            placeholder.SetActive(false);
            BTPill.SetActive(false);
            Grenade.SetActive(false);
        }
        else if (!isPainkiller && !isBT && !pickedUp)
        {
            bottle.SetActive(false);
            bullet.SetActive(true);
            placeholder.SetActive(false);
            BTPill.SetActive(false);
            Grenade.SetActive(false);
        }
        else if (!isPainkiller && isBT && !pickedUp)
        {
            gameObject.GetComponent<Animator>().SetBool("isPainkiller", true);
            bottle.SetActive(false);
            bullet.SetActive(false);
            placeholder.SetActive(false);
            BTPill.SetActive(true);
            Grenade.SetActive(false);
        }
        else if(isPainkiller && isBT && !pickedUp)
        {
            gameObject.GetComponent<Animator>().SetBool("isPainkiller", true);
            bottle.SetActive(false);
            bullet.SetActive(false);
            placeholder.SetActive(false);
            BTPill.SetActive(false);
            Grenade.SetActive(true);
        }
        sndmngr = FindObjectOfType<Sound_Manager>();
        
        pk = gameObject.GetComponent<PainKiller>();
        btp = gameObject.GetComponent<BulletTimePicky>();
        debug = FindObjectOfType<DebugScript>();
        pv = FindObjectOfType<Player_Values>();
        cm = FindObjectOfType<CheckpointManager>();
        plyrAnim = FindObjectOfType<PlayerAnimatorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTrigger)
        {
            sndmngr.Play("CollectPainkiller");
            if (isPainkiller && !isBT)
            {
                PainKiller();
            }
            else if (!isPainkiller && !isBT)
            {
                AmmoEffect();
            }
            else if (!isPainkiller && isBT)
            {
                BulletTime();
            }
            else if(isPainkiller && isBT)
            {
                GrenadePickup();
            }
        }
    }

    public void PlayerInTrigger()
    {
        isPlayerInTrigger = true;
    }
    public void PlayerLeftTrigger()
    {

    }
    void PainKiller()
    {
        //do whatever the painkiller is meant to do. Probably mess with the Player Values script.
        pickedUp = true;
        pk.PainKillerPickup();
        cm.StorePills();
        Destroy(gameObject);
    }
    void AmmoEffect()
    {
        //Add ammo to whatever gunthe player is currently carrying.
        pickedUp = true;
        debug.PickupAmmo();
        Destroy(gameObject);
    }
    void BulletTime()
    {
        pickedUp = true;
        btp.BTPillPickup();
        cm.StorePills();
        Destroy(gameObject);
    }

    void GrenadePickup()
    {
        pickedUp = true;
        pv.grenadeCount += 1;
        Destroy(gameObject);
    }
}

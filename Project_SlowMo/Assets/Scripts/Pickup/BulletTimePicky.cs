using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimePicky : MonoBehaviour
{
    Game_UI gui = null;
    Player_Values pv = null;
    BTPillParticle btpp = null;
    Sound_Manager sndmngr = null;

    private void Start()
    {
        gui = GameObject.Find("Game_UI").GetComponent<Game_UI>();
        pv = FindObjectOfType<Player_Values>();
        btpp = GameObject.Find("BTPill").GetComponent<BTPillParticle>();
        sndmngr = FindObjectOfType<Sound_Manager>();
    }

    public void BTPillPickup()
    {
        gui.baki += 1;
    }

    public void BTPillActivate()
    {
        if (gui.baki > 0)
        {
            sndmngr.Play("TakeDamage");
            gui.baki -= 1;
            pv.currBt = pv.currBt + 60;
            pv.UpdateBtBar();
            btpp.BTPillParticlePlay();
        }
    }
}

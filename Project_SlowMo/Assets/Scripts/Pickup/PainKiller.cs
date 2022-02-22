using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PainKiller : MonoBehaviour
{
    Game_UI gui;
    Player_Values pv;
    ParticleHealth ph;

    private void Start()
    {
        //gui = FindObjectOfType<Game_UI>();
        gui = GameObject.Find("Game_UI").GetComponent<Game_UI>();
        pv = FindObjectOfType<Player_Values>();
        ph = GameObject.Find("Particle System").GetComponent<ParticleHealth>();
    }

    public void PainKillerPickup()
    {
        gui.paki += 1;
    }

    public void PainKillerActivate()
    {
        if (gui.paki > 0)
        {
            gui.paki -= 1;
            pv.TakeDamage(-30);
            ph.PkParticle();
        }
    }
}

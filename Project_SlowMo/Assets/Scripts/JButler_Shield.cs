using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Shield : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    public bool play = true;

    // private
    Options_Menu opM = null;
    AudioSource audioS = null;

    // Start is called before the first frame update
    void Start()
    {
        opM = FindObjectOfType<Options_Menu>();
        audioS = GetComponent<AudioSource>();

        ChangeVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            ChangeVolume();
            audioS.Stop();
        }
        else if (Time.timeScale > 0 && audioS.isPlaying == false && play)
            audioS.Play();
    }

    //////////////////////////////////////////////////
    // Collsion Events
    //////////////////////////////////////////////////


    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void SetBulletHole(GameObject bh, RaycastHit hit)
    {
        Instantiate(bh, hit.point, Quaternion.LookRotation(hit.normal), transform);
    }



    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void ChangeVolume()
    {
        audioS.volume = opM.sfxValue/3f;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollTutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialText = null;

    MovementThirdPerson m3p = null;

    // Start is called before the first frame update
    void Start()
    {
        m3p = FindObjectOfType<MovementThirdPerson>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m3p.isNotTutorialCRS = true;
            TutorialText.SetActive(true);
            Invoke("RollingText", 4.0f);
        }
    }

    void RollingText()
    {
        TutorialText.SetActive(false);
    }

}

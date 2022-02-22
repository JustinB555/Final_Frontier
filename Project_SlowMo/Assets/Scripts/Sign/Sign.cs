using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeReference] GameObject UiText = null;

    private void OnTriggerStay(Collider other)
    {
        UiText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        UiText.SetActive(false);
    }

}

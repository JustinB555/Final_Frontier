using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    [SerializeField]
    GameObject otherCanvas = null;
    [SerializeField]
    GameObject controlsCanvas = null;

    bool isControlsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isControlsOpen)
        {
            controlsCanvas.SetActive(false);
        }
    }

        public void ToggleControls()
    {
        isControlsOpen = !isControlsOpen;
        if (isControlsOpen)
        {
            otherCanvas.SetActive(false);
            controlsCanvas.SetActive(true);
        }
        else
        {
            otherCanvas.SetActive(true);
            controlsCanvas.SetActive(false);
        }
    }
}

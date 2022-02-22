using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_CursorManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HideCursor();
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideCursor()
    {
        Cursor.visible = false;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ConfinedCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}

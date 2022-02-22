using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Follow : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Follow who?")]
    [Tooltip("Who is this Game Object going to follow.")]
    [SerializeField] Transform leader = null;
    [SerializeField] Transform[] leaders = null;
    [SerializeField] Transform rotation = null;
    [SerializeField] Vector3 offset = Vector3.zero;

    // private
    Transform us = null;
    bool allInActive = false;

    // Start is called before the first frame update
    void Start()
    {
        us = transform;
    }

    // Update is called once per frame
    void Update()
    {
        FollowTheLeader();
        Offset();
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    private void SetLeader()
    {
        int inactive = 0;
        foreach (Transform active in leaders)
        {
            if (active.gameObject.activeInHierarchy)
            {
                leader = active;
                rotation = active;
                inactive--;
                allInActive = false;
            }
            else if (inactive == leaders.Length)
            {
                allInActive = true;
                return;
            }
            else
            {
                inactive++;
                continue;
            }
        }
    }

    void FollowTheLeader()
    {
        if (leaders.Length == 0)
            us.position = leader.position;
        else
        {
            SetLeader();
            if (!allInActive)
            {
                us.position = leader.position;
                us.rotation = leader.rotation;
            }
        }
    }

    void Offset()
    {
        us.position += offset;
    }

}

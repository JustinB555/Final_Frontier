using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JButler_EnemyAreaCounter : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Mulitple Doors")]
    [Tooltip("How many clears do you need to open different door?\tShould equal the number of waves in this room.")]
    [SerializeField] int clears = 0;

    // private
    int curClear = 0;
    bool allDead = false;
    bool once = false;
    Dictionary<JButler_Enemy, bool> enemiesInArea = new Dictionary<JButler_Enemy, bool>();

    // Start is called before the first frame update
    void Start()
    {
        curClear = 0;
        //Debug.LogError(name + "'s Current Clear (at start) = " + curClear);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeStatues();

        if (AllEnemiesDead() && !once)
        {
            curClear++;
            //Debug.LogError(name + "'s Current Clear = " + curClear);
            if (curClear > clears)
                curClear = clears;

            once = true;
            allDead = true;
        }
    }

    //////////////////////////////////////////////////
    // Collision Events
    //////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<JButler_Enemy>() && !CheckEnemies(other))
        {
            AddEnemy(other);
        }
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public bool AllDead()
    {
        return allDead;
    }

    public int Clears()
    {
        return curClear;
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void ChangeStatues()
    {
        foreach (KeyValuePair<JButler_Enemy, bool> enemy in enemiesInArea.ToList())
            if (enemy.Value != enemy.Key.IsDead())
                enemiesInArea[enemy.Key] = enemy.Key.IsDead();
    }

    bool AllEnemiesDead()
    {
        foreach (KeyValuePair<JButler_Enemy, bool> enemy in enemiesInArea)
        {
            if (enemy.Key.gameObject.activeInHierarchy && enemy.Value == true)
                continue;
            else
            {
                once = false;
                allDead = false;
                return false;
            }
        }
        return true;
    }

    void AddEnemy(Collider other)
    {
        enemiesInArea.Add(other.GetComponentInParent<JButler_Enemy>(), other.GetComponentInParent<JButler_Enemy>().IsDead());
    }

    bool CheckEnemies(Collider other)
    {
        foreach (var name in enemiesInArea)
        {
            if (name.Key.name == other.GetComponentInParent<JButler_Enemy>().name)
                return true;
            else
                continue;
        }
        return false;
    }

    void CheckFields()
    {

    }
}

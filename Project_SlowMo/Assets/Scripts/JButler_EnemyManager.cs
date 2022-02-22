using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_EnemyManager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("Enemies in Scene")]
    [Tooltip("This keeps track of all enemies inside this scene.")]
    [SerializeField] JButler_Enemy[] enemies = null;

    [Space(5)]
    [Header("Active Enemies")]
    [Tooltip("This keeps track of all active enemies inside this scene.\nIf it is 0, then all enemies are inactive(dead).")]
    [SerializeField] int activeEnemies;

    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsOfType<JButler_Enemy>();
        Array.Sort(enemies, new EnemyComparer());
        data = FindObjectOfType<OptionsSaverScript>();
        CheckFields();

        if (data.worldValuesUsed)
            CountTheDead();
        else
            ReviveAll();
        AddTheTotal();
        AddTheLiving();
        TurnBackOff();
    }

    // Update is called once per frame
    void Update()
    {
        CheckArray();
        AddTheTotal();
        SetTheDead();
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public int NumberOfEnemies()
    {
        return activeEnemies;
    }

    public void KillAll()
    {
        foreach (JButler_Enemy enemy in enemies)
            enemy.Kill();
    }

    public void ReviveAll()
    {
        foreach (JButler_Enemy enemy in enemies)
            enemy.Revive();
    }

    public void ImmortalAll()
    {
        foreach (JButler_Enemy enemy in enemies)
            enemy.ToggleImmortal();
    }

    public void UnmoveableAll()
    {
        foreach (JButler_Enemy enemy in enemies)
            enemy.ToggleUnmoveable();
    }

    public void AddTheTotal()
    {
        int count = 0;
        for (int i = 0; i < enemies.Length; i++)
            if (!enemies[i].IsDead())
                count++;
        data.enemyCount = count;
    }

    public void AddTheLiving()
    {
        data.enemiesAlive.Clear();
        for (int i = 0; i < enemies.Length; i++)
            data.enemiesAlive.Add(i, enemies[i].IsDead());
    }

    public void CountTheDead()
    {
        foreach (KeyValuePair<int, bool> pair in data.enemiesAlive)
        {
            if (pair.Value)
                enemies[pair.Key].DontLoad();
        }
    }

    public void SetTheDead()
    {
        int temp = 0;

        foreach (KeyValuePair<int, bool> pair in data.enemiesAlive)
            if (enemies[pair.Key].IsDead() && !pair.Value)
                temp = pair.Key;
        data.enemiesAlive[temp] = enemies[temp].IsDead();
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void TurnBackOff()
    {
        foreach (JButler_Enemy enemy in enemies)
            enemy.TurnBackOff();
    }

    void CheckArray()
    {
        int active = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.activeInHierarchy && !enemies[i].IsDead())
                active++;
            activeEnemies = active;
        }
    }

    void CheckFields()
    {
        if (enemies.Length == 0)
            throw new System.Exception(name + "'s Enemies <b><color=red>array size is set to 0</color></b>!\tThat means that there is no enemies in the scene!");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find OptionsSaverScript</color></b>!\tMake sure that there is an active one in the scene at all times!");
    }
}


public class EnemyComparer : IComparer<JButler_Enemy>
{
    public int Compare(JButler_Enemy a, JButler_Enemy b)
    {
        return a.name.CompareTo(b.name);
    }
}
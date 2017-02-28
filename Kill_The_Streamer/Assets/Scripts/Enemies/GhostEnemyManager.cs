﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostEnemyManager : MonoBehaviour
{
    public GameObject m_ghostPrefab;

    private GameObject[] m_ghostGameObjects;
    private EnemyData[] m_ghostEnemyData;
    private AiGhost[] m_ghostGhostComponents;
    private int m_firstInactiveIndex = 0;

    private GameObject[] m_spawnLocations;

    private static GhostEnemyManager s_instance;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        GameObject spawnpoints = GameObject.FindGameObjectWithTag("Spawnpoints");

        m_spawnLocations = new GameObject[spawnpoints.transform.childCount];
        for (int i = 0; i < m_spawnLocations.Length; i++)
        {
            m_spawnLocations[i] = spawnpoints.transform.GetChild(i).gameObject;
        }
    }

    // Called at the beginning of the game by the EnemyManager
    public static void Init(Transform p_parent)
    {
        // Initializes the gameobject and component arrays
        s_instance.m_ghostGameObjects = new GameObject[Constants.MAX_ENEMIES];
        s_instance.m_ghostEnemyData = new EnemyData[Constants.MAX_ENEMIES];
        s_instance.m_ghostGhostComponents = new AiGhost[Constants.MAX_ENEMIES];

        for (int i = 0; i < Constants.MAX_ENEMIES; i++)
        {
            // Instantiates each enemy
            GameObject ghost = Instantiate<GameObject>(s_instance.m_ghostPrefab, Vector3.zero, Quaternion.identity, p_parent);
            EnemyData ghostData = ghost.GetComponent<EnemyData>();
            AiGhost ghostComponent = ghost.GetComponent<AiGhost>();

            // Sets the enemy's name and turns it off
            ghost.name = s_instance.m_ghostPrefab.name + " " + i;
            ghost.SetActive(false);

            // Saves the gameobject and components in the arrays
            s_instance.m_ghostGameObjects[i] = ghost;
            s_instance.m_ghostEnemyData[i] = ghostData;
            s_instance.m_ghostGhostComponents[i] = ghostComponent;
        }
    }

    // Called by the enemy manager when activating an enemy. Returns true if successful or false otherwise.
    public static GameObject ActivateNextEnemy(string p_twitchUsername, int p_spawnLocation)
    {
        // Prevents adding an enemy if there is no more room in the array
        if (s_instance.m_firstInactiveIndex == Constants.MAX_ENEMIES) return null;

        // Gets first inactive enemy gameobject
        GameObject ghost = s_instance.m_ghostGameObjects[s_instance.m_firstInactiveIndex];

        // Assigns the enemy's array index in the enemy data script
        s_instance.m_ghostEnemyData[s_instance.m_firstInactiveIndex].m_Index = s_instance.m_firstInactiveIndex;

        // Sets the enemy's name to the twich username
        ghost.name = p_twitchUsername;
        ghost.GetComponentInChildren<Text>().text = p_twitchUsername;

        // Validates the spawn location
        int spawnpoint = p_spawnLocation >= 0 && p_spawnLocation < s_instance.m_spawnLocations.Length ? p_spawnLocation : Random.Range(0, s_instance.m_spawnLocations.Length);

        // Sets the position of the enemy
        ghost.transform.position = s_instance.m_spawnLocations[spawnpoint].transform.position;

        // Enables the gameobject
        ghost.SetActive(true);

        // Increments the first inactive index
        s_instance.m_firstInactiveIndex++;

        // Returns the enemy gameobject
        return ghost;
    }

    // Called by the enemy manager when deactivating an enemy. Returns true
    public static bool DeactivateEnemy(int p_enemyIndex)
    {
        // Fails if the enemy index is invalid or if there are no active enemies 
        if (p_enemyIndex < 0 || p_enemyIndex >= s_instance.m_firstInactiveIndex || s_instance.m_firstInactiveIndex == 0) return false;

        // Temporarily saves the data from the enemy we are deactivating
        GameObject temp = s_instance.m_ghostGameObjects[p_enemyIndex];
        EnemyData tempEnemyData = s_instance.m_ghostEnemyData[p_enemyIndex];
        AiGhost tempAISeekFlee = s_instance.m_ghostGhostComponents[p_enemyIndex];

        // Deactivates the enemy
        temp.SetActive(false);

        // Moves the enemy at the end of the active section to the deactivated position
        s_instance.m_ghostGameObjects[p_enemyIndex] = s_instance.m_ghostGameObjects[s_instance.m_firstInactiveIndex - 1];
        s_instance.m_ghostEnemyData[p_enemyIndex] = s_instance.m_ghostEnemyData[s_instance.m_firstInactiveIndex - 1];
        s_instance.m_ghostGhostComponents[p_enemyIndex] = s_instance.m_ghostGhostComponents[s_instance.m_firstInactiveIndex - 1];

        // Moves the deactivated enemy to the start of the inactive section
        s_instance.m_ghostGameObjects[s_instance.m_firstInactiveIndex - 1] = temp;
        s_instance.m_ghostEnemyData[s_instance.m_firstInactiveIndex - 1] = tempEnemyData;
        s_instance.m_ghostGhostComponents[s_instance.m_firstInactiveIndex - 1] = tempAISeekFlee;

        // Makes sure the indices in the enemy data scripts are correct
        s_instance.m_ghostEnemyData[p_enemyIndex].m_Index = p_enemyIndex;
        s_instance.m_ghostEnemyData[s_instance.m_firstInactiveIndex - 1].m_Index = s_instance.m_firstInactiveIndex - 1;

        // Decrements the first inactive index
        s_instance.m_firstInactiveIndex--;

        return true;
    }

    public static GameObject GetActiveEnemy(int p_index)
    {
        if (p_index < 0 || p_index >= s_instance.m_firstInactiveIndex)
        {
            Debug.Log("Invalid index " + p_index + " in GhostEnemy array");
            return null;
        }

        return s_instance.m_ghostGameObjects[p_index];
    }

    public static GameObject[] GetAllEnemies(out int p_firstInactiveIndex)
    {
        p_firstInactiveIndex = s_instance.m_firstInactiveIndex;
        return s_instance.m_ghostGameObjects;
    }
}
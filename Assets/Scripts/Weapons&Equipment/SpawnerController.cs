using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject spawnerPrefab;

    private int stepNumber = -1;
    private int stepsToRaise;

    private List<Transform> spawnTransforms = new List<Transform>();

    public void BeginSpawnSequence(int stepsToRaise,int totalDuration, Vector3[][] playerSpawns)
    {
        this.stepsToRaise = stepsToRaise;

        for(int playerNum = 0; playerNum < playerSpawns.Length; playerNum++)
        {
            for(int instanceNum = 0; instanceNum < playerSpawns[playerNum].Length; instanceNum++)
            {
                GameObject spawner = Instantiate(spawnerPrefab);
                Transform transform = spawner.GetComponent<Transform>();
                transform.position = playerSpawns[playerNum][instanceNum];
                spawnTransforms.Add(transform);

            }
        }
        stepNumber = 0;
    }

    private void FixedUpdate()
    {
        if(stepNumber >= 0 && stepNumber < stepsToRaise)
        {
            stepNumber++;
            float yScale = (stepsToRaise - stepNumber) / (float)stepsToRaise * -1;

            foreach (Transform transform in spawnTransforms)
                transform.position = new Vector3(transform.position.x, yScale * 0.5f, transform.position.z);
        }
    }

    private void DestroyPlatforms()
    {
        foreach (Transform transform in spawnTransforms)
            Destroy(transform.gameObject);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class InstinctPlayerManager : PlayerManager
{
    private int maxProjectiles;
    private int maxEquipment;
    private List<int> projectilesRemaining;
    private List<int> equipmentRemaining;

    private int playerNumber;

    internal delegate void OnPlayerKilled(int playerNum);
    internal OnPlayerKilled PlayerKilledListener { get; set; }

    internal InstinctPlayerManager(int playerNumber, int maxProjectiles, int maxEquipment)
    {
        this.playerNumber = playerNumber;
        this.maxProjectiles = maxProjectiles;
        this.projectilesRemaining = new List<int>();
        this.maxEquipment = maxEquipment;
        this.equipmentRemaining = new List<int>();
    }

    internal override int GetMaxProjectiles(int roundNumber)
    {
        return maxProjectiles;
    }

    internal override int GetAvailableProjectiles(int roundNumber)
    {
        if (roundNumber < 0 || roundNumber >= projectilesRemaining.Count)
            throw new System.ArgumentException("Illegal round number passed to GetAvailableProjectiles");
        return projectilesRemaining[roundNumber];
    }

    internal override int GetProjectedProjectilesRemaining(int roundNumber, int asOfStepNumber)
    {
        return 0;
    }

    internal override int GetProjectedProjectilesRemaining(int asOfStepNumber)
    {
        return 0;
    }

    internal override int GetMaxEquipment(int roundNumber)
    {
        return maxEquipment;
    }

    internal override int GetAvailableEquipment(int roundNumber)
    {
        if (roundNumber < 0 || roundNumber >= equipmentRemaining.Count)
            throw new System.ArgumentException("Illegal round number passed to GetAvailableEquipment");
        return equipmentRemaining[roundNumber];
    }

    internal override int GetProjectedEquipmentRemaining(int roundNumber, int asOfStepNumber)
    {
        return 0;
    }

    //add another controlled player instance
    internal override void AppendNewRecording(PlayerController controller)
    {
        foreach(PlayerController playerObject in playerControllers)
        {
            PlayerHealth existingHealth = playerObject.GetComponent<PlayerHealth>();
            existingHealth.RemoveHealthChangeListener(OnPlayerHealthChange);
        }

        playerControllers.Add(controller);
        playerRecordings.Add(new MatchRecordingManager(controller));
        projectilesRemaining.Add(maxProjectiles);
        equipmentRemaining.Add(maxEquipment);
        PlayerHealth health = controller.GetComponent<PlayerHealth>();
        health.AddHealthChangeListener(OnPlayerHealthChange);

        int index = playerRecordings.Count - 1;
        controller.FireCallback = () =>
        {
            if(projectilesRemaining[index] > 0)
            {
                projectilesRemaining[index]--;
                return true;
            }
            return false;
        };
        controller.EquipmentCallback = () =>
        {
            if (equipmentRemaining[index] > 0)
            {
                equipmentRemaining[index]--;
                return true;
            }
            return false;
        };
    }

    internal override void ResetAll()
    {
        foreach (PlayerController controller in playerControllers)
            controller.OnReset();
        for(int i = 0; i < projectilesRemaining.Count && i < equipmentRemaining.Count; i++)
        {
            projectilesRemaining[i] = maxProjectiles;
            equipmentRemaining[i] = maxEquipment;
        }
    }

    private void OnPlayerHealthChange(int newHealth, int maxHealth, GameObject player)
    {
        if (newHealth <= 0)
        {
            PlayerKilledListener?.Invoke(playerNumber);
        }
    }
}

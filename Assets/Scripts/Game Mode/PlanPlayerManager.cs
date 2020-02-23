using System.Collections.Generic;
using UnityEngine;

/*
 * Class is responsible for managing all the recordings and instances of a single player
 * Knows the shared equipment and ammo numbers and has access to MatchRecordingManagers for each round
 * Must be synchronized with the match counter
 */

public class PlanPlayerManager
{
    private List<MatchRecordingManager> playerRecordings = new List<MatchRecordingManager>();
    private List<PlayerController> playerControllers = new List<PlayerController>();
    public int MaxProjectiles { get; set; } = 10;
    public int AvailableProjectiles { internal set; get; }
    public int ProjectedProjectilesRemaining(int asOfStepNumber)
    {
        int usage = MaxProjectiles - AvailableProjectiles;               //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedFireEventsAfter(asOfStepNumber);

        usage = MaxProjectiles - usage;                                 //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }
    public int MaxEquipment { get; set; } = 1;
    public int AvailableEquipment { internal set; get; }
    public int ProjectedEquipmentRemaining(int asOfStepNumber)
    {
        int usage = MaxEquipment - AvailableEquipment;                  //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedEquipmentEventsAfter(asOfStepNumber);

        usage = MaxEquipment - usage;                                   //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }

    public GameObject GetPlayerObject(int roundNumber)
    {
        if (roundNumber < 0 || roundNumber >= playerControllers.Count)
            throw new System.Exception("Passed in illegal round number to GetPlayerObject");
        return playerControllers[roundNumber].gameObject;
    }

    internal PlanPlayerManager(int availableProjectiles, int availableEquipment)
    {
        MaxProjectiles = availableProjectiles;
        AvailableProjectiles = availableProjectiles;
        MaxEquipment = availableEquipment;
        AvailableEquipment = availableEquipment;
    }

    //add another controlled player instance
    internal void AppendNewRecording(PlayerController controller)
    {
        playerControllers.Add(controller);
        controller.FireCallback = OnPlayerFireEvent;
        controller.EquipmentCallback = OnPlayerEquipmentEvent;
        playerRecordings.Add(new MatchRecordingManager(controller));
    }

    //called each frame to make players record/reproduce data for that frame
    internal void Step(int stepNumber)
    {
        foreach (MatchRecordingManager recording in playerRecordings)
        {
            recording.AppendNextSnapshot(stepNumber);
            recording.UtilizeFrame(stepNumber);
        }
    }

    //resets for the next match
    internal void FinishSequence()
    {
        foreach (MatchRecordingManager recording in playerRecordings)
            recording.Finish();
        AvailableProjectiles = MaxProjectiles;
        AvailableEquipment = MaxEquipment;
    }

    internal void DestroyAll()
    {
        foreach (PlayerController controller in playerControllers)
            Object.Destroy(controller);
    }

    internal bool RecordExistsForMatch(int matchNum)
    {
        return playerRecordings.Count > matchNum && matchNum >= 0;
    }

    internal bool OnPlayerFireEvent()
    {
        if (AvailableProjectiles > 0)
        {
            AvailableProjectiles--;
            return true;
        }
        return false;
    }

    internal bool OnPlayerEquipmentEvent()
    {
        if (AvailableEquipment > 0)
        {
            AvailableEquipment--;
            return true;
        }
        return false;
    }
}

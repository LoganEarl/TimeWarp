using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerManager
{
    private protected List<MatchRecordingManager> playerRecordings = new List<MatchRecordingManager>();
    private protected List<PlayerController> playerControllers = new List<PlayerController>();

    internal virtual PlayerController MainPlayer
    {
        get
        {
            if (playerControllers.Count > 0)
                return playerControllers[playerControllers.Count - 1];
            return null;
        }
    }

    internal abstract int GetMaxProjectiles(int roundNumber);
    internal int GetMaxProjectiles()
    {
        return GetMaxProjectiles(0);
    }
    internal abstract int GetAvailableProjectiles(int roundNumber);
    internal int GetAvailableProjectiles()
    {
        return GetAvailableProjectiles(0);
    }
    internal virtual int GetProjectedProjectilesRemaining(int roundNumber, int asOfStepNumber)
    {
        int usage = GetMaxProjectiles(roundNumber) - GetAvailableProjectiles(roundNumber);               //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedFireEventsAfter(asOfStepNumber);

        usage = GetMaxProjectiles(roundNumber) - usage;                                 //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }

    internal virtual int GetProjectedProjectilesRemaining(int asOfStepNumber)
    {
        int usage = GetMaxProjectiles() - GetAvailableProjectiles();               //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedFireEventsAfter(asOfStepNumber);

        usage = GetMaxProjectiles() - usage;                                 //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }

    internal abstract int GetMaxEquipment(int roundNumber);
    internal int GetMaxEquipment()
    {
        return GetMaxEquipment(0);
    }
    internal abstract int GetAvailableEquipment(int roundNumber);
    internal int GetAvailableEquipment()
    {
        return GetAvailableEquipment(0);
    }

    internal virtual int GetProjectedEquipmentRemaining(int roundNumber, int asOfStepNumber)
    {
        int usage = GetMaxEquipment(roundNumber) - GetAvailableEquipment(roundNumber);                  //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedEquipmentEventsAfter(asOfStepNumber);

        usage = GetMaxEquipment(roundNumber) - usage;                                   //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }

    public GameObject GetPlayerObject(int roundNumber)
    {
        if (roundNumber < 0 || roundNumber >= playerControllers.Count)
            throw new System.Exception("Passed in illegal round number to GetPlayerObject:" + roundNumber);
        return playerControllers[roundNumber].gameObject;
    }

    //add another controlled player instance
    internal abstract void AppendNewRecording(PlayerController controller);

    //called each frame to make players record/reproduce data for that frame
    internal void Step(int stepNumber)
    {
        foreach (MatchRecordingManager recording in playerRecordings)
        {
            recording.AppendNextSnapshot(stepNumber);
            recording.UtilizeFrame(stepNumber);
        }
    }

    //finishes recordings and attaches newest generation to replays
    internal void FinishSequence()
    {
        foreach (MatchRecordingManager recording in playerRecordings)
            recording.Finish();
        foreach (PlayerController controller in playerControllers)
            controller.SetUseSnapshots(true);
    }

    internal abstract void ResetAll();

    internal void DestroyAll()
    {
        foreach (PlayerController controller in playerControllers)
            Object.Destroy(controller.gameObject);
    }

    internal bool RecordExistsForMatch(int roundNum)
    {
        return playerRecordings.Count > roundNum && roundNum >= 0;
    }
}

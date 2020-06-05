using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Class responsible for keeping track of all of the snapshots of a single player for a single match
 * Is attached to a single player controller and stores info from the controller
 */
public class MatchRecordingManager
{
    private bool recordingComplete = false;         //set to true when the recording is finalized and ready for replay
    private PlayerController playerController;      //the attached player controller
    private List<PlayerSnapshot> snapshots = new List<PlayerSnapshot>();    //Player snapshots indexed by frame number
    private Dictionary<int, int> fireEvents = new Dictionary<int, int>();         //frame index cache of all the times the fire button was pressed
    private List<int> equipmentEvents = new List<int>();    //frame index cache of all the times the equipment button was pressed

    internal MatchRecordingManager(PlayerController playerController)
    {
        this.playerController = playerController;
        playerController.SetUseSnapshots(false);
    }

    internal int RecordedFireEventsAfter(int stepNum)
    {
        if (!recordingComplete)
            return 0;
        int total = 0;
        foreach(int fireIndex in fireEvents.Keys)
            if (fireIndex >= stepNum)
                total+= fireEvents[fireIndex];
        
        return total;
    }

    internal int RecordedEquipmentEventsAfter(int stepNum)
    {
        if (!recordingComplete)
            return 0;
        int total = 0;
        foreach (int equipmentIndex in equipmentEvents)
            if (equipmentIndex >= stepNum)
                total++;

        return total;
    }

    internal void AppendNextSnapshot(int snapshotIndex)
    {
        if (!recordingComplete)
        {
            PlayerSnapshot snapshot = playerController.GetSnapshot();
            snapshots.Add(snapshot);
            if (snapshot.Firing != 0)
                fireEvents[snapshotIndex] = snapshot.Firing;
            if (snapshot.UsingEquipment)
                equipmentEvents.Add(snapshotIndex);
        }
    }

    internal PlayerSnapshot UtilizeFrame(int frameNum)
    {
        if (recordingComplete && snapshots.Count > 0)
        {
            if (frameNum >= 0 && frameNum < snapshots.Count)
                playerController.SetSnapshot(snapshots[frameNum]);
            else if (frameNum < 0)
                playerController.SetSnapshot(snapshots[0]);
            else
                playerController.SetSnapshot(snapshots[snapshots.Count - 1]);
        }
        return null;
    }

    internal void Finish()
    {
        recordingComplete = true;
        playerController.SetUseSnapshots(true);
    }
}


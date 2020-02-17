using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MatchRecordingManager
{
    private bool recordingComplete = false;
    private PlayerController playerController;
    private List<PlayerSnapshot> snapshots = new List<PlayerSnapshot>();
    private List<int> fireEvents = new List<int>();

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
        foreach(int fireIndex in fireEvents)
            if (fireIndex > stepNum)
                total++;
        
        return total;
    }

    internal void AppendNextSnapshot(int snapshotIndex)
    {
        if (!recordingComplete)
        {
            PlayerSnapshot snapshot = playerController.GetSnapshot();
            snapshots.Add(snapshot);
            if (snapshot.Firing)
                fireEvents.Add(snapshotIndex);
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


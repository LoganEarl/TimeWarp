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

    internal MatchRecordingManager(PlayerController playerController)
    {
        this.playerController = playerController;
        playerController.SetUseSnapshots(false);
    }

    internal void AppendNextSnapshot()
    {
        if (!recordingComplete)
            snapshots.Add(playerController.GetSnapshot());
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


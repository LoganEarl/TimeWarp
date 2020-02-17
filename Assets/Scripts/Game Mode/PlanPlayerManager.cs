using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlanPlayerManager
{
    private List<MatchRecordingManager> playerRecordings = new List<MatchRecordingManager>();
    public int AvailableProjectiles { internal set; get; }

    internal PlanPlayerManager(int availableProjectiles)
    {
        this.AvailableProjectiles = availableProjectiles;
    }

    internal void AppendNewRecording(PlayerController controller)
    {
        playerRecordings.Add(new MatchRecordingManager(controller));
    }

    internal void Step(int stepNumber)
    {
        foreach (MatchRecordingManager recording in playerRecordings)
        {
            recording.AppendNextSnapshot();
            recording.UtilizeFrame(stepNumber);
        }
    }

    internal void FinishSequence()
    {
        foreach (MatchRecordingManager recording in playerRecordings)
            recording.Finish();
    }

    internal bool RecordExistsForMatch(int matchNum)
    {
        return playerRecordings.Count > matchNum && matchNum >= 0;
    }
}

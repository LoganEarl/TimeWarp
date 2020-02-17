using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlanPlayerManager
{
    private List<MatchRecordingManager> playerRecordings = new List<MatchRecordingManager>();
    private List<PlayerController> playerControllers = new List<PlayerController>();
    public int MaxProjectiles { get; set; } = 10;
    public int AvailableProjectiles { internal set; get; }
    public int ProjectedProjectilesRemaining(int asOfStepNumber)
    {
        int usage = MaxProjectiles- AvailableProjectiles;               //what we already used
        foreach (MatchRecordingManager recording in playerRecordings)   //what we are going to use in the future
            usage += recording.RecordedFireEventsAfter(asOfStepNumber);

        usage = MaxProjectiles - usage;                                 //what we have overall

        if (usage < 0)
            usage = 0;
        return usage;
    }

    internal PlanPlayerManager(int availableProjectiles)
    {
        this.AvailableProjectiles = availableProjectiles;
    }

    internal void AppendNewRecording(PlayerController controller)
    {
        playerControllers.Add(controller);
        controller.FireCallback = OnPlayerFireEvent;
        playerRecordings.Add(new MatchRecordingManager(controller));
    }

    internal void Step(int stepNumber)
    {
        foreach (MatchRecordingManager recording in playerRecordings)
        {
            recording.AppendNextSnapshot(stepNumber);
            recording.UtilizeFrame(stepNumber);
        }
    }

    internal void FinishSequence()
    {
        foreach (MatchRecordingManager recording in playerRecordings)
            recording.Finish();
        AvailableProjectiles = MaxProjectiles;
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
        else
            return false;
    }
}

/*
 * Class is responsible for managing all the recordings and instances of a single player
 * Knows the shared equipment and ammo numbers and has access to MatchRecordingManagers for each round
 * Must be synchronized with the match counter
 */

public class PlanPlayerManager: PlayerManager
{
    internal PlanPlayerManager(int availableProjectiles, int availableEquipment)
    {
        this.maxProjectiles = availableProjectiles;
        this.availableProjectiles = availableProjectiles;
        this.maxEquipment = availableEquipment;
        this.availableEquipment = availableEquipment;
    }

    private int maxProjectiles = 10;
    internal override int GetMaxProjectiles(int roundNumber)
    {
        return maxProjectiles;
    }
    private int availableProjectiles;
    internal override int GetAvailableProjectiles(int roundNumber)
    {
        return availableProjectiles;
    }

    private int maxEquipment = 1;
    internal override int GetMaxEquipment(int roundNumber)
    {
        return maxEquipment;
    }
    private int availableEquipment = 1;
    internal override int GetAvailableEquipment(int roundNumber)
    {
        return availableEquipment;
    }
   
    internal int NumberRecordingsAlive
    {
        get
        {
            int count = 0;
            foreach (PlayerController controller in playerControllers)
                if (controller.GetComponent<PlayerHealth>().Health > 0)
                    count++;
            return count;
        }
    }

    internal int TotalHealthRemaining
    {
        get
        {
            int count = 0;
            foreach (PlayerController controller in playerControllers)
                count += controller.GetComponent<PlayerHealth>().Health;
            return count;
        }
    }

    internal int TotalTimeAlive
    {
        get
        {
            int count = 0;
            foreach (PlayerController controller in playerControllers)
                count += controller.GetComponent<PlayerHealth>().TimeAlive;
            return count;
        }
    }

    //add another controlled player instance
    internal override void AppendNewRecording(PlayerController controller)
    {
        playerControllers.Add(controller);
        controller.FireCallback = OnPlayerFireEvent;
        controller.EquipmentCallback = OnPlayerEquipmentEvent;
        playerRecordings.Add(new MatchRecordingManager(controller));
    }

    internal override void ResetAll()
    {
        foreach (PlayerController controller in playerControllers)
            controller.OnReset();
        availableProjectiles = maxProjectiles;
        availableEquipment = maxEquipment;
    }

    internal bool OnPlayerFireEvent(int costToFire)
    {
        if (availableProjectiles >= costToFire)
        {
            availableProjectiles -= costToFire;
            return true;
        }
        return false;
    }

    internal bool OnPlayerEquipmentEvent()
    {
        if (availableEquipment > 0)
        {
            availableEquipment--;
            return true;
        }
        return false;
    }
}

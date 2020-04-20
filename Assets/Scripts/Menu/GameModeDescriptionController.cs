using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeDescriptionController : MonoBehaviour {
    [SerializeField]
    private Text gameText;

    private string description = "";

    public void Update()
    {
        gameText.text = this.description;
    }

    public void SetGameDescription(string gameModeName)
    {
        if (!gameModeName.Equals("") || gameModeName != null)
            if (gameModeName.Equals("Plan"))
                this.description = ThePlanDescription();
            else if (gameModeName.Equals("Lineage"))
                this.description = LineageDescription();
            else if (gameModeName.Equals("Instinct"))
                this.description = InstinctDescription();
                   
    }

    private string ThePlanDescription()
    {
        return "A gamemode that leverages the time travel feature to allow players to plan " +
               "out their turn in relative safety, protect their past selves, and attempt " +
               "to eliminate the enemy player’s past selves.";
    }

    private string LineageDescription()
    {
        return "A gamemode where the objective is to keep your past selves alive, while " +
               "attempting to eliminate one of your opponents past self. If your present " +
               "self dies, time warps back, and you get a chance to save yourself.";
    }

    private string InstinctDescription()
    {
        return "Only your current self, and that of your enemy matter. Say alive as you " +
            "both gain more an more iterations, and the field grows thick with the " +
            "projectiles of your past selves.";
    }
}

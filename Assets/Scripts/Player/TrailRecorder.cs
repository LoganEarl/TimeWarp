using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRecorder : MonoBehaviour
{
    private TrailRenderer trail;
    private PlayerController controller;
    private IGameMode gameMode;
    private bool setup = false;

    // Start is called before the first frame update
    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
    }

    public void Setup(PlayerController controller, IGameMode gameMode)
    {
        this.controller = controller;
        this.gameMode = gameMode;
        setup = true;

        trail.material = ColorManager.Instance.GetPlayerMaterial(controller.PlayerNumber, ColorManager.PlayerColorVarient.UI_PRIMARY_ACTIVE);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (setup)
        {
            bool active = !gameMode.GameState.GetPlayerPositionsLocked(controller.PlayerNumber, controller.RoundNumber) &&
                gameMode.GameState.GetPlayerVisible(controller.PlayerNumber, controller.RoundNumber) &&
                !controller.UsingSnapshots;
            trail.emitting = active;

            if (active)
            {
                if (controller.FiringGun)
                    PlaceProjectileIndicator();
                if (controller.UsingEquipment)
                    PlaceEquipmentIndicator();
            }
        }
    }

    private void PlaceProjectileIndicator()
    {
        GameObject indicator = Instantiate(controller.Weapon.ProjectileIconPrefab);
        Material material = ColorManager.Instance.GetPlayerMaterial(controller.PlayerNumber, ColorManager.PlayerColorVarient.UI_PRIMARY_ACTIVE);
        MeshRenderer primaryRenderer = indicator.GetComponent<MeshRenderer>();
        primaryRenderer.material = material;
        indicator.transform.position = gameObject.transform.position;
        indicator.transform.localRotation = Quaternion.LookRotation(controller.transform.forward, Vector3.up);
        gameMode.ClearOnMatchChange(indicator);
    }

    private void PlaceEquipmentIndicator()
    {
        GameObject indicator = Instantiate(controller.EquipmentIconPrefab);
        Material material = ColorManager.Instance.GetPlayerMaterial(controller.PlayerNumber, ColorManager.PlayerColorVarient.UI_PRIMARY_ACTIVE);
        MeshRenderer primaryRenderer = indicator.GetComponent<MeshRenderer>();
        primaryRenderer.material = material;
        indicator.transform.position = gameObject.transform.position;
        indicator.transform.localRotation = Quaternion.LookRotation(controller.transform.forward, Vector3.up);
        gameMode.ClearOnMatchChange(indicator);
    }
}

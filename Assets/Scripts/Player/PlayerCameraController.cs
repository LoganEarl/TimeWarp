﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera[] playerCameras;
    private PlayerController[] mainPlayers;
    private IGameMode gameMode;
    private bool setup = false;

    public void Setup(IGameMode gameMode, PlayerController[] mainPlayers)
    {
        this.gameMode = gameMode;
        this.mainPlayers = mainPlayers;
        setup = true;
    }

    public void Stop()
    {
        setup = false;
    }

    private void Update()
    {
        if (setup)
        {
            for(int playerIndex = 0; playerIndex < playerCameras.Length && playerIndex < mainPlayers.Length; playerIndex++)
            {
                bool playerVisible = gameMode.GameState.GetPlayerVisible(playerIndex, gameMode.RoundNumber);

                if (playerVisible)
                {
                    Vector3 delta = mainPlayers[playerIndex].CameraPosition - playerCameras[playerIndex].transform.position;

                    Vector3 adj = delta / 15.0f;
                    if (adj.magnitude > 0.001)
                    {
                        playerCameras[playerIndex].transform.position += adj;
                    }
                }
            }
        }
    }
}

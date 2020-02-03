using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a standin for a later system
public class PlayerController : MonoBehaviour, IRecordable
{
    [SerializeField]
    public int speed = 10;
    private Rigidbody rigidBody = null;
    private PlayerSnapshot currentSnapshot = null;
    private bool useSnapshots = false;

    private int playerNum;
    private int sourceRoundNum;

    void FixedUpdate()
    {
        if(rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        if (useSnapshots)
        {
            if(currentSnapshot != null)
            {
                rigidBody.position = currentSnapshot.Transformation;
                rigidBody.velocity = currentSnapshot.Force;
            }
        }
        else
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            rigidBody.AddForce(direction * speed);
        }
        
    }

    public void SetUseSnapshots(bool useSnapshots)
    {
        this.useSnapshots = useSnapshots;
    }

    public void SetSnapshot(PlayerSnapshot playerSnapshot)
    {
        currentSnapshot = playerSnapshot;
    }

    public PlayerSnapshot GetSnapshot()
    {
        if(rigidBody != null)
        {
            return new PlayerSnapshot(transform.position, rigidBody.velocity);
        }
        else
        {
            return new PlayerSnapshot(new Vector3(), new Vector3());
        }
    }

    public void SetPlayerInformation(int playerNum, int sourceRoundNum)
    {
        this.playerNum = playerNum;
        this.sourceRoundNum = sourceRoundNum;
    }
}

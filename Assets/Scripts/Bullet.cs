using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask playerMask;
    public AudioSource bounceSound;
    public float bulletDmg = 1;
    public float bouncesLeft = 4;
    public float maxLifeTime = 10f;

    void Start() {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other) {
        Rigidbody bulletInstance = GetComponent<Rigidbody>();

        if (other.tag == "Player" || bouncesLeft == 0)
            Destroy(gameObject, 0f);
        else if (other.tag == "Wall") {
            Vector3 tempVector,
                otherPos = other.gameObject.transform.position,
                otherScale = other.gameObject.transform.lossyScale,
                thisPos = gameObject.transform.position,
                thisScale = gameObject.transform.lossyScale;

            float tempX = otherPos.x / otherScale.x - thisPos.x / thisScale.x;
            float tempZ = otherPos.z / otherScale.z - thisPos.z / thisScale.z;

            if (Mathf.Abs(tempX) > Mathf.Abs(tempZ)) {
                tempVector = bulletInstance.velocity;
                tempVector.x = -tempVector.x;
                bulletInstance.velocity = tempVector;
            }
            else {
                tempVector = bulletInstance.velocity;
                tempVector.z = -tempVector.z;
                bulletInstance.velocity = tempVector;
            }

            bouncesLeft--;
        }
    }
}

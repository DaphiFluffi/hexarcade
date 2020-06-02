﻿using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private bool useLerp = false;
    [SerializeField]
    private float timeAlignment = 0.75f;
    [SerializeField]
    private bool focusTarget = false;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 15, 0);

    private Vector3 velocity = Vector3.zero;
    private Transform target;

    // experimental feature on
    [SerializeField]
    private bool experimentCamera = false;
    [SerializeField]
    private Vector3 changePositionOffset = new Vector3(0, 15, 0);
    private Rigidbody playerRB;
    private float changePosition;
    private Vector3 playerMoveDir, playerPrevPos;
    private float distance;
    //experimental feature off

    public void SetPosition(Transform player)
    {
        this.transform.position = player.position;
    }

    public void SetTarget(Transform player)
    {
        target = player;

        //experimental feature on
        playerRB = player.GetComponent<Rigidbody>();
        if (GameObject.Find("Map/UntaggedGameObjects/CameraChangePosition"))
        {
            GameObject cameraChanger = GameObject.Find("Map/UntaggedGameObjects/CameraChangePosition");
            changePosition = cameraChanger.transform.position.z;
        }
        //experimental feature off
    }

    /*
    *  The camera hovers offseted over a given GameObject with a slight delay. The goal to center the targeted GameObject is always set.
    *  The "strictness" of the Camera to focus the GameObject can be determined by a conditional call of the LookAt function, if prefered over just a shorter value of the timeAlignment variable. 
    */
    void LateUpdate()
    {
        if (experimentCamera)
        {
            //experimantel feature on
            playerMoveDir = target.transform.position - playerPrevPos;
            if (playerMoveDir != Vector3.zero)
            {
                playerMoveDir.Normalize();
                transform.position = target.transform.position - playerMoveDir * distance;

                Vector3 height = transform.position;

                if (transform.position.z < changePosition)
                {
                    height += offset;
                }
                else
                {
                    height += changePositionOffset;
                }


                transform.position = height;

                transform.LookAt(target.transform.position);

                playerPrevPos = target.transform.position;
                //experimental feature off
            }
        }
        else
        {
            if (!useLerp)
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, timeAlignment);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, timeAlignment);
            }
            if (focusTarget)
            {
                transform.LookAt(target, Vector3.forward);
            }
        }
    }
}


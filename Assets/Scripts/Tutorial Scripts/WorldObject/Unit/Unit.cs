﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Unit : WorldObject {

    public float moveSpeed, rotateSpeed;

    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);
        if (player && player.isHuman && currentlySelected)
        {
            if (hoverObject.name == "Ground")
            {
                player.hud.SetCursorState(CursorState.Move);
            }
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick(hitObject, hitPoint, controller);
        //only handle input if owned by a human player and currently selected
        if (player && player.isHuman && currentlySelected)
        {
            if (hitObject.name == "Ground" && hitPoint != ResourceManager.InvalidPosition)
            {
                float x = hitPoint.x;
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;

                Vector3 destination = new Vector3(x, y, z);
                StartMove(destination);
            }
        }
    }

    public void StartMove(Vector3 destination)
    {
        this.destination = destination;
        targetRotation = Quaternion.LookRotation(destination - transform.position);
        rotating = true;
        moving = false;
    }

    protected bool moving, rotating;

    protected virtual void Awake()
    {
        base.Awake();
        //Add extra code here
    }

    protected virtual void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        base.Update();
        if (rotating)
        {
            TurnToTarget();
        } else if (moving)
        {
            MakeMove();
        }
    }

    protected virtual void OnGUI()
    {
        base.OnGUI();
    }

    private Vector3 destination;
    private Quaternion targetRotation;

    private void TurnToTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
        //sometimes it gets stuck, exactly 180 degrees out in the calculation and does nothing, this check fixes that
        Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
        if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
        {
            rotating = false;
            moving = true;
        }
        CalculateBounds();
    }

    private void MakeMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
        if (transform.position == destination)
        {
            moving = false;
        }
        CalculateBounds();
    }
}

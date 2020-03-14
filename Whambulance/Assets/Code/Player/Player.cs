﻿using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static List<Player> All { get; set; } = new List<Player>();

    /// <summary>
    /// The current player in existence.
    /// </summary>
    public static Player Instance { get; private set; }

    /// <summary>
    /// Returns the radius of the player.
    /// </summary>
    public static float Radius => Instance != null ? Instance.radius : 0.3f;

    [SerializeField]
    private Transform carriedObjectRoot;

    [SerializeField]
    private float radius = 0.3f;

    /// <summary>
    /// The movement component attached to this player.
    /// </summary>
    public PlayerMovement Movement { get; private set; }

    /// <summary>
    /// The direction that the player should be moving in based on inputs.
    /// </summary>
    public Vector2 MovementInput
    {
        get
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(x, y);
            return input.normalized;
        }
    }

    /// <summary>
    /// The rotation that the player should be looking at based on movement input.
    /// </summary>
    public float Rotation { get; private set; }

    /// <summary>
    /// The objective that is being carried if any.
    /// </summary>
    public Objective CarryingObjective { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        Instance = this;
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    private void Update()
    {
        //send inputs to the movement thingy
        Vector2 input = MovementInput;
        Movement.Input = input;
        if (Input.GetButtonDown("Jump"))
        {
            FindCar();
        }

        //rotate the root
        if (input.sqrMagnitude > 0.5f)
        {
            Rotation = (Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg) - 90f;
        }

        transform.eulerAngles = new Vector3(0f, 0f, Rotation);
    }

    /// <summary>
    /// Puts this thing on the player to make it look like its being carried.
    /// </summary>
    public void Carry(Objective objective)
    {
        CarryingObjective = objective;
        CarryingObjective.BeganCarrying(this);

        //parent this to the back transform
        CarryingObjective.transform.SetParent(carriedObjectRoot);
        CarryingObjective.transform.localPosition = Vector3.zero;
        CarryingObjective.transform.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Drops the current carried objective if any is present.
    /// </summary>
    public void Drop()
    {
        if (CarryingObjective)
        {
            CarryingObjective.Dropped(this);
            CarryingObjective.transform.SetParent(null);
        }
    }

    //finds the cars within the circle collider's range
    void FindCar()
    {
        //Collects car's hit info and stores it in array
        Collider2D[] carsHit = Physics2D.OverlapCircleAll(transform.position + transform.up, 0.8f);
        if (carsHit.Length > 0)
        {
            for (int i = 0; i < carsHit.Length; i++)
            {
                if (carsHit[i].GetComponent<CarHit>())
                {
                    //Calls the hit car's launch function
                    carsHit[i].GetComponent<CarHit>().LaunchCar();
                }
            }
        }
    }

    /// <summary>
    /// Returns the closest player that overlaps with this position.
    /// </summary>
    public static Player Get(Vector2 position, float extraRadius = 0f)
    {
        int index = -1;
        float closest = float.MaxValue;
        for (int i = 0; i < All.Count; i++)
        {
            float distance = Vector2.SqrMagnitude(position - (Vector2)All[i].transform.position);
            if (distance < closest)
            {
                closest = distance;
                index = i;
            }
        }

        if (index != -1)
        {
            Player player = All[index];
            if (closest <= (player.radius + extraRadius) * (player.radius + extraRadius))
            {
                return player;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        //So we can see and adjust the OverlapCircle gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up, 0.8f);

        //also show the player radius just in case
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
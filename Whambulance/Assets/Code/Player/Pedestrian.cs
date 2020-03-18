using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : Player
{
    public enum PedestrianState
    {
        Thinking,
        Walking,
        Talking
    }

    [SerializeField]
    private float minThinkDuration = 4f;

    [SerializeField]
    private float maxThinkDuration = 6f;

    [SerializeField]
    private float minWalkDuration = 12f;

    [SerializeField]
    private float maxWalkDuration = 16f;

    [SerializeField, Range(0f, 100f)]
    private float chanceToTalk = 20f;

    private Vector2 input;
    private Waypoint waypoint;
    private PedestrianState state = PedestrianState.Walking;
    private float walkDuration;
    private float thinkDuration;
    private float thinkingTime;
    private float walkingTime;
    private float talkTime;
    private float nextCanTalk;

    /// <summary>
    /// The city block that this pedestrian belongs to.
    /// </summary>
    public CityBlock CityBlock { get; set; }

    public override Vector2 MovementInput => input;

    public override bool Punch
    {
        get
        {
            return false;
        }
    }

    private void Start()
    {
        //find closest waypoint
        waypoint = CityBlock.ClosestWaypoint(transform.position);
    }

    public override void Update()
    {
        base.Update();
        if (state != PedestrianState.Talking)
        {
            if (Time.time > nextCanTalk)
            {
                //check with nearby another pedestrian
                //if they wanna talk
                bool talk = Random.Range(0, 100) < chanceToTalk;
                const float TalkRadius = 0.8f;
                if (talk)
                {
                    foreach (Player player in All)
                    {
                        if (player is Pedestrian pedestrian)
                        {
                            if (pedestrian != this)
                            {
                                if (Vector2.SqrMagnitude(transform.position - pedestrian.transform.position) < TalkRadius * TalkRadius)
                                {
                                    state = PedestrianState.Talking;
                                    talkTime = 0f;
                                    pedestrian.state = PedestrianState.Talking;
                                    pedestrian.talkTime = 0.2f;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (state == PedestrianState.Walking)
        {
            walkingTime += Time.deltaTime;
            if (walkingTime > walkDuration)
            {
                state = PedestrianState.Thinking;
                thinkingTime = 0f;
                thinkDuration = Random.Range(minThinkDuration, maxThinkDuration);
            }
            else
            {
                //walk towards waypoint
                Vector2 dirToWaypoint = waypoint.transform.position - transform.position;
                input = dirToWaypoint.normalized;

                //too close to waypoint, so pick another one
                if (dirToWaypoint.sqrMagnitude < 0.3f * 0.3f)
                {
                    List<Waypoint> waypoints = CityBlock.GetConnectedWaypoints(waypoint);
                    waypoint = waypoints[Random.Range(0, waypoints.Count)];
                }
            }
        }
        else if (state == PedestrianState.Thinking)
        {
            //deciding which direction to go now
            input = Vector2.zero;

            thinkingTime += Time.deltaTime;
            if (thinkingTime > thinkDuration)
            {
                state = PedestrianState.Walking;
                walkingTime = 0f;
                walkDuration = Random.Range(minWalkDuration, maxWalkDuration);
                waypoint = CityBlock.ClosestWaypoint(transform.position);
            }
        }
        else if (state == PedestrianState.Talking)
        {
            //bla bla bla, do nothing
            input = Vector2.zero;

            //talking too much
            talkTime += Time.deltaTime;
            if (talkTime > 2f)
            {
                state = PedestrianState.Thinking;
                thinkingTime = 0f;
                thinkDuration = Random.Range(0.5f, 0.7f);
                nextCanTalk = Time.time + Random.Range(15f, 16f);
            }
        }
    }
}

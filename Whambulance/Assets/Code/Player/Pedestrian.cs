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
    private float personalBubbleRadius = 0.8f;

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
    private float avoidTime;

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
        Walk();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //show the avoidance radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, personalBubbleRadius);
    }

    private void Walk()
    {
        state = PedestrianState.Walking;
        walkingTime = 0f;
        walkDuration = Random.Range(minWalkDuration, maxWalkDuration);
        waypoint = CityBlock.ClosestWaypoint(transform.position);
    }

    private void Think()
    {
        state = PedestrianState.Thinking;
        thinkingTime = 0f;
        thinkDuration = Random.Range(minThinkDuration, maxThinkDuration);
        nextCanTalk = Time.time + Random.Range(15f, 16f);
    }

    private void Talk(Player other)
    {
        state = PedestrianState.Talking;
        talkTime = Random.Range(0f, 0.4f);

        //face the other person
        Vector2 dirToOther = (other.transform.position - transform.position).normalized;
        Rotation = Mathf.Atan2(dirToOther.y, dirToOther.x) * Mathf.Rad2Deg;
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
                        if (player != this)
                        {
                            if (Vector2.SqrMagnitude(transform.position - player.transform.position) < TalkRadius * TalkRadius)
                            {
                                Talk(player);
                                if (player is Pedestrian pedestrian)
                                {
                                    pedestrian.Talk(this);
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
                Think();
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

                //avoid other players by strafing left or right
                Vector2 avgAvoidancePoint = default;
                int playersAvoiding = 0;
                foreach (Player player in All)
                {
                    if (player != this)
                    {
                        //way to close to another player, avoid them at all costs!!!
                        Vector2 dirToOther = player.transform.position - transform.position;
                        if (dirToOther.sqrMagnitude < personalBubbleRadius * personalBubbleRadius)
                        {
                            avgAvoidancePoint += (Vector2)player.transform.position;
                            playersAvoiding++;
                        }
                    }
                }

                //avoid this
                if (playersAvoiding > 0)
                {
                    avgAvoidancePoint /= playersAvoiding;
                    Vector2 dirToOther = (avgAvoidancePoint - (Vector2)transform.position).normalized;
                    float angle = Mathf.Atan2(dirToOther.y, dirToOther.x) + 90f * Mathf.Deg2Rad;
                    Vector2 rotated = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    input = Vector2.Lerp(dirToWaypoint.normalized, rotated, 0.5f);
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
                Walk();
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
                Think();
            }
        }
    }
}

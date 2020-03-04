using UnityEngine;

public class Spawnpoint : Prop
{
    private void OnDrawGizmos()
    {
        float radius = 0.5f;
        Gizmos.color = Color.green;

        //this draws a plus
        Gizmos.DrawLine(transform.position + Vector3.up * radius, transform.position + Vector3.down * radius);
        Gizmos.DrawLine(transform.position + Vector3.right * radius, transform.position + Vector3.left * radius);
    }
}
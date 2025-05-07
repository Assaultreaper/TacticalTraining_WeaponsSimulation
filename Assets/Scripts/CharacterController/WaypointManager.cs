using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw a sphere at the waypoint
            Gizmos.DrawSphere(waypoints[i].position, 0.3f);

            // Draw a line to the next waypoint (if it exists)
            if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.color = Color.yellow;
            }
        }
    }
}

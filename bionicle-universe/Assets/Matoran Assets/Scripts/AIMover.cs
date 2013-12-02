using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(CharacterController))]
public class AIMover : MonoBehaviour 
{
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;

    public Path path;
    public float speed;
    public float nextWaypointDistance;
    private int curWaypoint;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        seeker.StartPath(transform.position, targetPosition, onPathComplete);
    }

    public void Update()
    {
        if (path == null)
            return;
        if (curWaypoint >= path.vectorPath.Count)
            return;
        Vector3 dir = (path.vectorPath[curWaypoint] - transform.position).normalized;
        dir *= (speed * Time.deltaTime);
        controller.SimpleMove(dir);
        if (Vector3.Distance(transform.position, path.vectorPath[curWaypoint]) < nextWaypointDistance)
        {
            curWaypoint++;
            return;
        }
    }

    public void onPathComplete(Path p)
    {
        Debug.Log("Path successfully generated");
        if (!p.error)
        {
            path = p;
            curWaypoint = 0;
        }
    }
}

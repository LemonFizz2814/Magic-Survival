using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public List<Transform> targets;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float directionMultiplier = 5.0f;

    private bool enableTrack = false;
    private bool trackAim = false;
    private Vector3 velocity;
    private Vector3 newPos;

    public Animator camAnim;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!enableTrack || targets.Count == 0) return;

        newPos = GetCenterPoint() + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1 || !trackAim) return targets[0].position;

        //Get direction
        //Vector3 dir = (targets[1].position - transform.position).normalized;

        //var bounds = new Bounds(targets[0].position, Vector3.zero);
        //bounds.Encapsulate(transform.position);
        //bounds.Encapsulate(dir);

        //for (int i = 0; i < targets.Count; i++)
        //{
        //    bounds.Encapsulate(targets[i].position);
        //}

        //Gotta get the player's rotation and multiply it with the object's forward transform to get
        //the direction the player model is facing
        Vector3 center = targets[1].rotation * targets[0].forward;


        return targets[0].position + (center * directionMultiplier);
    }

    public bool EnableTrack
    {
        get { return enableTrack; }
        set { enableTrack = value; }
    }

    public bool TrackAim
    {
        get { return trackAim; }
        set { trackAim = value; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public List<Transform> targets;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = 0.5f;

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

        Vector3 direction = (targets[1].position - targets[0].position).normalized;
        Vector3 camPos = targets[1].position + targets[1].forward * offset.z + targets[1].up * offset.y;
        Vector3 finalPos = targets[0].position + Quaternion.AngleAxis(targets[1].eulerAngles.y, Vector3.up) * direction * camPos.magnitude;


        return finalPos;
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

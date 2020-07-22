using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovePath : MonoBehaviour {

    [SerializeField]
    public Vector3 startPos;
    [SerializeField]
    public Vector3 finishPos;
    [SerializeField]
    public int w;
    
    [SerializeField]
    public int targetPoint;
    [SerializeField]
    public int targetPointsTotal;

    [SerializeField]
    public string animName;
    [SerializeField]
    public float moveSpeed;
    [SerializeField]
    public bool loop;
    [SerializeField]
    public bool forward;
    [SerializeField]
    public GameObject walkPath;

    public void MyStart(int _w, int _i, string anim, bool _loop, bool _forward, float _moveSpeed)
    {
        forward = _forward;
        moveSpeed = _moveSpeed;
        var _WalkPath = walkPath.GetComponent<WalkPath>();
        w = _w;
        targetPointsTotal = _WalkPath.getPointsTotal(0) - 2;

        loop = _loop;
        animName = anim;

        if(loop)
        {
            if(_i < targetPointsTotal && _i > 0)
            {
                if(forward)
                {
                    targetPoint = _i + 1;
                    finishPos = _WalkPath.getNextPoint(w, _i+1);
                }
                else 
                {
                    targetPoint = _i;
                    finishPos = _WalkPath.getNextPoint(w, _i);
                }
            }
            else
            {
                if(forward)
                {
                    targetPoint = 1;
                    finishPos = _WalkPath.getNextPoint(w, 1);
                }
                else 
                {
                    targetPoint = targetPointsTotal;
                    finishPos = _WalkPath.getNextPoint(w, targetPointsTotal);
                }
            }
        }

        else
        {
            if(forward)
            {
                targetPoint = _i + 1;
                finishPos = _WalkPath.getNextPoint(w, _i+1);
            }
            else 
            {
                targetPoint = _i;
                finishPos = _WalkPath.getNextPoint(w, _i);
            }
        }


    }

    void Start()
    {
        Vector3 targetPos = new Vector3(finishPos.x, transform.position.y, finishPos.z);
        transform.LookAt(targetPos);

        GetComponent<Animator>().CrossFade(animName, 0.1f, 0, Random.Range(0.0f, 1.0f));
        if(animName == "walk")
            GetComponent<Animator>().speed = moveSpeed * 1.2f;
        else if(animName == "run")
            GetComponent<Animator>().speed = moveSpeed / 3;
    }
    void Update ()
    {   
        RaycastHit hit;

        if(Physics.Raycast(transform.position + new Vector3(0, 2, 0), -transform.up, out hit))
        {
            finishPos.y = hit.point.y;
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

        }

        Vector3 targetPos = new Vector3(finishPos.x, transform.position.y, finishPos.z);

        var _WalkPath = walkPath.GetComponent<WalkPath>();

        if(Vector3.Distance(transform.position, finishPos) < 0.2f && animName == "walk" && ((loop) || (!loop && targetPoint > 0 && targetPoint < targetPointsTotal)))
        {

            if(forward)
            {
                if(targetPoint < targetPointsTotal)
                    targetPos = _WalkPath.getNextPoint(w, targetPoint + 1);
                else
                    targetPos = _WalkPath.getNextPoint(w, 0);
                targetPos.y = transform.position.y;
            }

            else
            {
                if(targetPoint > 0)
                    targetPos = _WalkPath.getNextPoint(w, targetPoint - 1);
                else
                    targetPos = _WalkPath.getNextPoint(w, targetPointsTotal);
                targetPos.y = transform.position.y;
            }
        }

        if(Vector3.Distance(transform.position, finishPos) < 0.5f && animName == "run" && ((loop) || (!loop && targetPoint > 0 && targetPoint < targetPointsTotal)))
        {

            if(forward)
            {
                if(targetPoint < targetPointsTotal)
                    targetPos = _WalkPath.getNextPoint(w, targetPoint + 1);
                else
                    targetPos = _WalkPath.getNextPoint(w, 0);
                targetPos.y = transform.position.y;
            }

            else
            {
                if(targetPoint > 0)
                    targetPos = _WalkPath.getNextPoint(w, targetPoint - 1);
                else
                    targetPos = _WalkPath.getNextPoint(w, targetPointsTotal);
                targetPos.y = transform.position.y;
            }
        }

        Vector3 targetVector = targetPos - transform.position;

        if(targetVector != Vector3.zero)
        {
            Quaternion look = Quaternion.identity;
            if(animName == "walk")
                look = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetVector), Time.deltaTime * 4f * moveSpeed);
            else if(animName == "run")
                look = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetVector), Time.deltaTime * 1.3f* moveSpeed);
            transform.rotation = look;
        }

        if (transform.position != finishPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPos, Time.deltaTime * 1.0f * moveSpeed);
        }
        else if (transform.position == finishPos && forward){

            if(targetPoint != targetPointsTotal)
            {   
                    targetPoint++;

                finishPos = _WalkPath.getNextPoint(w, targetPoint);
            }
            else if(targetPoint == targetPointsTotal)
            {
                if(loop)
                {
                    finishPos = _WalkPath.getStartPoint(w);

                        targetPoint = 0;
                }

                else
                {
                    _WalkPath.SpawnOnePeople(w, forward, moveSpeed);
                    Destroy(gameObject);
                }
            }

        }

        else if (transform.position == finishPos && !forward){

            if(targetPoint > 0)
            {   
                    targetPoint--;

                finishPos = _WalkPath.getNextPoint(w, targetPoint);
            }
            else if(targetPoint == 0)
            {
                if(loop)
                {
                    finishPos = _WalkPath.getNextPoint(w, targetPointsTotal);

                        targetPoint = targetPointsTotal;
                }

                else
                {
                    _WalkPath.SpawnOnePeople(w, forward, moveSpeed);
                    Destroy(gameObject);
                }
            }

        }

    }

}

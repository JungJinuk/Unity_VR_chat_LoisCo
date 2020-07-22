using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class WalkPath : MonoBehaviour {

    public GameObject[] peoplePrefabs;

    [HideInInspector]
    public List<Vector3> pathPoint = new List<Vector3>();
    [HideInInspector]
    public List<GameObject> pathPointTransform = new List<GameObject>();

    [HideInInspector] 
    public Vector3[,] points;

    [HideInInspector]
    public List<Vector3> CalcPoint = new List<Vector3>(); 

    public int numberOfWays;

    [HideInInspector]
    public int[] pointLength = new int[10];
    
    //[HideInInspector]
    //public int[] pointsTotal = new int[10000];

    public float lineSpacing;
    
    public float density;

    public bool loopPath;

    public enum EnumDir{Forward, Backward, HugLeft, HugRight, WeaveLeft, WeaveRight};
    public EnumDir direction;

    public enum EnumMove{Walk, Run};
    public EnumMove _moveType;
    public float moveSpeed = 1;

    [HideInInspector]
    public bool[] _forward;
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public GameObject par;

    public Vector3 getNextPoint(int w, int index)
    {
        return points[w, index];
    }

    public Vector3 getStartPoint(int w)
    {

        return points[w, 1];

    }

    public int getPointsTotal(int w)
    {

        return pointLength[w];

    }


    void Awake()
    {
        DrawCurved(false);
    }

    public void OnDrawGizmos()
    {
        DrawCurved(true);
    }

    public void DrawCurved(bool withDraw)
    {
        if(numberOfWays < 1) numberOfWays = 1;
        if(lineSpacing < 0.6f) lineSpacing = 0.6f;
        if(density < 0.1f) density = 0.1f;
        if(moveSpeed < 0.1f) moveSpeed = 0.1f;

        _forward = new bool[numberOfWays];

        isWalk = (_moveType.ToString() == "Walk") ? true : false;

        for(int w = 0; w < numberOfWays; w++)
        {

                if(direction.ToString() == "Forward")
                {
                    _forward[w] = true;
                }

                else if(direction.ToString() == "Backward")
                {
                    _forward[w] = false;
                }

                else if(direction.ToString() == "HugLeft")
                {
                    if((w + 2) % 2 == 0)
                        _forward[w] = true;
                    else
                        _forward[w] = false;
                }

                else if(direction.ToString() == "HugRight")
                {
                    if((w + 2) % 2 == 0)
                        _forward[w] = false;
                    else
                        _forward[w] = true;
                }

                else if(direction.ToString() == "WeaveLeft")
                {
                    if(w == 1 || w == 2 || (w - 1) % 4 == 0 || (w - 2) % 4 == 0)
                        _forward[w] = false;
                    else _forward[w] = true;
                }

                else if(direction.ToString() == "WeaveRight")
                {
                    if(w == 1 || w == 2 || (w - 1) % 4 == 0 || (w - 2) % 4 == 0)
                        _forward[w] = true;
                    else _forward[w] = false; 
                }

        }

        if (pathPoint.Count < 2) return;                
            points = new Vector3[numberOfWays + 2, pathPoint.Count + 2];

            pointLength[0] = pathPoint.Count + 2;           
           
            for (int i = 0; i < pathPoint.Count; i++)
            {
                points[0, i + 1] = pathPointTransform[i].transform.position;
            }

            points[0, 0] = points[0, 1];
            points[0, pointLength[0] - 1] = points[0, pointLength[0] - 2];
            
            for(int i = 0; i < pointLength[0]; i++)
                if(i != 0)
                {
                    if(withDraw)
                    { 
                        Gizmos.color = (_forward[0] ? Color.green : Color.red);
                        Gizmos.DrawLine(points[0, i], points[0, i - 1]);
                    }
                }
                if(loopPath)
                    if(withDraw)
                    {
                        Gizmos.color = (_forward[0] ? Color.green : Color.red);
                        Gizmos.DrawLine(points[0, 1], points[0, pointLength[0] - 2]);
                    }

            for(int w = 1; w < numberOfWays; w++)
            {   

            if(numberOfWays > 1)
            {   
                if(!loopPath)
                {
                    Vector3 vectorStart = points[0, 2] - points[0, 1];
                    Vector3 pointVectorStart = vectorStart;
                    pointVectorStart = Quaternion.Euler(0, -90, 0) * pointVectorStart;

                        if(w % 2 == 0)
                            pointVectorStart = pointVectorStart.normalized * (float) (w * 0.5f * lineSpacing);
                        else if(w % 2 == 1)
                            pointVectorStart = pointVectorStart.normalized * (float) ((w+1) * 0.5f * lineSpacing);

                    Vector3 pointStart1 = Vector3.zero;
                    if(w % 2 == 1)
                        pointStart1 = (points[0, 1] - pointVectorStart);
                    else if(w % 2 == 0)
                        pointStart1 = (points[0, 1] + pointVectorStart);
                
                    pointStart1.y = points[0, 1].y;

                    points[w, 0] = pointStart1;
                    points[w, 1] = pointStart1;
            

                    Vector3 vectorFinish = points[0, pointLength[0] - 3] - points[0, pointLength[0] - 2];
                    Vector3 pointVectorFinish = vectorFinish;
                    pointVectorFinish = Quaternion.Euler(0, 90, 0) * pointVectorFinish;

                        if(w % 2 == 0)
                            pointVectorFinish = pointVectorFinish.normalized * (float) (w * 0.5f * lineSpacing);
                        else if(w % 2 == 1)
                            pointVectorFinish = pointVectorFinish.normalized * (float) ((w+1) * 0.5f * lineSpacing);

                    Vector3 pointFinish1 = Vector3.zero;

                    if(w % 2 == 1)
                        pointFinish1 = points[0, pointLength[0] - 2] - pointVectorFinish;
                    else if(w % 2 == 0)
                        pointFinish1 = points[0, pointLength[0] - 2] + pointVectorFinish;

                    pointFinish1.y = points[0, pointLength[0] - 2].y;

                    points[w, pointLength[0] - 2] = pointFinish1;
                    points[w, pointLength[0] - 1] = pointFinish1;
                }

                else
                {
                    Vector3 vectorNext = points[0, pointLength[0] - 2] - points[0, 1];
                    Vector3 vectorPrev = points[0, 1] - points[0, 2];

                    Vector3 pointVector1 = vectorPrev;
                    Vector3 pointVector2 = vectorNext;

                    float angle = Mathf.DeltaAngle(Mathf.Atan2(pointVector1.x, pointVector1.z) * Mathf.Rad2Deg,
                            Mathf.Atan2(pointVector2.x, pointVector2.z) * Mathf.Rad2Deg);

                    if(w % 2 == 0)
                        pointVector1 = pointVector1.normalized * (float) (w * 0.5f * lineSpacing);
                    else if(w % 2 == 1)
                        pointVector1 = pointVector1.normalized * (float) ((w+1) * 0.5f * lineSpacing);

                    pointVector1 = Quaternion.Euler(0, 90 + angle / 2, 0) * pointVector1;

                    Vector3 point1 = Vector3.zero;

                    if(w % 2 == 1)
                    {
                        point1 = points[0, 1] - pointVector1;
                    }
                    else if(w % 2 == 0)
                        point1 = points[0, 1] + pointVector1;


                    point1.y = points[0, 1].y;

                    points[w, 1] = point1;
                    points[w, 0] = point1;

                    Vector3 vectorNext1 = points[0, pointLength[0] - 2] - points[0, 1];
                    Vector3 vectorPrev1 = points[0, pointLength[0] - 3] - points[0, pointLength[0] - 2];

                    Vector3 pointVector11 = vectorPrev1;
                    Vector3 pointVector22 = vectorNext1;

                    float angle2 = Mathf.DeltaAngle(Mathf.Atan2(pointVector11.x, pointVector11.z) * Mathf.Rad2Deg,
                            Mathf.Atan2(pointVector22.x, pointVector22.z) * Mathf.Rad2Deg);

                    if(w % 2 == 0)
                        pointVector11 = pointVector11.normalized * (float) (w * 0.5f * lineSpacing);
                    else if(w % 2 == 1)
                        pointVector11 = pointVector11.normalized * (float) ((w+1) * 0.5f * lineSpacing);

                    pointVector11 = Quaternion.Euler(0, 90 + angle2 / 2, 0) * pointVector11;

                    Vector3 point11 = Vector3.zero;
                    if(w % 2 == 1)
                    {
                        point11 = points[0, pointLength[0] - 2] - pointVector11;
                    }
                    else if(w % 2 == 0)
                        point11 = points[0, pointLength[0] - 2] + pointVector11;


                    point11.y = points[0, pointLength[0] - 2].y;

                    points[w, pointLength[0] - 2] = point11;
                    points[w, pointLength[0] - 1] = point11;
                }

                for(int i = 2; i < pointLength[0] -2; i++)
                {
                    Vector3 vectorNext = points[0, i] - points[0, i+1];
                    Vector3 vectorPrev = points[0, i-1] - points[0, i];

                    Vector3 pointVector1 = vectorPrev;
                    Vector3 pointVector2 = vectorNext;

                    float angle = Mathf.DeltaAngle(Mathf.Atan2(pointVector1.x, pointVector1.z) * Mathf.Rad2Deg,
                            Mathf.Atan2(pointVector2.x, pointVector2.z) * Mathf.Rad2Deg);

                    if(w % 2 == 0)
                        pointVector1 = pointVector1.normalized * (float) (w * 0.5f * lineSpacing);
                    else if(w % 2 == 1)
                        pointVector1 = pointVector1.normalized * (float) ((w+1) * 0.5f * lineSpacing);

                    pointVector1 = Quaternion.Euler(0, 90 + angle / 2, 0) * pointVector1;

                    Vector3 point1 = Vector3.zero;
                    if(w % 2 == 1)
                    {
                        point1 = points[0, i] - pointVector1;
                    }
                    else if(w % 2 == 0)
                        point1 = points[0, i] + pointVector1;


                    point1.y = points[0, i].y;

                    points[w, i] = point1;

                    if(withDraw)
                    {
                        Gizmos.color = (_forward[w] ? Color.green : Color.red);
                        Gizmos.DrawLine(points[w, i-1], points[w, i]);
                    }
                }

                if(withDraw)
                {
                    Gizmos.color = (_forward[w] ? Color.green : Color.red);
                    Gizmos.DrawLine(points[w, pointLength[0] - 2], points[w, pointLength[0] - 3]);
                }

                if(withDraw && loopPath)  
                {
                    Gizmos.color = (_forward[w] ? Color.green : Color.red);
                    Gizmos.DrawLine(points[w, 1], points[w, pointLength[0] - 2]);
                }
            }
        }
    }
    public void SpawnOnePeople(int w, bool forward, float mSpeed)
    {
        int prefabNum = UnityEngine.Random.Range(0, peoplePrefabs.Length);
        var people = gameObject;

        if(!forward)
            people = Instantiate(peoplePrefabs[prefabNum], points[w, pointLength[0] - 2], Quaternion.identity) as GameObject;
        else
            people = Instantiate(peoplePrefabs[prefabNum], points[w, 1], Quaternion.identity) as GameObject;
        var _movePath = people.AddComponent<MovePath>();

        people.transform.parent = par.transform;
        _movePath.walkPath = gameObject;

        string animName;
        if(isWalk)
            animName = "walk";
        else
            animName = "run";

        if(!forward)
        {
            _movePath.MyStart(w, pointLength[0] - 2, animName, loopPath, forward, mSpeed);
            people.transform.LookAt(points[w, pointLength[0] - 3]);
        }
        else
        {
            _movePath.MyStart(w, 1, animName, loopPath, forward, mSpeed);
            people.transform.LookAt(points[w, 2]);
        }
    }

    public void SpawnPeople()
    {   

        if(par == null)
        {
            par = new GameObject();
            par.transform.parent = gameObject.transform;
            par.name = "people";
        }

        int s;
        if(!loopPath)
            s = pointLength[0] - 2;
        else
            s = pointLength[0] - 1;

            for(int b = 0; b < numberOfWays; b++)
            {   

                float mSpeed = moveSpeed + UnityEngine.Random.Range(moveSpeed * -0.15f, moveSpeed * 0.15f);

        for(int a = 1; a < s; a++)
        {
                bool forward_ = false;
                if(direction.ToString() == "Forward")
                {
                    forward_ = true;
                }

                else if(direction.ToString() == "Backward")
                {
                    forward_ = false;
                }

                else if(direction.ToString() == "HugLeft")
                {
                    if((b + 2) % 2 == 0)
                        forward_ = true;
                    else
                        forward_ = false;
                }

                else if(direction.ToString() == "HugRight")
                {
                    if((b + 2) % 2 == 0)
                        forward_ = false;
                    else
                        forward_ = true;
                }

                else if(direction.ToString() == "WeaveLeft")
                {
                    if(b == 1 || b == 2 || (b - 1) % 4 == 0 || (b - 2) % 4 == 0)
                        forward_ = false;
                    else forward_ = true;
                }

                else if(direction.ToString() == "WeaveRight")
                {
                    if(b == 1 || b == 2 || (b - 1) % 4 == 0 || (b - 2) % 4 == 0)
                        forward_ = true;
                    else forward_ = false; 
                }

                Vector3 myVector = Vector3.zero;
                if(loopPath && a == s-1)
                    myVector = points[b, 1] - points[b, s];
                else
                    myVector = points[b, a + 1] - points[b, a];
                    
                float myVectorMagnitude = myVector.magnitude;
                int peopleCount = (int) ((density / 5.0f) * myVectorMagnitude);

                if(peopleCount < 1) peopleCount = 1;

                float step = myVectorMagnitude / peopleCount;
                float totalStep = 0;

                for(int i = 0; i < peopleCount; i++)
                {


                    float randomPos = 0;
                    if(i == 0)
                        randomPos = UnityEngine.Random.Range(totalStep, step - 0.5f);
                    else 
                        randomPos = UnityEngine.Random.Range(totalStep, ((i+1) * step) - 0.5f);
                    totalStep = randomPos + 0.5f;

                    int prefabNum = UnityEngine.Random.Range(0, peoplePrefabs.Length);
                    var people = gameObject;

                    if(forward_)
                        people = Instantiate(peoplePrefabs[prefabNum], points[b, a] + myVector * (randomPos / myVectorMagnitude), Quaternion.identity) as GameObject;
                    else
                        people = Instantiate(peoplePrefabs[prefabNum], points[b, a] + myVector * (randomPos / myVectorMagnitude), Quaternion.identity) as GameObject;
                    var _movePath = people.AddComponent<MovePath>();

                    //curCount++;

                    people.transform.parent = par.transform;
                    _movePath.walkPath = gameObject;

                    string animName;
                    if(isWalk)
                        animName = "walk";
                    else
                        animName = "run";

                    _movePath.MyStart(b, a, animName, loopPath, forward_, mSpeed);

                    //if(curCount >= maxCount) return;
                }
            }
        }
    }
}

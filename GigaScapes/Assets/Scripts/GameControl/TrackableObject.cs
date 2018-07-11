using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackableObject : MonoBehaviour {

    public DetectorField ParticleField;

    public int DeltaThreshold = 10;
    public float DeltaRatio = 0.5f;

    public float Size = 1f;
    private float Halfsize = 0.5f;

    public float KillSize = 0.33f;

    public float speed = 0.2f;

    public bool MoveToPosition = true;
    public bool MoveToNormalVelocity = false;

    public Vector3 posAvg = Vector3.zero;
    public Vector3 negAvg = Vector3.zero;
    public Vector3 AccAvg = Vector3.zero;

    public Vector3 Velocity = Vector3.zero; //normalized
    private Vector3 Velocity0 = Vector3.zero;

    public Vector3 TargetPos = Vector3.zero;
    public Vector3 AccuratePos = Vector3.zero;

    public Vector3 ApparentAccel = Vector3.zero;

    public bool Managed = false;

    private Dictionary<int[], Detector> LocalKillList = new Dictionary<int[], Detector>();

    //public Vector3 Garbagge;

    private void Start()
    {
        //Garbagge = ParticleField.FromLocalToWorld(Vector3.one);

        //Debug.Log(ParticleField.FromLocalToWorld(Vector3.one).x.ToString());
    }

    void Update() {
        if (!Managed)
        {
            CalculatePos();
            VectorDebug();

            if (MoveToPosition)
            {
                MoveToPos2();
            }
            else if (MoveToNormalVelocity)
            {
                MoveOnConstVelocity();
            }
        }
    }

    public Dictionary<int[], Detector> ManagedUpdate(Dictionary<int[], Detector> killList)
    {
        if (Managed)
        {
            CalculatePosKILL(killList);
            VectorDebug();

            if (MoveToPosition)
            {
                MoveToPos2();
            }
            else if (MoveToNormalVelocity)
            {
                MoveOnConstVelocity();
            }

            foreach (KeyValuePair<int[], Detector> kvp in ParticleField.On)
            {
                if (!killList.ContainsKey(kvp.Key))
                {

                    if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
                    {
                        if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                        {
                            killList.Add(kvp.Key,kvp.Value);
                        }
                    }
                }
            }

            return killList;
        }
        else
        {

            return new Dictionary<int[], Detector>();
        }
    }
    

    void CalculatePos()
    {
        Vector3 localPos = ParticleField.FromWorldToLocal(transform.position);

        //Vector3 localPos = transform.position - ParticleField.transform.position;

        //for(int x = (int)(localPos.x - Halfsize); x < (int)(localPos.x - Halfsize))

        transform.localScale = Size * Vector3.one;
        Halfsize = Size / 2f;

        int posCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.PosDelta)
        {
            if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
            {
                if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                {
                    posAvg += kvp.Value.transform.localPosition;
                    posCount++;
                }
            }
        }
        if (posCount > 0)
        {
            posAvg = posAvg / (float)posCount;
        }

        int negCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.PosDelta)
        {
            if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
            {
                if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                {
                    negAvg += kvp.Value.transform.localPosition;
                    negCount++;
                }
            }
        }
        if (negCount > 0)
        {
            negAvg = negAvg / (float)negCount;
        }

        int AccCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.On)
        {
            if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
            {
                if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                {
                    AccAvg += kvp.Value.transform.localPosition;
                    AccCount++;
                }
            }
        }
        if (AccCount > 0)
        {
            AccAvg = AccAvg / (float)AccCount;
        }

        if(AccCount > DeltaThreshold*2)
        {
            AccuratePos = ParticleField.FromLocalToWorld(AccAvg);
        }

        ///number of points good enough
        if (posCount >= DeltaThreshold && negCount >= DeltaThreshold)
        {
            // ratio of pos delta to neg delta is within bounds
            if ((float)posCount / (float)negCount > DeltaRatio && (float)posCount / (float)negCount <= 1 ||
                (float)negCount / (float)posCount > DeltaRatio && (float)negCount / (float)posCount <= 1)
            {
                TargetPos = ParticleField.FromLocalToWorld((posAvg + negAvg) / (float)2);
               
                Velocity = ParticleField.FromLocalToWDirection(posAvg - negAvg);
                Velocity = Velocity.normalized;
                ApparentAccel = Velocity - Velocity0;
                Velocity0 = Velocity;
            }
        }
        else
        {
            TargetPos = transform.position;
            Velocity = Vector3.zero;
            ApparentAccel = Velocity - Velocity0;
            Velocity0 = Velocity;
        }
    }

    void CalculatePosKILL(Dictionary<int[], Detector> killList)
    {
        Vector3 localPos = ParticleField.FromWorldToLocal(transform.position);

        //Vector3 localPos = transform.position - ParticleField.transform.position;

        //for(int x = (int)(localPos.x - Size); x < (int)(localPos.x - Size))

        transform.localScale = Size * Vector3.one;
        Halfsize = Size / 2f;

        int posCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.PosDelta)
        {
            if (!killList.ContainsKey(kvp.Key))
            {

                if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
                {
                    if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                    {
                        posAvg += kvp.Value.transform.localPosition;
                        posCount++;
                    }
                }
            }
        }
        if (posCount > 0)
        {
            posAvg = posAvg / (float)posCount;
        }

        int negCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.PosDelta)
        {
            if (!killList.ContainsKey(kvp.Key))
            {
                if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
                {
                    if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                    {
                        negAvg += kvp.Value.transform.localPosition;
                        negCount++;
                    }
                }
            }
        }
        if (negCount > 0)
        {
            negAvg = negAvg / (float)negCount;
        }

        int AccCount = 0;
        foreach (KeyValuePair<int[], Detector> kvp in ParticleField.On)
        {
            if (!killList.ContainsKey(kvp.Key))
            {
                if (kvp.Value.transform.position.x > (transform.position.x - Halfsize) && kvp.Value.transform.position.x < (transform.position.x + Halfsize))
                {
                    if (kvp.Value.transform.position.z > (transform.position.z - Halfsize) && kvp.Value.transform.position.z < (transform.position.z + Halfsize))
                    {
                        AccAvg += kvp.Value.transform.localPosition;
                        AccCount++;
                    }
                }
            }
        }
        if (AccCount > 0)
        {
            AccAvg = AccAvg / (float)AccCount;
        }

        if (AccCount > DeltaThreshold * 2)
        {
            AccuratePos = ParticleField.FromLocalToWorld(AccAvg);
        }

        ///number of points good enough
        if (posCount >= DeltaThreshold && negCount >= DeltaThreshold)
        {
            // ratio of pos delta to neg delta is within bounds
            if ((float)posCount / (float)negCount > DeltaRatio && (float)posCount / (float)negCount <= 1 ||
                (float)negCount / (float)posCount > DeltaRatio && (float)negCount / (float)posCount <= 1)
            {
                TargetPos = ParticleField.FromLocalToWorld((posAvg + negAvg) / (float)2);

                Velocity = ParticleField.FromLocalToWDirection(posAvg - negAvg);
                Velocity = Velocity.normalized;
                ApparentAccel = Velocity - Velocity0;
                Velocity0 = Velocity;
            }
        }
        else
        {
            TargetPos = transform.position;
            Velocity = Vector3.zero;
            ApparentAccel = Velocity - Velocity0;
            Velocity0 = Velocity;
        }
    }

    void VectorDebug()
    {
        Debug.DrawLine(TargetPos, TargetPos + Vector3.up, Color.yellow);
        Debug.DrawLine(transform.position, TargetPos, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + Velocity * 10, Color.green);
    }

    void MoveToPos()
    {
        if (TargetPos != Vector3.zero)
        {
            transform.position = TargetPos;
        }
    }
    void MoveToPos2()
    {
        if (AccuratePos != Vector3.zero)
        {
            transform.position = AccuratePos;
        }
    }

    void MoveOnConstVelocity()
    {
        transform.position += Velocity * speed;
    }
}

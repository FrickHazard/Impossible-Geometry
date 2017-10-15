using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpossibleSegementType {
    Spacer,
    Caster,
};


public class ImpossibleSegement {

    public Vector3 Start;

    public Vector3 End;

    public ImpossibleSegement StartSegment = null;

    public ImpossibleSegement EndSegment = null;

    public ImpossibleSegement(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public void CastInterection(Camera camera)
    {
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpossibleSegementType {
    Spacer,
    Caster,
    Eater,
};


public class ImpossibleSegment {

    public Vector3 Start;

    public Vector3 End;

    public Vector3 Normal;

    public ImpossibleSegment StartSegment = null;

    public ImpossibleSegment EndSegment = null;

    public ImpossibleSegementType SegmentType = ImpossibleSegementType.Caster;

    public ImpossibleSegment(Vector3 start, Vector3 end, Vector3 normal)
    {
        Normal = normal;
        Start = start;
        End = end;
    }

}

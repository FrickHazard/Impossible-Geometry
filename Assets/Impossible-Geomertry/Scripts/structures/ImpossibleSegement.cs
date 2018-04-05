using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpossibleSegmentType {
    Spacer,
    Caster,
    Eater,
};


public class ImpossibleSegment {

    public Vector3 Start;

    public Vector3 End;

    public Vector3 Normal;

    public ImpossibleSegmentType SegmentType;

    public ImpossibleSegment(Vector3 start, Vector3 end, Vector3 normal, ImpossibleSegmentType segmentType)
    {
        Normal = normal;
        Start = start;
        End = end;
        SegmentType = segmentType;
    }

}

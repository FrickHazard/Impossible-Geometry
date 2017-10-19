﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpossibleSegementType {
    Spacer,
    Caster,
};


public class ImpossibleSegment {

    public Vector3 Start;

    public Vector3 End;

    public ImpossibleSegment StartSegment = null;

    public ImpossibleSegment EndSegment = null;

    public ImpossibleSegementType SegmentType = ImpossibleSegementType.Caster;

    public ImpossibleSegment(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public void CastInterection(Camera camera)
    {
        
    }

}

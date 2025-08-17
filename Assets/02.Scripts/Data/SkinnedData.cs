using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class SkinnedData
{
    public string SharedMeshName;
    public Bounds Bounds;
    public List<string> Bones;
    public string RootBoneName;
}

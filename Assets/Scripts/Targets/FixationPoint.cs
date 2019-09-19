using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationPoint : ExperimentObject
{
    public MeshRenderer fixMat;
    private Shader unlit;
    private Shader specular;

    void Start()
    {
        fixMat = GetComponentInChildren<MeshRenderer>();
    }

    public override void ResetColor()
    {
        fixMat.material = default_mat;
    }

    public override void ApplyColor()
    {
        
        fixMat.material = (_color == Color.red) ? red_mat : 
                 (_color == Color.blue) ? blue_mat : 
                 (_color == Color.green) ? green_mat : default_mat;
    }
}

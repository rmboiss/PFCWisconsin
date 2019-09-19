using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : ExperimentObject
{
    private MeshRenderer bodyMat;

    public void Start ()
    {
        bodyMat = GetComponentInChildren<MeshRenderer>();
    }

    public override void ResetColor()
    {
        bodyMat.material = default_mat;
    }

    public override void ApplyColor()
    {
        bodyMat.material = (_color == Color.red) ? red_mat :
                 (_color == Color.blue) ? blue_mat :
                 (_color == Color.green) ? green_mat :
                 default_mat;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.Inputs
{
    /// <summary>
    /// command for selecting left mous button
    /// </summary>
    public interface IMouseClickCommand
    {
        void ExecuteMouseClick(Vector3 inputPosition);
    }
}


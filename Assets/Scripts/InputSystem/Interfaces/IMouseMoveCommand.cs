using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.Inputs
{
    /// <summary>
    /// command for when player moves the mouse
    /// </summary>
    public interface IMouseMoveCommand
    {
        void ExecuteMouseMove(Vector3 inputPosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceOrigin.DesignPatterns;

namespace SpaceOrigin.Inputs
{
    /// <summary>
    /// class for handling inputs, not generic
    /// </summary>
    public class InputHandler : Singleton<InputHandler>
    {
        public IMouseClickCommand mouseClickCommand;
        public IMouseMoveCommand mouseMoveCommand;

        private Vector3 _lastMousePosition;

        void Update()
        {
            Vector3 inputPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0) && mouseClickCommand != null)
            {
                mouseClickCommand.ExecuteMouseClick(inputPosition);
            }

            if (_lastMousePosition != inputPosition && mouseMoveCommand != null) 
            {
                mouseMoveCommand.ExecuteMouseMove(inputPosition);
            }

            _lastMousePosition = inputPosition;
        }
    }
}

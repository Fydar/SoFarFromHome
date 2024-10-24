﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{

    float horiz, vert;

    public static bool GetLeftMouse()
    {
        return Input.GetMouseButton(0);
    }

    public static bool GetLeftMouseDown ()
    {
        return Input.GetMouseButtonDown(0);
    }

    public static bool GetRightMouseDown ()
    {
        return Input.GetMouseButtonDown(1);
    }

    public static Vector2 GetMousePos ()
    {
        return Input.mousePosition;
    }

    public static float GetMouseX ()
    {
        return Input.GetAxisRaw("Mouse X");
    }

    public static float GetMouseY ()
    {
        return -Input.GetAxisRaw("Mouse Y");
    }

    public static void ShowMouse (bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    public static bool GetPauseButtonDown ()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}

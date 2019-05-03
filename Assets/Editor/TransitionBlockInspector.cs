using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transition))]
public class TransitionBlockInspector : Editor
{
    private bool toShow = true;
    private bool once = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Transition transition = (Transition)target;

        toShow = GUILayout.Toggle(toShow, "Show Player and Level");

        if (toShow && once)
        {
            transition.Show();
            once = false;
        }
        else if(toShow == false && once == false)
        {
            transition.Kill();
            once = true;
        }
        transition.modify();
        if (GUILayout.Button("Start at Right Or Up"))
            transition.Set(0);
        if (GUILayout.Button("Start at Left Or Down"))
            transition.Set(1);
    }

    public void OnDisable()
    {
        Transition transition = (Transition)target;
        transition.Kill();
    }
}

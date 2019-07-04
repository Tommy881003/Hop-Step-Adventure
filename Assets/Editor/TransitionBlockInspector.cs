using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Transition))]
public class TransitionBlockInspector : Editor
{
    private Vector2 LevelSize = new Vector2(40,22f);
    private bool showMessage = false;
    private int currentIndex;
    private PlayerControl player;
    private CameraHolder holder;

    public enum OPTIONS
    {
        Mid = 0,
        LeftDown = 1,
        LeftUp = 2,
        RightDown = 3,
        RightUp = 4
    }

    public OPTIONS opt;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        holder = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHolder>();
        Transition transition = (Transition)target;
        transition.spawnIndex = 0;
        if (transition.Spawn.Count() > 0)
            transition.SpawningPoint = transition.Spawn[0];
        else
            transition.SpawningPoint = Vector2.zero;
        currentIndex = 0;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Transition transition = (Transition)target;
        Vector2 BoxSize = transition.gameObject.GetComponent<BoxCollider2D>().size;

        if (BoxSize != transition.LevelSize)
            showMessage = false;

        opt = (OPTIONS)EditorGUILayout.EnumPopup("Anchor:", opt);

        int operation = (int)opt;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("ApplySize"))
        {
            if (LevelSize.x >= 40 && LevelSize.y >= 22f)
            {
                transition.apply(operation);
                showMessage = true;
            }
            else
            {
                if (EditorUtility.DisplayDialog("Anomaly Detected", "Either the height or width of the level is smaller than the acceptable value. (40 x 22)", "Reset to default"))
                    transition.ResetSize(BoxSize);
            }
        }

        if (GUILayout.Button("FixEdges"))
            transition.FixEdge();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Vector2 toSpawn = transition.SpawningPoint;
        if (GUILayout.Button("AddSpawn"))
        {
            if (Mathf.Abs(toSpawn.x) > (0.5f * LevelSize.x - 1) || Mathf.Abs(toSpawn.y) > (0.5f * LevelSize.y - 1))
                EditorUtility.DisplayDialog("Anomaly Detected", "Spawning point is too close to or over the edges.", "Ok");
            else
                transition.AddSpawn(Vector2.zero);
        }
        if (GUILayout.Button("RemoveSpawn"))
        {
            if (transition.Spawn.Count - 1 < transition.spawnIndex)
                EditorUtility.DisplayDialog("Anomaly Detected", "Invalid index input.", "Ok");
            else
            {
                transition.RemoveSpawn(transition.spawnIndex);
                transition.spawnIndex = 0;
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("ModifySpawn"))
        {
            if (transition.spawnIndex > transition.Spawn.Count - 1 || Mathf.Abs(toSpawn.x) > (0.5f * LevelSize.x - 1) || Mathf.Abs(toSpawn.y) > (0.5f * LevelSize.y - 1))
                EditorUtility.DisplayDialog("Anomaly Detected", "Spawning point is too close to or over the edges.", "Ok");
            else
                transition.ModifySpawn(transition.spawnIndex, toSpawn);
        }

        if(GUILayout.Button("StartHere"))
        {
            if(transition.spawnIndex > transition.Spawn.Count - 1)
                EditorUtility.DisplayDialog("Anomaly Detected", "Invalid index input.", "Ok");
            else
            {
                holder.corner = new Vector2(transition.transform.position.x - (0.5f * (transition.LevelSize.x - 40f)), transition.transform.position.y - (0.5f * (transition.LevelSize.y - 22)));
                holder.corner2 = new Vector2(transition.transform.position.x + (0.5f * (transition.LevelSize.x - 40f)), transition.transform.position.y + (0.5f * (transition.LevelSize.y - 22)));
                player.currentLevel = transition.LevelIndex;
                player.transform.position = new Vector3(transition.transform.position.x + transition.Spawn[transition.spawnIndex].x, transition.transform.position.y + transition.Spawn[transition.spawnIndex].y, player.transform.position.z);
                holder.transform.position = transition.CamPos(player.transform.position);
            }
        }

        if (showMessage)
        EditorGUILayout.HelpBox("Success!", MessageType.Info);

        if (currentIndex != transition.spawnIndex)
        {
            currentIndex = transition.spawnIndex;
            if(transition.spawnIndex <= transition.Spawn.Count() - 1)
                transition.SpawningPoint = transition.Spawn[currentIndex];
        }
        if (transition.spawnIndex > transition.Spawn.Count() - 1)
            EditorGUILayout.HelpBox("Invalid Spawn Index.", MessageType.Warning);
    }

    public void OnSceneGUI()
    {
        Transition transition = (Transition)target;
        Vector2 pos = transition.transform.position;
        LevelSize = transition.LevelSize;
        Vector3[] verts = new Vector3[]
        {
            new Vector3(pos.x - (0.5f * LevelSize.x), pos.y - (0.5f * LevelSize.y), -10),
            new Vector3(pos.x - (0.5f * LevelSize.x), pos.y + (0.5f * LevelSize.y), -10),
            new Vector3(pos.x + (0.5f * LevelSize.x), pos.y + (0.5f * LevelSize.y), -10),
            new Vector3(pos.x + (0.5f * LevelSize.x), pos.y - (0.5f * LevelSize.y), -10)
        };
        Color fill = new Color(1, 1, 1, 0.1f);
        Color outline = ((LevelSize.x >= 40 && LevelSize.y >= 22) ? Color.yellow : Color.red);
        Handles.DrawSolidRectangleWithOutline(verts, fill, outline);

        for(int i = 0; i < transition.Spawn.Count(); i ++)
        {
            Vector2 point = transition.Spawn[i];
            Vector3[] vert = new Vector3[]
            {
            new Vector3(pos.x + point.x + 0.7f, pos.y + point.y, -10),
            new Vector3(pos.x + point.x - 0.7f, pos.y + point.y, -10),
            new Vector3(pos.x + point.x - 0.7f, pos.y + point.y - 1, -10),
            new Vector3(pos.x + point.x + 0.7f, pos.y + point.y - 1, -10)
            };
            if(i == currentIndex)
                Handles.DrawSolidRectangleWithOutline(vert, new Color(0, 1, 0, 0.1f), Color.green);
            else
                Handles.DrawSolidRectangleWithOutline(vert, new Color(0, 0, 0, 0.2f), Color.black);
        }
        SceneView.RepaintAll();
    }

    public void OnDisable()
    {
        showMessage = false;
        Transition transition = (Transition)target;
        LevelSize = transition.LevelSize;
        Vector2 BoxSize = transition.gameObject.GetComponent<BoxCollider2D>().size;
        if(BoxSize != LevelSize)
        {
            if(LevelSize.x >= 40 && LevelSize.y >= 22)
            {
                if (EditorUtility.DisplayDialog("Anomaly Detected", "You haven't applied the changes yet.", "Apply changes"))
                    transition.apply(0);
            }
            else
            {
                if (EditorUtility.DisplayDialog("Anomaly Detected", "You haven't applied the changes yet, and the sizes are smaller than the acceptable values. (40 x 22)", "Apply default settings"))
                {
                    transition.ResetSize(BoxSize);
                    transition.apply(0);
                }
            }
        }
    }
}

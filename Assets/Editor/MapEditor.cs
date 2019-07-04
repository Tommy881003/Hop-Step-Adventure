using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    GameObject[] prefabs;
    GameObject selectedPrefab;
    GameObject followCursor;
    SpriteRenderer CursorSprite;
    string[] dropdown;
    int length,index,rotationDegree = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Map map = (Map)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            int i = map.haveEmpty();
            GameObject newLevel = Instantiate(map.levelPrefab, Vector3.zero, Quaternion.identity);
            if(i >= 0)
            {
                newLevel.name = "Level" + i;
                map.currentLevel = i;
            }
            int a = map.addLevel(newLevel, i);
            map.levelParent = newLevel.transform.Find("LevelCarrier").gameObject;
            newLevel.GetComponent<Transition>().LevelIndex = a;
            if(i < 0)
            {
                newLevel.name = "Level" + a;
                map.currentLevel = a;
            }
        }
        if(GUILayout.Button("Remove"))
        {
            int i = map.currentLevel;
            if(EditorUtility.DisplayDialog("Remove Level in graph?", "Do you want to remove Level" + i + " in graph ?", "Yes", "No"))
            {
                if (map.checkValid(i) == false)
                    EditorUtility.DisplayDialog("Anomaly detected", "Invalid operation.", "Ok");
                else
                {
                    if (EditorUtility.DisplayDialog("Remove Level?", "Do you also want to remove Level" + i + " ITSELF ? (Irreversible)", "Yes", "No"))
                        map.removeLevel(i, true);
                    else
                        map.removeLevel(i, false);
                    if (map.levels.Count() >= 0)
                    {
                        int a = 0;
                        while (map.levels[a] == null)
                            a++;
                        map.levelParent = map.levels[a].transform.Find("LevelCarrier").gameObject;
                        map.currentLevel = a;
                    }
                    else
                    {
                        map.levelParent = null;
                        map.currentLevel = -1;
                    }
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delink"))
        {
            Vector2Int ab = map.toLink;
            if (map.checkValid(ab.x, ab.y) == false)
                EditorUtility.DisplayDialog("Anomaly detected", "Invalid operation.", "Ok");
            else
                map.removeLink(ab.x, ab.y);
        }
        if (GUILayout.Button("Link"))
        {
            Vector2Int ab = map.toLink;
            if (map.checkValid(ab.x, ab.y) == false)
                EditorUtility.DisplayDialog("Anomaly detected", "Invalid operation.", "Ok");
            else
                map.buildLink(ab.x, ab.y);
        }
        GUILayout.EndHorizontal();
        if(map.currentLevel < 0 || map.currentLevel >= map.levels.Count())
            EditorGUILayout.HelpBox("You have not set the valid level yet.", MessageType.Warning);
        else if(map.levels[map.currentLevel] != null)
            map.levelParent = map.levels[map.currentLevel].transform.Find("LevelCarrier").gameObject;
        else
            EditorGUILayout.HelpBox("The current level index does not exist yet.", MessageType.Warning);
    }

    private void OnEnable()
    {
        //Load all prefabs as objects from the 'Prefabs' folder
        Object[] obj = Resources.LoadAll("Prefabs", typeof(GameObject));

        //initialize the game object array
        prefabs = new GameObject[obj.Length];
        dropdown = new string[obj.Length + 1];
        dropdown[0] = "None";

        //store the game objects in the array
        length = 0;
        for (int i = 0; i < obj.Length; i++)
        {
            prefabs[i] = (GameObject)obj[i];
            dropdown[i + 1] = obj[i].name;
            length++;
        }

        Map map = (Map)target;
        followCursor = map.followCursor;
        if (followCursor != null)
        {
            CursorSprite = followCursor.GetComponent<SpriteRenderer>();
            CursorSprite.color = new Color(1, 1, 1, 0.5f);
        }
        if (map.levels.Count() > 0)
        {
            int a = 0;
            while (map.levels[a] == null)
                a++;
            map.levelParent = map.levels[a].transform.Find("LevelCarrier").gameObject;
            map.currentLevel = a;
        }
        else
        {
            map.levelParent = null;
            map.currentLevel = -1;
        }
    }

    void OnSceneGUI()
    {
        Map map = (Map)target;

        bool canOperate = !(map.currentLevel < 0 || map.currentLevel >= map.levels.Count() || map.levels[map.currentLevel] == null);
        if (canOperate)
        {
            drawRect(map.levels[map.currentLevel], Color.yellow);
            List<int> toRect = map.levels[map.currentLevel].GetComponent<Transition>().link;
            foreach (int i in toRect)
                drawRect(map.levels[i], Color.blue);
        }

        GUILayout.BeginArea(new Rect(10, 10, 400, 200));

        index = EditorGUILayout.Popup(index, dropdown);
        if (prefabs != null && index > 0)
        {
            selectedPrefab = prefabs[index - 1];
        }
        else
            selectedPrefab = null;

        GUILayout.BeginHorizontal();
        GUILayout.Box("Map Edit Mode");
        
        if (selectedPrefab != null && selectedPrefab.GetComponent<Interactable>() != null)
        {
            selectedPrefab.GetComponent<Interactable>().level = map.currentLevel;
            GUILayout.Box("Level Index : " + map.currentLevel.ToString());
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (selectedPrefab != null && selectedPrefab.GetComponent<Rotatable>() != null)
        {
            GUILayout.Box("Degree: " + rotationDegree);
        }
        else
            rotationDegree = 0;
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        Vector3 spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        Vector2 gridPosition = new Vector2(Mathf.Floor(spawnPosition.x) + 0.5f, Mathf.Floor(spawnPosition.y) + 0.5f);
        
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z)
            rotationDegree = (rotationDegree + 270) % 360;

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
            rotationDegree = (rotationDegree + 90) % 360;

        //if LMB pressed, spawn the selected prefab
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && selectedPrefab != null)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(gridPosition, Vector2.zero);
            RaycastHit2D within = Physics2D.Raycast(gridPosition, Vector2.zero, 100, (1 << 2));
            if (canOperate == false)
                EditorUtility.DisplayDialog("Anomaly detected", "Invalid level index.", "Ok");
            else if ((hitInfo.collider == null || (hitInfo.collider.GetComponent<Ground>() == null && hitInfo.collider.name != selectedPrefab.name)) && within.collider.gameObject == map.levels[map.currentLevel])
            {
                Spawn(gridPosition,map);
                if (selectedPrefab.GetComponent<Multisprites>() != null)
                {
                    for (int y = 1; y >= -1; y--)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            Vector2 newpos = new Vector2(gridPosition.x + x, gridPosition.y + y);
                            RaycastHit2D[] hits = Physics2D.RaycastAll(newpos, Vector2.zero);
                            foreach(RaycastHit2D hit in hits)
                            {
                                if (hit.collider != null && hit.collider.gameObject.GetComponent<Multisprites>() != null)
                                {
                                    string tileName = hit.collider.gameObject.name;
                                    hit.collider.GetComponent<SpritePicker>().PickSprite(tileName);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            index = 0;
            selectedPrefab = null;
        }

        //if X pressed, delete the gameobject on the choosen coordinate (if such object exist) 
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(gridPosition, Vector2.zero);
            RaycastHit2D within = Physics2D.Raycast(gridPosition, Vector2.zero, 100, (1 << 2));
            if (canOperate == false)
                EditorUtility.DisplayDialog("Anomaly detected", "Invalid level index.", "Ok");
            else if (hitInfo.collider != null && within.collider.gameObject == map.levels[map.currentLevel])
            {
                string temptag = hitInfo.collider.gameObject.tag;
                DestroyImmediate(hitInfo.collider.gameObject);
                if (temptag == "Multisprites")
                {
                    for (int y = 1; y >= -1; y--)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            Vector2 newpos = new Vector2(gridPosition.x + x, gridPosition.y + y);
                            RaycastHit2D[] hits = Physics2D.RaycastAll(newpos, Vector2.zero);
                            foreach(RaycastHit2D hit in hits)
                            {
                                if (hit.collider != null && hit.collider.gameObject.GetComponent<Multisprites>() != null)
                                {
                                    string tileName = hit.collider.gameObject.name;
                                    hit.collider.GetComponent<SpritePicker>().PickSprite(tileName);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (followCursor != null)
        {
            if (selectedPrefab != null)
                CursorSprite.sprite = selectedPrefab.GetComponent<SpriteRenderer>().sprite;
            else
                CursorSprite.sprite = null;

            followCursor.transform.position = gridPosition;
            followCursor.transform.rotation = Quaternion.Euler(0, 0, rotationDegree);
        }

        SceneView.RepaintAll();
    }

    void Spawn(Vector2 _spawnPosition,Map input)
    {
        GameObject go = (GameObject)Instantiate(selectedPrefab, new Vector3(_spawnPosition.x, _spawnPosition.y, 0), Quaternion.Euler(0,0,rotationDegree));
        go.name = selectedPrefab.name;
        if (input.levelParent != null && go.name != "Player" && go.name != "TransitionBlock")
            go.transform.SetParent(input.levelParent.transform);
    }

    void drawRect(GameObject box, Color outline)
    {
        Vector2 pos = box.transform.position;
        Vector2 LevelSize = box.GetComponent<Transition>().LevelSize;
        Vector3[] verts = new Vector3[]
        {
            new Vector3(pos.x - (0.5f * LevelSize.x), pos.y - (0.5f * LevelSize.y), -10),
            new Vector3(pos.x - (0.5f * LevelSize.x), pos.y + (0.5f * LevelSize.y), -10),
            new Vector3(pos.x + (0.5f * LevelSize.x), pos.y + (0.5f * LevelSize.y), -10),
            new Vector3(pos.x + (0.5f * LevelSize.x), pos.y - (0.5f * LevelSize.y), -10)
        };
        Color fill = new Color(1, 1, 1, ((outline == Color.yellow)? 0.1f : 0));
        Handles.DrawSolidRectangleWithOutline(verts, fill, outline);
    }
}
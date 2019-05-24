using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    GameObject[] prefabs;
    GameObject selectedPrefab;
    string[] dropdown = new string[] { "All", "BinaryFactory" };
    int length,index,rotationDegree = 0;

    void OnSceneGUI()
    {
        Map map = (Map)target;
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));

        //Load all prefabs as objects from the 'Prefabs' folder
        Object[] obj = Resources.LoadAll("Prefabs", typeof(GameObject));
        

        //initialize the game object array
        prefabs = new GameObject[obj.Length];
        index = EditorGUILayout.Popup(index, dropdown);

        //store the game objects in the array
        length = 0;
        for (int i = 0; i < obj.Length; i++)
        {
            switch(index)
            {
                case 0:
                    prefabs[i] = (GameObject)obj[i];
                    length++;
                    break;
                case 1:
                    if(((GameObject)obj[i]).GetComponent<BinaryFactory>() != null)
                    {
                        prefabs[length] = (GameObject)obj[i];
                        length++;
                    }
                    break;
                default:
                    Debug.Log("Unrecognized Option");
                    break;
            }
        }

        GUILayout.BeginHorizontal();

        if (prefabs != null)
        {
            int elementsInThisRow = 0;
            for (int i = 0; i < length; i++)
            {
                elementsInThisRow++;

                //get the texture from the prefabs
                Texture prefabTexture = prefabs[i].GetComponent<SpriteRenderer>().sprite.texture;

                //create one button for earch prefabs 
                //if a button is clicked, select that prefab and focus on the scene view
                if (GUILayout.Button(prefabTexture, GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
                {
                    selectedPrefab = prefabs[i];
                    EditorWindow.FocusWindowIfItsOpen<SceneView>();
                }

                //move to next row after creating a certain number of buttons so it doesn't overflow horizontally
                if (elementsInThisRow > Screen.width / 100)
                {
                    elementsInThisRow = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Box("Map Edit Mode");
        if (selectedPrefab == null)
        {
            GUILayout.Box("No prefab selected!");
        }
        else
        {
            string prefabName = selectedPrefab.name;
            GUILayout.Box(prefabName);
        }
        if (selectedPrefab != null && selectedPrefab.GetComponent<Interactable>() != null)
        {
            selectedPrefab.GetComponent<Interactable>().level = EditorGUILayout.IntField("Level Index", selectedPrefab.GetComponent<Interactable>().level);
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
            if (hitInfo.collider == null || (hitInfo.collider.GetComponent<Ground>() == null && hitInfo.collider.name != selectedPrefab.name))
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
                                    if (tileName == "ToggleBlock")
                                        hit.collider.GetComponent<SpritePicker>().PickSprite(tileName, 1);
                                    else
                                        hit.collider.GetComponent<SpritePicker>().PickSprite(tileName);
                                }
                            }
                        }
                    }
                }
            }
        }

        //if RMB pressed, set the selected prefab to null
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            selectedPrefab = null;
        }

        //if X pressed, delete the gameobject on the choosen coordinate (if such object exist) 
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(gridPosition, Vector2.zero);
            if (hitInfo.collider != null)
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
                                    if (tileName == "ToggleBlock")
                                        hit.collider.GetComponent<SpritePicker>().PickSprite(tileName, 1);
                                    else
                                        hit.collider.GetComponent<SpritePicker>().PickSprite(tileName);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (selectedPrefab != null)
        {
            Texture prefabTexture = selectedPrefab.GetComponent<SpriteRenderer>().sprite.texture;
            Handles.Label(gridPosition, prefabTexture);
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
}
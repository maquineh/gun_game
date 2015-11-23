using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {

        MapGenerator mapa = target as MapGenerator;

        if(DrawDefaultInspector()){
            mapa.generateMap();
        }
  
        if(GUILayout.Button("Generate Map")){
            mapa.generateMap();
        }
    }
}

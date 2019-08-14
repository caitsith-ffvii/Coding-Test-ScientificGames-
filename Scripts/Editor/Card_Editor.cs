using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Card))]
public class Card_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        var t = (target as Card);
        if (t.transform.childCount == 0)
        {
            if (GUILayout.Button("Add \"Card Back\""))
            {
                GameObject back = Instantiate(t.gameObject);
                DestroyImmediate(back.GetComponent<Card>());
                DestroyImmediate(back.GetComponent<Collider>());
                back.transform.SetParent(t.transform);
                back.transform.localPosition = Vector3.zero;
                back.transform.localEulerAngles = new Vector3(180, 0, 0);
                back.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("CardBack"); //Gives an errorLog stating there could be leak from generating new materials
                //Added this part just to show I can make custom editors.
            }
        }
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
}

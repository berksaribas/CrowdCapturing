using UnityEditor;
using UnityEngine;
using Util;

namespace Editor
{
    [CustomPropertyDrawer(typeof(Baker))]
    public class BakerPropertyDrawer : PropertyDrawer
    {
        public override async void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float margin = 5;
            var defaultHeight = EditorGUI.GetPropertyHeight(property);
            
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.PropertyField(position, property, label, true);

            position.y += defaultHeight + margin;
            position.height -= defaultHeight + margin;
            
            if (!property.isExpanded)
            {
                GUI.Label(position, "asd");
                return;
            }

            var component = property.serializedObject.targetObject as MonoBehaviour;
            var baker = fieldInfo.GetValue(component) as Baker;

            if (baker.Baking)
            {
                var r = EditorGUILayout.BeginVertical();
                GUILayout.Space(20);

                EditorGUI.ProgressBar(
                    r,
                    baker.Progress,
                    $"Baking... %{baker.Progress * 100f}"
                );
                
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Cancel Baking"))
                {
                    baker.Cancel();
                }
            }
            else
            {
                if (baker.Baked)
                {
                    var leftSide = new Rect(position);
                    leftSide.width -= (position.width + margin) / 2f;

                    if (GUI.Button(leftSide, "Clear"))
                    {
                        baker.Clear();
                    }

                    var rightSide = new Rect(leftSide);
                    rightSide.x += (position.width + 2 * margin) / 2f;
                    
                    if (GUI.Button(rightSide, "Bake it!"))
                    {
                        await baker.Bake(component);
                    }
                }
                else
                {
                    if (GUI.Button(position, "Bake it!"))
                    {
                        await baker.Bake(component);
                    }
                }
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                var baker = fieldInfo.GetValue(property.serializedObject.targetObject) as Baker;
                
                if (baker.Baking)
                    return EditorGUI.GetPropertyHeight(property) + 50f;
                
                return EditorGUI.GetPropertyHeight(property) + 30f;
            }

            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
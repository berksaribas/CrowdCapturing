using UnityEditor;
using UnityEngine;
using Util;

namespace Editor
{
    [CustomPropertyDrawer(typeof(Baker))]
    public class BakerPropertyDrawer : PropertyDrawer
    {
        private Baker baker;
        private MonoBehaviour component;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;    // Property is always expanded
            
            component = property.serializedObject.targetObject as MonoBehaviour;
            baker = fieldInfo.GetValue(component) as Baker;
            
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var verticalMargin = EditorGUIUtility.standardVerticalSpacing;
            var horizontalMargin = verticalMargin / 2f;
            
            var currentLine = new Rect(position) {height = lineHeight};

            var shouldShowSaveFields = !baker.IsBaked;
            if (shouldShowSaveFields)
            {
                var propertyHeight = EditorGUI.GetPropertyHeight(property, GUIContent.none);
                currentLine.height = propertyHeight - lineHeight;
                EditorGUI.PropertyField(currentLine, property, new GUIContent(property.displayName), true);
                currentLine.y += propertyHeight + verticalMargin;
                currentLine.height = lineHeight;
            }
            else
            {
                EditorGUI.PropertyField(currentLine, property, new GUIContent(property.displayName), false);
                currentLine.y += lineHeight + verticalMargin;
            }
            
            var leftSide = new Rect(currentLine);
            leftSide.width = (currentLine.width - horizontalMargin) * 0.75f;
            
            var rightSide = new Rect(currentLine);
            rightSide.width -= leftSide.width + horizontalMargin;
            rightSide.x += leftSide.width + horizontalMargin;
            
            if (baker.IsBaking)
            {
                GUI.Label(leftSide, "Baking...");
                
                if (GUI.Button(rightSide, "Cancel"))
                    Cancel();
            }
            else
            {
                if (baker.IsBaked)
                {
                    var bakedAsset = AssetDatabase.LoadAssetAtPath<Object>(
                        IOHelper.GetAssetPath(baker.SavePath, baker.SaveName)
                    );
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.ObjectField(leftSide, bakedAsset, typeof(Object), false);
                    EditorGUI.EndDisabledGroup();

                    if (GUI.Button(rightSide, "Clear"))
                        baker.Clear();
                }
                else
                {
                    if (GUI.Button(currentLine, "Bake it!"))
                        Bake();
                }
            }
            currentLine.y += lineHeight + verticalMargin;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var verticalMargin = EditorGUIUtility.standardVerticalSpacing;
            
            var lineCount = 1;
            
            var baker = fieldInfo.GetValue(property.serializedObject.targetObject) as Baker;
            
            if (baker.IsBaked)
                lineCount += 1;
            else
                lineCount += 3;

            return lineCount * (lineHeight + verticalMargin);
        }

        private void Bake()
        {
            baker.CreateBakeAction(component);
            
            EditorApplication.update += BakeOnUpdate;
        }

        private void Cancel()
        {
            Debug.Log("Canceled Baking!");
            EditorApplication.update -= BakeOnUpdate;
        }

        private void BakeOnUpdate()
        {
            Debug.Log("Calling baking action");
            baker.BakeAction();
            
            EditorApplication.update -= BakeOnUpdate;
        }
    }
}
// Public Domain. NO WARRANTIES. License: https://opensource.org/licenses/0BSD

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    static readonly Regex rArrayItemProp = new Regex(@"\.Array\.data\[(\d+)\]$");
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.name == "data")
        {
            var matchArrayItem = rArrayItemProp.Match(property.propertyPath);
            if (matchArrayItem.Success) // property is array item
                label.text = "Element " + matchArrayItem.Groups[1].Value; // label should not be GUID in array item
        }
        if (!property.NextVisible(true)) return;
        // could use 128-bit type to serialize, like rectIntValue; but it would be less legible in text assets
        var oldGuid = property.stringValue;
        // NOTE: doesn't handle missing references, which are shown as "None (Scene Asset)"
        var oldPath = AssetDatabase.GUIDToAssetPath(oldGuid);
        var oldObj = AssetDatabase.LoadAssetAtPath<SceneAsset>(oldPath);
        var newObj = EditorGUI.ObjectField(position, label, oldObj, typeof(SceneAsset), false) as SceneAsset;
        if (newObj == oldObj) return;
        if (newObj == null)
            property.stringValue = "";
        else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(newObj, out string newGuid, out long _))
            property.stringValue = newGuid;
    }
}

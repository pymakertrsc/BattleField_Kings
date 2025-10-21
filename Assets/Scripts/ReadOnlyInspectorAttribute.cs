using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


//readonlyInspector是unity用于扩展Inspector功能的基类
public class ReadOnlyInspector : PropertyAttribute
{

}

//确保以下代码旨在unity编译器下运行，不影响最终打包效果
#if UNITY_EDITOR


//告诉 Unity 编辑器：当检测到字段被 [ReadOnlyInspector] 标记时，调用 ReadOnlyDrawer 类中的 OnGUI 和 GetPropertyHeight 方法来绘制该字段
[CustomPropertyDrawer(typeof(ReadOnlyInspector))]

//最终实现 “字段在 Inspector 中可见但不可编辑” 的效果（通过 GUI.enabled = false 禁用交互）
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

#endif

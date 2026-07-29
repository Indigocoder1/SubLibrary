/*
Copyright © 2022 Metious
This work is free. You can redistribute it and/or modify it under the
terms of the Do What The Fuck You Want To Public License, Version 2,
as published by Sam Hocevar. See http://www.wtfpl.net/ for more details.
*/

using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomPropertyDrawer(typeof(Enum), true)]
public class EnumDrawer : PropertyDrawer
{
    private AdvancedStringOptionsDropdown _dropdown;
    private SerializedProperty _property;
    private Rect _buttonRect;

    private object underlyingObj;
    private FieldInfo objectInfo;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (objectInfo == null)
        {
            objectInfo = property.serializedObject.targetObject.GetType().GetField(property.propertyPath);
            if(objectInfo != null)
            {
                underlyingObj = objectInfo.GetValue(property.serializedObject.targetObject);
            }
        }
        else
        {
            underlyingObj = objectInfo.GetValue(property.serializedObject.targetObject);
        }

        if (underlyingObj != null && underlyingObj.GetType().GetCustomAttribute<FlagsAttribute>() != null)
        {
            HandleFlagEnum(position, property, label);
            return;
        }

        HandleSearchEnum(position, property, label);
    }

    private void HandleFlagEnum(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginProperty(position, label, property);

        Enum enumNew = EditorGUI.EnumFlagsField(position, underlyingObj as Enum);
        objectInfo.SetValue(property.serializedObject.targetObject, enumNew);

        EditorGUI.EndProperty();
    }

    private void HandleSearchEnum(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_dropdown == null)
        {
            _dropdown = new AdvancedStringOptionsDropdown(property.enumDisplayNames);
            _dropdown.OnOptionSelected += OnDropdownOptionSelected;
        }
        position = EditorGUI.PrefixLabel(position, label);

        if (Event.current.type == EventType.Repaint)
            _buttonRect = position;

        Rect dropdownRect = _buttonRect;
        float offset = property.enumDisplayNames.Length * EditorGUIUtility.singleLineHeight;
        if (position.y > 0)
        {
            dropdownRect.y -= offset;
        }
        else
        {
            dropdownRect.y += offset;
        }

        if (GUI.Button(
                position,
                new GUIContent(property.enumDisplayNames[Mathf.Clamp(property.enumValueIndex, 0, property.enumDisplayNames.Length - 1)]),
                EditorStyles.popup
            ))
        {
            _dropdown.Show(dropdownRect);
            _property = property;
        }
    }

    private void OnDropdownOptionSelected(int index)
    {
        _property.enumValueIndex = index;
        _property.serializedObject.ApplyModifiedProperties();
    }
}

public class AdvancedStringOptionsDropdown : AdvancedDropdown
{
    private string[] _enumNames;

    public event Action<int> OnOptionSelected;

    public AdvancedStringOptionsDropdown(string[] stringOptions) : base(new AdvancedDropdownState())
    {
        _enumNames = stringOptions;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        OnOptionSelected?.Invoke(item.id);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("");

        for (int i = 0; i < _enumNames.Length; i++)
        {
            var item = new AdvancedDropdownItem(_enumNames[i])
            {
                id = i
            };

            root.AddChild(item);
        }

        return root;
    }
}
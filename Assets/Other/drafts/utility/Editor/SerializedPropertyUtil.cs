using System;
using UnityEditor;
using UnityEngine;

public static partial class SerializedPropertyUtil {

	public static object GetValue(this SerializedProperty property) => property.propertyType switch {
		SerializedPropertyType.Generic => throw new NotImplementedException(),
		SerializedPropertyType.Integer => property.intValue,
		SerializedPropertyType.Boolean => property.boolValue,
		SerializedPropertyType.Float => property.floatValue,
		SerializedPropertyType.String => property.stringValue,
		SerializedPropertyType.Color => property.colorValue,
		SerializedPropertyType.ObjectReference => property.objectReferenceValue,
		SerializedPropertyType.LayerMask => throw new NotImplementedException(),
		SerializedPropertyType.Enum => property.enumValueIndex,
		SerializedPropertyType.Vector2 => property.vector2Value,
		SerializedPropertyType.Vector3 => property.vector3Value,
		SerializedPropertyType.Vector4 => property.vector4Value,
		SerializedPropertyType.Rect => property.rectValue,
		SerializedPropertyType.ArraySize => throw new NotImplementedException(),
		SerializedPropertyType.Character => throw new NotImplementedException(),
		SerializedPropertyType.AnimationCurve => property.animationCurveValue,
		SerializedPropertyType.Bounds => property.boundsValue,
		SerializedPropertyType.Gradient => property.gradientValue,
		SerializedPropertyType.Quaternion => property.quaternionValue,
		SerializedPropertyType.ExposedReference => throw new NotImplementedException(),
		SerializedPropertyType.FixedBufferSize => throw new NotImplementedException(),
		SerializedPropertyType.Vector2Int => property.vector2IntValue,
		SerializedPropertyType.Vector3Int => property.vector3IntValue,
		SerializedPropertyType.RectInt => property.rectIntValue,
		SerializedPropertyType.BoundsInt => property.boundsIntValue,
		SerializedPropertyType.ManagedReference => throw new NotImplementedException(),
		SerializedPropertyType.Hash128 => throw new NotImplementedException(),
		_ => throw new NotImplementedException(),
	};

	public static void SetValue(this SerializedProperty property, object value) {
		switch(property.propertyType) {
			case SerializedPropertyType.Generic: throw new NotImplementedException();
			case SerializedPropertyType.Integer: property.intValue = (int)value; break;
			case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
			case SerializedPropertyType.Float: property.floatValue = (float)value; break;
			case SerializedPropertyType.String: property.stringValue = (string)value; break;
			case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
			case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (UnityEngine.Object)value; break;
			case SerializedPropertyType.LayerMask: property.uintValue = (uint)value; throw new NotImplementedException();
			case SerializedPropertyType.Enum: property.enumValueIndex = (int)value; break;
			case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
			case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
			case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;
			case SerializedPropertyType.Rect: property.rectValue = (Rect)value; break;
			case SerializedPropertyType.ArraySize: throw new NotImplementedException();
			case SerializedPropertyType.Character: throw new NotImplementedException();
			case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; break;
			case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; break;
			case SerializedPropertyType.Gradient: property.gradientValue = (Gradient)value; break;
			case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; break;
			case SerializedPropertyType.ExposedReference: throw new NotImplementedException();
			case SerializedPropertyType.FixedBufferSize: throw new NotImplementedException();
			case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; break;
			case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; break;
			case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; break;
			case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; break;
			case SerializedPropertyType.ManagedReference: property.managedReferenceValue = value; break;
			case SerializedPropertyType.Hash128: throw new NotImplementedException();
			default: throw new NotImplementedException();
		}
		property.serializedObject.ApplyModifiedProperties();
	}
}

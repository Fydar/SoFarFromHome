﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (Spring))]
public class SpringDrawer : PropertyDrawer
{
	int samples = 32;
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 180;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Rect springPowerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

		Rect springDamperRect = new Rect(position.x, springPowerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
			position.width, EditorGUIUtility.singleLineHeight);

		Rect remainingRect = new Rect(position.x, springDamperRect.yMax + EditorGUIUtility.standardVerticalSpacing,
			position.width, position.yMax - springDamperRect.yMax);


		SerializedProperty springPowerProperty = property.FindPropertyRelative("Power");
		SerializedProperty springDamperProperty = property.FindPropertyRelative("Damper");

		EditorGUI.PropertyField(springPowerRect, springPowerProperty);
		EditorGUI.PropertyField(springDamperRect, springDamperProperty);

		if (Event.current.type == EventType.Repaint)
		{
			//EditorStyles.helpBox.Draw(remainingRect, GUIContent.none, 0);
			EditorGUI.DrawRect(remainingRect, new Color(0.1f, 0.1f, 0.1f, 1.0f));

			List<float> cache = new List<float>();

			Spring spring = new Spring();
			spring.Power = springPowerProperty.floatValue;
			spring.Damper = springDamperProperty.floatValue;

			for (int i = 0; i < samples; i++)
			{
				cache.Add(spring.Value);

				spring.Update(1.0f / 60.0f);
			}

			Handles.BeginGUI();
			Vector3[] graphInfo = new Vector3[cache.Count];
			
			for (int i = 0; i < cache.Count; i++)
			{
				float time = ((float)i) / (cache.Count - 1);
				float value = cache[i];


				graphInfo[i] = new Vector3(Mathf.Lerp(remainingRect.xMin, remainingRect.xMax, time),
					Mathf.Lerp(remainingRect.yMax, remainingRect.yMin, value), 0);

				if (i != 0)
					Handles.DrawLine(graphInfo[i - 1], graphInfo[i]);
			}

			Handles.color = Color.green;
			Handles.DrawPolyLine(graphInfo);

			Handles.EndGUI();
			
		}
	}

	void DrawLine(Vector3 start, Vector3 end)
	{

	}
}

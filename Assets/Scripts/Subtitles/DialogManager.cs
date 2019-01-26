﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	[Serializable]
	public struct DisplayMapping
	{
		public Dialog.DisplayArea.Options Area;
		public DialogDisplay Display;
	}

	public Dialog PlayOnStart;
	public CanvasGroup Fader;

	public DisplayMapping[] Displays;
	public float fadeInTime = 0.5f;
	public float fadeOutTime = 0.5f;
	public Queue<Dialog> MessageQueue = new Queue<Dialog> ();

	private AudioSource soundPlayer;

	private void Start ()
	{
		foreach (var findDisplay in Displays)
		{
			findDisplay.Display.gameObject.SetActive (false);
		}

		StartCoroutine (Process());
	}

	IEnumerator<YieldInstruction> Process()
	{
		MessageQueue.Enqueue (PlayOnStart);
		soundPlayer = gameObject.AddComponent<AudioSource> ();

		while (true)
		{
			while (MessageQueue.Count == 0)
				yield return null;

			var currentMessage = MessageQueue.Dequeue ();

			if (currentMessage.Enqueues != null)
				MessageQueue.Enqueue (currentMessage.Enqueues);

			foreach (var displayOptions in currentMessage.Displays)
			{
				displayOptions.CachePosiiton = 0;
			}

			yield return new WaitForSeconds (currentMessage.Delay);
			if (currentMessage.PlaySound != null)
				soundPlayer.PlayOneShot (currentMessage.PlaySound);

			foreach(var displayOptions in currentMessage.Displays)
			{
				foreach(var findDisplay in Displays)
				{
					if (findDisplay.Area == displayOptions.Area)
					{
						findDisplay.Display.gameObject.SetActive (true);
						if (displayOptions.WriteMode == Dialog.DisplayArea.TextSettings.Appear)
							findDisplay.Display.text.text = displayOptions.Text;
						else
							findDisplay.Display.text.text = "";
					}
					else
					{
						findDisplay.Display.gameObject.SetActive (false);
					}
				}
			}

			foreach(var time in new TimedLoop (fadeInTime))
			{
				Fader.alpha = time;
				yield return null;
			}

			while (true)
			{
				bool hasTyped = false;
				foreach (var displayOptions in currentMessage.Displays)
				{
					foreach (var findDisplay in Displays)
					{
						if (findDisplay.Area == displayOptions.Area)
						{
							if (displayOptions.WriteMode == Dialog.DisplayArea.TextSettings.Typewriter &&
								displayOptions.CachePosiiton != displayOptions.Text.Length - 1)
							{
								findDisplay.Display.text.text += displayOptions.Text[displayOptions.CachePosiiton];
								displayOptions.CachePosiiton++;
								hasTyped = true;
								yield return new WaitForSeconds (displayOptions.CharacterDisplay);
							}
						}
					}
				}
				if (!hasTyped)
					break;
			}

			yield return new WaitForSeconds (currentMessage.Duration);

			foreach (var time in new TimedLoop (fadeOutTime))
			{
				Fader.alpha = 1.0f - time;
				yield return null;
			}

			yield return null;
		}
	}
}

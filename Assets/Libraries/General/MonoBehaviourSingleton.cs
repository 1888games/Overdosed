using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject coreGameObject = new GameObject(typeof(T).Name);

				instance = coreGameObject.AddComponent<T>();
			}

			return instance;
		}
	}

	private static T instance;

	protected virtual void Awake()
	{
		if (instance == null)
			instance = GetComponent<T>();
		else
			DestroyImmediate(this);
	}
}
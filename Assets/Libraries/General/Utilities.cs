using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
	#region Member Variables


	#endregion

	#region Properties

	public static double SystemTimeInMilliseconds { get { return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalMilliseconds; } }

	public static float WorldWidth	{ get { return 2f * Camera.main.orthographicSize * Camera.main.aspect; } }
	public static float WorldHeight	{ get { return 2f * Camera.main.orthographicSize; } }

	#endregion

	#region Public Methods

	public static int stringToInt (string s) {

		int v = 0;
		if (int.TryParse(s,out v))
			return v;
		return 0;
	
	}
		


	/// <summary>
	/// Returns to mouse position
	/// </summary>
	public static Vector2 MousePosition()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return (Vector2)Input.mousePosition;
		#else
		if (Input.touchCount > 0)
		{
			return Input.touches[0].position;
		}

		return Vector2.zero;
		#endif
	}

	/// <summary>
	/// Returns true if a mouse down event happened, false otherwise
	/// </summary>
	public static bool MouseDown()
	{
		return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began);
	}
	
	/// <summary>
	/// Returns true if a mouse up event happened, false otherwise
	/// </summary>
	public static bool MouseUp()
	{
		return (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended));
	}
	
	/// <summary>
	/// Returns true if no mouse events are happening, false otherwise
	/// </summary>
	public static bool MouseNone()
	{
		return (!Input.GetMouseButton(0) && Input.touchCount == 0);
	}

	/// <summary>
	/// Converts to json string.
	/// </summary>
	public static string ConvertToJsonString(object data)
	{
		string jsonString = "";
		
		if (data is IDictionary)
		{
			Dictionary<string, object> dic = data as Dictionary<string, object>;
			
			jsonString += "{";
			
			List<string> keys = new List<string>(dic.Keys);
			
			for (int i = 0; i < keys.Count; i++)
			{
				if (i != 0)
				{
					jsonString += ",";
				}
				
				jsonString += string.Format("\"{0}\":{1}", keys[i], ConvertToJsonString(dic[keys[i]]));
			}
			
			jsonString += "}";
		}
		else if (data is IList)
		{
			IList list = data as IList;
			
			jsonString += "[";
			
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					jsonString += ",";
				}
				
				jsonString += ConvertToJsonString(list[i]);
			}
			
			jsonString += "]";
		}
		else if (data is string)
		{
			// If the data is a string then we need to inclose it in quotation marks
			jsonString += "\"" + data + "\"";
		}
		else
		{
			// Else just return what ever data is as a string
			jsonString += data.ToString();
		}
		
		return jsonString;
	}

	public static string FloatToTime (float toConvert, string format){
		switch (format){
		case "00.0":
			return string.Format("{0:00}:{1:0}", 
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*10) % 10));//miliseconds
			break;
		case "#0.0":
			return string.Format("{0:#0}:{1:0}", 
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*10) % 10));//miliseconds
			break;
		case "00.00":
			return string.Format("{0:00}:{1:00}", 
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*100) % 100));//miliseconds
			break;
		case "00.000":
			return string.Format("{0:00}:{1:000}", 
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*1000) % 1000));//miliseconds
			break;
		case "#00.000":
			return string.Format("{0:#00}:{1:000}", 
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*1000) % 1000));//miliseconds
			break;
		case "#0:00":
			return string.Format("{0:#0}:{1:00}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60);//seconds
			break;
		case "#00:00":
			return string.Format("{0:#00}:{1:00}", 
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60);//seconds
			break;
		case "0:00.0":
			return string.Format("{0:0}:{1:00}.{2:0}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*10) % 10));//miliseconds
			break;
		case "#0:00.0":
			return string.Format("{0:#0}:{1:00}.{2:0}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*10) % 10));//miliseconds
			break;
		case "0:00.00":
			return string.Format("{0:0}:{1:00}.{2:00}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*100) % 100));//miliseconds
			break;
		case "#0:00.00":
			return string.Format("{0:#0}:{1:00}.{2:00}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*100) % 100));//miliseconds
			break;
		case "0:00.000":
			return string.Format("{0:0}:{1:00}.{2:000}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*1000) % 1000));//miliseconds
			break;
		case "#0:00.000":
			return string.Format("{0:#0}:{1:00}.{2:000}",
				Mathf.Floor(toConvert / 60),//minutes
				Mathf.Floor(toConvert) % 60,//seconds
				Mathf.Floor((toConvert*1000) % 1000));//miliseconds
			break;
		}
		return "error";
	}

	#endregion
}

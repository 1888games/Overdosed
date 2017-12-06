using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public static class TimedEvent {

	public static Dictionary<string, System.Diagnostics.Stopwatch> stopwatches = new Dictionary<string, System.Diagnostics.Stopwatch>();
	public static Dictionary<string, int> longestTime = new Dictionary<string, int> ();

	public static void Click (string name) {

		//Debug.Log (name);

		if (stopwatches.ContainsKey (name) == false) {

			System.Diagnostics.Stopwatch newSw = new System.Diagnostics.Stopwatch ();

			stopwatches.Add (name, newSw);
			longestTime.Add (name, 0);
			newSw.Start ();
			return;

		}

	
		System.Diagnostics.Stopwatch sw = stopwatches [name];
		int record = longestTime [name];

	
		if (sw.IsRunning) {

			sw.Stop ();



			if (sw.Elapsed.Milliseconds > record) {

				longestTime [name] = (int)sw.ElapsedMilliseconds;
				Debug.Log ("Time taken: " + name + " - " + sw.Elapsed.Milliseconds + "ms");
			}


		}

		else {

			sw.Reset();
			sw.Start();

		}


	}


}

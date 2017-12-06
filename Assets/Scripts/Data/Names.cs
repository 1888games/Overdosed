using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Names : MonoBehaviourSingleton<Names> {


	public List<string> maleNames;
	public List<string> femaleNames;


	// Use this for initialization
	void Start () {

		maleNames = new List<string> ();
		femaleNames = new List<string> ();

		DoMaleNames ();
		DoFemaleNames ();
	}


	public void ShuffleList<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}


	// Update is called once per frame
	void Update () {
		
	}


	void DoFemaleNames() {

		femaleNames.Add ("Caroline Bluff");
		femaleNames.Add ("Karen Plinth");
		femaleNames.Add ("Rita Dice");
		femaleNames.Add ("Irene Loud");
		femaleNames.Add ("Jane Latch");
		femaleNames.Add ("Carol Aloof");
		femaleNames.Add ("Kelly Drive");
		femaleNames.Add ("Harriet Lift");
		femaleNames.Add ("Paula Block");
		femaleNames.Add ("Jessica Pertain");
		femaleNames.Add ("Alison Code");
		femaleNames.Add ("Nicola Peep");
		femaleNames.Add ("Patty Jape");
		femaleNames.Add ("Vanessa Brink");
		femaleNames.Add ("Justine Deft");
		femaleNames.Add ("Shauna Race");
		femaleNames.Add ("Aurelia Sticker");
		femaleNames.Add ("Wendy Gist");
		femaleNames.Add ("Carol Nature");
		femaleNames.Add ("Jessica Reach");
		femaleNames.Add ("Rachel Source");
		femaleNames.Add ("Anne Progress");
		femaleNames.Add ("Alexandria Blemish");
		femaleNames.Add ("Scarlett Intray");
		femaleNames.Add ("Valerie Tuft");
		femaleNames.Add ("Sue Conjure");
		femaleNames.Add ("Alison Team");
		femaleNames.Add ("Sunita Range");
		femaleNames.Add ("Paula Scrap");
		femaleNames.Add ("Sheena Steam");
		femaleNames.Add ("Jenny Patches");
		femaleNames.Add ("Tania Net");
		femaleNames.Add ("Sarah Glut");
		femaleNames.Add ("Yvonne Loot");
		femaleNames.Add ("Yvonne Seek");
		femaleNames.Add ("Cindy Travels");

		femaleNames.Add ("Rita Clasp");
		femaleNames.Add ("Mary Space");
		femaleNames.Add ("Jane Click");
		femaleNames.Add ("Lisa Portrait");
		femaleNames.Add ("Kate Stoop");
		femaleNames.Add ("Justine Aft");
		femaleNames.Add ("Carol Reason");
		femaleNames.Add ("Dawn Tablet");
		femaleNames.Add ("Ailsa Stitch");
		femaleNames.Add ("Vivienne Achieve");
		femaleNames.Add ("Jenny Seat");
		femaleNames.Add ("Sandra Jest");
		femaleNames.Add ("Wendy Shove");
		femaleNames.Add ("Zoe Laminate");
		femaleNames.Add ("Teresa Hornet");
		femaleNames.Add ("Lynn Sketch");
		femaleNames.Add ("Ruth Measure");
		femaleNames.Add ("Julie Control");
		femaleNames.Add ("Karen Paste");
		femaleNames.Add ("Emma Dabble");
		femaleNames.Add ("Sally Mash");
		femaleNames.Add ("Mary Collection");
		femaleNames.Add ("Sian Settle");
		femaleNames.Add ("Sally Tote");
		femaleNames.Add ("Gill Harness");
		femaleNames.Add ("Irene Error");
		femaleNames.Add ("Roz Setting");
		femaleNames.Add ("Pamela Soar");
		femaleNames.Add ("Lilian Strike");
		femaleNames.Add ("Brenda Need");
		femaleNames.Add ("Miriam Bubb");
	

		ShuffleList (femaleNames);

	}

	void DoMaleNames() {


		maleNames.Add ("Ian Thought");
		maleNames.Add ("Peter Glance");
		maleNames.Add ("Stephen Oblique");
		maleNames.Add ("Raymond Chance");
		maleNames.Add ("Tony Query");
		maleNames.Add ("Lionel Mood");
		maleNames.Add ("Carl Wisp");
		maleNames.Add ("Larry State");
		maleNames.Add ("Jake Broil");
		maleNames.Add ("Ken Drawer");
		maleNames.Add ("Winston Slot");
		maleNames.Add ("Shaun Silk");
		maleNames.Add ("Des Pine");
		maleNames.Add ("Eugine Horns");
		maleNames.Add ("Darren Wires");
		maleNames.Add ("George Scuttle");
		maleNames.Add ("Roy Heft");
		maleNames.Add ("Patrick Safe");
		maleNames.Add ("Peter Tilt");
		maleNames.Add ("Tim Fan");
		maleNames.Add ("Ian Touch");
		maleNames.Add ("Brian Track");
		maleNames.Add ("Phil Layer");
		maleNames.Add ("Dennis Build");
		maleNames.Add ("Yuri Stint");
		maleNames.Add ("Darryl Cover");
		maleNames.Add ("Simon Brink");
		maleNames.Add ("Wally Bench");
		maleNames.Add ("Frank Grasp");
		maleNames.Add ("Zach Style");


		maleNames.Add ("Ivan Sirloin");
		maleNames.Add ("Gavin Reach");
		maleNames.Add ("Alan Stork");
		maleNames.Add ("Jared Game");
		maleNames.Add ("Arnold System");
		maleNames.Add ("Ralf Wrexham");
		maleNames.Add ("Donald Site");
		maleNames.Add ("Brent Crush");
		maleNames.Add ("Gerry Slalom");
		maleNames.Add ("Eric Shine");
		maleNames.Add ("Arnie Feature");
		maleNames.Add ("Hank Blaze");
		maleNames.Add ("Theo Extend");
		maleNames.Add ("Lionel Skeet");
		maleNames.Add ("Vince Trawl");
		maleNames.Add ("Ryan Verve");
		maleNames.Add ("Keith Graft");
		maleNames.Add ("Alan Timepiece");
		maleNames.Add ("Alistair Loop");
		maleNames.Add ("Neal Render");
		maleNames.Add ("Chris Stroke");
		maleNames.Add ("Benjamin Scoop");
		maleNames.Add ("Robin Mix");
		maleNames.Add ("Justin Sponsor");
		maleNames.Add ("Marty Ticker");
		maleNames.Add ("Glenn Parade");
		maleNames.Add ("Ollie Barb");
		maleNames.Add ("Perry Rang");
		maleNames.Add ("Walter Belt");
		maleNames.Add ("George Milk");


		maleNames.Add ("Ollie Clip");
		maleNames.Add ("Carlos Envoke");
		maleNames.Add ("Jeff Stamina");
		maleNames.Add ("Iain Stuff");
		maleNames.Add ("Scott Distance");
		maleNames.Add ("Curtis Measure");
		maleNames.Add ("Peter Align");
		maleNames.Add ("Chris Adaption");
		maleNames.Add ("Shaun Conference");
		maleNames.Add ("Wally Stance");
		maleNames.Add ("Scott Carpet");
		maleNames.Add ("Stephen Socket");
		maleNames.Add ("Frank Lease");

		ShuffleList (maleNames);

	}
}




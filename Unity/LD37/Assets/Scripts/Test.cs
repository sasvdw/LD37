using System.Collections;
using System.Collections.Generic;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Rooms;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var cousins = new List<Cousin>();
	    for(int i = 1; i <= 4; i++)
	    {
	        cousins.Add(new Cousin(string.Format("TestCousin{0}", i)));
	    }

	    var cousinsToAddToBuilding = new CousinsToAddToBuilding(cousins);
	    new Building(cousinsToAddToBuilding, new ItemToSpawnSelector());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


class WeaponManager
{
	public static List<Weapon> currentExplosions; // Position and Effect Range of Explosion
    private static WeaponManager instance;
 	private static List<Weapon> firedWeapon;
	//
	public static List<float> damageEffectViaType = new List<float>() // type arranged as in enum
	{	5.0f , 5.0f , 5.0f , 5.0f , 5.0f , 5.0f , 5.0f, 5.0f	};
	//{ Infanty, Tank, AirDefense, Ship, Artillery, Helicopter, LightVehicle , Aircraft};
	
    public WeaponManager()
    {
		firedWeapon = new List<Weapon> ();
		currentExplosions = new List<Weapon> ();
    }
  
    public static WeaponManager getInstance()
    {
        if (instance == null)
            instance = new WeaponManager();
        return instance;
    }
	
		public void addFireMissile (Weapon missile)
	{
		firedWeapon.Add (missile);
	}
	
	public void addExplosion(Weapon explosion)
	{
		currentExplosions.Add(explosion);		
	}
	
    public virtual void Update()
    {		
		if(currentExplosions.Count!=0) // clear from previous frame
		currentExplosions.Clear();
		
				//Update Missiles
			for (int i = 0; i < firedWeapon.Count; ++i)
			if (!firedWeapon [i].isDead ())
				firedWeapon [i].Update ();
			else
				firedWeapon.RemoveAt (i);
		
	}

}

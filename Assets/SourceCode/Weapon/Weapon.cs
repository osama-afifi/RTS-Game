using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Weapon
{
    private static Weapon instance;

	public List<int> damageEffectViaType;
    protected GameObject fireBody;
	protected GameObject targetObject;
    protected Vector3 destination, fireSpot, direction;
	protected float lifeTime;
    protected float speed;
    protected double lastFire;
	protected float effectRange;
    protected bool dead;
	protected Vector3 explodedPosition;
	//protected float damageFactor;
	
	
    public Weapon()
    {
        dead = false;
        lastFire = 1;
		lifeTime = 10.0f;
		
    }
    public virtual void destroyBody()
    {
    }
    public bool isDead()
    {
        Debug.Log("dead " + dead);
        return dead;
    }
    public virtual void checkMissileLife()
    {
		if( Physics.CheckSphere(fireBody.transform.position,0.005f) || lastFire>lifeTime ) //  0.005 is the radius of the collison sphere
		{
            
			WeaponManager.getInstance().addExplosion(this);
			explodedPosition = fireBody.transform.position;
			destroyBody();
		}
    }
    public static Weapon getInstance()
    {
        if (instance == null)
            instance = new Weapon();
        return instance;
    }
	
	public Vector3 getExplodedPosition()
	{
		return explodedPosition;	
	}
	
	public float getEffectRange()
	{
		return effectRange;		
	}
	
    public virtual void Update()
    { }
	public int getDamage(unitType type)
	{
		return damageEffectViaType[(int)type];
		
	}
}

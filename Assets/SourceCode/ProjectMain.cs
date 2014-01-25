using UnityEngine;
using System.Collections;

public class ProjectMain : MonoBehaviour
{
	public float gameTime = 0.0f;
	// Use this for initialization
	void Start ()
	{
		Screen.fullScreen = true;
		UnitManager.getInstance ().addTwoDefaultPlayers ();
		TerrainManager.getInstance ().Start ();
		
		//Player 1
		
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (5, 10, 30)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (5, 10, 35)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (5, 10, 40)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Cobra (new Vector3 (10, 15, 30)), 0);
		UnitManager.getInstance ().addPlayersUnit (new F16 (new Vector3 (10, 30, 35)), 0);
		UnitManager.getInstance ().addPlayersUnit (new F16 (new Vector3 (10, 30, 40)), 0);
		UnitManager.getInstance ().addPlayersUnit (new F16 (new Vector3 (10, 30, 30)), 0);
		UnitManager.getInstance ().addPlayersUnit (new Humvee (new Vector3 (20, 15, 35)), 0);
		UnitManager.getInstance ().addPlayersUnit (new Humvee (new Vector3 (20, 15, 40)), 0);
		gameTime = 0.0f;
		while (gameTime<=0.1f) {
			gameTime += Time.deltaTime;
				
		}
		UnitManager.getInstance ().addPlayersUnit (new M109 (new Vector3 (45, 10, 30)), 0);
		UnitManager.getInstance ().addPlayersUnit (new M109 (new Vector3 (50, 10, 35)), 0);
		UnitManager.getInstance ().addPlayersUnit (new M109 (new Vector3 (55, 10, 40)), 0);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (60, 10, 30)), 0);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (65, 10, 35)), 0);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (70, 10, 40)), 0);
		for (int i = 0; i<4; i++)
			for (int j = 0; j<4; j++) {
				UnitManager.getInstance ().addPlayersUnit (new M1Abrams (new Vector3 (10 + 5 * i, 10, 10 + 5 * j)), 0);
				gameTime = 0.0f;
				while (gameTime<=0.1f) {
					gameTime += Time.deltaTime;
				
				}
				
			}
		// Player 2
		
		for (int i = 0; i<4; i++)
			for (int j = 0; j<4; j++) {	
				UnitManager.getInstance ().addPlayersUnit (new Merkava4 (new Vector3 (210 - 5 * i, 10, 40 + 5 * j)), 1);
				gameTime = 0.0f;
				while (gameTime<=0.1f) {
					gameTime += Time.deltaTime;
				
				}
			
			}
		UnitManager.getInstance ().addPlayersUnit (new Apache (new Vector3 (230, 15, 60)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Apache (new Vector3 (230, 15, 70)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Apache (new Vector3 (230, 15, 80)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Cobra (new Vector3 (220, 15, 60)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Cobra (new Vector3 (220, 15, 70)), 1);
		gameTime = 0.0f;
		while (gameTime<=0.1f) {
			gameTime += Time.deltaTime;
				
		}
		UnitManager.getInstance ().addPlayersUnit (new Cobra (new Vector3 (220, 15, 80)), 1);
		UnitManager.getInstance ().addPlayersUnit (new M109 (new Vector3 (235, 10, 50)), 1);
		UnitManager.getInstance ().addPlayersUnit (new M109 (new Vector3 (235, 10, 40)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (230, 10, 50)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (230, 10, 40)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (230, 10, 45)), 1);
		UnitManager.getInstance ().addPlayersUnit (new Avenger (new Vector3 (230, 10, 55)), 1);

	}
	
	// Update is called once per frame
	void Update ()
	{
		TerrainManager.Update ();
		CameraControl.getInstance ().Update ();
		InputManager.getInstance ().Update ();
		WeaponManager.getInstance ().Update ();
		UnitManager.getInstance ().Update ();

	}

	void OnGUI ()
	{
		GUIManager.getInstance ().Update ();
	}
}

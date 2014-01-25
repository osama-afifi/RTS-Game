using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum cursorState {neutral, selected, attack, snipe};

class GUIManager
{
    private static GUIManager instance;
    private bool showRect;
    private Rect selectionRect;
	private Texture2D cursorNeutral;
	private Texture2D cursorAttack;
	private Texture2D cursorSelected;
	Vector2 hotSpot;

    public void setSelectionBox(bool b, Rect rect) 
	{ showRect = b; selectionRect = rect; }

    public static GUIManager getInstance()
    {
        if (instance == null)
        {
            instance = new GUIManager();
        }
        return instance;
    }

    private GUIManager()
    {
		// Screen.showCursor = false;
		cursorNeutral = (Texture2D)Resources.Load("Cursors/cursor_neutral.png");
		cursorAttack = (Texture2D)Resources.Load("Cursors/cursor_attack.png");
		cursorSelected = (Texture2D)Resources.Load("Cursors/cursor_selected.png");
		hotSpot = Vector2.zero;
    }

    public void Update()
    {
        if (showRect)
                GUI.Box(selectionRect, "");
		
//  GUI.DrawTexture ( new Rect(Input.mousePosition.x-cursorNeutral.width/2 + cursorNeutral.width/2, (Screen.height-Input.mousePosition.y)-cursorNeutral.height/2 + cursorNeutral.height/2, cursorNeutral.width, cursorNeutral.height),cursorNeutral);
	
    }
	
}

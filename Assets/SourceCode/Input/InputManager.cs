using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InputManager
{
    private static InputManager instance;
    private bool selectionBox;
    private Vector3 oldMouse;
    private Vector3 selectionStart;
    private Rect rectangleBox;
    private float lastClickTime;
    private float doubleClickTime = 0.25f;

    public static InputManager getInstance()
    {
        if (instance == null)
            instance = new InputManager();
        return instance;
    }
    public InputManager()
    {
        lastClickTime = Time.timeSinceLevelLoad;
    }
    void oneUnitSelection()
    {
        Debug.Log("trying one click");
        RaycastHit hit;
        Ray mouseRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out hit, 500))
        {
            GameObject trg = hit.collider.gameObject;
             if (trg != null && trg.name!="Terrain")
                UnitManager.getInstance().trySelect(trg);
        
        }
    }
    void multipleUnitSelection()
    {
        selectionBox = false;
        RaycastHit hit;
        Ray mouseRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out hit, 500))
            if (hit.collider.gameObject != null)
                UpdateBoxSelection(new Vector3(), new Vector3(Screen.width, Screen.height, 0), hit.collider.gameObject);
    }
	
	
    public void Update()
    {
        //for single and double select .. if user choose one unit
        if (Input.GetMouseButtonDown(0))
        {
            UnitManager.getInstance().resetSelection();
            if (Time.timeSinceLevelLoad - lastClickTime < doubleClickTime)
                multipleUnitSelection();
            else
                oneUnitSelection();
            lastClickTime = Time.timeSinceLevelLoad;
        }
        //right click for move or attack
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray mouseRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out hit, 500))
                if (hit.collider.gameObject.name == "Terrain"  && Input.GetKey(KeyCode.LeftControl)==false)
                {
                    UnitManager.getInstance().orderMove(hit.point);
                    Debug.Log(hit.collider.gameObject.name);
                }
                else
                {
                    Debug.Log("attacking" + hit.collider.gameObject);
                    UnitManager.getInstance().orderAttack(hit.collider.gameObject);
                    Debug.Log(hit.collider.gameObject.name);
                }
        }

        //selection box starts whenever any click happens .. until mouse button is up
        if (Input.GetMouseButtonDown(0))
        {
            selectionBox = true;
            selectionStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectionBox = false;
            GUIManager.getInstance().setSelectionBox(false, rectangleBox);
            rectangleBox = Rect.MinMaxRect(0, 0, 0, 0);
        }

        if (selectionBox && oldMouse != Input.mousePosition)
        {
            if (Vector3.Distance(selectionStart, Input.mousePosition) < 15f)
                oneUnitSelection();
            else
            {
                UpdateBoxSelection(selectionStart, Input.mousePosition, null);
                GUIManager.getInstance().setSelectionBox(true, rectangleBox);
            }
        }

        //old mouse position update
        oldMouse = Input.mousePosition;
    }
    private void UpdateBoxSelection(Vector3 from, Vector3 to, GameObject type)
    {
        RaycastHit hit1, hit2;
        Ray mouseRay1 = Camera.mainCamera.ScreenPointToRay(from);
        Ray mouseRay2 = Camera.mainCamera.ScreenPointToRay(to);
        if (Physics.Raycast(mouseRay1, out hit1, 500) && Physics.Raycast(mouseRay2, out hit2, 500))
        {
            //draw box info
            float minx = Mathf.Min(from.x, to.x);
            float miny = Mathf.Min(from.y, to.y);
            float maxx = Mathf.Max(from.x, to.x);
            float maxy = Mathf.Max(from.y, to.y);
            float w = maxx - minx;
            float h = maxy - miny;
            //not double click?
            if (w != Screen.width || h != Screen.height)
            {
                rectangleBox = Rect.MinMaxRect(minx, Screen.height - maxy, maxx, Screen.height - miny);
            }
            //selection
            UnitManager.getInstance().boxSelection(hit1.point, hit2.point, type);
        }
    }


}
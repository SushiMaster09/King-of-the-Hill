using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour 
{
	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	private int activeElevation;

	private bool applyColor;
    private bool applyElevation = true;

    private bool isDrag;
    private HexDirection dragDirection;
    private HexCell previousCell;

    private enum OptionalToggle
    { 
        Ignore, Yes, No
    }

    private OptionalToggle riverMode;

    private void Awake() 
	{
		SelectColor(0);
	}

	private void Update() 
	{
		if (Input.GetMouseButton(0) &&!EventSystem.current.IsPointerOverGameObject()) 
		{
			HandleInput();
		}
        else
        {
            previousCell = null;
        }
    }

	private void HandleInput() 
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit)) 
		{
            HexCell currentCell = hexGrid.GetCell(hit.point);
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    private void EditCells(HexCell cell)
    {
        if (applyColor)
        {
            cell.Color = activeColor;
        }

        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }
    }

    public void SelectColor(int index) 
	{
        applyColor = index >= 0;

        if (applyColor)
        {
            activeColor = colors[index];
        }
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }
}
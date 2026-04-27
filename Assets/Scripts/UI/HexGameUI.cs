using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HexGameUI : MonoBehaviour 
{
	private InputAction selectAction, commandAction, positionAction;

    public HexGrid grid;

	private HexCell currentCell;

	private HexUnit selectedUnit;

    private void Awake()
    {
        selectAction = InputSystem.actions.FindAction("Interact");
        commandAction = InputSystem.actions.FindAction("Command");
        positionAction = InputSystem.actions.FindAction("Position");
    }

    public void SetEditMode (bool toggle) 
	{
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
        if (toggle)
        {
            Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
        }
        else
        {
            Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
        }
    }

	private void Update () 
	{
		if (!EventSystem.current.IsPointerOverGameObject()) 
		{
            if (selectAction.WasPerformedThisFrame())
            {
                DoSelection();
            }
            else if (selectedUnit)
            {
                if (commandAction.WasPerformedThisFrame())
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }
	}

	private void DoSelection () 
	{
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) 
		{
			selectedUnit = currentCell.Unit;
		}
	}

	private void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    private void DoMove () 
	{
		if (grid.HasPath) 
		{
            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
		}
	}

	private bool UpdateCurrentCell () 
	{
		HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(positionAction.ReadValue<Vector2>()));
		if (cell != currentCell) 
		{
			currentCell = cell;
			return true;
		}

		return false;
	}
}
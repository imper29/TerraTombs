using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHandler : MonoBehaviour
{
    /// <summary>
    /// A delegate for mouse related events.
    /// </summary>
    /// <param name="mouseState">The state of the mouse.</param>
    public delegate void OnMouseDelegate(ref MouseState mouseState);
    /// <summary>
    /// A delegate for mouse button related events.
    /// </summary>
    /// <param name="mouseState">The state of the mouse.</param>
    /// <param name="buttons">Which buttons were changed / are held.</param>
    public delegate void OnMouseButtonDelegate(ref MouseState mouseState, MouseButton buttons);

    /// <summary>
    /// Called while the mouse moves.
    /// </summary>
    public static event OnMouseDelegate OnMouseMoved;
    /// <summary>
    /// Called when a mouse button is clicked.
    /// </summary>
    public static event OnMouseButtonDelegate OnButtonClicked;
    /// <summary>
    /// Called while a mouse button is held and the mouse is moved.
    /// </summary>
    public static event OnMouseButtonDelegate OnButtonDragged;
    /// <summary>
    /// Called when a mouse button is released.
    /// </summary>
    public static event OnMouseButtonDelegate OnButtonReleased;

    /// <summary>
    /// The position of the mouse in the last frame.
    /// </summary>
    private Vector2 lastMousePosition;
    /// <summary>
    /// The mouse buttons that were pressed in the last frame.
    /// </summary>
    private MouseButton lastMouseButtons;
    /// <summary>
    /// The times each mouse button was last pressed.
    /// </summary>
    private float leftClickTime, rightClickTime, middleClickTime;
    /// <summary>
    /// The positions each mouse button was last pressed.
    /// </summary>
    private Vector2 leftClickPosition, rightClickPosition, middleClickPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        //Get the mouse position.
        Vector2 mousePosition = Input.mousePosition;

        //Get the mouse state mask.
        MouseButton mouseState = MouseButton.None;
        if (Input.GetMouseButton(0))
            mouseState |= MouseButton.Left;
        if (Input.GetMouseButton(1))
            mouseState |= MouseButton.Right;
        if (Input.GetMouseButton(2))
            mouseState |= MouseButton.Middle;

        //Update the mouse click times and positions.
        if (Input.GetMouseButtonDown(0))
        {
            leftClickTime = Time.unscaledTime;
            leftClickPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClickTime = Time.unscaledTime;
            rightClickPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(2))
        {
            middleClickTime = Time.unscaledTime;
            middleClickPosition = Input.mousePosition;
        }

        //Create the mouse state for this frame.
        MouseState state = new MouseState(lastMousePosition, mousePosition, lastMouseButtons, mouseState, leftClickTime, rightClickTime, middleClickTime, leftClickPosition, rightClickPosition, middleClickPosition);

        //Check for mouse clicks.
        MouseButton buttons = state.GetClickedButtons();
        if (buttons != MouseButton.None)
            OnButtonClicked?.Invoke(ref state, buttons);
        //Check for mouse releases.
        buttons = state.GetReleasedButtons();
        if (buttons != MouseButton.None)
            OnButtonReleased?.Invoke(ref state, buttons);

        //Check for mouse movement.
        if (state.GetDeltaMousePosition() != Vector2.zero)
        {
            OnMouseMoved?.Invoke(ref state);

            //Check for held buttons and do the OnButtonDragged event..
            buttons = state.GetDraggedButtons();
            if (buttons != MouseButton.None)
                OnButtonDragged?.Invoke(ref state, buttons);
        }

        //Update the last mouse stats to be the current mouse stats.
        lastMousePosition = mousePosition;
        lastMouseButtons = mouseState;
    }
}

public struct MouseState
{
    /// <summary>
    /// Determines if the mouse is over a GUI element.
    /// </summary>
    private readonly bool mouseIsOverGUI;

    /// <summary>
    /// The position of the mouse in the last frame.
    /// </summary>
    private readonly Vector2 lastMousePosition;
    /// <summary>
    /// The position of the mouse in the current frame.
    /// </summary>
    private readonly Vector2 currentMousePosition;

    /// <summary>
    /// The mouse buttons that were pressed in the last frame.
    /// </summary>
    private readonly MouseButton lastButtons;
    /// <summary>
    /// The mouse buttons that were pressed in the current frame.
    /// </summary>
    private readonly MouseButton currentButtons;

    /// <summary>
    /// How long each mouse button has been held down.
    /// </summary>
    private readonly float leftClickDuration, rightClickDuration, middleClickDuration;
    /// <summary>
    /// The positions each mouse button was last pressed.
    /// </summary>
    private readonly Vector2 leftClickPosition, rightClickPosition, middleClickPosition;


    public MouseState(Vector2 lastMousePosition, Vector2 currentMousePosition, MouseButton lastButtons, MouseButton currentButtons, float leftClickTime, float rightClickTime, float middleClickTime, Vector2 leftClickPosition, Vector2 rightClickPosition, Vector2 middleClickPosition)
    {
        if (EventSystem.current != null)
            mouseIsOverGUI = EventSystem.current.IsPointerOverGameObject();
        else
            mouseIsOverGUI = false;

        this.lastMousePosition = lastMousePosition;
        this.currentMousePosition = currentMousePosition;

        this.lastButtons = lastButtons;
        this.currentButtons = currentButtons;

        this.leftClickPosition = leftClickPosition;
        this.rightClickPosition = rightClickPosition;
        this.middleClickPosition = middleClickPosition;

        leftClickDuration = Time.unscaledTime - leftClickTime;
        rightClickDuration = Time.unscaledTime - rightClickTime;
        middleClickDuration = Time.unscaledTime - middleClickTime;
    }


    /// <summary>
    /// Determines if the mouse is over a GUI element.
    /// </summary>
    /// <returns>True if the mouse is over a GUI element.</returns>
    public bool IsOverGUI()
    {
        return mouseIsOverGUI;
    }

    /// <summary>
    /// Gets the last position of the mouse.
    /// </summary>
    /// <returns>The last mouse position.</returns>
    public Vector2 GetLastMousePosition()
    {
        return lastMousePosition;
    }
    /// <summary>
    /// Gets the current position of the mouse.
    /// </summary>
    /// <returns>The current mouse position.</returns>
    public Vector2 GetCurrentMousePosition()
    {
        return currentMousePosition;
    }
    /// <summary>
    /// Gets the mouse's change in position.
    /// </summary>
    /// <returns>The mouse's change in position.</returns>
    public Vector2 GetDeltaMousePosition()
    {
        return currentMousePosition - lastMousePosition;
    }

    /// <summary>
    /// Gets all the buttons that were pressed this frame and were not pressed last frame.
    /// </summary>
    /// <returns>All the buttons that were just pressed this frame.</returns>
    public MouseButton GetClickedButtons()
    {
        return ~lastButtons & currentButtons;
    }
    /// <summary>
    /// Determines if a button in a button mask was clicked down in the current frame.
    /// </summary>
    /// <param name="buttonMask">The buttons to check.</param>
    /// <returns>True if any of the buttons from the buttonMask were just pressed this frame.</returns>
    public bool ButtonWasClicked(MouseButton buttonMask)
    {
        return (buttonMask & GetClickedButtons()) != MouseButton.None;
    }
    /// <summary>
    /// Finds the position a button was originally pressed.
    /// </summary>
    /// <param name="button">The button to find a click position for.</param>
    /// <returns>The position a button was originally pressed.</returns>
    public Vector2 GetClickedPosition(MouseButton button)
    {
        if (button == MouseButton.Left)
            return leftClickPosition;
        if (button == MouseButton.Right)
            return rightClickPosition;
        if (button == MouseButton.Middle)
            return middleClickPosition;

        return Vector2.zero;
    }

    /// <summary>
    /// Gets all the buttons that were released this frame.
    /// </summary>
    /// <returns>The buttons that were released this frame.</returns>
    public MouseButton GetReleasedButtons()
    {
        return lastButtons & ~currentButtons;
    }
    /// <summary>
    /// Determines if a button in a button mask was released in the current frame.
    /// </summary>
    /// <param name="buttonMask">The buttons to check.</param>
    /// <returns>True if any of the buttons from the buttonMask were just released this frame.</returns>
    public bool ButtonWasReleased(MouseButton buttonMask)
    {
        return (buttonMask & GetReleasedButtons()) != MouseButton.None;
    }

    /// <summary>
    /// Gets all the buttons that are pressed this frame.
    /// </summary>
    /// <returns>All the buttons that are pressed this frame.</returns>
    public MouseButton GetHeldButtons()
    {
        return currentButtons;
    }
    /// <summary>
    /// Finds how long a button has been pressed.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>How long a button has been pressed.</returns>
    public float GetHeldDuration(MouseButton button)
    {
        if (button == MouseButton.Left)
            return leftClickDuration;
        if (button == MouseButton.Right)
            return rightClickDuration;
        if (button == MouseButton.Middle)
            return middleClickDuration;

        return 0f;
    }

    /// <summary>
    /// Gets all the buttons that were pressed this frame and last frame.
    /// </summary>
    /// <returns></returns>
    public MouseButton GetDraggedButtons()
    {
        return lastButtons & currentButtons;
    }
}
[System.Flags]
public enum MouseButton
{
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Middle = 1 << 2
}
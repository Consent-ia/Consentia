using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenu : MonoBehaviour
{
    public void Menu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.ToggleMenu();
        }
    }
}

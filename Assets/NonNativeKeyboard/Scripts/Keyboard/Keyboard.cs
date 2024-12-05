using BNG;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keyboard control, key input processing
/// </summary>
namespace VR.Test.Keyboard
{
    public class Keyboard : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _inputField = null;
        [SerializeField]
        [Tooltip("Specify an InputActionSet for when using the Unity Input system. These actions will be enabled on load.")]
        public UnityEngine.InputSystem.InputActionAsset actionSet;
        private List<KeyboardButtonBase> _keyboardButtons = new();
        
        private KeyboardButtonBase _currentButton;
        
        private bool _IsShifted;
        private int _caretPosition;
        [SerializeField]
        private InputAction _leftThumbstick;
        [SerializeField]
        private InputAction _leftThumbstickDown;

        private void Start()
        {
            GetInputThumbstick();
        }

        private void GetInputThumbstick()
        {
            _leftThumbstick = InputBridge.Instance.CreateInputAction("leftThumbstick", "<XRController>{LeftHand}/{primary2DAxis}", true);
            _leftThumbstickDown = InputBridge.Instance.CreateInputAction("leftThumbstickDown", "<XRController>{LeftHand}/{primary2DAxisClick}", false);
            

            _leftThumbstick.performed += ctx => GetNextOrPreviousButton(ctx.ReadValue<Vector2>());
            _leftThumbstickDown.performed += ctx => OnClick();
        }

        /// <summary>
        /// Function to find the next or previous button in the specified direction
        /// </summary>
        private void GetNextOrPreviousButton(Vector2 direction)
        {
            if (_currentButton == null)
            {
                _currentButton = GetFirstButton();
                return;
            }

            var currentPosition = _currentButton.PositionOnKeyboard;
            var nextPosition = currentPosition + direction;

            // Handling column and row boundary transitions
            var maxXIndex = GetMaxXIndex(currentPosition.y);
            var maxYIndex = GetMaxYIndex(currentPosition.x);

            var wrapAroundX = (nextPosition.x < 0) ? maxXIndex : (nextPosition.x >= maxXIndex) ? 0 : nextPosition.x;
            var wrapAroundY = (nextPosition.y < 0) ? maxYIndex : (nextPosition.y >= maxXIndex) ? 0 : nextPosition.y;

            // Search for button in updated position
            var wrapAroundPosition = new Vector2(wrapAroundX, wrapAroundY);
            var wrapAroundButton = _keyboardButtons.FirstOrDefault(b => b.PositionOnKeyboard == wrapAroundPosition);

            if (wrapAroundButton != null)
            {
                _currentButton = wrapAroundButton;
                _currentButton.OnSelected();
            }
            else
            {
                // Find the closest button in the row if the button is not found at the updated position
                var nearestInRow = _keyboardButtons.FirstOrDefault(b => b.PositionOnKeyboard.y == wrapAroundPosition.y);

                if (nearestInRow != null)
                {
                    _currentButton = nearestInRow;
                    _currentButton.OnSelected();
                }
                else
                {
                    // Find the closest button in a column if the button is not found in the row or at the updated position
                    var nearestInColumn = _keyboardButtons.FirstOrDefault(b => b.PositionOnKeyboard.x == wrapAroundPosition.x);

                    if (nearestInColumn != null)
                    {
                        _currentButton = nearestInColumn;
                        _currentButton.OnSelected();
                    }
                }
            }
        }
        
        public void OnClick()
        {
            print("OnClick");
            if (_currentButton == null) 
                return;
            
            _currentButton.OnClick();
        }

        /// <summary>
        /// Get the first button when using manual keyboard input for the first time
        /// </summary>
        private KeyboardButtonBase GetFirstButton()
        {
            var button = _keyboardButtons.FirstOrDefault(b => b.PositionOnKeyboard == new Vector2Int(0, 0));
            button.OnSelected();
            return button;
        }

        private int GetMaxXIndex(int targetRow)
        {
            var maxXPosition = 0;

            foreach (var button in _keyboardButtons)
            {
                if (button.PositionOnKeyboard.y == targetRow && button.PositionOnKeyboard.x > maxXPosition)
                {
                    maxXPosition = button.PositionOnKeyboard.x;
                }
            }
            return maxXPosition + 1;
        }

        private int GetMaxYIndex(int targetColumn)
        {
            var maxYPosition = 0;

            foreach (var button in _keyboardButtons)
            {
                if (button.PositionOnKeyboard.x == targetColumn && button.PositionOnKeyboard.y > maxYPosition)
                {
                    maxYPosition = button.PositionOnKeyboard.y;
                }
            }
            return maxYPosition + 1;
        }

        private void UpdateCaretPosition(int newPos) => _inputField.caretPosition = newPos;

        /// <summary>
        /// Change case of buttons
        /// </summary>
        /// <param name="keyboardButton"></param>
        private void SwitchButtonsRegister()
        {
            foreach (var button in _keyboardButtons)
            {
                if (_IsShifted)
                    button.ToUpper();
                else
                    button.ToLower();
            }
        }
       
        /// <summary>
        /// Adds a button to the list of buttons
        /// </summary>
        /// <param name="keyboardButton"></param>
        public void AddButton(KeyboardButtonBase keyboardButton)
        {
            _keyboardButtons.Add(keyboardButton);
        }

        /// <summary>
        /// Add Symbol.
        /// </summary>
        public void AddSymbol(string symbol)
        {
            string value = symbol; // Initialize value with the original symbol
            char charToChange = symbol[0]; // Get the first character of the symbol

            // Check if the shift value is present and modify the case accordingly
            if (_IsShifted)
            {
                charToChange = char.ToUpper(charToChange); // Convert the character to uppercase
            }
            else
            {
                charToChange = char.ToLower(charToChange); // Convert the character to lowercase
            }

            value = charToChange.ToString(); // Convert the modified character back to a string

            _caretPosition = _inputField.caretPosition;

            _inputField.text = _inputField.text.Insert(_caretPosition, value);
            _caretPosition += value.Length;

            UpdateCaretPosition(_caretPosition);
        }

        /// <summary>
        /// Insert a space character.
        /// </summary>
        public void Space()
        {
            _caretPosition = _inputField.caretPosition;
            _inputField.text = _inputField.text.Insert(_caretPosition++, " ");

            UpdateCaretPosition(_caretPosition);
        }

        /// <summary>
        /// Set the keyboard to a single action shift state.
        /// </summary>
        public void Shift()
        {

            _IsShifted = !_IsShifted;
            SwitchButtonsRegister();
        }

        /// <summary>
        /// Delete the character before the caret.
        /// </summary>
        public void Backspace()
        {
            // check if text is selected
            if (_inputField.selectionFocusPosition != _inputField.caretPosition || _inputField.selectionAnchorPosition != _inputField.caretPosition)
            {
                if (_inputField.selectionAnchorPosition > _inputField.selectionFocusPosition) // right to left
                {
                    _inputField.text = _inputField.text.Substring(0, _inputField.selectionFocusPosition) + _inputField.text.Substring(_inputField.selectionAnchorPosition);
                    _inputField.caretPosition = _inputField.selectionFocusPosition;
                }
                else // left to right
                {
                    _inputField.text = _inputField.text.Substring(0, _inputField.selectionAnchorPosition) + _inputField.text.Substring(_inputField.selectionFocusPosition);
                    _inputField.caretPosition = _inputField.selectionAnchorPosition;
                }

                _caretPosition = _inputField.caretPosition;
                _inputField.selectionAnchorPosition = _caretPosition;
                _inputField.selectionFocusPosition = _caretPosition;
            }
            else
            {
                _caretPosition = _inputField.caretPosition;

                if (_caretPosition > 0)
                {
                    --_caretPosition;
                    _inputField.text = _inputField.text.Remove(_caretPosition, 1);
                    UpdateCaretPosition(_caretPosition);
                }
            }
        }

        /// <summary>
        /// Enter or submit.
        /// </summary>
        public void Enter()
        {
            print("Enter or submit");
        }
    }
}


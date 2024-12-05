/// <summary>
/// Backspace button
/// </summary>

namespace VR.Test.Keyboard
{
    public class KeyBackspace : KeyboardButtonBase
    {
        #region public methods

        public override void OnClick()
        {
            _keyboard.Backspace();
        }

        #endregion public methods
    }
}

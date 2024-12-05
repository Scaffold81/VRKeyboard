/// <summary>
/// Shift button
/// </summary>

namespace VR.Test.Keyboard
{
    public class KeyShift : KeyboardButtonBase
    {
        #region public methods

        public override void OnClick()
        {
            _keyboard.Shift();
        }

        #endregion public methods
    }
}

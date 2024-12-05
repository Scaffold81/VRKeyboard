/// <summary>
/// Shift button
/// </summary>
namespace VR.Test.Keyboard
{
    public class KeySpace : KeyboardButtonBase
    {
        #region public methods
        public override void OnClick()
        {
            _keyboard.Space();
        }
        #endregion public methods
    }
}

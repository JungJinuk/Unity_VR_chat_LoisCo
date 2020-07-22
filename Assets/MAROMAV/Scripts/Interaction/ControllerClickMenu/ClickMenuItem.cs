using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    /// Data structure to hold information about each menu item.
    [CreateAssetMenu(
      fileName = "ClickMenuItem",
      menuName = "MAROMAV/ClickMenu/Item",
      order = 1000)]
    /// Serializes a section of the menu.
    public class ClickMenuItem : ScriptableObject
    {
        [Tooltip("Assign a unique id for this item.")]
        public int id;
        [Tooltip("Foreground sprite")]
        public Sprite icon;
        [Tooltip("Optional background sprite")]
        public Sprite background;
        [Tooltip("Text shown on hover")]
        public string toolTip;
        public bool closeAfterSelected = true;
    }
}

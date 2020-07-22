
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    [CreateAssetMenu(
      fileName = "ClickMenuTree",
      menuName = "MAROMAV/ClickMenu/Tree",
      order = 1000)]
    /// Serializes the entire contents of a menu.
    public class ClickMenuTree : ClickMenuItem
    {
        [Tooltip("The contents of the menu")]
        public AssetTree tree;
    }
}

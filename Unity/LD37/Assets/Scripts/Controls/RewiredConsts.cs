/* Rewired Constants
   This list was generated on 12/10/2016 10:16:00 AM
   The list applies to only the Rewired Input Manager from which it was generated.
   If you use a different Rewired Input Manager, you will have to generate a new list.
   If you make changes to the exported items in the Rewired Input Manager, you will need to regenerate this list.
*/

namespace RewiredConsts {
    public static class Action {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move left & right")]
        public const int MoveHorizontal = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move up & down")]
        public const int MoveVertical = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Activate the currently carried item")]
        public const int Activate = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Picks up an item or drops the current item")]
        public const int PickupOrDrop = 3;
    }
    public static class Category {
        public const int Default = 0;
    }
    public static class Layout {
        public static class Joystick {
            public const int Default = 0;
        }
        public static class Keyboard {
            public const int Default = 0;
        }
        public static class Mouse {
            public const int Default = 0;
        }
        public static class CustomController {
            public const int Default = 0;
        }
    }
}

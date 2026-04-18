using Godot;
namespace LOSUtils
{
    public class UI {
        public static bool IsMouseOverUI(Viewport vwprt)
        {
            if (vwprt.GuiGetFocusOwner() != null)
                if (vwprt.GuiGetFocusOwner().Name.ToString().Contains("IGNORE"))
                    return false;
            return vwprt.GuiGetFocusOwner() != null;
        }
    }
    public class MathA
    {
        public static int Compare(int[] ticks, int tick)
        {
            int temp = 0;
            foreach (float t in ticks)
            {
                temp += t > tick ? 0:1; 
            }
            return temp;
        }
    }
}
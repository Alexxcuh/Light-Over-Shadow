using Godot;
namespace LOSUtils
{
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
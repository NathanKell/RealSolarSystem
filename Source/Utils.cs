using System.Linq;
using UnityEngine;

namespace RealSolarSystem
{
    public class Utils : MonoBehaviour
    {
        public static string Vec3ToString(Vector3 v)
        {
            return "(" + v.x + ", " + v.y + ", " + v.z + ")";
        }
        public static string Vec4ToString(Vector4 v)
        {
            return "(" + v.x + ", " + v.y + ", " + v.z + ", " + v.w + ")";
        }
        public static void PrintComponents(Transform t)
        {
            print("Transform " + t.name + " pos" + Vec3ToString(t.position) + ", lp" + Vec3ToString(t.localPosition) + ", scale" + Vec3ToString(t.lossyScale) +",  ls" + Vec3ToString(t.localScale) + " has components:");
            foreach (Component c in t.GetComponents(typeof(Component)).ToList())
            {
                print(c.name + " (" + c.GetType() + ")");
                if (c is MeshRenderer)
                    print(" ( material " + ((MeshRenderer)c).material.name + ", shader " + ((MeshRenderer)c).material.shader.name);
            }

        }
        public static void PrintTransformRecursive(Transform t)
        {
            PrintComponents(t);
            for (int i = 0; i < t.transform.childCount; i++)
                PrintTransformRecursive(t.transform.GetChild(i));
        }

        public static void PrintTransformUp(Transform t)
        {
            if(t.parent != null)
            {
                print("Parent of the above: " + t.parent.name);
                PrintTransformUp(t.parent);
            }
        }

        public static void dumpKeys(AnimationCurve c)
        {
            if (c == null)
                print("NULL");
            else if (c.keys.Length == 0)
                print("NO KEYS");
            else
                for (int i = 0; i < c.keys.Length; i++)
                    print("key," + i + " = " + c.keys[i].time + " " + c.keys[i].value + " " + c.keys[i].inTangent + " " + c.keys[i].outTangent);

        }
    }
}

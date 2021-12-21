using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial.Levels
{
    public static partial class Util
    {
        public static readonly Vector3Int Vector3IntForward = new Vector3Int(0, 0, 1);
        public static readonly Vector3Int Vector3IntBack = new Vector3Int(0, 0, -1);

        ///<summary>Gets the worldspace Vector3Int equivalent of the cardinal.</summary>
        ///<arg name=original>Has to be a singular value, not multiple flags</arg>
        public static Vector3Int GetDirection(this Cardinals original)
        {
            switch(original)
            {
                default:
                    return Vector3Int.zero;
                case Cardinals.North:
                    return Vector3IntForward;
                case Cardinals.East:
                    return Vector3Int.right;
                case Cardinals.South:
                    return Vector3IntBack;
                case Cardinals.West:
                    return Vector3Int.left;
            }
        }

        ///<summary>refer to GetDirection</summary>
        public static Vector3 GetDirectionF(this Cardinals original)
        {
            switch(original)
            {
                default:
                    return Vector3.zero;
                case Cardinals.North:
                    return Vector3.forward;
                case Cardinals.East:
                    return Vector3.right;
                case Cardinals.South:
                    return Vector3.back;
                case Cardinals.West:
                    return Vector3.left;
            }
        }

        public static Cardinals RotateBy90ClockWise(this Cardinals original)
        {
            if(original == Cardinals.Undefined) return Cardinals.Undefined;
            Cardinals rotated = 0;
            if(original.HasFlag(Cardinals.North))
                rotated |= Cardinals.East;
            if(original.HasFlag(Cardinals.East))
                rotated |= Cardinals.South;
            if(original.HasFlag(Cardinals.South))
                rotated |= Cardinals.West;
            if(original.HasFlag(Cardinals.West))
                rotated |= Cardinals.North;
            return rotated;
        }
        
        public static List<Cardinals> Seperate(this Cardinals original)
        {
            List<Cardinals> list = new List<Cardinals>();
            if(original.HasFlag(Cardinals.North))
                list.Add(Cardinals.North);
            if(original.HasFlag(Cardinals.East))
                list.Add(Cardinals.East);
            if(original.HasFlag(Cardinals.South))
                list.Add(Cardinals.South);
            if(original.HasFlag(Cardinals.West))
                list.Add(Cardinals.West);
            return list;
        }

        public static Cardinals Next(this Cardinals original)
        {
            switch(original)
            {
                default:
                    return Cardinals.Undefined;
                case Cardinals.North:
                    return Cardinals.East;
                case Cardinals.East:
                    return Cardinals.South;
                case Cardinals.South:
                    return Cardinals.West;
                case Cardinals.West:
                    return Cardinals.Undefined;
            }
        }

        //god i hate this, but its better than nesting switch cases.
        //There may be a better way of doing this with some index mapping and then getting the angle based in the index difference !!!
        public static float AngleTo(this Cardinals from, Cardinals to)
        {
            if(from == Cardinals.North)
                if(to == Cardinals.North)
                    return 0f;
                else if(to == Cardinals.East)
                    return 90f;
                else if(to == Cardinals.South)
                    return 180f;
                else
                    return 270f;
            else if(from == Cardinals.East)
                if(to == Cardinals.North)
                    return 270f;
                else if(to == Cardinals.East)
                    return 0f;
                else if(to == Cardinals.South)
                    return 90f;
                else
                    return 180f;
            else if(from == Cardinals.South)
                if(to == Cardinals.North)
                    return 180f;
                else if(to == Cardinals.East)
                    return 270f;
                else if(to == Cardinals.South)
                    return 0f;
                else
                    return 90f;
            else //West or undefined.
                if(to == Cardinals.North)
                    return 90f;
                else if(to == Cardinals.East)
                    return 180f;
                else if(to == Cardinals.South)
                    return 270f;
                else
                    return 0f;
        }

        //just a small helper to figure out whether this is a straight.
        public static bool DescribesStraight(this Cardinals cardinals)
         => cardinals.HasFlag(Cardinals.North | Cardinals.South) || cardinals.HasFlag(Cardinals.East | Cardinals.West);

        public static Cardinals Invert(this Cardinals original)
        {
            //this works because every cardinal has its counterpart two bits to the left/right. //mnope it doesnt work
            //combine the results with an or, and clear the remaining part with AND 31.
            //return original == Cardinals.Undefined? Cardinals.Undefined : (Cardinals)(((int)original << 2 | (int)original >> 2) & 0x01111);
            Cardinals output = Cardinals.None;
            if(original == Cardinals.Undefined)
                return Cardinals.Undefined;
            if(original.HasFlag(Cardinals.North))
                output |= Cardinals.South;
            if(original.HasFlag(Cardinals.East))
                output |= Cardinals.West;
            if(original.HasFlag(Cardinals.South))
                output |= Cardinals.North;
            if(original.HasFlag(Cardinals.West))
                output |= Cardinals.East;
            return output;
        }
    
        public static bool HasAnyFlag(this Cardinals original, Cardinals provider)
        {
            foreach(var c in provider.Seperate())
                if(original.HasFlag(c))
                    return true;
            return false;
        }
    }
}
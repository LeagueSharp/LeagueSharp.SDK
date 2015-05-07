/*******************************************************************************
*                                                                              *
* Author    :  Angus Johnson                                                   *
* Version   :  6.2.1                                                           *
* Date      :  31 October 2014                                                 *
* Website   :  http://www.angusj.com                                           *
* Copyright :  Angus Johnson 2010-2014                                         *
*                                                                              *
* License:                                                                     *
* Use, modification & distribution is subject to Boost Software License Ver 1. *
* http://www.boost.org/LICENSE_1_0.txt                                         *
*                                                                              *
* Attributions:                                                                *
* The code in this library is an extension of Bala Vatti's clipping algorithm: *
* "A generic solution to polygon clipping"                                     *
* Communications of the ACM, Vol 35, Issue 7 (July 1992) pp 56-63.             *
* http://portal.acm.org/citation.cfm?id=129906                                 *
*                                                                              *
* Computer graphics and geometric modeling: implementation and algorithms      *
* By Max K. Agoston                                                            *
* Springer; 1 edition (January 4, 2005)                                        *
* http://books.google.com/books?q=vatti+clipping+agoston                       *
*                                                                              *
* See also:                                                                    *
* "Polygon Offsetting by Computing Winding Numbers"                            *
* Paper no. DETC2005-85513 pp. 565-575                                         *
* ASME 2005 International Design Engineering Technical Conferences             *
* and Computers and Information in Engineering Conference (IDETC/CIE2005)      *
* September 24-28, 2005 , Long Beach, California, USA                          *
* http://www.me.berkeley.edu/~mcmains/pubs/DAC05OffsetPolygon.pdf              *
*                                                                              *
*******************************************************************************/

/*******************************************************************************
*                                                                              *
* This is a translation of the Delphi Clipper library and the naming style     *
* used has retained a Delphi flavour.                                          *
*                                                                              *
*******************************************************************************/

//use_int32: When enabled 32bit ints are used instead of 64bit ints. This
//improve performance but coordinate values are limited to the range +/- 46340
//#define use_int32

//use_xyz: adds a Z member to IntPoint. Adds a minor cost to performance.
//#define use_xyz

//use_lines: Enables open path clipping. Adds a very minor cost to performance.
//#define use_lines

//use_deprecated: Enables temporary support for the obsolete functions
//#define use_deprecated


using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueSharp.CommonEx.Clipper
{
#if use_int32
    using cInt = Int32;
#else
    using cInt = Int64;
#endif
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    ///     Points that are made out of doubles.
    /// </summary>
    public struct DoublePoint
    {
        /// <summary>
        ///     The x
        /// </summary>
        public double X;

        /// <summary>
        ///     The y
        /// </summary>
        public double Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoublePoint" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public DoublePoint(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoublePoint" /> struct.
        /// </summary>
        /// <param name="dp">The doublepoint.</param>
        public DoublePoint(DoublePoint dp)
        {
            X = dp.X;
            Y = dp.Y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoublePoint" /> struct.
        /// </summary>
        /// <param name="ip">The intpoint.</param>
        public DoublePoint(IntPoint ip)
        {
            X = ip.X;
            Y = ip.Y;
        }
    };


    //------------------------------------------------------------------------------
    // PolyTree & PolyNode classes
    //------------------------------------------------------------------------------

    /// <summary>
    ///     Tree of PolyNodes.
    /// </summary>
    public class PolyTree : PolyNode
    {
        internal List<PolyNode> MAllPolys = new List<PolyNode>();

        /// <summary>
        ///     Gets the total.
        /// </summary>
        /// <value>
        ///     The total.
        /// </value>
        public int Total
        {
            get
            {
                var result = MAllPolys.Count;
                //with negative offsets, ignore the hidden outer polygon ...
                if (result > 0 && MChilds[0] != MAllPolys[0])
                {
                    result--;
                }
                return result;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="PolyTree" /> class.
        /// </summary>
        ~PolyTree()
        {
            Clear();
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < MAllPolys.Count; i++)
            {
                MAllPolys[i] = null;
            }
            MAllPolys.Clear();
            MChilds.Clear();
        }

        /// <summary>
        ///     Gets the first.
        /// </summary>
        /// <returns></returns>
        public PolyNode GetFirst()
        {
            return MChilds.Count > 0 ? MChilds[0] : null;
        }
    }

    /// <summary>
    ///     A point at which lines or pathways intersect or branch, a central or connecting point.
    /// </summary>
    public class PolyNode
    {
        internal List<PolyNode> MChilds = new List<PolyNode>();
        internal EndType MEndtype;
        internal int MIndex;
        internal JoinType MJointype;
        internal PolyNode MParent;
        internal Path MPolygon = new Path();

        /// <summary>
        ///     Gets the child count.
        /// </summary>
        /// <value>
        ///     The child count.
        /// </value>
        public int ChildCount
        {
            get { return MChilds.Count; }
        }

        /// <summary>
        ///     Gets the contour.
        /// </summary>
        /// <value>
        ///     The contour.
        /// </value>
        public Path Contour
        {
            get { return MPolygon; }
        }

        /// <summary>
        ///     Gets the childs.
        /// </summary>
        /// <value>
        ///     The childs.
        /// </value>
        public List<PolyNode> Childs
        {
            get { return MChilds; }
        }

        /// <summary>
        ///     Gets the parent.
        /// </summary>
        /// <value>
        ///     The parent.
        /// </value>
        public PolyNode Parent
        {
            get { return MParent; }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is hole.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is hole; otherwise, <c>false</c>.
        /// </value>
        public bool IsHole
        {
            get { return IsHoleNode(); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; set; }

        private bool IsHoleNode()
        {
            var result = true;
            var node = MParent;
            while (node != null)
            {
                result = !result;
                node = node.MParent;
            }
            return result;
        }

        internal void AddChild(PolyNode child)
        {
            var cnt = MChilds.Count;
            MChilds.Add(child);
            child.MParent = this;
            child.MIndex = cnt;
        }

        /// <summary>
        ///     Gets the next.
        /// </summary>
        /// <returns></returns>
        public PolyNode GetNext()
        {
            return MChilds.Count > 0 ? MChilds[0] : GetNextSiblingUp();
        }

        internal PolyNode GetNextSiblingUp()
        {
            if (MParent == null)
            {
                return null;
            }
            return MIndex == MParent.MChilds.Count - 1 ? MParent.GetNextSiblingUp() : MParent.MChilds[MIndex + 1];
        }
    }


    //------------------------------------------------------------------------------
    // Int128 struct (enables safe math on signed 64bit integers)
    // eg Int128 val1((Int64)9223372036854775807); //ie 2^63 -1
    //    Int128 val2((Int64)9223372036854775807);
    //    Int128 val3 = val1 * val2;
    //    val3.ToString => "85070591730234615847396907784232501249" (8.5e+37)
    //------------------------------------------------------------------------------


    internal struct Int128
    {
        private long _hi;
        private ulong _lo;

        public Int128(long lo)
        {
            _lo = (ulong) lo;
            if (lo < 0)
            {
                _hi = -1;
            }
            else
            {
                _hi = 0;
            }
        }

        public Int128(long hi, ulong lo)
        {
            _lo = lo;
            _hi = hi;
        }

        public Int128(Int128 val)
        {
            _hi = val._hi;
            _lo = val._lo;
        }

        public bool IsNegative()
        {
            return _hi < 0;
        }

        public static bool operator ==(Int128 val1, Int128 val2)
        {
            return (object) val1 == (object) val2 || val1._hi == val2._hi && val1._lo == val2._lo;
        }

        public static bool operator !=(Int128 val1, Int128 val2)
        {
            return !(val1 == val2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Int128))
            {
                return false;
            }
            var i128 = (Int128) obj;
            return (i128._hi == _hi && i128._lo == _lo);
        }

        public override int GetHashCode()
        {
            return _hi.GetHashCode() ^ _lo.GetHashCode();
        }

        public static bool operator >(Int128 val1, Int128 val2)
        {
            if (val1._hi != val2._hi)
            {
                return val1._hi > val2._hi;
            }
            return val1._lo > val2._lo;
        }

        public static bool operator <(Int128 val1, Int128 val2)
        {
            return (val1._hi != val2._hi) ? val1._hi < val2._hi : val1._lo < val2._lo;
        }

        public static Int128 operator +(Int128 lhs, Int128 rhs)
        {
            lhs._hi += rhs._hi;
            lhs._lo += rhs._lo;
            if (lhs._lo < rhs._lo)
            {
                lhs._hi++;
            }
            return lhs;
        }

        public static Int128 operator -(Int128 lhs, Int128 rhs)
        {
            return lhs + -rhs;
        }

        public static Int128 operator -(Int128 val)
        {
            return val._lo == 0 ? new Int128(-val._hi, 0) : new Int128(~val._hi, ~val._lo + 1);
        }

        public static explicit operator double(Int128 val)
        {
            const double shift64 = 18446744073709551616.0; //2^64
            return val._hi < 0
                ? (val._lo == 0 ? val._hi * shift64 : -(~val._lo + ~val._hi * shift64))
                : val._lo + val._hi * shift64;
        }

        //nb: Constructing two new Int128 objects every time we want to multiply longs  
        //is slow. So, although calling the Int128Mul method doesn't look as clean, the 
        //code runs significantly faster than if we'd used the * operator.

        public static Int128 Int128Mul(long lhs, long rhs)
        {
            var negate = (lhs < 0) != (rhs < 0);
            if (lhs < 0)
            {
                lhs = -lhs;
            }
            if (rhs < 0)
            {
                rhs = -rhs;
            }
            var int1Hi = (ulong) lhs >> 32;
            var int1Lo = (ulong) lhs & 0xFFFFFFFF;
            var int2Hi = (ulong) rhs >> 32;
            var int2Lo = (ulong) rhs & 0xFFFFFFFF;

            //nb: see comments in clipper.pas
            var a = int1Hi * int2Hi;
            var b = int1Lo * int2Lo;
            var c = int1Hi * int2Lo + int1Lo * int2Hi;

            ulong lo;
            var hi = (long) (a + (c >> 32));

            unchecked
            {
                lo = (c << 32) + b;
            }
            if (lo < b)
            {
                hi++;
            }
            var result = new Int128(hi, lo);
            return negate ? -result : result;
        }
    };

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------


    /// <summary>
    ///     A point whose values are Integers.
    /// </summary>
    public struct IntPoint
    {
        /// <summary>
        ///     The X
        /// </summary>
        public cInt X;

        /// <summary>
        ///     The Y
        /// </summary>
        public cInt Y;

#if use_xyz
    /// <summary>
    ///     The Z
    /// </summary>
        public cInt Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public IntPoint(cInt x, cInt y, cInt z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public IntPoint(double x, double y, double z = 0)
        {
            X = (cInt) x;
            Y = (cInt) y;
            Z = (cInt) z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntPoint"/> struct.
        /// </summary>
        /// <param name="dp">The dp.</param>
        public IntPoint(DoublePoint dp)
        {
            X = (cInt) dp.X;
            Y = (cInt) dp.Y;
            Z = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntPoint"/> struct.
        /// </summary>
        /// <param name="pt">The pt.</param>
        public IntPoint(IntPoint pt)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
        }
#else

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntPoint" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public IntPoint(cInt x, cInt y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntPoint" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public IntPoint(double x, double y)
        {
            X = (cInt) x;
            Y = (cInt) y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntPoint" /> struct.
        /// </summary>
        /// <param name="pt">The pt.</param>
        public IntPoint(IntPoint pt)
        {
            X = pt.X;
            Y = pt.Y;
        }
#endif

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(IntPoint a, IntPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(IntPoint a, IntPoint b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IntPoint))
            {
                return false;
            }
            var a = (IntPoint) obj;
            return (X == a.X) && (Y == a.Y);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            //simply prevents a compiler warning
            return base.GetHashCode();
        }
    } // end struct IntPoint

    /// <summary>
    ///     A rectangle whose points are integers.
    /// </summary>
    public struct IntRect
    {
        /// <summary>
        ///     The bottom
        /// </summary>
        public cInt Bottom;

        /// <summary>
        ///     The left
        /// </summary>
        public cInt Left;

        /// <summary>
        ///     The right
        /// </summary>
        public cInt Right;

        /// <summary>
        ///     The top
        /// </summary>
        public cInt Top;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntRect" /> struct.
        /// </summary>
        /// <param name="l">The left.</param>
        /// <param name="t">The top.</param>
        /// <param name="r">The righ.</param>
        /// <param name="b">The bottom.</param>
        public IntRect(cInt l, cInt t, cInt r, cInt b)
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntRect" /> struct.
        /// </summary>
        /// <param name="ir">The <see cref="IntRect" />.</param>
        public IntRect(IntRect ir)
        {
            Left = ir.Left;
            Top = ir.Top;
            Right = ir.Right;
            Bottom = ir.Bottom;
        }
    }

    /// <summary>
    ///     The type of clipping.
    /// </summary>
    public enum ClipType
    {
        /// <summary>
        ///     Create regions where both subject and clip polygons are filled.
        /// </summary>
        CtIntersection,

        /// <summary>
        ///     Create regions where either subject or clip polygons (or both) are filled.
        /// </summary>
        CtUnion,

        /// <summary>
        ///     Create regions where subject polygons are filled except where clip polygons are filled
        /// </summary>
        CtDifference,

        /// <summary>
        ///     Create regions where either subject or clip polygons are filled but not where both are filled
        /// </summary>
        CtXor
    };

    /// <summary>
    ///     The type of polygon.
    /// </summary>
    public enum PolyType
    {
        /// <summary>
        ///     Subject
        /// </summary>
        PtSubject,

        /// <summary>
        ///     Clip
        /// </summary>
        PtClip
    };

    //By far the most widely used winding rules for polygon filling are
    //EvenOdd & NonZero (GDI, GDI+, XLib, OpenGL, Cairo, AGG, Quartz, SVG, Gr32)
    //Others rules include Positive, Negative and ABS_GTR_EQ_TWO (only in OpenGL)
    //see http://glprogramming.com/red/chapter11.html

    /// <summary>
    ///     The type of winding rules for polygon filling.
    /// </summary>
    public enum PolyFillType
    {
        /// <summary>
        ///     Also known as Alternate Filling. Odd numbered sub-regions are filled, while even numbered sub-regions are not.
        /// </summary>
        PftEvenOdd,

        /// <summary>
        ///     All non-zero sub-regions are filled.
        /// </summary>
        PftNonZero,

        /// <summary>
        ///     All sub-regions with winding counts > 0 are filled.
        /// </summary>
        PftPositive,

        /// <summary>
        ///     All sub-regions with winding counts &lt; 0 are filled
        /// </summary>
        PftNegative
    };

    /// <summary>
    ///     Type of joining.
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        ///     Squaring is applied uniformally at all convex edge joins at 1 x delta.
        /// </summary>
        JtSquare,

        /// <summary>
        ///     While flattened paths can never perfectly trace an arc, they are approximated by a series of arc chords. <see cref="ClipperOffset.ArcTolerance"/>
        /// </summary>
        JtRound,

        /// <summary>
        ///     There's a necessary limit to mitered joins since offsetting edges that join at very acute angles will produce
        ///     excessively long and narrow 'spikes'. To contain these potential spikes, the <see cref="ClipperOffset.MiterLimit"/>
        ///     property specifies a maximum distance that vertices will be offset (in multiples of delta). For any given edge
        ///     join, when miter offsetting would exceed that maximum distance, 'square' joining is applied.
        /// </summary>
        JtMiter
    };

    /// <summary>
    ///     Type of end.
    /// </summary>
    public enum EndType
    {
        /// <summary>
        ///     Ends are joined using the JoinType value and the path filled as a polygon.
        /// </summary>
        EtClosedPolygon,

        /// <summary>
        ///     Ends are joined using the JoinType value and the path filled as a polyline.
        /// </summary>
        EtClosedLine,

        /// <summary>
        ///     Ends are squared off with no extension.
        /// </summary>
        EtOpenButt,

        /// <summary>
        ///    Ends are squared off and extended delta units.
        /// </summary>
        EtOpenSquare,

        /// <summary>
        ///     Ends are rounded off and extended delta units.
        /// </summary>
        EtOpenRound
    };

    internal enum EdgeSide
    {
        EsLeft,
        EsRight
    };

    internal enum Direction
    {
        DRightToLeft,
        DLeftToRight
    };

    internal class Edge
    {
        internal IntPoint Bot;
        internal IntPoint Curr;
        internal IntPoint Delta;
        internal double Dx;
        internal Edge Next;
        internal Edge NextInAel;
        internal Edge NextInLml;
        internal Edge NextInSel;
        internal int OutIdx;
        internal PolyType PolyTyp;
        internal Edge Prev;
        internal Edge PrevInAel;
        internal Edge PrevInSel;
        internal EdgeSide Side;
        internal IntPoint Top;
        internal int WindCnt;
        internal int WindCnt2; //winding count of the opposite polytype
        internal int WindDelta; //1 or -1 depending on winding direction
    };

    /// <summary>
    ///     A point at which lines intersect.
    /// </summary>
    public class IntersectNode
    {
        internal Edge Edge1;
        internal Edge Edge2;
        internal IntPoint Pt;
    };

    /// <summary>
    ///     Compares <see cref="IntersectNode" />s for the .Sort method.
    /// </summary>
    public class MyIntersectNodeSort : IComparer<IntersectNode>
    {
        /// <summary>
        ///     Compares the specified nodes.
        /// </summary>
        /// <param name="node1">The node1.</param>
        /// <param name="node2">The node2.</param>
        /// <returns></returns>
        public int Compare(IntersectNode node1, IntersectNode node2)
        {
            var i = node2.Pt.Y - node1.Pt.Y;
            if (i > 0)
            {
                return 1;
            }
            if (i < 0)
            {
                return -1;
            }
            return 0;
        }
    }

    internal class LocalMinima
    {
        internal Edge LeftBound;
        internal LocalMinima Next;
        internal Edge RightBound;
        internal cInt Y;
    };

    internal class Scanbeam
    {
        internal Scanbeam Next;
        internal cInt Y;
    };

    internal class OutRec
    {
        internal OutPt BottomPt;
        internal OutRec FirstLeft; //see comments in clipper.pas
        internal int Idx;
        internal bool IsHole;
        internal bool IsOpen;
        internal PolyNode PolyNode;
        internal OutPt Pts;
    };

    internal class OutPt
    {
        internal int Idx;
        internal OutPt Next;
        internal OutPt Prev;
        internal IntPoint Pt;
    };

    internal class Join
    {
        internal IntPoint OffPt;
        internal OutPt OutPt1;
        internal OutPt OutPt2;
    };

    /// <summary>
    ///     Base clipper.
    /// </summary>
    public class ClipperBase
    {
        /// <summary>
        ///     The horizontal
        /// </summary>
        protected const double Horizontal = -3.4E+38;

        /// <summary>
        ///     The skip
        /// </summary>
        protected const int Skip = -2;

        /// <summary>
        ///     The unassigned
        /// </summary>
        protected const int Unassigned = -1;

        /// <summary>
        ///     The tolerance
        /// </summary>
        protected const double Tolerance = 1.0E-20;

        internal static bool NearZero(double val)
        {
            return (val > -Tolerance) && (val < Tolerance);
        }

#if use_int32
    /// <summary>
    ///     The low range
    /// </summary>
        public const cInt LoRange = 0x7FFF;

        /// <summary>
        ///     The high range
        /// </summary>
        public const cInt HiRange = 0x7FFF;
#else
        /// <summary>
        ///     The low range
        /// </summary>
        public const cInt LoRange = 0x3FFFFFFF;

        /// <summary>
        ///     The high range
        /// </summary>
        public const cInt HiRange = 0x3FFFFFFFFFFFFFFFL;
#endif

        internal LocalMinima MMinimaList;
        internal LocalMinima MCurrentLm;
        internal List<List<Edge>> MEdges = new List<List<Edge>>();
        internal bool MUseFullRange;
        internal bool MHasOpenPaths;

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets or sets a value indicating whether to preserve the collinear.
        /// </summary>
        /// <value>
        ///     <c>true</c> if preserve the collinear; otherwise, <c>false</c>.
        /// </value>
        public bool PreserveCollinear { get; set; }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Swaps the specified value.
        /// </summary>
        /// <param name="val1">Value 1.</param>
        /// <param name="val2">Value 2.</param>
        public void Swap(ref cInt val1, ref cInt val2)
        {
            var tmp = val1;
            val1 = val2;
            val2 = tmp;
        }

        //------------------------------------------------------------------------------

        internal static bool IsHorizontal(Edge e)
        {
            return e.Delta.Y == 0;
        }

        //------------------------------------------------------------------------------

        internal bool PointIsVertex(IntPoint pt, OutPt pp)
        {
            var pp2 = pp;
            do
            {
                if (pp2.Pt == pt)
                {
                    return true;
                }
                pp2 = pp2.Next;
            } while (pp2 != pp);
            return false;
        }

        //------------------------------------------------------------------------------

        internal bool PointOnLineSegment(IntPoint pt, IntPoint linePt1, IntPoint linePt2, bool useFullRange)
        {
            if (useFullRange)
            {
                return ((pt.X == linePt1.X) && (pt.Y == linePt1.Y)) || ((pt.X == linePt2.X) && (pt.Y == linePt2.Y)) ||
                       (((pt.X > linePt1.X) == (pt.X < linePt2.X)) && ((pt.Y > linePt1.Y) == (pt.Y < linePt2.Y)) &&
                        ((Int128.Int128Mul((pt.X - linePt1.X), (linePt2.Y - linePt1.Y)) ==
                          Int128.Int128Mul((linePt2.X - linePt1.X), (pt.Y - linePt1.Y)))));
            }
            return ((pt.X == linePt1.X) && (pt.Y == linePt1.Y)) || ((pt.X == linePt2.X) && (pt.Y == linePt2.Y)) ||
                   (((pt.X > linePt1.X) == (pt.X < linePt2.X)) && ((pt.Y > linePt1.Y) == (pt.Y < linePt2.Y)) &&
                    ((pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y)));
        }

        //------------------------------------------------------------------------------

        internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool useFullRange)
        {
            var pp2 = pp;
            while (true)
            {
                if (PointOnLineSegment(pt, pp2.Pt, pp2.Next.Pt, useFullRange))
                {
                    return true;
                }
                pp2 = pp2.Next;
                if (pp2 == pp)
                {
                    break;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------

        internal static bool SlopesEqual(Edge e1, Edge e2, bool useFullRange)
        {
            if (useFullRange)
            {
                return Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
            }
            return e1.Delta.Y * (e2.Delta.X) == e1.Delta.X * (e2.Delta.Y);
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Checks if the slope is equal.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="pt3">The PT3.</param>
        /// <param name="useFullRange">if set to <c>true</c>, will use the full range.</param>
        /// <returns></returns>
        protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool useFullRange)
        {
            if (useFullRange)
            {
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
            }
            return (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Checks if the slopes are equaal.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="pt3">The PT3.</param>
        /// <param name="pt4">The PT4.</param>
        /// <param name="useFullRange">if set to <c>true</c>, will use full range.</param>
        /// <returns></returns>
        protected static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool useFullRange)
        {
            if (useFullRange)
            {
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
            }
            return (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0;
        }

        //------------------------------------------------------------------------------

        internal ClipperBase() //constructor (nb: no external instantiation)
        {
            MMinimaList = null;
            MCurrentLm = null;
            MUseFullRange = false;
            MHasOpenPaths = false;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            DisposeLocalMinimaList();
            foreach (var t in MEdges)
            {
                for (var j = 0; j < t.Count; ++j)
                {
                    t[j] = null;
                }
                t.Clear();
            }
            MEdges.Clear();
            MUseFullRange = false;
            MHasOpenPaths = false;
        }

        //------------------------------------------------------------------------------

        private void DisposeLocalMinimaList()
        {
            while (MMinimaList != null)
            {
                var tmpLm = MMinimaList.Next;
                MMinimaList = null;
                MMinimaList = tmpLm;
            }
            MCurrentLm = null;
        }

        //------------------------------------------------------------------------------

        private void RangeTest(IntPoint pt, ref bool useFullRange)
        {
            if (useFullRange)
            {
                if (pt.X > HiRange || pt.Y > HiRange || -pt.X > HiRange || -pt.Y > HiRange)
                {
                    throw new ClipperException("Coordinate outside allowed range");
                }
            }
            else if (pt.X > LoRange || pt.Y > LoRange || -pt.X > LoRange || -pt.Y > LoRange)
            {
                useFullRange = true;
                RangeTest(pt, ref useFullRange);
            }
        }

        //------------------------------------------------------------------------------

        private void InitEdge(Edge e, Edge eNext, Edge ePrev, IntPoint pt)
        {
            e.Next = eNext;
            e.Prev = ePrev;
            e.Curr = pt;
            e.OutIdx = Unassigned;
        }

        //------------------------------------------------------------------------------

        private void InitEdge2(Edge e, PolyType polyType)
        {
            if (e.Curr.Y >= e.Next.Curr.Y)
            {
                e.Bot = e.Curr;
                e.Top = e.Next.Curr;
            }
            else
            {
                e.Top = e.Curr;
                e.Bot = e.Next.Curr;
            }
            SetDx(e);
            e.PolyTyp = polyType;
        }

        //------------------------------------------------------------------------------

        private static Edge FindNextLocMin(Edge e)
        {
            for (;;)
            {
                while (e.Bot != e.Prev.Bot || e.Curr == e.Top)
                {
                    e = e.Next;
                }
                if (Math.Abs(e.Dx - Horizontal) > float.Epsilon && Math.Abs(e.Prev.Dx - Horizontal) > float.Epsilon)
                {
                    break;
                }
                while (Math.Abs(e.Prev.Dx - Horizontal) < float.Epsilon)
                {
                    e = e.Prev;
                }
                var e2 = e;
                while (Math.Abs(e.Dx - Horizontal) < float.Epsilon)
                {
                    e = e.Next;
                }
                if (e.Top.Y == e.Prev.Bot.Y)
                {
                    continue; //ie just an intermediate horz.
                }
                if (e2.Prev.Bot.X < e.Bot.X)
                {
                    e = e2;
                }
                break;
            }
            return e;
        }

        //------------------------------------------------------------------------------

        private Edge ProcessBound(Edge e, bool leftBoundIsForward)
        {
            Edge eStart, result = e;
            Edge horz;

            if (result.OutIdx == Skip)
            {
                //check if there are edges beyond the skip edge in the bound and if so
                //create another LocMin and calling ProcessBound once more ...
                e = result;
                if (leftBoundIsForward)
                {
                    while (e.Top.Y == e.Next.Bot.Y)
                    {
                        e = e.Next;
                    }
                    while (e != result && Math.Abs(e.Dx - Horizontal) < float.Epsilon)
                    {
                        e = e.Prev;
                    }
                }
                else
                {
                    while (e.Top.Y == e.Prev.Bot.Y)
                    {
                        e = e.Prev;
                    }
                    while (e != result && Math.Abs(e.Dx - Horizontal) < float.Epsilon)
                    {
                        e = e.Next;
                    }
                }
                if (e == result)
                {
                    result = leftBoundIsForward ? e.Next : e.Prev;
                }
                else
                {
                    //there are more edges in the bound beyond result starting with E
                    e = leftBoundIsForward ? result.Next : result.Prev;
                    var locMin = new LocalMinima { Next = null, Y = e.Bot.Y, LeftBound = null, RightBound = e };
                    e.WindDelta = 0;
                    result = ProcessBound(e, leftBoundIsForward);
                    InsertLocalMinima(locMin);
                }
                return result;
            }

            if (Math.Abs(e.Dx - Horizontal) < float.Epsilon)
            {
                //We need to be careful with open paths because this may not be a
                //true local minima (ie E may be following a skip edge).
                //Also, consecutive horz. edges may start heading left before going right.
                eStart = leftBoundIsForward ? e.Prev : e.Next;
                if (eStart.OutIdx != Skip)
                {
                    if (Math.Abs(eStart.Dx - Horizontal) < float.Epsilon) //ie an adjoining horizontal skip edge
                    {
                        if (eStart.Bot.X != e.Bot.X && eStart.Top.X != e.Bot.X)
                        {
                            ReverseHorizontal(e);
                        }
                    }
                    else if (eStart.Bot.X != e.Bot.X)
                    {
                        ReverseHorizontal(e);
                    }
                }
            }

            eStart = e;
            if (leftBoundIsForward)
            {
                while (result.Top.Y == result.Next.Bot.Y && result.Next.OutIdx != Skip)
                {
                    result = result.Next;
                }
                if (Math.Abs(result.Dx - Horizontal) < float.Epsilon && result.Next.OutIdx != Skip)
                {
                    //nb: at the top of a bound, horizontals are added to the bound
                    //only when the preceding edge attaches to the horizontal's left vertex
                    //unless a Skip edge is encountered when that becomes the top divide
                    horz = result;
                    while (Math.Abs(horz.Prev.Dx - Horizontal) < float.Epsilon)
                    {
                        horz = horz.Prev;
                    }
                    if (horz.Prev.Top.X > result.Next.Top.X)
                    {
                        result = horz.Prev;
                    }
                }
                while (e != result)
                {
                    e.NextInLml = e.Next;
                    if (Math.Abs(e.Dx - Horizontal) < float.Epsilon && e != eStart && e.Bot.X != e.Prev.Top.X)
                    {
                        ReverseHorizontal(e);
                    }
                    e = e.Next;
                }
                if (Math.Abs(e.Dx - Horizontal) < float.Epsilon && e != eStart && e.Bot.X != e.Prev.Top.X)
                {
                    ReverseHorizontal(e);
                }
                result = result.Next; //move to the edge just beyond current bound
            }
            else
            {
                while (result.Top.Y == result.Prev.Bot.Y && result.Prev.OutIdx != Skip)
                {
                    result = result.Prev;
                }
                if (Math.Abs(result.Dx - Horizontal) < float.Epsilon && result.Prev.OutIdx != Skip)
                {
                    horz = result;
                    while (Math.Abs(horz.Next.Dx - Horizontal) < float.Epsilon)
                    {
                        horz = horz.Next;
                    }
                    if (horz.Next.Top.X == result.Prev.Top.X)
                    {
                        result = horz.Next;
                    }
                    else if (horz.Next.Top.X > result.Prev.Top.X)
                    {
                        result = horz.Next;
                    }
                }

                while (e != result)
                {
                    e.NextInLml = e.Prev;
                    if (Math.Abs(e.Dx - Horizontal) < float.Epsilon && e != eStart && e.Bot.X != e.Next.Top.X)
                    {
                        ReverseHorizontal(e);
                    }
                    e = e.Prev;
                }
                if (Math.Abs(e.Dx - Horizontal) < float.Epsilon && e != eStart && e.Bot.X != e.Next.Top.X)
                {
                    ReverseHorizontal(e);
                }
                result = result.Prev; //move to the edge just beyond current bound
            }
            return result;
        }

        //------------------------------------------------------------------------------


        /// <summary>
        ///     Adds the path.
        /// </summary>
        /// <param name="pg">The path.</param>
        /// <param name="polyType">Type of the polygpm.</param>
        /// <param name="closed">Gets of the path is closed or not.</param>
        /// <returns></returns>
        /// <exception cref="ClipperException">Open paths have been disabled.</exception>
        public bool AddPath(Path pg, PolyType polyType, bool closed)
        {
#if use_lines
      if (!closed && polyType == PolyType.PtClip)
        throw new ClipperException("AddPath: Open paths must be subject.");
#else
            if (!closed)
            {
                throw new ClipperException("AddPath: Open paths have been disabled.");
            }
#endif

            var highI = pg.Count - 1;
            while (highI > 0 && (pg[highI] == pg[0]))
            {
                --highI;
            }
            while (highI > 0 && (pg[highI] == pg[highI - 1]))
            {
                --highI;
            }
            if ((highI < 2))
            {
                return false;
            }

            //create a new edge array ...
            var edges = new List<Edge>(highI + 1);
            for (var i = 0; i <= highI; i++)
            {
                edges.Add(new Edge());
            }

            var isFlat = true;

            //1. Basic (first) edge initialization ...
            edges[1].Curr = pg[1];
            RangeTest(pg[0], ref MUseFullRange);
            RangeTest(pg[highI], ref MUseFullRange);
            InitEdge(edges[0], edges[1], edges[highI], pg[0]);
            InitEdge(edges[highI], edges[0], edges[highI - 1], pg[highI]);
            for (var i = highI - 1; i >= 1; --i)
            {
                RangeTest(pg[i], ref MUseFullRange);
                InitEdge(edges[i], edges[i + 1], edges[i - 1], pg[i]);
            }
            var eStart = edges[0];

            //2. Remove duplicate vertices, and (when closed) collinear edges ...
            Edge e = eStart, eLoopStop = eStart;
            for (;;)
            {
                //nb: allows matching start and end points when not Closed ...
                if (e.Curr == e.Next.Curr)
                {
                    if (e == e.Next)
                    {
                        break;
                    }
                    if (e == eStart)
                    {
                        eStart = e.Next;
                    }
                    e = RemoveEdge(e);
                    eLoopStop = e;
                    continue;
                }
                if (e.Prev == e.Next)
                {
                    break; //only two vertices
                }
                if (SlopesEqual(e.Prev.Curr, e.Curr, e.Next.Curr, MUseFullRange) &&
                    (!PreserveCollinear || !Pt2IsBetweenPt1AndPt3(e.Prev.Curr, e.Curr, e.Next.Curr)))
                {
                    //Collinear edges are allowed for open paths but in closed paths
                    //the default is to merge adjacent collinear edges into a single edge.
                    //However, if the PreserveCollinear property is enabled, only overlapping
                    //collinear edges (ie spikes) will be removed from closed paths.
                    if (e == eStart)
                    {
                        eStart = e.Next;
                    }
                    e = RemoveEdge(e);
                    e = e.Prev;
                    eLoopStop = e;
                    continue;
                }
                e = e.Next;
                if ((e == eLoopStop))
                {
                    break;
                }
            }

            if (((e.Prev == e.Next)))
            {
                return false;
            }

            //3. Do second stage of edge initialization ...
            e = eStart;
            do
            {
                InitEdge2(e, polyType);
                e = e.Next;
                if (isFlat && e.Curr.Y != eStart.Curr.Y)
                {
                    isFlat = false;
                }
            } while (e != eStart);

            //4. Finally, add edge bounds to LocalMinima list ...

            //Totally flat paths must be handled differently when adding them
            //to LocalMinima list to avoid endless loops etc ...
            if (isFlat)
            {
                return false;
            }

            MEdges.Add(edges);
            Edge eMin = null;

            //workaround to avoid an endless loop in the while loop below when
            //open paths have matching start and end points ...
            if (e.Prev.Bot == e.Prev.Top)
            {
                e = e.Next;
            }

            for (;;)
            {
                e = FindNextLocMin(e);
                if (e == eMin)
                {
                    break;
                }
                if (eMin == null)
                {
                    eMin = e;
                }

                //E and E.Prev now share a local minima (left aligned if horizontal).
                //Compare their slopes to find which starts which bound ...
                var locMin = new LocalMinima { Next = null, Y = e.Bot.Y };
                bool leftBoundIsForward;
                if (e.Dx < e.Prev.Dx)
                {
                    locMin.LeftBound = e.Prev;
                    locMin.RightBound = e;
                    leftBoundIsForward = false; //Q.nextInLML = Q.prev
                }
                else
                {
                    locMin.LeftBound = e;
                    locMin.RightBound = e.Prev;
                    leftBoundIsForward = true; //Q.nextInLML = Q.next
                }
                locMin.LeftBound.Side = EdgeSide.EsLeft;
                locMin.RightBound.Side = EdgeSide.EsRight;

                if (locMin.LeftBound.Next == locMin.RightBound)
                {
                    locMin.LeftBound.WindDelta = -1;
                }
                else
                {
                    locMin.LeftBound.WindDelta = 1;
                }
                locMin.RightBound.WindDelta = -locMin.LeftBound.WindDelta;

                e = ProcessBound(locMin.LeftBound, leftBoundIsForward);
                if (e.OutIdx == Skip)
                {
                    e = ProcessBound(e, leftBoundIsForward);
                }

                var e2 = ProcessBound(locMin.RightBound, !leftBoundIsForward);
                if (e2.OutIdx == Skip)
                {
                    e2 = ProcessBound(e2, !leftBoundIsForward);
                }

                if (locMin.LeftBound.OutIdx == Skip)
                {
                    locMin.LeftBound = null;
                }
                else if (locMin.RightBound.OutIdx == Skip)
                {
                    locMin.RightBound = null;
                }
                InsertLocalMinima(locMin);
                if (!leftBoundIsForward)
                {
                    e = e2;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Adds the paths.
        /// </summary>
        /// <param name="ppg">The paths.</param>
        /// <param name="polyType">Type of the poly.</param>
        /// <param name="closed">if set to <c>true</c>, closes the path.</param>
        /// <returns></returns>
        public bool AddPaths(Paths ppg, PolyType polyType, bool closed)
        {
            return ppg.Any(t => AddPath(t, polyType, closed));
        }

        //------------------------------------------------------------------------------

        internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
        {
            if ((pt1 == pt3) || (pt1 == pt2) || (pt3 == pt2))
            {
                return false;
            }
            if (pt1.X != pt3.X)
            {
                return (pt2.X > pt1.X) == (pt2.X < pt3.X);
            }
            return (pt2.Y > pt1.Y) == (pt2.Y < pt3.Y);
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Removes an edge.
        /// </summary>
        /// <param name="e">The edge.</param>
        /// <returns></returns>
        private Edge RemoveEdge(Edge e)
        {
            //removes e from double_linked_list (but without removing from memory)
            e.Prev.Next = e.Next;
            e.Next.Prev = e.Prev;
            var result = e.Next;
            e.Prev = null; //flag as removed (see ClipperBase.Clear)
            return result;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Sets the delta x.
        /// </summary>
        /// <param name="e">The edge.</param>
        private void SetDx(Edge e)
        {
            e.Delta.X = (e.Top.X - e.Bot.X);
            e.Delta.Y = (e.Top.Y - e.Bot.Y);
            if (e.Delta.Y == 0)
            {
                e.Dx = Horizontal;
            }
            else
            {
                e.Dx = (double) (e.Delta.X) / (e.Delta.Y);
            }
        }

        //---------------------------------------------------------------------------

        private void InsertLocalMinima(LocalMinima newLm)
        {
            if (MMinimaList == null)
            {
                MMinimaList = newLm;
            }
            else if (newLm.Y >= MMinimaList.Y)
            {
                newLm.Next = MMinimaList;
                MMinimaList = newLm;
            }
            else
            {
                var tmpLm = MMinimaList;
                while (tmpLm.Next != null && (newLm.Y < tmpLm.Next.Y))
                {
                    tmpLm = tmpLm.Next;
                }
                newLm.Next = tmpLm.Next;
                tmpLm.Next = newLm;
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Pops the local minima.
        /// </summary>
        protected void PopLocalMinima()
        {
            if (MCurrentLm == null)
            {
                return;
            }
            MCurrentLm = MCurrentLm.Next;
        }

        //------------------------------------------------------------------------------

        private void ReverseHorizontal(Edge e)
        {
            //swap horizontal edges' top and bottom x's so they follow the natural
            //progression of the bounds - ie so their xbots will align with the
            //adjoining lower edge. [Helpful in the ProcessHorizontal() method.]
            Swap(ref e.Top.X, ref e.Bot.X);
#if use_xyz
      Swap(ref e.Top.Z, ref e.Bot.Z);
#endif
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        protected virtual void Reset()
        {
            MCurrentLm = MMinimaList;
            if (MCurrentLm == null)
            {
                return; //ie nothing to process
            }

            //reset all edges ...
            var lm = MMinimaList;
            while (lm != null)
            {
                var e = lm.LeftBound;
                if (e != null)
                {
                    e.Curr = e.Bot;
                    e.Side = EdgeSide.EsLeft;
                    e.OutIdx = Unassigned;
                }
                e = lm.RightBound;
                if (e != null)
                {
                    e.Curr = e.Bot;
                    e.Side = EdgeSide.EsRight;
                    e.OutIdx = Unassigned;
                }
                lm = lm.Next;
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets the bounds.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public static IntRect GetBounds(Paths paths)
        {
            int i = 0, cnt = paths.Count;
            while (i < cnt && paths[i].Count == 0)
            {
                i++;
            }
            if (i == cnt)
            {
                return new IntRect(0, 0, 0, 0);
            }
            var result = new IntRect { Left = paths[i][0].X };
            result.Right = result.Left;
            result.Top = paths[i][0].Y;
            result.Bottom = result.Top;
            for (; i < cnt; i++)
            {
                for (var j = 0; j < paths[i].Count; j++)
                {
                    if (paths[i][j].X < result.Left)
                    {
                        result.Left = paths[i][j].X;
                    }
                    else if (paths[i][j].X > result.Right)
                    {
                        result.Right = paths[i][j].X;
                    }
                    if (paths[i][j].Y < result.Top)
                    {
                        result.Top = paths[i][j].Y;
                    }
                    else if (paths[i][j].Y > result.Bottom)
                    {
                        result.Bottom = paths[i][j].Y;
                    }
                }
            }
            return result;
        }
    } //end ClipperBase

    /// <summary>
    ///     Clips polygons.
    /// </summary>
    public class Clipper : ClipperBase
    {
        //InitOptions that can be passed to the constructor ...
        /// <summary>
        ///     Reverses the solution
        /// </summary>
        public const int IoReverseSolution = 1;

        /// <summary>
        ///     Makes the clipping scrictly simple.
        /// </summary>
        public const int IoStrictlySimple = 2;

        /// <summary>
        ///     Perserves the collinear.
        /// </summary>
        public const int IoPreserveCollinear = 4;

        private readonly List<OutRec> _mPolyOuts;
        private ClipType _mClipType;
        private Scanbeam _mScanbeam;
        private Edge _mActiveEdges;
        private Edge _mSortedEdges;
        private readonly List<IntersectNode> _mIntersectList;
        private readonly IComparer<IntersectNode> _mIntersectNodeComparer;
        private bool _mExecuteLocked;
        private PolyFillType _mClipFillType;
        private PolyFillType _mSubjFillType;
        private readonly List<Join> _mJoins;
        private readonly List<Join> _mGhostJoins;
        private bool _mUsingPolyTree;
#if use_xyz
    /// <summary>
    ///     Z-Fill callback delegate.
    /// </summary>
    /// <param name="bot1">Bottom point 1</param>
    /// <param name="top1">Top point 1</param>
    /// <param name="bot2">Bottom point 1</param>
    /// <param name="top2">Top point 1</param>
    /// <param name="pt">Reference point.</param>
        public delegate void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt);

        /// <summary>
        ///     Z-Fill function.
        /// </summary>
        public ZFillCallback ZFillFunction { get; set; }
#endif

        /// <summary>
        ///     Initializes a new instance of the <see cref="Clipper" /> class.
        /// </summary>
        /// <param name="initOptions">The initialize options.</param>
        public Clipper(int initOptions = 0) //constructor
        {
            _mScanbeam = null;
            _mActiveEdges = null;
            _mSortedEdges = null;
            _mIntersectList = new List<IntersectNode>();
            _mIntersectNodeComparer = new MyIntersectNodeSort();
            _mExecuteLocked = false;
            _mUsingPolyTree = false;
            _mPolyOuts = new List<OutRec>();
            _mJoins = new List<Join>();
            _mGhostJoins = new List<Join>();
            ReverseSolution = (IoReverseSolution & initOptions) != 0;
            StrictlySimple = (IoStrictlySimple & initOptions) != 0;
            PreserveCollinear = (IoPreserveCollinear & initOptions) != 0;
#if use_xyz
          ZFillFunction = null;
#endif
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            _mScanbeam = null;
            _mActiveEdges = null;
            _mSortedEdges = null;
            var lm = MMinimaList;
            while (lm != null)
            {
                InsertScanbeam(lm.Y);
                lm = lm.Next;
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets or sets a value indicating whether to reverse the solution.
        /// </summary>
        /// <value>
        ///     <c>true</c> if reversing the solution; otherwise, <c>false</c>.
        /// </value>
        public bool ReverseSolution { get; set; }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets or sets a value indicating whether clipping is strictly simple.
        /// </summary>
        /// <value>
        ///     <c>true</c> if clipping is strictly simple; otherwise, <c>false</c>.
        /// </value>
        public bool StrictlySimple { get; set; }

        //------------------------------------------------------------------------------

        private void InsertScanbeam(cInt y)
        {
            if (_mScanbeam == null)
            {
                _mScanbeam = new Scanbeam { Next = null, Y = y };
            }
            else if (y > _mScanbeam.Y)
            {
                var newSb = new Scanbeam { Y = y, Next = _mScanbeam };
                _mScanbeam = newSb;
            }
            else
            {
                var sb2 = _mScanbeam;
                while (sb2.Next != null && (y <= sb2.Next.Y))
                {
                    sb2 = sb2.Next;
                }
                if (y == sb2.Y)
                {
                    return; //ie ignores duplicates
                }
                var newSb = new Scanbeam { Y = y, Next = sb2.Next };
                sb2.Next = newSb;
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified clipping.
        /// </summary>
        /// <param name="clipType">Type of the clip.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="subjFillType">Type of the subject fill.</param>
        /// <param name="clipFillType">Type of the clip fill.</param>
        /// <returns></returns>
        /// <exception cref="ClipperException">Error: PolyTree struct is need for open path clipping.</exception>
        public bool Execute(ClipType clipType, Paths solution, PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (_mExecuteLocked)
            {
                return false;
            }
            if (MHasOpenPaths)
            {
                throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
            }

            _mExecuteLocked = true;
            solution.Clear();
            _mSubjFillType = subjFillType;
            _mClipFillType = clipFillType;
            _mClipType = clipType;
            _mUsingPolyTree = false;
            bool succeeded;
            try
            {
                succeeded = ExecuteInternal();
                //build the return polygons ...
                if (succeeded)
                {
                    BuildResult(solution);
                }
            }
            finally
            {
                DisposeAllPolyPts();
                _mExecuteLocked = false;
            }
            return succeeded;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified clipping.
        /// </summary>
        /// <param name="clipType">Type of the clip.</param>
        /// <param name="polytree">The polytree.</param>
        /// <param name="subjFillType">Type of the subject fill.</param>
        /// <param name="clipFillType">Type of the clip fill.</param>
        /// <returns></returns>
        public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (_mExecuteLocked)
            {
                return false;
            }
            _mExecuteLocked = true;
            _mSubjFillType = subjFillType;
            _mClipFillType = clipFillType;
            _mClipType = clipType;
            _mUsingPolyTree = true;
            bool succeeded;
            try
            {
                succeeded = ExecuteInternal();
                //build the return polygons ...
                if (succeeded)
                {
                    BuildResult2(polytree);
                }
            }
            finally
            {
                DisposeAllPolyPts();
                _mExecuteLocked = false;
            }
            return succeeded;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified clip type.
        /// </summary>
        /// <param name="clipType">Type of the clip.</param>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        public bool Execute(ClipType clipType, Paths solution)
        {
            return Execute(clipType, solution, PolyFillType.PftEvenOdd, PolyFillType.PftEvenOdd);
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified clip type.
        /// </summary>
        /// <param name="clipType">Type of the clip.</param>
        /// <param name="polytree">The polytree.</param>
        /// <returns></returns>
        public bool Execute(ClipType clipType, PolyTree polytree)
        {
            return Execute(clipType, polytree, PolyFillType.PftEvenOdd, PolyFillType.PftEvenOdd);
        }

        //------------------------------------------------------------------------------

        internal void FixHoleLinkage(OutRec outRec)
        {
            //skip if an outermost polygon or
            //already already points to the correct FirstLeft ...
            if (outRec.FirstLeft == null || (outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null))
            {
                return;
            }

            var orfl = outRec.FirstLeft;
            while (orfl != null && ((orfl.IsHole == outRec.IsHole) || orfl.Pts == null))
            {
                orfl = orfl.FirstLeft;
            }
            outRec.FirstLeft = orfl;
        }

        //------------------------------------------------------------------------------

        private bool ExecuteInternal()
        {
            try
            {
                Reset();
                if (MCurrentLm == null)
                {
                    return false;
                }

                var botY = PopScanbeam();
                do
                {
                    InsertLocalMinimaIntoAel(botY);
                    _mGhostJoins.Clear();
                    ProcessHorizontals(false);
                    if (_mScanbeam == null)
                    {
                        break;
                    }
                    var topY = PopScanbeam();
                    if (!ProcessIntersections(topY))
                    {
                        return false;
                    }
                    ProcessEdgesAtTopOfScanbeam(topY);
                    botY = topY;
                } while (_mScanbeam != null || MCurrentLm != null);

                //fix orientations ...
                foreach (var outRec in
                    _mPolyOuts.Where(outRec => outRec.Pts != null && !outRec.IsOpen)
                        .Where(outRec => (outRec.IsHole ^ ReverseSolution) == (Area(outRec) > 0)))
                {
                    ReversePolyPtLinks(outRec.Pts);
                }

                JoinCommonEdges();

                foreach (var outRec in _mPolyOuts.Where(outRec => outRec.Pts != null && !outRec.IsOpen))
                {
                    FixupOutPolygon(outRec);
                }

                if (StrictlySimple)
                {
                    DoSimplePolygons();
                }
                return true;
            }
                //catch { return false; }
            finally
            {
                _mJoins.Clear();
                _mGhostJoins.Clear();
            }
        }

        //------------------------------------------------------------------------------

        private cInt PopScanbeam()
        {
            var y = _mScanbeam.Y;
            _mScanbeam = _mScanbeam.Next;
            return y;
        }

        //------------------------------------------------------------------------------

        private void DisposeAllPolyPts()
        {
            for (var i = 0; i < _mPolyOuts.Count; ++i)
            {
                DisposeOutRec(i);
            }
            _mPolyOuts.Clear();
        }

        //------------------------------------------------------------------------------

        private void DisposeOutRec(int index)
        {
            var outRec = _mPolyOuts[index];
            outRec.Pts = null;
            _mPolyOuts[index] = null;
        }

        //------------------------------------------------------------------------------

        private void AddJoin(OutPt op1, OutPt op2, IntPoint offPt)
        {
            var j = new Join { OutPt1 = op1, OutPt2 = op2, OffPt = offPt };
            _mJoins.Add(j);
        }

        //------------------------------------------------------------------------------

        private void AddGhostJoin(OutPt op, IntPoint offPt)
        {
            var j = new Join { OutPt1 = op, OffPt = offPt };
            _mGhostJoins.Add(j);
        }

        //------------------------------------------------------------------------------

#if use_xyz
      internal void SetZ(ref IntPoint pt, Edge e1, Edge e2)
      {
          if (pt.Z != 0 || ZFillFunction == null) return;
          if (pt == e1.Bot) pt.Z = e1.Bot.Z;
          else if (pt == e1.Top) pt.Z = e1.Top.Z;
          else if (pt == e2.Bot) pt.Z = e2.Bot.Z;
          else if (pt == e2.Top) pt.Z = e2.Top.Z;
          else ZFillFunction(e1.Bot, e1.Top, e2.Bot, e2.Top, ref pt);
      }

        //------------------------------------------------------------------------------
#endif

        private void InsertLocalMinimaIntoAel(cInt botY)
        {
            while (MCurrentLm != null && (MCurrentLm.Y == botY))
            {
                var lb = MCurrentLm.LeftBound;
                var rb = MCurrentLm.RightBound;
                PopLocalMinima();

                OutPt op1 = null;
                if (lb == null)
                {
                    InsertEdgeIntoAel(rb, null);
                    SetWindingCount(rb);
                    if (IsContributing(rb))
                    {
                        op1 = AddOutPt(rb, rb.Bot);
                    }
                }
                else if (rb == null)
                {
                    InsertEdgeIntoAel(lb, null);
                    SetWindingCount(lb);
                    if (IsContributing(lb))
                    {
                        op1 = AddOutPt(lb, lb.Bot);
                    }
                    InsertScanbeam(lb.Top.Y);
                }
                else
                {
                    InsertEdgeIntoAel(lb, null);
                    InsertEdgeIntoAel(rb, lb);
                    SetWindingCount(lb);
                    rb.WindCnt = lb.WindCnt;
                    rb.WindCnt2 = lb.WindCnt2;
                    if (IsContributing(lb))
                    {
                        op1 = AddLocalMinPoly(lb, rb, lb.Bot);
                    }
                    InsertScanbeam(lb.Top.Y);
                }

                if (rb != null)
                {
                    if (IsHorizontal(rb))
                    {
                        AddEdgeToSel(rb);
                    }
                    else
                    {
                        InsertScanbeam(rb.Top.Y);
                    }
                }

                if (lb == null || rb == null)
                {
                    continue;
                }

                //if output polygons share an Edge with a horizontal rb, they'll need joining later ...
                if (op1 != null && IsHorizontal(rb) && _mGhostJoins.Count > 0 && rb.WindDelta != 0)
                {
                    foreach (var j in
                        _mGhostJoins.Where(j => HorzSegmentsOverlap(j.OutPt1.Pt.X, j.OffPt.X, rb.Bot.X, rb.Top.X)))
                    {
                        AddJoin(j.OutPt1, op1, j.OffPt);
                    }
                }

                if (lb.OutIdx >= 0 && lb.PrevInAel != null && lb.PrevInAel.Curr.X == lb.Bot.X &&
                    lb.PrevInAel.OutIdx >= 0 && SlopesEqual(lb.PrevInAel, lb, MUseFullRange) && lb.WindDelta != 0 &&
                    lb.PrevInAel.WindDelta != 0)
                {
                    var op2 = AddOutPt(lb.PrevInAel, lb.Bot);
                    AddJoin(op1, op2, lb.Top);
                }

                if (lb.NextInAel != rb)
                {
                    if (rb.OutIdx >= 0 && rb.PrevInAel.OutIdx >= 0 && SlopesEqual(rb.PrevInAel, rb, MUseFullRange) &&
                        rb.WindDelta != 0 && rb.PrevInAel.WindDelta != 0)
                    {
                        var op2 = AddOutPt(rb.PrevInAel, rb.Bot);
                        AddJoin(op1, op2, rb.Top);
                    }

                    var e = lb.NextInAel;
                    if (e != null)
                    {
                        while (e != rb)
                        {
                            //nb: For calculating winding counts etc, IntersectEdges() assumes
                            //that param1 will be to the right of param2 ABOVE the intersection ...
                            IntersectEdges(rb, e, lb.Curr); //order important here
                            e = e.NextInAel;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        private void InsertEdgeIntoAel(Edge edge, Edge startEdge)
        {
            if (_mActiveEdges == null)
            {
                edge.PrevInAel = null;
                edge.NextInAel = null;
                _mActiveEdges = edge;
            }
            else if (startEdge == null && E2InsertsBeforeE1(_mActiveEdges, edge))
            {
                edge.PrevInAel = null;
                edge.NextInAel = _mActiveEdges;
                _mActiveEdges.PrevInAel = edge;
                _mActiveEdges = edge;
            }
            else
            {
                if (startEdge == null)
                {
                    startEdge = _mActiveEdges;
                }
                while (startEdge.NextInAel != null && !E2InsertsBeforeE1(startEdge.NextInAel, edge))
                {
                    startEdge = startEdge.NextInAel;
                }
                edge.NextInAel = startEdge.NextInAel;
                if (startEdge.NextInAel != null)
                {
                    startEdge.NextInAel.PrevInAel = edge;
                }
                edge.PrevInAel = startEdge;
                startEdge.NextInAel = edge;
            }
        }

        //----------------------------------------------------------------------

        private bool E2InsertsBeforeE1(Edge e1, Edge e2)
        {
            if (e2.Curr.X == e1.Curr.X)
            {
                if (e2.Top.Y > e1.Top.Y)
                {
                    return e2.Top.X < TopX(e1, e2.Top.Y);
                }
                return e1.Top.X > TopX(e2, e1.Top.Y);
            }
            return e2.Curr.X < e1.Curr.X;
        }

        //------------------------------------------------------------------------------

        private bool IsEvenOddFillType(Edge edge)
        {
            if (edge.PolyTyp == PolyType.PtSubject)
            {
                return _mSubjFillType == PolyFillType.PftEvenOdd;
            }
            return _mClipFillType == PolyFillType.PftEvenOdd;
        }

        //------------------------------------------------------------------------------

        private bool IsEvenOddAltFillType(Edge edge)
        {
            if (edge.PolyTyp == PolyType.PtSubject)
            {
                return _mClipFillType == PolyFillType.PftEvenOdd;
            }
            return _mSubjFillType == PolyFillType.PftEvenOdd;
        }

        //------------------------------------------------------------------------------

        private bool IsContributing(Edge edge)
        {
            PolyFillType pft, pft2;
            if (edge.PolyTyp == PolyType.PtSubject)
            {
                pft = _mSubjFillType;
                pft2 = _mClipFillType;
            }
            else
            {
                pft = _mClipFillType;
                pft2 = _mSubjFillType;
            }

            switch (pft)
            {
                case PolyFillType.PftEvenOdd:
                    //return false if a subj line has been flagged as inside a subj polygon
                    if (edge.WindDelta == 0 && edge.WindCnt != 1)
                    {
                        return false;
                    }
                    break;
                case PolyFillType.PftNonZero:
                    if (Math.Abs(edge.WindCnt) != 1)
                    {
                        return false;
                    }
                    break;
                case PolyFillType.PftPositive:
                    if (edge.WindCnt != 1)
                    {
                        return false;
                    }
                    break;
                default: //PolyFillType.pftNegative
                    if (edge.WindCnt != -1)
                    {
                        return false;
                    }
                    break;
            }

            switch (_mClipType)
            {
                case ClipType.CtIntersection:
                    switch (pft2)
                    {
                        case PolyFillType.PftEvenOdd:
                        case PolyFillType.PftNonZero:
                            return (edge.WindCnt2 != 0);
                        case PolyFillType.PftPositive:
                            return (edge.WindCnt2 > 0);
                        default:
                            return (edge.WindCnt2 < 0);
                    }
                case ClipType.CtUnion:
                    switch (pft2)
                    {
                        case PolyFillType.PftEvenOdd:
                        case PolyFillType.PftNonZero:
                            return (edge.WindCnt2 == 0);
                        case PolyFillType.PftPositive:
                            return (edge.WindCnt2 <= 0);
                        default:
                            return (edge.WindCnt2 >= 0);
                    }
                case ClipType.CtDifference:
                    if (edge.PolyTyp == PolyType.PtSubject)
                    {
                        switch (pft2)
                        {
                            case PolyFillType.PftEvenOdd:
                            case PolyFillType.PftNonZero:
                                return (edge.WindCnt2 == 0);
                            case PolyFillType.PftPositive:
                                return (edge.WindCnt2 <= 0);
                            default:
                                return (edge.WindCnt2 >= 0);
                        }
                    }
                    switch (pft2)
                    {
                        case PolyFillType.PftEvenOdd:
                        case PolyFillType.PftNonZero:
                            return (edge.WindCnt2 != 0);
                        case PolyFillType.PftPositive:
                            return (edge.WindCnt2 > 0);
                        default:
                            return (edge.WindCnt2 < 0);
                    }
                case ClipType.CtXor:
                    if (edge.WindDelta == 0) //XOr always contributing unless open
                    {
                        switch (pft2)
                        {
                            case PolyFillType.PftEvenOdd:
                            case PolyFillType.PftNonZero:
                                return (edge.WindCnt2 == 0);
                            case PolyFillType.PftPositive:
                                return (edge.WindCnt2 <= 0);
                            default:
                                return (edge.WindCnt2 >= 0);
                        }
                    }
                    return true;
            }
            return true;
        }

        //------------------------------------------------------------------------------

        private void SetWindingCount(Edge edge)
        {
            var e = edge.PrevInAel;
            //find the edge of the same polytype that immediately preceeds 'edge' in AEL
            while (e != null && ((e.PolyTyp != edge.PolyTyp) || (e.WindDelta == 0)))
            {
                e = e.PrevInAel;
            }
            if (e == null)
            {
                edge.WindCnt = (edge.WindDelta == 0 ? 1 : edge.WindDelta);
                edge.WindCnt2 = 0;
                e = _mActiveEdges; //ie get ready to calc WindCnt2
            }
            else if (edge.WindDelta == 0 && _mClipType != ClipType.CtUnion)
            {
                edge.WindCnt = 1;
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAel; //ie get ready to calc WindCnt2
            }
            else if (IsEvenOddFillType(edge))
            {
                //EvenOdd filling ...
                if (edge.WindDelta == 0)
                {
                    //are we inside a subj polygon ...
                    var inside = true;
                    var e2 = e.PrevInAel;
                    while (e2 != null)
                    {
                        if (e2.PolyTyp == e.PolyTyp && e2.WindDelta != 0)
                        {
                            inside = !inside;
                        }
                        e2 = e2.PrevInAel;
                    }
                    edge.WindCnt = (inside ? 0 : 1);
                }
                else
                {
                    edge.WindCnt = edge.WindDelta;
                }
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAel; //ie get ready to calc WindCnt2
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                if (e.WindCnt * e.WindDelta < 0)
                {
                    //prev edge is 'decreasing' WindCount (WC) toward zero
                    //so we're outside the previous polygon ...
                    if (Math.Abs(e.WindCnt) > 1)
                    {
                        //outside prev poly but still inside another.
                        //when reversing direction of prev poly use the same WC 
                        if (e.WindDelta * edge.WindDelta < 0)
                        {
                            edge.WindCnt = e.WindCnt;
                        }
                        //otherwise continue to 'decrease' WC ...
                        else
                        {
                            edge.WindCnt = e.WindCnt + edge.WindDelta;
                        }
                    }
                    else
                    {
                        //now outside all polys of same polytype so set own WC ...
                        edge.WindCnt = (edge.WindDelta == 0 ? 1 : edge.WindDelta);
                    }
                }
                else
                {
                    //prev edge is 'increasing' WindCount (WC) away from zero
                    //so we're inside the previous polygon ...
                    if (edge.WindDelta == 0)
                    {
                        edge.WindCnt = (e.WindCnt < 0 ? e.WindCnt - 1 : e.WindCnt + 1);
                    }
                    //if wind direction is reversing prev then use same WC
                    else if (e.WindDelta * edge.WindDelta < 0)
                    {
                        edge.WindCnt = e.WindCnt;
                    }
                    //otherwise add to WC ...
                    else
                    {
                        edge.WindCnt = e.WindCnt + edge.WindDelta;
                    }
                }
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAel; //ie get ready to calc WindCnt2
            }

            //update WindCnt2 ...
            if (IsEvenOddAltFillType(edge))
            {
                //EvenOdd filling ...
                while (e != edge)
                {
                    if (e.WindDelta != 0)
                    {
                        edge.WindCnt2 = (edge.WindCnt2 == 0 ? 1 : 0);
                    }
                    e = e.NextInAel;
                }
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                while (e != edge)
                {
                    edge.WindCnt2 += e.WindDelta;
                    e = e.NextInAel;
                }
            }
        }

        //------------------------------------------------------------------------------

        private void AddEdgeToSel(Edge edge)
        {
            //SEL pointers in PEdge are reused to build a list of horizontal edges.
            //However, we don't need to worry about order with horizontal edge processing.
            if (_mSortedEdges == null)
            {
                _mSortedEdges = edge;
                edge.PrevInSel = null;
                edge.NextInSel = null;
            }
            else
            {
                edge.NextInSel = _mSortedEdges;
                edge.PrevInSel = null;
                _mSortedEdges.PrevInSel = edge;
                _mSortedEdges = edge;
            }
        }

        //------------------------------------------------------------------------------

        private void CopyAeltoSel()
        {
            var e = _mActiveEdges;
            _mSortedEdges = e;
            while (e != null)
            {
                e.PrevInSel = e.PrevInAel;
                e.NextInSel = e.NextInAel;
                e = e.NextInAel;
            }
        }

        //------------------------------------------------------------------------------

        private void SwapPositionsInAel(Edge edge1, Edge edge2)
        {
            //check that one or other edge hasn't already been removed from AEL ...
            if (edge1.NextInAel == edge1.PrevInAel || edge2.NextInAel == edge2.PrevInAel)
            {
                return;
            }

            if (edge1.NextInAel == edge2)
            {
                var next = edge2.NextInAel;
                if (next != null)
                {
                    next.PrevInAel = edge1;
                }
                var prev = edge1.PrevInAel;
                if (prev != null)
                {
                    prev.NextInAel = edge2;
                }
                edge2.PrevInAel = prev;
                edge2.NextInAel = edge1;
                edge1.PrevInAel = edge2;
                edge1.NextInAel = next;
            }
            else if (edge2.NextInAel == edge1)
            {
                var next = edge1.NextInAel;
                if (next != null)
                {
                    next.PrevInAel = edge2;
                }
                var prev = edge2.PrevInAel;
                if (prev != null)
                {
                    prev.NextInAel = edge1;
                }
                edge1.PrevInAel = prev;
                edge1.NextInAel = edge2;
                edge2.PrevInAel = edge1;
                edge2.NextInAel = next;
            }
            else
            {
                var next = edge1.NextInAel;
                var prev = edge1.PrevInAel;
                edge1.NextInAel = edge2.NextInAel;
                if (edge1.NextInAel != null)
                {
                    edge1.NextInAel.PrevInAel = edge1;
                }
                edge1.PrevInAel = edge2.PrevInAel;
                if (edge1.PrevInAel != null)
                {
                    edge1.PrevInAel.NextInAel = edge1;
                }
                edge2.NextInAel = next;
                if (edge2.NextInAel != null)
                {
                    edge2.NextInAel.PrevInAel = edge2;
                }
                edge2.PrevInAel = prev;
                if (edge2.PrevInAel != null)
                {
                    edge2.PrevInAel.NextInAel = edge2;
                }
            }

            if (edge1.PrevInAel == null)
            {
                _mActiveEdges = edge1;
            }
            else if (edge2.PrevInAel == null)
            {
                _mActiveEdges = edge2;
            }
        }

        //------------------------------------------------------------------------------

        private void SwapPositionsInSel(Edge edge1, Edge edge2)
        {
            if (edge1.NextInSel == null && edge1.PrevInSel == null)
            {
                return;
            }
            if (edge2.NextInSel == null && edge2.PrevInSel == null)
            {
                return;
            }

            if (edge1.NextInSel == edge2)
            {
                var next = edge2.NextInSel;
                if (next != null)
                {
                    next.PrevInSel = edge1;
                }
                var prev = edge1.PrevInSel;
                if (prev != null)
                {
                    prev.NextInSel = edge2;
                }
                edge2.PrevInSel = prev;
                edge2.NextInSel = edge1;
                edge1.PrevInSel = edge2;
                edge1.NextInSel = next;
            }
            else if (edge2.NextInSel == edge1)
            {
                var next = edge1.NextInSel;
                if (next != null)
                {
                    next.PrevInSel = edge2;
                }
                var prev = edge2.PrevInSel;
                if (prev != null)
                {
                    prev.NextInSel = edge1;
                }
                edge1.PrevInSel = prev;
                edge1.NextInSel = edge2;
                edge2.PrevInSel = edge1;
                edge2.NextInSel = next;
            }
            else
            {
                var next = edge1.NextInSel;
                var prev = edge1.PrevInSel;
                edge1.NextInSel = edge2.NextInSel;
                if (edge1.NextInSel != null)
                {
                    edge1.NextInSel.PrevInSel = edge1;
                }
                edge1.PrevInSel = edge2.PrevInSel;
                if (edge1.PrevInSel != null)
                {
                    edge1.PrevInSel.NextInSel = edge1;
                }
                edge2.NextInSel = next;
                if (edge2.NextInSel != null)
                {
                    edge2.NextInSel.PrevInSel = edge2;
                }
                edge2.PrevInSel = prev;
                if (edge2.PrevInSel != null)
                {
                    edge2.PrevInSel.NextInSel = edge2;
                }
            }

            if (edge1.PrevInSel == null)
            {
                _mSortedEdges = edge1;
            }
            else if (edge2.PrevInSel == null)
            {
                _mSortedEdges = edge2;
            }
        }

        //------------------------------------------------------------------------------


        private void AddLocalMaxPoly(Edge e1, Edge e2, IntPoint pt)
        {
            AddOutPt(e1, pt);
            if (e2.WindDelta == 0)
            {
                AddOutPt(e2, pt);
            }
            if (e1.OutIdx == e2.OutIdx)
            {
                e1.OutIdx = Unassigned;
                e2.OutIdx = Unassigned;
            }
            else if (e1.OutIdx < e2.OutIdx)
            {
                AppendPolygon(e1, e2);
            }
            else
            {
                AppendPolygon(e2, e1);
            }
        }

        //------------------------------------------------------------------------------

        private OutPt AddLocalMinPoly(Edge e1, Edge e2, IntPoint pt)
        {
            OutPt result;
            Edge e, prevE;
            if (IsHorizontal(e2) || (e1.Dx > e2.Dx))
            {
                result = AddOutPt(e1, pt);
                e2.OutIdx = e1.OutIdx;
                e1.Side = EdgeSide.EsLeft;
                e2.Side = EdgeSide.EsRight;
                e = e1;
                prevE = e.PrevInAel == e2 ? e2.PrevInAel : e.PrevInAel;
            }
            else
            {
                result = AddOutPt(e2, pt);
                e1.OutIdx = e2.OutIdx;
                e1.Side = EdgeSide.EsRight;
                e2.Side = EdgeSide.EsLeft;
                e = e2;
                prevE = e.PrevInAel == e1 ? e1.PrevInAel : e.PrevInAel;
            }

            if (prevE != null && prevE.OutIdx >= 0 && (TopX(prevE, pt.Y) == TopX(e, pt.Y)) &&
                SlopesEqual(e, prevE, MUseFullRange) && (e.WindDelta != 0) && (prevE.WindDelta != 0))
            {
                var outPt = AddOutPt(prevE, pt);
                AddJoin(result, outPt, e.Top);
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private OutRec CreateOutRec()
        {
            var result = new OutRec
            {
                Idx = Unassigned,
                IsHole = false,
                IsOpen = false,
                FirstLeft = null,
                Pts = null,
                BottomPt = null,
                PolyNode = null
            };
            _mPolyOuts.Add(result);
            result.Idx = _mPolyOuts.Count - 1;
            return result;
        }

        //------------------------------------------------------------------------------

        private OutPt AddOutPt(Edge e, IntPoint pt)
        {
            var toFront = (e.Side == EdgeSide.EsLeft);
            if (e.OutIdx < 0)
            {
                var outRec = CreateOutRec();
                outRec.IsOpen = (e.WindDelta == 0);
                var newOp = new OutPt();
                outRec.Pts = newOp;
                newOp.Idx = outRec.Idx;
                newOp.Pt = pt;
                newOp.Next = newOp;
                newOp.Prev = newOp;
                if (!outRec.IsOpen)
                {
                    SetHoleState(e, outRec);
                }
                e.OutIdx = outRec.Idx; //nb: do this after SetZ !
                return newOp;
            }
            else
            {
                var outRec = _mPolyOuts[e.OutIdx];
                //OutRec.Pts is the 'Left-most' point & OutRec.Pts.Prev is the 'Right-most'
                var op = outRec.Pts;
                if (toFront && pt == op.Pt)
                {
                    return op;
                }
                if (!toFront && pt == op.Prev.Pt)
                {
                    return op.Prev;
                }

                var newOp = new OutPt { Idx = outRec.Idx, Pt = pt, Next = op, Prev = op.Prev };
                newOp.Prev.Next = newOp;
                op.Prev = newOp;
                if (toFront)
                {
                    outRec.Pts = newOp;
                }
                return newOp;
            }
        }

        //------------------------------------------------------------------------------

        internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
        {
            var tmp = new IntPoint(pt1);
            pt1 = pt2;
            pt2 = tmp;
        }

        //------------------------------------------------------------------------------

        private bool HorzSegmentsOverlap(cInt seg1A, cInt seg1B, cInt seg2A, cInt seg2B)
        {
            if (seg1A > seg1B)
            {
                Swap(ref seg1A, ref seg1B);
            }
            if (seg2A > seg2B)
            {
                Swap(ref seg2A, ref seg2B);
            }
            return (seg1A < seg2B) && (seg2A < seg1B);
        }

        //------------------------------------------------------------------------------

        private void SetHoleState(Edge e, OutRec outRec)
        {
            var isHole = false;
            var e2 = e.PrevInAel;
            while (e2 != null)
            {
                if (e2.OutIdx >= 0 && e2.WindDelta != 0)
                {
                    isHole = !isHole;
                    if (outRec.FirstLeft == null)
                    {
                        outRec.FirstLeft = _mPolyOuts[e2.OutIdx];
                    }
                }
                e2 = e2.PrevInAel;
            }
            if (isHole)
            {
                outRec.IsHole = true;
            }
        }

        //------------------------------------------------------------------------------

        private double GetDx(IntPoint pt1, IntPoint pt2)
        {
            if (pt1.Y == pt2.Y)
            {
                return Horizontal;
            }
            return (double) (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
        }

        //---------------------------------------------------------------------------

        private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
        {
            var p = btmPt1.Prev;
            while ((p.Pt == btmPt1.Pt) && (p != btmPt1))
            {
                p = p.Prev;
            }
            var dx1P = Math.Abs(GetDx(btmPt1.Pt, p.Pt));
            p = btmPt1.Next;
            while ((p.Pt == btmPt1.Pt) && (p != btmPt1))
            {
                p = p.Next;
            }
            var dx1N = Math.Abs(GetDx(btmPt1.Pt, p.Pt));

            p = btmPt2.Prev;
            while ((p.Pt == btmPt2.Pt) && (p != btmPt2))
            {
                p = p.Prev;
            }
            var dx2P = Math.Abs(GetDx(btmPt2.Pt, p.Pt));
            p = btmPt2.Next;
            while ((p.Pt == btmPt2.Pt) && (p != btmPt2))
            {
                p = p.Next;
            }
            var dx2N = Math.Abs(GetDx(btmPt2.Pt, p.Pt));
            return (dx1P >= dx2P && dx1P >= dx2N) || (dx1N >= dx2P && dx1N >= dx2N);
        }

        //------------------------------------------------------------------------------

        private OutPt GetBottomPt(OutPt pp)
        {
            OutPt dups = null;
            var p = pp.Next;
            while (p != pp)
            {
                if (p.Pt.Y > pp.Pt.Y)
                {
                    pp = p;
                    dups = null;
                }
                else if (p.Pt.Y == pp.Pt.Y && p.Pt.X <= pp.Pt.X)
                {
                    if (p.Pt.X < pp.Pt.X)
                    {
                        dups = null;
                        pp = p;
                    }
                    else
                    {
                        if (p.Next != pp && p.Prev != pp)
                        {
                            dups = p;
                        }
                    }
                }
                p = p.Next;
            }
            if (dups != null)
            {
                //there appears to be at least 2 vertices at bottomPt so ...
                while (dups != p)
                {
                    if (!FirstIsBottomPt(p, dups))
                    {
                        pp = dups;
                    }
                    dups = dups.Next;
                    while (dups.Pt != pp.Pt)
                    {
                        dups = dups.Next;
                    }
                }
            }
            return pp;
        }

        //------------------------------------------------------------------------------

        private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
        {
            //work out which polygon fragment has the correct hole state ...
            if (outRec1.BottomPt == null)
            {
                outRec1.BottomPt = GetBottomPt(outRec1.Pts);
            }
            if (outRec2.BottomPt == null)
            {
                outRec2.BottomPt = GetBottomPt(outRec2.Pts);
            }
            var bPt1 = outRec1.BottomPt;
            var bPt2 = outRec2.BottomPt;
            if (bPt1.Pt.Y > bPt2.Pt.Y)
            {
                return outRec1;
            }
            if (bPt1.Pt.Y < bPt2.Pt.Y)
            {
                return outRec2;
            }
            if (bPt1.Pt.X < bPt2.Pt.X)
            {
                return outRec1;
            }
            if (bPt1.Pt.X > bPt2.Pt.X)
            {
                return outRec2;
            }
            if (bPt1.Next == bPt1)
            {
                return outRec2;
            }
            if (bPt2.Next == bPt2)
            {
                return outRec1;
            }
            if (FirstIsBottomPt(bPt1, bPt2))
            {
                return outRec1;
            }
            return outRec2;
        }

        //------------------------------------------------------------------------------

        private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
        {
            do
            {
                outRec1 = outRec1.FirstLeft;
                if (outRec1 == outRec2)
                {
                    return true;
                }
            } while (outRec1 != null);
            return false;
        }

        //------------------------------------------------------------------------------

        private OutRec GetOutRec(int idx)
        {
            var outrec = _mPolyOuts[idx];
            while (outrec != _mPolyOuts[outrec.Idx])
            {
                outrec = _mPolyOuts[outrec.Idx];
            }
            return outrec;
        }

        //------------------------------------------------------------------------------

        private void AppendPolygon(Edge e1, Edge e2)
        {
            //get the start and ends of both output polygons ...
            var outRec1 = _mPolyOuts[e1.OutIdx];
            var outRec2 = _mPolyOuts[e2.OutIdx];

            OutRec holeStateRec;
            if (Param1RightOfParam2(outRec1, outRec2))
            {
                holeStateRec = outRec2;
            }
            else if (Param1RightOfParam2(outRec2, outRec1))
            {
                holeStateRec = outRec1;
            }
            else
            {
                holeStateRec = GetLowermostRec(outRec1, outRec2);
            }

            var p1Lft = outRec1.Pts;
            var p1Rt = p1Lft.Prev;
            var p2Lft = outRec2.Pts;
            var p2Rt = p2Lft.Prev;

            EdgeSide side;
            //join e2 poly onto e1 poly and delete pointers to e2 ...
            if (e1.Side == EdgeSide.EsLeft)
            {
                if (e2.Side == EdgeSide.EsLeft)
                {
                    //z y x a b c
                    ReversePolyPtLinks(p2Lft);
                    p2Lft.Next = p1Lft;
                    p1Lft.Prev = p2Lft;
                    p1Rt.Next = p2Rt;
                    p2Rt.Prev = p1Rt;
                    outRec1.Pts = p2Rt;
                }
                else
                {
                    //x y z a b c
                    p2Rt.Next = p1Lft;
                    p1Lft.Prev = p2Rt;
                    p2Lft.Prev = p1Rt;
                    p1Rt.Next = p2Lft;
                    outRec1.Pts = p2Lft;
                }
                side = EdgeSide.EsLeft;
            }
            else
            {
                if (e2.Side == EdgeSide.EsRight)
                {
                    //a b c z y x
                    ReversePolyPtLinks(p2Lft);
                    p1Rt.Next = p2Rt;
                    p2Rt.Prev = p1Rt;
                    p2Lft.Next = p1Lft;
                    p1Lft.Prev = p2Lft;
                }
                else
                {
                    //a b c x y z
                    p1Rt.Next = p2Lft;
                    p2Lft.Prev = p1Rt;
                    p1Lft.Prev = p2Rt;
                    p2Rt.Next = p1Lft;
                }
                side = EdgeSide.EsRight;
            }

            outRec1.BottomPt = null;
            if (holeStateRec == outRec2)
            {
                if (outRec2.FirstLeft != outRec1)
                {
                    outRec1.FirstLeft = outRec2.FirstLeft;
                }
                outRec1.IsHole = outRec2.IsHole;
            }
            outRec2.Pts = null;
            outRec2.BottomPt = null;

            outRec2.FirstLeft = outRec1;

            var okIdx = e1.OutIdx;
            var obsoleteIdx = e2.OutIdx;

            e1.OutIdx = Unassigned; //nb: safe because we only get here via AddLocalMaxPoly
            e2.OutIdx = Unassigned;

            var e = _mActiveEdges;
            while (e != null)
            {
                if (e.OutIdx == obsoleteIdx)
                {
                    e.OutIdx = okIdx;
                    e.Side = side;
                    break;
                }
                e = e.NextInAel;
            }
            outRec2.Idx = outRec1.Idx;
        }

        //------------------------------------------------------------------------------

        private void ReversePolyPtLinks(OutPt pp)
        {
            if (pp == null)
            {
                return;
            }
            var pp1 = pp;
            do
            {
                var pp2 = pp1.Next;
                pp1.Next = pp1.Prev;
                pp1.Prev = pp2;
                pp1 = pp2;
            } while (pp1 != pp);
        }

        //------------------------------------------------------------------------------

        private static void SwapSides(Edge edge1, Edge edge2)
        {
            var side = edge1.Side;
            edge1.Side = edge2.Side;
            edge2.Side = side;
        }

        //------------------------------------------------------------------------------

        private static void SwapPolyIndexes(Edge edge1, Edge edge2)
        {
            var outIdx = edge1.OutIdx;
            edge1.OutIdx = edge2.OutIdx;
            edge2.OutIdx = outIdx;
        }

        //------------------------------------------------------------------------------

        private void IntersectEdges(Edge e1, Edge e2, IntPoint pt)
        {
            //e1 will be to the left of e2 BELOW the intersection. Therefore e1 is before
            //e2 in AEL except when e1 is being inserted at the intersection point ...

            var e1Contributing = (e1.OutIdx >= 0);
            var e2Contributing = (e2.OutIdx >= 0);

#if use_xyz
          SetZ(ref pt, e1, e2);
#endif

#if use_lines
    //if either edge is on an OPEN path ...
          if (e1.WindDelta == 0 || e2.WindDelta == 0)
          {
            //ignore subject-subject open path intersections UNLESS they
            //are both open paths, AND they are both 'contributing maximas' ...
            if (e1.WindDelta == 0 && e2.WindDelta == 0) return;
            //if intersecting a subj line with a subj poly ...
            else if (e1.PolyTyp == e2.PolyTyp && 
              e1.WindDelta != e2.WindDelta && _mClipType == ClipType.CtUnion)
            {
              if (e1.WindDelta == 0)
              {
                if (e2Contributing)
                {
                  AddOutPt(e1, pt);
                  if (e1Contributing) e1.OutIdx = Unassigned;
                }
              }
              else
              {
                if (e1Contributing)
                {
                  AddOutPt(e2, pt);
                  if (e2Contributing) e2.OutIdx = Unassigned;
                }
              }
            }
            else if (e1.PolyTyp != e2.PolyTyp)
            {
              if ((e1.WindDelta == 0) && Math.Abs(e2.WindCnt) == 1 &&
                (_mClipType != ClipType.CtUnion || e2.WindCnt2 == 0))
              {
                AddOutPt(e1, pt);
                if (e1Contributing) e1.OutIdx = Unassigned;
              }
              else if ((e2.WindDelta == 0) && (Math.Abs(e1.WindCnt) == 1) &&
                (_mClipType != ClipType.CtUnion || e1.WindCnt2 == 0))
              {
                AddOutPt(e2, pt);
                if (e2Contributing) e2.OutIdx = Unassigned;
              }
            }
            return;
          }
#endif

            //update winding counts...
            //assumes that e1 will be to the Right of e2 ABOVE the intersection
            if (e1.PolyTyp == e2.PolyTyp)
            {
                if (IsEvenOddFillType(e1))
                {
                    var oldE1WindCnt = e1.WindCnt;
                    e1.WindCnt = e2.WindCnt;
                    e2.WindCnt = oldE1WindCnt;
                }
                else
                {
                    if (e1.WindCnt + e2.WindDelta == 0)
                    {
                        e1.WindCnt = -e1.WindCnt;
                    }
                    else
                    {
                        e1.WindCnt += e2.WindDelta;
                    }
                    if (e2.WindCnt - e1.WindDelta == 0)
                    {
                        e2.WindCnt = -e2.WindCnt;
                    }
                    else
                    {
                        e2.WindCnt -= e1.WindDelta;
                    }
                }
            }
            else
            {
                if (!IsEvenOddFillType(e2))
                {
                    e1.WindCnt2 += e2.WindDelta;
                }
                else
                {
                    e1.WindCnt2 = (e1.WindCnt2 == 0) ? 1 : 0;
                }
                if (!IsEvenOddFillType(e1))
                {
                    e2.WindCnt2 -= e1.WindDelta;
                }
                else
                {
                    e2.WindCnt2 = (e2.WindCnt2 == 0) ? 1 : 0;
                }
            }

            PolyFillType e1FillType, e2FillType, e1FillType2, e2FillType2;
            if (e1.PolyTyp == PolyType.PtSubject)
            {
                e1FillType = _mSubjFillType;
                e1FillType2 = _mClipFillType;
            }
            else
            {
                e1FillType = _mClipFillType;
                e1FillType2 = _mSubjFillType;
            }
            if (e2.PolyTyp == PolyType.PtSubject)
            {
                e2FillType = _mSubjFillType;
                e2FillType2 = _mClipFillType;
            }
            else
            {
                e2FillType = _mClipFillType;
                e2FillType2 = _mSubjFillType;
            }

            int e1Wc, e2Wc;
            switch (e1FillType)
            {
                case PolyFillType.PftPositive:
                    e1Wc = e1.WindCnt;
                    break;
                case PolyFillType.PftNegative:
                    e1Wc = -e1.WindCnt;
                    break;
                default:
                    e1Wc = Math.Abs(e1.WindCnt);
                    break;
            }
            switch (e2FillType)
            {
                case PolyFillType.PftPositive:
                    e2Wc = e2.WindCnt;
                    break;
                case PolyFillType.PftNegative:
                    e2Wc = -e2.WindCnt;
                    break;
                default:
                    e2Wc = Math.Abs(e2.WindCnt);
                    break;
            }

            if (e1Contributing && e2Contributing)
            {
                if ((e1Wc != 0 && e1Wc != 1) || (e2Wc != 0 && e2Wc != 1) ||
                    (e1.PolyTyp != e2.PolyTyp && _mClipType != ClipType.CtXor))
                {
                    AddLocalMaxPoly(e1, e2, pt);
                }
                else
                {
                    AddOutPt(e1, pt);
                    AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e1Contributing)
            {
                if (e2Wc == 0 || e2Wc == 1)
                {
                    AddOutPt(e1, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e2Contributing)
            {
                if (e1Wc == 0 || e1Wc == 1)
                {
                    AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if ((e1Wc == 0 || e1Wc == 1) && (e2Wc == 0 || e2Wc == 1))
            {
                //neither edge is currently contributing ...
                cInt e1Wc2, e2Wc2;
                switch (e1FillType2)
                {
                    case PolyFillType.PftPositive:
                        e1Wc2 = e1.WindCnt2;
                        break;
                    case PolyFillType.PftNegative:
                        e1Wc2 = -e1.WindCnt2;
                        break;
                    default:
                        e1Wc2 = Math.Abs(e1.WindCnt2);
                        break;
                }
                switch (e2FillType2)
                {
                    case PolyFillType.PftPositive:
                        e2Wc2 = e2.WindCnt2;
                        break;
                    case PolyFillType.PftNegative:
                        e2Wc2 = -e2.WindCnt2;
                        break;
                    default:
                        e2Wc2 = Math.Abs(e2.WindCnt2);
                        break;
                }

                if (e1.PolyTyp != e2.PolyTyp)
                {
                    AddLocalMinPoly(e1, e2, pt);
                }
                else if (e1Wc == 1 && e2Wc == 1)
                {
                    switch (_mClipType)
                    {
                        case ClipType.CtIntersection:
                            if (e1Wc2 > 0 && e2Wc2 > 0)
                            {
                                AddLocalMinPoly(e1, e2, pt);
                            }
                            break;
                        case ClipType.CtUnion:
                            if (e1Wc2 <= 0 && e2Wc2 <= 0)
                            {
                                AddLocalMinPoly(e1, e2, pt);
                            }
                            break;
                        case ClipType.CtDifference:
                            if (((e1.PolyTyp == PolyType.PtClip) && (e1Wc2 > 0) && (e2Wc2 > 0)) ||
                                ((e1.PolyTyp == PolyType.PtSubject) && (e1Wc2 <= 0) && (e2Wc2 <= 0)))
                            {
                                AddLocalMinPoly(e1, e2, pt);
                            }
                            break;
                        case ClipType.CtXor:
                            AddLocalMinPoly(e1, e2, pt);
                            break;
                    }
                }
                else
                {
                    SwapSides(e1, e2);
                }
            }
        }

        //------------------------------------------------------------------------------

        private void DeleteFromAel(Edge e)
        {
            var aelPrev = e.PrevInAel;
            var aelNext = e.NextInAel;
            if (aelPrev == null && aelNext == null && (e != _mActiveEdges))
            {
                return; //already deleted
            }
            if (aelPrev != null)
            {
                aelPrev.NextInAel = aelNext;
            }
            else
            {
                _mActiveEdges = aelNext;
            }
            if (aelNext != null)
            {
                aelNext.PrevInAel = aelPrev;
            }
            e.NextInAel = null;
            e.PrevInAel = null;
        }

        //------------------------------------------------------------------------------

        private void DeleteFromSel(Edge e)
        {
            var selPrev = e.PrevInSel;
            var selNext = e.NextInSel;
            if (selPrev == null && selNext == null && (e != _mSortedEdges))
            {
                return; //already deleted
            }
            if (selPrev != null)
            {
                selPrev.NextInSel = selNext;
            }
            else
            {
                _mSortedEdges = selNext;
            }
            if (selNext != null)
            {
                selNext.PrevInSel = selPrev;
            }
            e.NextInSel = null;
            e.PrevInSel = null;
        }

        //------------------------------------------------------------------------------

        private void UpdateEdgeIntoAel(ref Edge e)
        {
            if (e.NextInLml == null)
            {
                throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
            }
            var aelPrev = e.PrevInAel;
            var aelNext = e.NextInAel;
            e.NextInLml.OutIdx = e.OutIdx;
            if (aelPrev != null)
            {
                aelPrev.NextInAel = e.NextInLml;
            }
            else
            {
                _mActiveEdges = e.NextInLml;
            }
            if (aelNext != null)
            {
                aelNext.PrevInAel = e.NextInLml;
            }
            e.NextInLml.Side = e.Side;
            e.NextInLml.WindDelta = e.WindDelta;
            e.NextInLml.WindCnt = e.WindCnt;
            e.NextInLml.WindCnt2 = e.WindCnt2;
            e = e.NextInLml;
            e.Curr = e.Bot;
            e.PrevInAel = aelPrev;
            e.NextInAel = aelNext;
            if (!IsHorizontal(e))
            {
                InsertScanbeam(e.Top.Y);
            }
        }

        //------------------------------------------------------------------------------

        private void ProcessHorizontals(bool isTopOfScanbeam)
        {
            var horzEdge = _mSortedEdges;
            while (horzEdge != null)
            {
                DeleteFromSel(horzEdge);
                ProcessHorizontal(horzEdge, isTopOfScanbeam);
                horzEdge = _mSortedEdges;
            }
        }

        //------------------------------------------------------------------------------

        private void GetHorzDirection(Edge horzEdge, out Direction dir, out cInt left, out cInt right)
        {
            if (horzEdge.Bot.X < horzEdge.Top.X)
            {
                left = horzEdge.Bot.X;
                right = horzEdge.Top.X;
                dir = Direction.DLeftToRight;
            }
            else
            {
                left = horzEdge.Top.X;
                right = horzEdge.Bot.X;
                dir = Direction.DRightToLeft;
            }
        }

        //------------------------------------------------------------------------

        private void ProcessHorizontal(Edge horzEdge, bool isTopOfScanbeam)
        {
            Direction dir;
            cInt horzLeft, horzRight;

            GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);

            Edge eLastHorz = horzEdge, eMaxPair = null;
            while (eLastHorz.NextInLml != null && IsHorizontal(eLastHorz.NextInLml))
            {
                eLastHorz = eLastHorz.NextInLml;
            }
            if (eLastHorz.NextInLml == null)
            {
                eMaxPair = GetMaximaPair(eLastHorz);
            }

            for (;;)
            {
                var isLastHorz = (horzEdge == eLastHorz);
                var e = GetNextInAel(horzEdge, dir);
                while (e != null)
                {
                    //Break if we've got to the end of an intermediate horizontal edge ...
                    //nb: Smaller Dx's are to the right of larger Dx's ABOVE the horizontal.
                    if (e.Curr.X == horzEdge.Top.X && horzEdge.NextInLml != null && e.Dx < horzEdge.NextInLml.Dx)
                    {
                        break;
                    }

                    var eNext = GetNextInAel(e, dir); //saves eNext for later

                    if ((dir == Direction.DLeftToRight && e.Curr.X <= horzRight) ||
                        (dir == Direction.DRightToLeft && e.Curr.X >= horzLeft))
                    {
                        //so far we're still in range of the horizontal Edge  but make sure
                        //we're at the last of consec. horizontals when matching with eMaxPair
                        if (e == eMaxPair && isLastHorz)
                        {
                            if (horzEdge.OutIdx >= 0)
                            {
                                var op1 = AddOutPt(horzEdge, horzEdge.Top);
                                var eNextHorz = _mSortedEdges;
                                while (eNextHorz != null)
                                {
                                    if (eNextHorz.OutIdx >= 0 &&
                                        HorzSegmentsOverlap(
                                            horzEdge.Bot.X, horzEdge.Top.X, eNextHorz.Bot.X, eNextHorz.Top.X))
                                    {
                                        var op2 = AddOutPt(eNextHorz, eNextHorz.Bot);
                                        AddJoin(op2, op1, eNextHorz.Top);
                                    }
                                    eNextHorz = eNextHorz.NextInSel;
                                }
                                AddGhostJoin(op1, horzEdge.Bot);
                                AddLocalMaxPoly(horzEdge, eMaxPair, horzEdge.Top);
                            }
                            DeleteFromAel(horzEdge);
                            DeleteFromAel(eMaxPair);
                            return;
                        }
                        if (dir == Direction.DLeftToRight)
                        {
                            var pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                            IntersectEdges(horzEdge, e, pt);
                        }
                        else
                        {
                            var pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                            IntersectEdges(e, horzEdge, pt);
                        }
                        SwapPositionsInAel(horzEdge, e);
                    }
                    else if ((dir == Direction.DLeftToRight && e.Curr.X >= horzRight) ||
                             (dir == Direction.DRightToLeft && e.Curr.X <= horzLeft))
                    {
                        break;
                    }
                    e = eNext;
                } //end while

                if (horzEdge.NextInLml != null && IsHorizontal(horzEdge.NextInLml))
                {
                    UpdateEdgeIntoAel(ref horzEdge);
                    if (horzEdge.OutIdx >= 0)
                    {
                        AddOutPt(horzEdge, horzEdge.Bot);
                    }
                    GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);
                }
                else
                {
                    break;
                }
            } //end for (;;)

            if (horzEdge.NextInLml != null)
            {
                if (horzEdge.OutIdx >= 0)
                {
                    var op1 = AddOutPt(horzEdge, horzEdge.Top);
                    if (isTopOfScanbeam)
                    {
                        AddGhostJoin(op1, horzEdge.Bot);
                    }

                    UpdateEdgeIntoAel(ref horzEdge);
                    if (horzEdge.WindDelta == 0)
                    {
                        return;
                    }
                    //nb: HorzEdge is no longer horizontal here
                    var ePrev = horzEdge.PrevInAel;
                    var eNext = horzEdge.NextInAel;
                    if (ePrev != null && ePrev.Curr.X == horzEdge.Bot.X && ePrev.Curr.Y == horzEdge.Bot.Y &&
                        ePrev.WindDelta != 0 &&
                        (ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y && SlopesEqual(horzEdge, ePrev, MUseFullRange)))
                    {
                        var op2 = AddOutPt(ePrev, horzEdge.Bot);
                        AddJoin(op1, op2, horzEdge.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == horzEdge.Bot.X && eNext.Curr.Y == horzEdge.Bot.Y &&
                             eNext.WindDelta != 0 && eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y &&
                             SlopesEqual(horzEdge, eNext, MUseFullRange))
                    {
                        var op2 = AddOutPt(eNext, horzEdge.Bot);
                        AddJoin(op1, op2, horzEdge.Top);
                    }
                }
                else
                {
                    UpdateEdgeIntoAel(ref horzEdge);
                }
            }
            else
            {
                if (horzEdge.OutIdx >= 0)
                {
                    AddOutPt(horzEdge, horzEdge.Top);
                }
                DeleteFromAel(horzEdge);
            }
        }

        //------------------------------------------------------------------------------

        private static Edge GetNextInAel(Edge e, Direction direction)
        {
            return direction == Direction.DLeftToRight ? e.NextInAel : e.PrevInAel;
        }

        //------------------------------------------------------------------------------

        private static bool IsMaxima(Edge e, double y)
        {
            return (e != null && Math.Abs(e.Top.Y - y) < float.Epsilon && e.NextInLml == null);
        }

        //------------------------------------------------------------------------------

        private static bool IsIntermediate(Edge e, double y)
        {
            return (Math.Abs(e.Top.Y - y) < float.Epsilon && e.NextInLml != null);
        }

        //------------------------------------------------------------------------------

        private static Edge GetMaximaPair(Edge e)
        {
            Edge result = null;
            if ((e.Next.Top == e.Top) && e.Next.NextInLml == null)
            {
                result = e.Next;
            }
            else if ((e.Prev.Top == e.Top) && e.Prev.NextInLml == null)
            {
                result = e.Prev;
            }
            if (result != null &&
                (result.OutIdx == Skip || (result.NextInAel == result.PrevInAel && !IsHorizontal(result))))
            {
                return null;
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private bool ProcessIntersections(cInt topY)
        {
            if (_mActiveEdges == null)
            {
                return true;
            }
            try
            {
                BuildIntersectList(topY);
                if (_mIntersectList.Count == 0)
                {
                    return true;
                }
                if (_mIntersectList.Count == 1 || FixupIntersectionOrder())
                {
                    ProcessIntersectList();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                _mSortedEdges = null;
                _mIntersectList.Clear();
                throw new ClipperException("ProcessIntersections error");
            }
            _mSortedEdges = null;
            return true;
        }

        //------------------------------------------------------------------------------

        private void BuildIntersectList(cInt topY)
        {
            if (_mActiveEdges == null)
            {
                return;
            }

            //prepare for sorting ...
            var e = _mActiveEdges;
            _mSortedEdges = e;
            while (e != null)
            {
                e.PrevInSel = e.PrevInAel;
                e.NextInSel = e.NextInAel;
                e.Curr.X = TopX(e, topY);
                e = e.NextInAel;
            }

            //bubblesort ...
            var isModified = true;
            while (isModified && _mSortedEdges != null)
            {
                isModified = false;
                e = _mSortedEdges;
                while (e.NextInSel != null)
                {
                    var eNext = e.NextInSel;
                    if (e.Curr.X > eNext.Curr.X)
                    {
                        IntPoint pt;
                        IntersectPoint(e, eNext, out pt);
                        var newNode = new IntersectNode { Edge1 = e, Edge2 = eNext, Pt = pt };
                        _mIntersectList.Add(newNode);

                        SwapPositionsInSel(e, eNext);
                        isModified = true;
                    }
                    else
                    {
                        e = eNext;
                    }
                }
                if (e.PrevInSel != null)
                {
                    e.PrevInSel.NextInSel = null;
                }
                else
                {
                    break;
                }
            }
            _mSortedEdges = null;
        }

        //------------------------------------------------------------------------------

        private static bool EdgesAdjacent(IntersectNode inode)
        {
            return (inode.Edge1.NextInSel == inode.Edge2) || (inode.Edge1.PrevInSel == inode.Edge2);
        }

        //------------------------------------------------------------------------------

        private bool FixupIntersectionOrder()
        {
            //pre-condition: intersections are sorted bottom-most first.
            //Now it's crucial that intersections are made only between adjacent edges,
            //so to ensure this the order of intersections may need adjusting ...
            _mIntersectList.Sort(_mIntersectNodeComparer);

            CopyAeltoSel();
            var cnt = _mIntersectList.Count;
            for (var i = 0; i < cnt; i++)
            {
                if (!EdgesAdjacent(_mIntersectList[i]))
                {
                    var j = i + 1;
                    while (j < cnt && !EdgesAdjacent(_mIntersectList[j]))
                    {
                        j++;
                    }
                    if (j == cnt)
                    {
                        return false;
                    }

                    var tmp = _mIntersectList[i];
                    _mIntersectList[i] = _mIntersectList[j];
                    _mIntersectList[j] = tmp;
                }
                SwapPositionsInSel(_mIntersectList[i].Edge1, _mIntersectList[i].Edge2);
            }
            return true;
        }

        //------------------------------------------------------------------------------

        private void ProcessIntersectList()
        {
            foreach (var iNode in _mIntersectList)
            {
                {
                    IntersectEdges(iNode.Edge1, iNode.Edge2, iNode.Pt);
                    SwapPositionsInAel(iNode.Edge1, iNode.Edge2);
                }
            }
            _mIntersectList.Clear();
        }

        //------------------------------------------------------------------------------

        internal static cInt Round(double value)
        {
            return value < 0 ? (cInt) (value - 0.5) : (cInt) (value + 0.5);
        }

        //------------------------------------------------------------------------------

        private static cInt TopX(Edge edge, cInt currentY)
        {
            if (currentY == edge.Top.Y)
            {
                return edge.Top.X;
            }
            return edge.Bot.X + Round(edge.Dx * (currentY - edge.Bot.Y));
        }

        //------------------------------------------------------------------------------

        private void IntersectPoint(Edge edge1, Edge edge2, out IntPoint ip)
        {
            ip = new IntPoint();
            double b1, b2;
            //nb: with very large coordinate values, it's possible for SlopesEqual() to 
            //return false but for the edge.Dx value be equal due to double precision rounding.
            if (Math.Abs(edge1.Dx - edge2.Dx) < float.Epsilon)
            {
                ip.Y = edge1.Curr.Y;
                ip.X = TopX(edge1, ip.Y);
                return;
            }

            if (edge1.Delta.X == 0)
            {
                ip.X = edge1.Bot.X;
                if (IsHorizontal(edge2))
                {
                    ip.Y = edge2.Bot.Y;
                }
                else
                {
                    b2 = edge2.Bot.Y - (edge2.Bot.X / edge2.Dx);
                    ip.Y = Round(ip.X / edge2.Dx + b2);
                }
            }
            else if (edge2.Delta.X == 0)
            {
                ip.X = edge2.Bot.X;
                if (IsHorizontal(edge1))
                {
                    ip.Y = edge1.Bot.Y;
                }
                else
                {
                    b1 = edge1.Bot.Y - (edge1.Bot.X / edge1.Dx);
                    ip.Y = Round(ip.X / edge1.Dx + b1);
                }
            }
            else
            {
                b1 = edge1.Bot.X - edge1.Bot.Y * edge1.Dx;
                b2 = edge2.Bot.X - edge2.Bot.Y * edge2.Dx;
                var q = (b2 - b1) / (edge1.Dx - edge2.Dx);
                ip.Y = Round(q);
                ip.X = Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx) ? Round(edge1.Dx * q + b1) : Round(edge2.Dx * q + b2);
            }

            if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
            {
                ip.Y = edge1.Top.Y > edge2.Top.Y ? edge1.Top.Y : edge2.Top.Y;
                ip.X = TopX(Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx) ? edge1 : edge2, ip.Y);
            }
            //finally, don't allow 'ip' to be BELOW curr.Y (ie bottom of scanbeam) ...
            if (ip.Y > edge1.Curr.Y)
            {
                ip.Y = edge1.Curr.Y;
                //better to use the more vertical edge to derive X ...
                ip.X = TopX(Math.Abs(edge1.Dx) > Math.Abs(edge2.Dx) ? edge2 : edge1, ip.Y);
            }
        }

        //------------------------------------------------------------------------------

        private void ProcessEdgesAtTopOfScanbeam(cInt topY)
        {
            var e = _mActiveEdges;
            while (e != null)
            {
                //1. process maxima, treating them as if they're 'bent' horizontal edges,
                //   but exclude maxima with horizontal edges. nb: e can't be a horizontal.
                var isMaximaEdge = IsMaxima(e, topY);

                if (isMaximaEdge)
                {
                    var eMaxPair = GetMaximaPair(e);
                    isMaximaEdge = (eMaxPair == null || !IsHorizontal(eMaxPair));
                }

                if (isMaximaEdge)
                {
                    var ePrev = e.PrevInAel;
                    DoMaxima(e);
                    e = ePrev == null ? _mActiveEdges : ePrev.NextInAel;
                }
                else
                {
                    //2. promote horizontal edges, otherwise update Curr.X and Curr.Y ...
                    if (IsIntermediate(e, topY) && IsHorizontal(e.NextInLml))
                    {
                        UpdateEdgeIntoAel(ref e);
                        if (e.OutIdx >= 0)
                        {
                            AddOutPt(e, e.Bot);
                        }
                        AddEdgeToSel(e);
                    }
                    else
                    {
                        e.Curr.X = TopX(e, topY);
                        e.Curr.Y = topY;
                    }

                    if (StrictlySimple)
                    {
                        var ePrev = e.PrevInAel;
                        if ((e.OutIdx >= 0) && (e.WindDelta != 0) && ePrev != null && (ePrev.OutIdx >= 0) &&
                            (ePrev.Curr.X == e.Curr.X) && (ePrev.WindDelta != 0))
                        {
                            var ip = new IntPoint(e.Curr);
#if use_xyz
                SetZ(ref ip, ePrev, e);
#endif
                            var op = AddOutPt(ePrev, ip);
                            var op2 = AddOutPt(e, ip);
                            AddJoin(op, op2, ip); //StrictlySimple (type-3) join
                        }
                    }

                    e = e.NextInAel;
                }
            }

            //3. Process horizontals at the Top of the scanbeam ...
            ProcessHorizontals(true);

            //4. Promote intermediate vertices ...
            e = _mActiveEdges;
            while (e != null)
            {
                if (IsIntermediate(e, topY))
                {
                    OutPt op = null;
                    if (e.OutIdx >= 0)
                    {
                        op = AddOutPt(e, e.Top);
                    }
                    UpdateEdgeIntoAel(ref e);

                    //if output polygons share an edge, they'll need joining later ...
                    var ePrev = e.PrevInAel;
                    var eNext = e.NextInAel;
                    if (ePrev != null && ePrev.Curr.X == e.Bot.X && ePrev.Curr.Y == e.Bot.Y && op != null &&
                        ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y && SlopesEqual(e, ePrev, MUseFullRange) &&
                        (e.WindDelta != 0) && (ePrev.WindDelta != 0))
                    {
                        var op2 = AddOutPt(ePrev, e.Bot);
                        AddJoin(op, op2, e.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == e.Bot.X && eNext.Curr.Y == e.Bot.Y && op != null &&
                             eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y && SlopesEqual(e, eNext, MUseFullRange) &&
                             (e.WindDelta != 0) && (eNext.WindDelta != 0))
                    {
                        var op2 = AddOutPt(eNext, e.Bot);
                        AddJoin(op, op2, e.Top);
                    }
                }
                e = e.NextInAel;
            }
        }

        //------------------------------------------------------------------------------

        private void DoMaxima(Edge e)
        {
            var eMaxPair = GetMaximaPair(e);
            if (eMaxPair == null)
            {
                if (e.OutIdx >= 0)
                {
                    AddOutPt(e, e.Top);
                }
                DeleteFromAel(e);
                return;
            }

            var eNext = e.NextInAel;
            while (eNext != null && eNext != eMaxPair)
            {
                IntersectEdges(e, eNext, e.Top);
                SwapPositionsInAel(e, eNext);
                eNext = e.NextInAel;
            }

            if (e.OutIdx == Unassigned && eMaxPair.OutIdx == Unassigned)
            {
                DeleteFromAel(e);
                DeleteFromAel(eMaxPair);
            }
            else if (e.OutIdx >= 0 && eMaxPair.OutIdx >= 0)
            {
                if (e.OutIdx >= 0)
                {
                    AddLocalMaxPoly(e, eMaxPair, e.Top);
                }
                DeleteFromAel(e);
                DeleteFromAel(eMaxPair);
            }
#if use_lines
        else if (e.WindDelta == 0)
        {
          if (e.OutIdx >= 0) 
          {
            AddOutPt(e, e.Top);
            e.OutIdx = Unassigned;
          }
          DeleteFromAel(e);

          if (eMaxPair.OutIdx >= 0)
          {
            AddOutPt(eMaxPair, e.Top);
            eMaxPair.OutIdx = Unassigned;
          }
          DeleteFromAel(eMaxPair);
        } 
#endif
            else
            {
                throw new ClipperException("DoMaxima error");
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Reverses the paths.
        /// </summary>
        /// <param name="polys">The paths.</param>
        public static void ReversePaths(Paths polys)
        {
            foreach (var poly in polys)
            {
                poly.Reverse();
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Orientates the specified path.
        /// </summary>
        /// <param name="poly">The path.</param>
        /// <returns></returns>
        public static bool Orientation(Path poly)
        {
            return Area(poly) >= 0;
        }

        //------------------------------------------------------------------------------

        private int PointCount(OutPt pts)
        {
            if (pts == null)
            {
                return 0;
            }
            var result = 0;
            var p = pts;
            do
            {
                result++;
                p = p.Next;
            } while (p != pts);
            return result;
        }

        //------------------------------------------------------------------------------

        private void BuildResult(Paths polyg)
        {
            polyg.Clear();
            polyg.Capacity = _mPolyOuts.Count;
            foreach (var outRec in _mPolyOuts)
            {
                if (outRec.Pts == null)
                {
                    continue;
                }
                var p = outRec.Pts.Prev;
                var cnt = PointCount(p);
                if (cnt < 2)
                {
                    continue;
                }
                var pg = new Path(cnt);
                for (var j = 0; j < cnt; j++)
                {
                    pg.Add(p.Pt);
                    p = p.Prev;
                }
                polyg.Add(pg);
            }
        }

        //------------------------------------------------------------------------------

        private void BuildResult2(PolyTree polytree)
        {
            polytree.Clear();

            //add each output polygon/contour to polytree ...
            polytree.MAllPolys.Capacity = _mPolyOuts.Count;
            foreach (var outRec in _mPolyOuts)
            {
                var cnt = PointCount(outRec.Pts);
                if ((outRec.IsOpen && cnt < 2) || (!outRec.IsOpen && cnt < 3))
                {
                    continue;
                }
                FixHoleLinkage(outRec);
                var pn = new PolyNode();
                polytree.MAllPolys.Add(pn);
                outRec.PolyNode = pn;
                pn.MPolygon.Capacity = cnt;
                var op = outRec.Pts.Prev;
                for (var j = 0; j < cnt; j++)
                {
                    pn.MPolygon.Add(op.Pt);
                    op = op.Prev;
                }
            }

            //fixup PolyNode links etc ...
            polytree.MChilds.Capacity = _mPolyOuts.Count;
            foreach (var outRec in _mPolyOuts.Where(outRec => outRec.PolyNode != null))
            {
                if (outRec.IsOpen)
                {
                    outRec.PolyNode.IsOpen = true;
                    polytree.AddChild(outRec.PolyNode);
                }
                else if (outRec.FirstLeft != null && outRec.FirstLeft.PolyNode != null)
                {
                    outRec.FirstLeft.PolyNode.AddChild(outRec.PolyNode);
                }
                else
                {
                    polytree.AddChild(outRec.PolyNode);
                }
            }
        }

        //------------------------------------------------------------------------------

        private void FixupOutPolygon(OutRec outRec)
        {
            //FixupOutPolygon() - removes duplicate points and simplifies consecutive
            //parallel edges by removing the middle vertex.
            OutPt lastOk = null;
            outRec.BottomPt = null;
            var pp = outRec.Pts;
            for (;;)
            {
                if (pp.Prev == pp || pp.Prev == pp.Next)
                {
                    outRec.Pts = null;
                    return;
                }
                //test for duplicate points and collinear edges ...
                if ((pp.Pt == pp.Next.Pt) || (pp.Pt == pp.Prev.Pt) ||
                    (SlopesEqual(pp.Prev.Pt, pp.Pt, pp.Next.Pt, MUseFullRange) &&
                     (!PreserveCollinear || !Pt2IsBetweenPt1AndPt3(pp.Prev.Pt, pp.Pt, pp.Next.Pt))))
                {
                    lastOk = null;
                    pp.Prev.Next = pp.Next;
                    pp.Next.Prev = pp.Prev;
                    pp = pp.Prev;
                }
                else if (pp == lastOk)
                {
                    break;
                }
                else
                {
                    if (lastOk == null)
                    {
                        lastOk = pp;
                    }
                    pp = pp.Next;
                }
            }
            outRec.Pts = pp;
        }

        //------------------------------------------------------------------------------

        private static OutPt DupOutPt(OutPt outPt, bool insertAfter)
        {
            var result = new OutPt { Pt = outPt.Pt, Idx = outPt.Idx };
            if (insertAfter)
            {
                result.Next = outPt.Next;
                result.Prev = outPt;
                outPt.Next.Prev = result;
                outPt.Next = result;
            }
            else
            {
                result.Prev = outPt.Prev;
                result.Next = outPt;
                outPt.Prev.Next = result;
                outPt.Prev = result;
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private bool GetOverlap(cInt a1, cInt a2, cInt b1, cInt b2, out cInt left, out cInt right)
        {
            if (a1 < a2)
            {
                if (b1 < b2)
                {
                    left = Math.Max(a1, b1);
                    right = Math.Min(a2, b2);
                }
                else
                {
                    left = Math.Max(a1, b2);
                    right = Math.Min(a2, b1);
                }
            }
            else
            {
                if (b1 < b2)
                {
                    left = Math.Max(a2, b1);
                    right = Math.Min(a1, b2);
                }
                else
                {
                    left = Math.Max(a2, b2);
                    right = Math.Min(a1, b1);
                }
            }
            return left < right;
        }

        //------------------------------------------------------------------------------

        private bool JoinHorz(OutPt op1, OutPt op1B, OutPt op2, OutPt op2B, IntPoint pt, bool discardLeft)
        {
            var dir1 = (op1.Pt.X > op1B.Pt.X ? Direction.DRightToLeft : Direction.DLeftToRight);
            var dir2 = (op2.Pt.X > op2B.Pt.X ? Direction.DRightToLeft : Direction.DLeftToRight);
            if (dir1 == dir2)
            {
                return false;
            }

            //When DiscardLeft, we want Op1b to be on the Left of Op1, otherwise we
            //want Op1b to be on the Right. (And likewise with Op2 and Op2b.)
            //So, to facilitate this while inserting Op1b and Op2b ...
            //when DiscardLeft, make sure we're AT or RIGHT of Pt before adding Op1b,
            //otherwise make sure we're AT or LEFT of Pt. (Likewise with Op2b.)
            if (dir1 == Direction.DLeftToRight)
            {
                while (op1.Next.Pt.X <= pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == pt.Y)
                {
                    op1 = op1.Next;
                }
                if (discardLeft && (op1.Pt.X != pt.X))
                {
                    op1 = op1.Next;
                }
                op1B = DupOutPt(op1, !discardLeft);
                if (op1B.Pt != pt)
                {
                    op1 = op1B;
                    op1.Pt = pt;
                    op1B = DupOutPt(op1, !discardLeft);
                }
            }
            else
            {
                while (op1.Next.Pt.X >= pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == pt.Y)
                {
                    op1 = op1.Next;
                }
                if (!discardLeft && (op1.Pt.X != pt.X))
                {
                    op1 = op1.Next;
                }
                op1B = DupOutPt(op1, discardLeft);
                if (op1B.Pt != pt)
                {
                    op1 = op1B;
                    op1.Pt = pt;
                    op1B = DupOutPt(op1, discardLeft);
                }
            }

            if (dir2 == Direction.DLeftToRight)
            {
                while (op2.Next.Pt.X <= pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == pt.Y)
                {
                    op2 = op2.Next;
                }
                if (discardLeft && (op2.Pt.X != pt.X))
                {
                    op2 = op2.Next;
                }
                op2B = DupOutPt(op2, !discardLeft);
                if (op2B.Pt != pt)
                {
                    op2 = op2B;
                    op2.Pt = pt;
                    op2B = DupOutPt(op2, !discardLeft);
                }
            }
            else
            {
                while (op2.Next.Pt.X >= pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == pt.Y)
                {
                    op2 = op2.Next;
                }
                if (!discardLeft && (op2.Pt.X != pt.X))
                {
                    op2 = op2.Next;
                }
                op2B = DupOutPt(op2, discardLeft);
                if (op2B.Pt != pt)
                {
                    op2 = op2B;
                    op2.Pt = pt;
                    op2B = DupOutPt(op2, discardLeft);
                }
            }

            if ((dir1 == Direction.DLeftToRight) == discardLeft)
            {
                op1.Prev = op2;
                op2.Next = op1;
                op1B.Next = op2B;
                op2B.Prev = op1B;
            }
            else
            {
                op1.Next = op2;
                op2.Prev = op1;
                op1B.Prev = op2B;
                op2B.Next = op1B;
            }
            return true;
        }

        //------------------------------------------------------------------------------

        private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
        {
            OutPt op1 = j.OutPt1, op1B;
            OutPt op2 = j.OutPt2, op2B;

            //There are 3 kinds of joins for output polygons ...
            //1. Horizontal joins where Join.OutPt1 & Join.OutPt2 are a vertices anywhere
            //along (horizontal) collinear edges (& Join.OffPt is on the same horizontal).
            //2. Non-horizontal joins where Join.OutPt1 & Join.OutPt2 are at the same
            //location at the Bottom of the overlapping segment (& Join.OffPt is above).
            //3. StrictlySimple joins where edges touch but are not collinear and where
            //Join.OutPt1, Join.OutPt2 & Join.OffPt all share the same point.
            var isHorizontal = (j.OutPt1.Pt.Y == j.OffPt.Y);

            bool reverse1;
            bool reverse2;
            if (isHorizontal && (j.OffPt == j.OutPt1.Pt) && (j.OffPt == j.OutPt2.Pt))
            {
                //Strictly Simple join ...
                if (outRec1 != outRec2)
                {
                    return false;
                }
                op1B = j.OutPt1.Next;
                while (op1B != op1 && (op1B.Pt == j.OffPt))
                {
                    op1B = op1B.Next;
                }
                reverse1 = (op1B.Pt.Y > j.OffPt.Y);
                op2B = j.OutPt2.Next;
                while (op2B != op2 && (op2B.Pt == j.OffPt))
                {
                    op2B = op2B.Next;
                }
                reverse2 = (op2B.Pt.Y > j.OffPt.Y);
                if (reverse1 == reverse2)
                {
                    return false;
                }
                if (reverse1)
                {
                    op1B = DupOutPt(op1, false);
                    op2B = DupOutPt(op2, true);
                    op1.Prev = op2;
                    op2.Next = op1;
                    op1B.Next = op2B;
                    op2B.Prev = op1B;
                    j.OutPt1 = op1;
                    j.OutPt2 = op1B;
                    return true;
                }
                op1B = DupOutPt(op1, true);
                op2B = DupOutPt(op2, false);
                op1.Next = op2;
                op2.Prev = op1;
                op1B.Prev = op2B;
                op2B.Next = op1B;
                j.OutPt1 = op1;
                j.OutPt2 = op1B;
                return true;
            }
            if (isHorizontal)
            {
                //treat horizontal joins differently to non-horizontal joins since with
                //them we're not yet sure where the overlapping is. OutPt1.Pt & OutPt2.Pt
                //may be anywhere along the horizontal edge.
                op1B = op1;
                while (op1.Prev.Pt.Y == op1.Pt.Y && op1.Prev != op1B && op1.Prev != op2)
                {
                    op1 = op1.Prev;
                }
                while (op1B.Next.Pt.Y == op1B.Pt.Y && op1B.Next != op1 && op1B.Next != op2)
                {
                    op1B = op1B.Next;
                }
                if (op1B.Next == op1 || op1B.Next == op2)
                {
                    return false; //a flat 'polygon'
                }

                op2B = op2;
                while (op2.Prev.Pt.Y == op2.Pt.Y && op2.Prev != op2B && op2.Prev != op1B)
                {
                    op2 = op2.Prev;
                }
                while (op2B.Next.Pt.Y == op2B.Pt.Y && op2B.Next != op2 && op2B.Next != op1)
                {
                    op2B = op2B.Next;
                }
                if (op2B.Next == op2 || op2B.Next == op1)
                {
                    return false; //a flat 'polygon'
                }

                cInt left, right;
                //Op1 -. Op1b & Op2 -. Op2b are the extremites of the horizontal edges
                if (!GetOverlap(op1.Pt.X, op1B.Pt.X, op2.Pt.X, op2B.Pt.X, out left, out right))
                {
                    return false;
                }

                //DiscardLeftSide: when overlapping edges are joined, a spike will created
                //which needs to be cleaned up. However, we don't want Op1 or Op2 caught up
                //on the discard Side as either may still be needed for other joins ...
                IntPoint pt;
                bool discardLeftSide;
                if (op1.Pt.X >= left && op1.Pt.X <= right)
                {
                    pt = op1.Pt;
                    discardLeftSide = (op1.Pt.X > op1B.Pt.X);
                }
                else if (op2.Pt.X >= left && op2.Pt.X <= right)
                {
                    pt = op2.Pt;
                    discardLeftSide = (op2.Pt.X > op2B.Pt.X);
                }
                else if (op1B.Pt.X >= left && op1B.Pt.X <= right)
                {
                    pt = op1B.Pt;
                    discardLeftSide = op1B.Pt.X > op1.Pt.X;
                }
                else
                {
                    pt = op2B.Pt;
                    discardLeftSide = (op2B.Pt.X > op2.Pt.X);
                }
                j.OutPt1 = op1;
                j.OutPt2 = op2;
                return JoinHorz(op1, op1B, op2, op2B, pt, discardLeftSide);
            }
            //nb: For non-horizontal joins ...
            //    1. Jr.OutPt1.Pt.Y == Jr.OutPt2.Pt.Y
            //    2. Jr.OutPt1.Pt > Jr.OffPt.Y

            //make sure the polygons are correctly oriented ...
            op1B = op1.Next;
            while ((op1B.Pt == op1.Pt) && (op1B != op1))
            {
                op1B = op1B.Next;
            }
            reverse1 = ((op1B.Pt.Y > op1.Pt.Y) || !SlopesEqual(op1.Pt, op1B.Pt, j.OffPt, MUseFullRange));
            if (reverse1)
            {
                op1B = op1.Prev;
                while ((op1B.Pt == op1.Pt) && (op1B != op1))
                {
                    op1B = op1B.Prev;
                }
                if ((op1B.Pt.Y > op1.Pt.Y) || !SlopesEqual(op1.Pt, op1B.Pt, j.OffPt, MUseFullRange))
                {
                    return false;
                }
            }
            op2B = op2.Next;
            while ((op2B.Pt == op2.Pt) && (op2B != op2))
            {
                op2B = op2B.Next;
            }
            reverse2 = ((op2B.Pt.Y > op2.Pt.Y) || !SlopesEqual(op2.Pt, op2B.Pt, j.OffPt, MUseFullRange));
            if (reverse2)
            {
                op2B = op2.Prev;
                while ((op2B.Pt == op2.Pt) && (op2B != op2))
                {
                    op2B = op2B.Prev;
                }
                if ((op2B.Pt.Y > op2.Pt.Y) || !SlopesEqual(op2.Pt, op2B.Pt, j.OffPt, MUseFullRange))
                {
                    return false;
                }
            }

            if ((op1B == op1) || (op2B == op2) || (op1B == op2B) || ((outRec1 == outRec2) && (reverse1 == reverse2)))
            {
                return false;
            }

            if (reverse1)
            {
                op1B = DupOutPt(op1, false);
                op2B = DupOutPt(op2, true);
                op1.Prev = op2;
                op2.Next = op1;
                op1B.Next = op2B;
                op2B.Prev = op1B;
                j.OutPt1 = op1;
                j.OutPt2 = op1B;
                return true;
            }
            op1B = DupOutPt(op1, true);
            op2B = DupOutPt(op2, false);
            op1.Next = op2;
            op2.Prev = op1;
            op1B.Prev = op2B;
            op2B.Next = op1B;
            j.OutPt1 = op1;
            j.OutPt2 = op1B;
            return true;
        }

        //----------------------------------------------------------------------

        /// <summary>
        ///     Checks if a point is in the polygon.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static int PointInPolygon(IntPoint pt, Path path)
        {
            //returns 0 if false, +1 if true, -1 if pt ON polygon boundary
            //See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann & Agathos
            //http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.88.5498&rep=rep1&type=pdf
            int result = 0, cnt = path.Count;
            if (cnt < 3)
            {
                return 0;
            }
            var ip = path[0];
            for (var i = 1; i <= cnt; ++i)
            {
                var ipNext = (i == cnt ? path[0] : path[i]);
                if (ipNext.Y == pt.Y)
                {
                    if ((ipNext.X == pt.X) || (ip.Y == pt.Y && ((ipNext.X > pt.X) == (ip.X < pt.X))))
                    {
                        return -1;
                    }
                }
                if ((ip.Y < pt.Y) != (ipNext.Y < pt.Y))
                {
                    if (ip.X >= pt.X)
                    {
                        if (ipNext.X > pt.X)
                        {
                            result = 1 - result;
                        }
                        else
                        {
                            var d = (double) (ip.X - pt.X) * (ipNext.Y - pt.Y) -
                                    (double) (ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (Math.Abs(d) < float.Epsilon)
                            {
                                return -1;
                            }
                            if ((d > 0) == (ipNext.Y > ip.Y))
                            {
                                result = 1 - result;
                            }
                        }
                    }
                    else
                    {
                        if (ipNext.X > pt.X)
                        {
                            var d = (double) (ip.X - pt.X) * (ipNext.Y - pt.Y) -
                                    (double) (ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (Math.Abs(d) < float.Epsilon)
                            {
                                return -1;
                            }
                            if ((d > 0) == (ipNext.Y > ip.Y))
                            {
                                result = 1 - result;
                            }
                        }
                    }
                }
                ip = ipNext;
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private static int PointInPolygon(IntPoint pt, OutPt op)
        {
            //returns 0 if false, +1 if true, -1 if pt ON polygon boundary
            //See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann & Agathos
            //http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.88.5498&rep=rep1&type=pdf
            var result = 0;
            var startOp = op;
            cInt ptx = pt.X, pty = pt.Y;
            cInt poly0X = op.Pt.X, poly0Y = op.Pt.Y;
            do
            {
                op = op.Next;
                cInt poly1X = op.Pt.X, poly1Y = op.Pt.Y;

                if (poly1Y == pty)
                {
                    if ((poly1X == ptx) || (poly0Y == pty && ((poly1X > ptx) == (poly0X < ptx))))
                    {
                        return -1;
                    }
                }
                if ((poly0Y < pty) != (poly1Y < pty))
                {
                    if (poly0X >= ptx)
                    {
                        if (poly1X > ptx)
                        {
                            result = 1 - result;
                        }
                        else
                        {
                            var d = (double) (poly0X - ptx) * (poly1Y - pty) - (double) (poly1X - ptx) * (poly0Y - pty);
                            if (Math.Abs(d) < float.Epsilon)
                            {
                                return -1;
                            }
                            if ((d > 0) == (poly1Y > poly0Y))
                            {
                                result = 1 - result;
                            }
                        }
                    }
                    else
                    {
                        if (poly1X > ptx)
                        {
                            var d = (double) (poly0X - ptx) * (poly1Y - pty) - (double) (poly1X - ptx) * (poly0Y - pty);
                            if (Math.Abs(d) < float.Epsilon)
                            {
                                return -1;
                            }
                            if ((d > 0) == (poly1Y > poly0Y))
                            {
                                result = 1 - result;
                            }
                        }
                    }
                }
                poly0X = poly1X;
                poly0Y = poly1Y;
            } while (startOp != op);
            return result;
        }

        //------------------------------------------------------------------------------

        private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
        {
            var op = outPt1;
            do
            {
                //nb: PointInPolygon returns 0 if false, +1 if true, -1 if pt on polygon
                var res = PointInPolygon(op.Pt, outPt2);
                if (res >= 0)
                {
                    return res > 0;
                }
                op = op.Next;
            } while (op != outPt1);
            return true;
        }

        //----------------------------------------------------------------------

        private void FixupFirstLefts1(OutRec oldOutRec, OutRec newOutRec)
        {
            foreach (var outRec in from outRec in _mPolyOuts
                where outRec.Pts != null && outRec.FirstLeft != null
                let firstLeft = ParseFirstLeft(outRec.FirstLeft)
                where firstLeft == oldOutRec
                where Poly2ContainsPoly1(outRec.Pts, newOutRec.Pts)
                select outRec)
            {
                outRec.FirstLeft = newOutRec;
            }
        }

        //----------------------------------------------------------------------

        private void FixupFirstLefts2(OutRec oldOutRec, OutRec newOutRec)
        {
            foreach (var outRec in _mPolyOuts.Where(outRec => outRec.FirstLeft == oldOutRec))
            {
                outRec.FirstLeft = newOutRec;
            }
        }

        //----------------------------------------------------------------------

        private static OutRec ParseFirstLeft(OutRec firstLeft)
        {
            while (firstLeft != null && firstLeft.Pts == null)
            {
                firstLeft = firstLeft.FirstLeft;
            }
            return firstLeft;
        }

        //------------------------------------------------------------------------------

        private void JoinCommonEdges()
        {
            foreach (var @join in _mJoins)
            {
                var outRec1 = GetOutRec(@join.OutPt1.Idx);
                var outRec2 = GetOutRec(@join.OutPt2.Idx);

                if (outRec1.Pts == null || outRec2.Pts == null)
                {
                    continue;
                }

                //get the polygon fragment with the correct hole state (FirstLeft)
                //before calling JoinPoints() ...
                OutRec holeStateRec;
                if (outRec1 == outRec2)
                {
                    holeStateRec = outRec1;
                }
                else if (Param1RightOfParam2(outRec1, outRec2))
                {
                    holeStateRec = outRec2;
                }
                else if (Param1RightOfParam2(outRec2, outRec1))
                {
                    holeStateRec = outRec1;
                }
                else
                {
                    holeStateRec = GetLowermostRec(outRec1, outRec2);
                }

                if (!JoinPoints(@join, outRec1, outRec2))
                {
                    continue;
                }

                if (outRec1 == outRec2)
                {
                    //instead of joining two polygons, we've just created a new one by
                    //splitting one polygon into two.
                    outRec1.Pts = @join.OutPt1;
                    outRec1.BottomPt = null;
                    outRec2 = CreateOutRec();
                    outRec2.Pts = @join.OutPt2;

                    //update all OutRec2.Pts Idx's ...
                    UpdateOutPtIdxs(outRec2);

                    //We now need to check every OutRec.FirstLeft pointer. If it points
                    //to OutRec1 it may need to point to OutRec2 instead ...
                    if (_mUsingPolyTree)
                    {
                        for (var j = 0; j < _mPolyOuts.Count - 1; j++)
                        {
                            var oRec = _mPolyOuts[j];
                            if (oRec.Pts == null || ParseFirstLeft(oRec.FirstLeft) != outRec1 ||
                                oRec.IsHole == outRec1.IsHole)
                            {
                                continue;
                            }
                            if (Poly2ContainsPoly1(oRec.Pts, @join.OutPt2))
                            {
                                oRec.FirstLeft = outRec2;
                            }
                        }
                    }

                    if (Poly2ContainsPoly1(outRec2.Pts, outRec1.Pts))
                    {
                        //outRec2 is contained by outRec1 ...
                        outRec2.IsHole = !outRec1.IsHole;
                        outRec2.FirstLeft = outRec1;

                        //fixup FirstLeft pointers that may need reassigning to OutRec1
                        if (_mUsingPolyTree)
                        {
                            FixupFirstLefts2(outRec2, outRec1);
                        }

                        if ((outRec2.IsHole ^ ReverseSolution) == (Area(outRec2) > 0))
                        {
                            ReversePolyPtLinks(outRec2.Pts);
                        }
                    }
                    else if (Poly2ContainsPoly1(outRec1.Pts, outRec2.Pts))
                    {
                        //outRec1 is contained by outRec2 ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec1.IsHole = !outRec2.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;
                        outRec1.FirstLeft = outRec2;

                        //fixup FirstLeft pointers that may need reassigning to OutRec1
                        if (_mUsingPolyTree)
                        {
                            FixupFirstLefts2(outRec1, outRec2);
                        }

                        if ((outRec1.IsHole ^ ReverseSolution) == (Area(outRec1) > 0))
                        {
                            ReversePolyPtLinks(outRec1.Pts);
                        }
                    }
                    else
                    {
                        //the 2 polygons are completely separate ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;

                        //fixup FirstLeft pointers that may need reassigning to OutRec2
                        if (_mUsingPolyTree)
                        {
                            FixupFirstLefts1(outRec1, outRec2);
                        }
                    }
                }
                else
                {
                    //joined 2 polygons together ...

                    outRec2.Pts = null;
                    outRec2.BottomPt = null;
                    outRec2.Idx = outRec1.Idx;

                    outRec1.IsHole = holeStateRec.IsHole;
                    if (holeStateRec == outRec2)
                    {
                        outRec1.FirstLeft = outRec2.FirstLeft;
                    }
                    outRec2.FirstLeft = outRec1;

                    //fixup FirstLeft pointers that may need reassigning to OutRec1
                    if (_mUsingPolyTree)
                    {
                        FixupFirstLefts2(outRec2, outRec1);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        private static void UpdateOutPtIdxs(OutRec outrec)
        {
            var op = outrec.Pts;
            do
            {
                op.Idx = outrec.Idx;
                op = op.Prev;
            } while (op != outrec.Pts);
        }

        //------------------------------------------------------------------------------

        private void DoSimplePolygons()
        {
            var i = 0;
            while (i < _mPolyOuts.Count)
            {
                var outrec = _mPolyOuts[i++];
                var op = outrec.Pts;
                if (op == null || outrec.IsOpen)
                {
                    continue;
                }
                do //for each Pt in Polygon until duplicate found do ...
                {
                    var op2 = op.Next;
                    while (op2 != outrec.Pts)
                    {
                        if ((op.Pt == op2.Pt) && op2.Next != op && op2.Prev != op)
                        {
                            //split the polygon into two ...
                            var op3 = op.Prev;
                            var op4 = op2.Prev;
                            op.Prev = op4;
                            op4.Next = op;
                            op2.Prev = op3;
                            op3.Next = op2;

                            outrec.Pts = op;
                            var outrec2 = CreateOutRec();
                            outrec2.Pts = op2;
                            UpdateOutPtIdxs(outrec2);
                            if (Poly2ContainsPoly1(outrec2.Pts, outrec.Pts))
                            {
                                //OutRec2 is contained by OutRec1 ...
                                outrec2.IsHole = !outrec.IsHole;
                                outrec2.FirstLeft = outrec;
                                if (_mUsingPolyTree)
                                {
                                    FixupFirstLefts2(outrec2, outrec);
                                }
                            }
                            else if (Poly2ContainsPoly1(outrec.Pts, outrec2.Pts))
                            {
                                //OutRec1 is contained by OutRec2 ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec.IsHole = !outrec2.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                                outrec.FirstLeft = outrec2;
                                if (_mUsingPolyTree)
                                {
                                    FixupFirstLefts2(outrec, outrec2);
                                }
                            }
                            else
                            {
                                //the 2 polygons are separate ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                                if (_mUsingPolyTree)
                                {
                                    FixupFirstLefts1(outrec, outrec2);
                                }
                            }
                            op2 = op; //ie get ready for the next iteration
                        }
                        op2 = op2.Next;
                    }
                    op = op.Next;
                } while (op != outrec.Pts);
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets the area of the specified polygon.
        /// </summary>
        /// <param name="poly">The polygon.</param>
        /// <returns></returns>
        public static double Area(Path poly)
        {
            var cnt = poly.Count;
            if (cnt < 3)
            {
                return 0;
            }
            double a = 0;
            for (int i = 0, j = cnt - 1; i < cnt; ++i)
            {
                a += ((double) poly[j].X + poly[i].X) * ((double) poly[j].Y - poly[i].Y);
                j = i;
            }
            return -a * 0.5;
        }

        //------------------------------------------------------------------------------

        private static double Area(OutRec outRec)
        {
            var op = outRec.Pts;
            if (op == null)
            {
                return 0;
            }
            double a = 0;
            do
            {
                a = a + (op.Prev.Pt.X + op.Pt.X) * (double) (op.Prev.Pt.Y - op.Pt.Y);
                op = op.Next;
            } while (op != outRec.Pts);
            return a * 0.5;
        }

        //------------------------------------------------------------------------------
        // SimplifyPolygon functions ...
        // Convert self-intersecting polygons into simple polygons
        //------------------------------------------------------------------------------

        /// <summary>
        ///     Simplifies the polygon.
        /// </summary>
        /// <param name="poly">The polygon.</param>
        /// <param name="fillType">Type of the fill.</param>
        /// <returns></returns>
        public static Paths SimplifyPolygon(Path poly, PolyFillType fillType = PolyFillType.PftEvenOdd)
        {
            var result = new Paths();
            var c = new Clipper { StrictlySimple = true };
            c.AddPath(poly, PolyType.PtSubject, true);
            c.Execute(ClipType.CtUnion, result, fillType, fillType);
            return result;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Simplifies the polygons.
        /// </summary>
        /// <param name="polys">The polygon.</param>
        /// <param name="fillType">Type of the fill.</param>
        /// <returns></returns>
        public static Paths SimplifyPolygons(Paths polys, PolyFillType fillType = PolyFillType.PftEvenOdd)
        {
            var result = new Paths();
            var c = new Clipper { StrictlySimple = true };
            c.AddPaths(polys, PolyType.PtSubject, true);
            c.Execute(ClipType.CtUnion, result, fillType, fillType);
            return result;
        }

        //------------------------------------------------------------------------------

        private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
        {
            //The equation of a line in general form (Ax + By + C = 0)
            //given 2 points (x¹,y¹) & (x²,y²) is ...
            //(y¹ - y²)x + (x² - x¹)y + (y² - y¹)x¹ - (x² - x¹)y¹ = 0
            //A = (y¹ - y²); B = (x² - x¹); C = (y² - y¹)x¹ - (x² - x¹)y¹
            //perpendicular distance of point (x³,y³) = (Ax³ + By³ + C)/Sqrt(A² + B²)
            //see http://en.wikipedia.org/wiki/Perpendicular_distance
            double a = ln1.Y - ln2.Y;
            double b = ln2.X - ln1.X;
            var c = a * ln1.X + b * ln1.Y;
            c = a * pt.X + b * pt.Y - c;
            return (c * c) / (a * a + b * b);
        }

        //---------------------------------------------------------------------------

        private static bool SlopesNearCollinear(IntPoint pt1, IntPoint pt2, IntPoint pt3, double distSqrd)
        {
            //this function is more accurate when the point that's GEOMETRICALLY 
            //between the other 2 points is the one that's tested for distance.  
            //nb: with 'spikes', either pt1 or pt3 is geometrically between the other pts                    
            if (Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y))
            {
                if ((pt1.X > pt2.X) == (pt1.X < pt3.X))
                {
                    return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
                }
                if ((pt2.X > pt1.X) == (pt2.X < pt3.X))
                {
                    return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
                }
                return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
            }
            if ((pt1.Y > pt2.Y) == (pt1.Y < pt3.Y))
            {
                return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
            }
            if ((pt2.Y > pt1.Y) == (pt2.Y < pt3.Y))
            {
                return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
            }
            return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
        }

        //------------------------------------------------------------------------------

        private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
        {
            var dx = (double) pt1.X - pt2.X;
            var dy = (double) pt1.Y - pt2.Y;
            return ((dx * dx) + (dy * dy) <= distSqrd);
        }

        //------------------------------------------------------------------------------

        private static OutPt ExcludeOp(OutPt op)
        {
            var result = op.Prev;
            result.Next = op.Next;
            op.Next.Prev = result;
            result.Idx = 0;
            return result;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Cleans the polygon.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static Path CleanPolygon(Path path, double distance = 1.415)
        {
            //distance = proximity in units/pixels below which vertices will be stripped. 
            //Default ~= sqrt(2) so when adjacent vertices or semi-adjacent vertices have 
            //both x & y coords within 1 unit, then the second vertex will be stripped.

            var cnt = path.Count;

            if (cnt == 0)
            {
                return new Path();
            }

            var outPts = new OutPt[cnt];
            for (var i = 0; i < cnt; ++i)
            {
                outPts[i] = new OutPt();
            }

            for (var i = 0; i < cnt; ++i)
            {
                outPts[i].Pt = path[i];
                outPts[i].Next = outPts[(i + 1) % cnt];
                outPts[i].Next.Prev = outPts[i];
                outPts[i].Idx = 0;
            }

            var distSqrd = distance * distance;
            var op = outPts[0];
            while (op.Idx == 0 && op.Next != op.Prev)
            {
                if (PointsAreClose(op.Pt, op.Prev.Pt, distSqrd))
                {
                    op = ExcludeOp(op);
                    cnt--;
                }
                else if (PointsAreClose(op.Prev.Pt, op.Next.Pt, distSqrd))
                {
                    ExcludeOp(op.Next);
                    op = ExcludeOp(op);
                    cnt -= 2;
                }
                else if (SlopesNearCollinear(op.Prev.Pt, op.Pt, op.Next.Pt, distSqrd))
                {
                    op = ExcludeOp(op);
                    cnt--;
                }
                else
                {
                    op.Idx = 1;
                    op = op.Next;
                }
            }

            if (cnt < 3)
            {
                cnt = 0;
            }
            var result = new Path(cnt);
            for (var i = 0; i < cnt; ++i)
            {
                result.Add(op.Pt);
                op = op.Next;
            }
            return result;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Cleans the polygons.
        /// </summary>
        /// <param name="polys">The polygon.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static Paths CleanPolygons(Paths polys, double distance = 1.415)
        {
            var result = new Paths(polys.Count);
            result.AddRange(polys.Select(t => CleanPolygon(t, distance)));
            return result;
        }

        //------------------------------------------------------------------------------

        internal static Paths Minkowski(Path pattern, Path path, bool isSum, bool isClosed)
        {
            var delta = (isClosed ? 1 : 0);
            var polyCnt = pattern.Count;
            var pathCnt = path.Count;
            var result = new Paths(pathCnt);
            if (isSum)
            {
                for (var i = 0; i < pathCnt; i++)
                {
                    var p = new Path(polyCnt);
                    p.AddRange(pattern.Select(ip => new IntPoint(path[i].X + ip.X, path[i].Y + ip.Y)));
                    result.Add(p);
                }
            }
            else
            {
                for (var i = 0; i < pathCnt; i++)
                {
                    var p = new Path(polyCnt);
                    p.AddRange(pattern.Select(ip => new IntPoint(path[i].X - ip.X, path[i].Y - ip.Y)));
                    result.Add(p);
                }
            }

            var quads = new Paths((pathCnt + delta) * (polyCnt + 1));
            for (var i = 0; i < pathCnt - 1 + delta; i++)
            {
                for (var j = 0; j < polyCnt; j++)
                {
                    var quad = new Path(4)
                    {
                        result[i % pathCnt][j % polyCnt],
                        result[(i + 1) % pathCnt][j % polyCnt],
                        result[(i + 1) % pathCnt][(j + 1) % polyCnt],
                        result[i % pathCnt][(j + 1) % polyCnt]
                    };
                    if (!Orientation(quad))
                    {
                        quad.Reverse();
                    }
                    quads.Add(quad);
                }
            }
            return quads;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets the Minkowskis sum.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="path">The path.</param>
        /// <param name="pathIsClosed">Whether the path is closed or not.</param>
        /// <returns></returns>
        public static Paths MinkowskiSum(Path pattern, Path path, bool pathIsClosed)
        {
            var paths = Minkowski(pattern, path, true, pathIsClosed);
            var c = new Clipper();
            c.AddPaths(paths, PolyType.PtSubject, true);
            c.Execute(ClipType.CtUnion, paths, PolyFillType.PftNonZero, PolyFillType.PftNonZero);
            return paths;
        }

        //------------------------------------------------------------------------------

        private static Path TranslatePath(IReadOnlyList<IntPoint> path, IntPoint delta)
        {
            var outPath = new Path(path.Count);
            for (var i = 0; i < path.Count; i++)
            {
                outPath.Add(new IntPoint(path[i].X + delta.X, path[i].Y + delta.Y));
            }
            return outPath;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets the Minkowskis sum.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="pathIsClosed">Whether the path is closed or not.</param>
        /// <returns></returns>
        public static Paths MinkowskiSum(Path pattern, Paths paths, bool pathIsClosed)
        {
            var solution = new Paths();
            var c = new Clipper();
            foreach (var t in paths)
            {
                var tmp = Minkowski(pattern, t, true, pathIsClosed);
                c.AddPaths(tmp, PolyType.PtSubject, true);
                if (pathIsClosed)
                {
                    var path = TranslatePath(t, pattern[0]);
                    c.AddPath(path, PolyType.PtClip, true);
                }
            }
            c.Execute(ClipType.CtUnion, solution, PolyFillType.PftNonZero, PolyFillType.PftNonZero);
            return solution;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Gets the Minkowskis difference.
        /// </summary>
        /// <param name="poly1">Polygon 1.</param>
        /// <param name="poly2">Polygon 2.</param>
        /// <returns></returns>
        public static Paths MinkowskiDiff(Path poly1, Path poly2)
        {
            var paths = Minkowski(poly1, poly2, false, true);
            var c = new Clipper();
            c.AddPaths(paths, PolyType.PtSubject, true);
            c.Execute(ClipType.CtUnion, paths, PolyFillType.PftNonZero, PolyFillType.PftNonZero);
            return paths;
        }

        //------------------------------------------------------------------------------

        internal enum NodeType
        {
            NtAny,
            NtOpen,
            NtClosed
        };

        /// <summary>
        ///     Converts a <see cref="PolyTree" /> to a <see cref="Paths" />.
        /// </summary>
        /// <param name="polytree">The polytree.</param>
        /// <returns></returns>
        public static Paths PolyTreeToPaths(PolyTree polytree)
        {
            var result = new Paths { Capacity = polytree.Total };
            AddPolyNodeToPaths(polytree, NodeType.NtAny, result);
            return result;
        }

        //------------------------------------------------------------------------------

        internal static void AddPolyNodeToPaths(PolyNode polynode, NodeType nt, Paths paths)
        {
            var match = true;
            switch (nt)
            {
                case NodeType.NtOpen:
                    return;
                case NodeType.NtClosed:
                    match = !polynode.IsOpen;
                    break;
            }

            if (polynode.MPolygon.Count > 0 && match)
            {
                paths.Add(polynode.MPolygon);
            }
            foreach (var pn in polynode.Childs)
            {
                AddPolyNodeToPaths(pn, nt, paths);
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Opens the paths from poly tree.
        /// </summary>
        /// <param name="polytree">The polytree.</param>
        /// <returns></returns>
        public static Paths OpenPathsFromPolyTree(PolyTree polytree)
        {
            var result = new Paths { Capacity = polytree.ChildCount };
            for (var i = 0; i < polytree.ChildCount; i++)
            {
                if (polytree.Childs[i].IsOpen)
                {
                    result.Add(polytree.Childs[i].MPolygon);
                }
            }
            return result;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Closeds the paths from poly tree.
        /// </summary>
        /// <param name="polytree">The polytree.</param>
        /// <returns></returns>
        public static Paths ClosedPathsFromPolyTree(PolyTree polytree)
        {
            var result = new Paths { Capacity = polytree.Total };
            AddPolyNodeToPaths(polytree, NodeType.NtClosed, result);
            return result;
        }

        //------------------------------------------------------------------------------
    } //end Clipper

    /// <summary>
    ///     Clipping offset.
    /// </summary>
    public class ClipperOffset
    {
        private const double TwoPi = Math.PI * 2;
        private const double DefArcTolerance = 0.25;
        private readonly List<DoublePoint> _mNormals = new List<DoublePoint>();
        private readonly PolyNode _mPolyNodes = new PolyNode();
        private double _mDelta, _mSinA, _mSin, _mCos;
        private Path _mDestPoly;
        private Paths _mDestPolys;
        private IntPoint _mLowest;
        private double _mMiterLim, _mStepsPerRad;
        private Path _mSrcPoly;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClipperOffset" /> class.
        /// </summary>
        /// <param name="miterLimit">The miter limit.</param>
        /// <param name="arcTolerance">The arc tolerance.</param>
        public ClipperOffset(double miterLimit = 2.0, double arcTolerance = DefArcTolerance)
        {
            MiterLimit = miterLimit;
            ArcTolerance = arcTolerance;
            _mLowest.X = -1;
        }

        /// <summary>
        ///     Gets or sets the arc tolerance.
        /// </summary>
        /// <value>
        ///     The arc tolerance.
        /// </value>
        public double ArcTolerance { get; set; }

        /// <summary>
        ///     Gets or sets the miter limit.
        /// </summary>
        /// <value>
        ///     The miter limit.
        /// </value>
        public double MiterLimit { get; set; }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            _mPolyNodes.Childs.Clear();
            _mLowest.X = -1;
        }

        //------------------------------------------------------------------------------

        internal static cInt Round(double value)
        {
            return value < 0 ? (cInt) (value - 0.5) : (cInt) (value + 0.5);
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Adds the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="endType">The end type.</param>
        public void AddPath(Path path, JoinType joinType, EndType endType)
        {
            var highI = path.Count - 1;
            if (highI < 0)
            {
                return;
            }
            var newNode = new PolyNode { MJointype = joinType, MEndtype = endType };

            //strip duplicate points from path and also get index to the lowest point ...
            if (endType == EndType.EtClosedLine || endType == EndType.EtClosedPolygon)
            {
                while (highI > 0 && path[0] == path[highI])
                {
                    highI--;
                }
            }
            newNode.MPolygon.Capacity = highI + 1;
            newNode.MPolygon.Add(path[0]);
            int j = 0, k = 0;
            for (var i = 1; i <= highI; i++)
            {
                if (newNode.MPolygon[j] != path[i])
                {
                    j++;
                    newNode.MPolygon.Add(path[i]);
                    if (path[i].Y > newNode.MPolygon[k].Y ||
                        (path[i].Y == newNode.MPolygon[k].Y && path[i].X < newNode.MPolygon[k].X))
                    {
                        k = j;
                    }
                }
            }
            if (endType == EndType.EtClosedPolygon && j < 2)
            {
                return;
            }

            _mPolyNodes.AddChild(newNode);

            //if this path's lowest pt is lower than all the others then update m_lowest
            if (endType != EndType.EtClosedPolygon)
            {
                return;
            }
            if (_mLowest.X < 0)
            {
                _mLowest = new IntPoint(_mPolyNodes.ChildCount - 1, k);
            }
            else
            {
#if use_int32
                var ip = _mPolyNodes.Childs[_mLowest.X].MPolygon[_mLowest.Y];
#else
                var ip = _mPolyNodes.Childs[(int) _mLowest.X].MPolygon[(int) _mLowest.Y];
#endif
                if (newNode.MPolygon[k].Y > ip.Y || (newNode.MPolygon[k].Y == ip.Y && newNode.MPolygon[k].X < ip.X))
                {
                    _mLowest = new IntPoint(_mPolyNodes.ChildCount - 1, k);
                }
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Adds the paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="endType">The end type.</param>
        public void AddPaths(Paths paths, JoinType joinType, EndType endType)
        {
            foreach (var p in paths)
            {
                AddPath(p, joinType, endType);
            }
        }

        //------------------------------------------------------------------------------

        private void FixOrientations()
        {
            //fixup orientations of all closed paths if the orientation of the
            //closed path with the lowermost vertex is wrong ...
#if use_int32
            if (_mLowest.X >= 0 && !Clipper.Orientation(_mPolyNodes.Childs[_mLowest.X].MPolygon))
#else
            if (_mLowest.X >= 0 && !Clipper.Orientation(_mPolyNodes.Childs[(int) _mLowest.X].MPolygon))
#endif
            {
                for (var i = 0; i < _mPolyNodes.ChildCount; i++)
                {
                    var node = _mPolyNodes.Childs[i];
                    if (node.MEndtype == EndType.EtClosedPolygon ||
                        (node.MEndtype == EndType.EtClosedLine && Clipper.Orientation(node.MPolygon)))
                    {
                        node.MPolygon.Reverse();
                    }
                }
            }
            else
            {
                for (var i = 0; i < _mPolyNodes.ChildCount; i++)
                {
                    var node = _mPolyNodes.Childs[i];
                    if (node.MEndtype == EndType.EtClosedLine && !Clipper.Orientation(node.MPolygon))
                    {
                        node.MPolygon.Reverse();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
        {
            double dx = (pt2.X - pt1.X);
            double dy = (pt2.Y - pt1.Y);
            if ((Math.Abs(dx) < float.Epsilon) && (Math.Abs(dy) < float.Epsilon))
            {
                return new DoublePoint();
            }

            var f = 1 * 1.0 / Math.Sqrt(dx * dx + dy * dy);
            dx *= f;
            dy *= f;

            return new DoublePoint(dy, -dx);
        }

        //------------------------------------------------------------------------------

        private void DoOffset(double delta)
        {
            _mDestPolys = new Paths();
            _mDelta = delta;

            //if Zero offset, just copy any CLOSED polygons to m_p and return ...
            if (ClipperBase.NearZero(delta))
            {
                _mDestPolys.Capacity = _mPolyNodes.ChildCount;
                for (var i = 0; i < _mPolyNodes.ChildCount; i++)
                {
                    var node = _mPolyNodes.Childs[i];
                    if (node.MEndtype == EndType.EtClosedPolygon)
                    {
                        _mDestPolys.Add(node.MPolygon);
                    }
                }
                return;
            }

            //see offset_triginometry3.svg in the documentation folder ...
            if (MiterLimit > 2)
            {
                _mMiterLim = 2 / (MiterLimit * MiterLimit);
            }
            else
            {
                _mMiterLim = 0.5;
            }

            double y;
            if (ArcTolerance <= 0.0)
            {
                y = DefArcTolerance;
            }
            else if (ArcTolerance > Math.Abs(delta) * DefArcTolerance)
            {
                y = Math.Abs(delta) * DefArcTolerance;
            }
            else
            {
                y = ArcTolerance;
            }
            //see offset_triginometry2.svg in the documentation folder ...
            var steps = Math.PI / Math.Acos(1 - y / Math.Abs(delta));
            _mSin = Math.Sin(TwoPi / steps);
            _mCos = Math.Cos(TwoPi / steps);
            _mStepsPerRad = steps / TwoPi;
            if (delta < 0.0)
            {
                _mSin = -_mSin;
            }

            _mDestPolys.Capacity = _mPolyNodes.ChildCount * 2;
            for (var i = 0; i < _mPolyNodes.ChildCount; i++)
            {
                var node = _mPolyNodes.Childs[i];
                _mSrcPoly = node.MPolygon;

                var len = _mSrcPoly.Count;

                if (len == 0 || (delta <= 0 && (len < 3 || node.MEndtype != EndType.EtClosedPolygon)))
                {
                    continue;
                }

                _mDestPoly = new Path();

                if (len == 1)
                {
                    if (node.MJointype == JoinType.JtRound)
                    {
                        var x = 1.0;
                        y = 0.0;
                        for (var j = 1; j <= steps; j++)
                        {
                            _mDestPoly.Add(
                                new IntPoint(Round(_mSrcPoly[0].X + x * delta), Round(_mSrcPoly[0].Y + y * delta)));
                            var x2 = x;
                            x = x * _mCos - _mSin * y;
                            y = x2 * _mSin + y * _mCos;
                        }
                    }
                    else
                    {
                        var x = -1.0;
                        y = -1.0;
                        for (var j = 0; j < 4; ++j)
                        {
                            _mDestPoly.Add(
                                new IntPoint(Round(_mSrcPoly[0].X + x * delta), Round(_mSrcPoly[0].Y + y * delta)));
                            if (x < 0)
                            {
                                x = 1;
                            }
                            else if (y < 0)
                            {
                                y = 1;
                            }
                            else
                            {
                                x = -1;
                            }
                        }
                    }
                    _mDestPolys.Add(_mDestPoly);
                    continue;
                }

                //build m_normals ...
                _mNormals.Clear();
                _mNormals.Capacity = len;
                for (var j = 0; j < len - 1; j++)
                {
                    _mNormals.Add(GetUnitNormal(_mSrcPoly[j], _mSrcPoly[j + 1]));
                }
                if (node.MEndtype == EndType.EtClosedLine || node.MEndtype == EndType.EtClosedPolygon)
                {
                    _mNormals.Add(GetUnitNormal(_mSrcPoly[len - 1], _mSrcPoly[0]));
                }
                else
                {
                    _mNormals.Add(new DoublePoint(_mNormals[len - 2]));
                }

                if (node.MEndtype == EndType.EtClosedPolygon)
                {
                    var k = len - 1;
                    for (var j = 0; j < len; j++)
                    {
                        OffsetPoint(j, ref k, node.MJointype);
                    }
                    _mDestPolys.Add(_mDestPoly);
                }
                else if (node.MEndtype == EndType.EtClosedLine)
                {
                    var k = len - 1;
                    for (var j = 0; j < len; j++)
                    {
                        OffsetPoint(j, ref k, node.MJointype);
                    }
                    _mDestPolys.Add(_mDestPoly);
                    _mDestPoly = new Path();
                    //re-build m_normals ...
                    var n = _mNormals[len - 1];
                    for (var j = len - 1; j > 0; j--)
                    {
                        _mNormals[j] = new DoublePoint(-_mNormals[j - 1].X, -_mNormals[j - 1].Y);
                    }
                    _mNormals[0] = new DoublePoint(-n.X, -n.Y);
                    k = 0;
                    for (var j = len - 1; j >= 0; j--)
                    {
                        OffsetPoint(j, ref k, node.MJointype);
                    }
                    _mDestPolys.Add(_mDestPoly);
                }
                else
                {
                    var k = 0;
                    for (var j = 1; j < len - 1; ++j)
                    {
                        OffsetPoint(j, ref k, node.MJointype);
                    }

                    IntPoint pt1;
                    if (node.MEndtype == EndType.EtOpenButt)
                    {
                        var j = len - 1;
                        pt1 = new IntPoint(
                            Round(_mSrcPoly[j].X + _mNormals[j].X * delta),
                            Round(_mSrcPoly[j].Y + _mNormals[j].Y * delta));
                        _mDestPoly.Add(pt1);
                        pt1 = new IntPoint(
                            Round(_mSrcPoly[j].X - _mNormals[j].X * delta),
                            Round(_mSrcPoly[j].Y - _mNormals[j].Y * delta));
                        _mDestPoly.Add(pt1);
                    }
                    else
                    {
                        var j = len - 1;
                        k = len - 2;
                        _mSinA = 0;
                        _mNormals[j] = new DoublePoint(-_mNormals[j].X, -_mNormals[j].Y);
                        if (node.MEndtype == EndType.EtOpenSquare)
                        {
                            DoSquare(j, k);
                        }
                        else
                        {
                            DoRound(j, k);
                        }
                    }

                    //re-build m_normals ...
                    for (var j = len - 1; j > 0; j--)
                    {
                        _mNormals[j] = new DoublePoint(-_mNormals[j - 1].X, -_mNormals[j - 1].Y);
                    }

                    _mNormals[0] = new DoublePoint(-_mNormals[1].X, -_mNormals[1].Y);

                    k = len - 1;
                    for (var j = k - 1; j > 0; --j)
                    {
                        OffsetPoint(j, ref k, node.MJointype);
                    }

                    if (node.MEndtype == EndType.EtOpenButt)
                    {
                        pt1 = new IntPoint(
                            Round(_mSrcPoly[0].X - _mNormals[0].X * delta),
                            Round(_mSrcPoly[0].Y - _mNormals[0].Y * delta));
                        _mDestPoly.Add(pt1);
                        pt1 = new IntPoint(
                            Round(_mSrcPoly[0].X + _mNormals[0].X * delta),
                            Round(_mSrcPoly[0].Y + _mNormals[0].Y * delta));
                        _mDestPoly.Add(pt1);
                    }
                    else
                    {
                        _mSinA = 0;
                        if (node.MEndtype == EndType.EtOpenSquare)
                        {
                            DoSquare(0, 1);
                        }
                        else
                        {
                            DoRound(0, 1);
                        }
                    }
                    _mDestPolys.Add(_mDestPoly);
                }
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The delta.</param>
        public void Execute(ref Paths solution, double delta)
        {
            solution.Clear();
            FixOrientations();
            DoOffset(delta);
            //now clean up 'corners' ...
            var clpr = new Clipper();
            clpr.AddPaths(_mDestPolys, PolyType.PtSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.CtUnion, solution, PolyFillType.PftPositive, PolyFillType.PftPositive);
            }
            else
            {
                var r = ClipperBase.GetBounds(_mDestPolys);
                var outer = new Path(4)
                {
                    new IntPoint(r.Left - 10, r.Bottom + 10),
                    new IntPoint(r.Right + 10, r.Bottom + 10),
                    new IntPoint(r.Right + 10, r.Top - 10),
                    new IntPoint(r.Left - 10, r.Top - 10)
                };


                clpr.AddPath(outer, PolyType.PtSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.CtUnion, solution, PolyFillType.PftNegative, PolyFillType.PftNegative);
                if (solution.Count > 0)
                {
                    solution.RemoveAt(0);
                }
            }
        }

        //------------------------------------------------------------------------------

        /// <summary>
        ///     Executes the specified solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The delta.</param>
        public void Execute(ref PolyTree solution, double delta)
        {
            solution.Clear();
            FixOrientations();
            DoOffset(delta);

            //now clean up 'corners' ...
            var clpr = new Clipper();
            clpr.AddPaths(_mDestPolys, PolyType.PtSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.CtUnion, solution, PolyFillType.PftPositive, PolyFillType.PftPositive);
            }
            else
            {
                var r = ClipperBase.GetBounds(_mDestPolys);
                var outer = new Path(4)
                {
                    new IntPoint(r.Left - 10, r.Bottom + 10),
                    new IntPoint(r.Right + 10, r.Bottom + 10),
                    new IntPoint(r.Right + 10, r.Top - 10),
                    new IntPoint(r.Left - 10, r.Top - 10)
                };


                clpr.AddPath(outer, PolyType.PtSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.CtUnion, solution, PolyFillType.PftNegative, PolyFillType.PftNegative);
                //remove the outer PolyNode rectangle ...
                if (solution.ChildCount == 1 && solution.Childs[0].ChildCount > 0)
                {
                    var outerNode = solution.Childs[0];
                    solution.Childs.Capacity = outerNode.ChildCount;
                    solution.Childs[0] = outerNode.Childs[0];
                    solution.Childs[0].MParent = solution;
                    for (var i = 1; i < outerNode.ChildCount; i++)
                    {
                        solution.AddChild(outerNode.Childs[i]);
                    }
                }
                else
                {
                    solution.Clear();
                }
            }
        }

        //------------------------------------------------------------------------------

        private void OffsetPoint(int j, ref int k, JoinType jointype)
        {
            //cross product ...
            _mSinA = (_mNormals[k].X * _mNormals[j].Y - _mNormals[j].X * _mNormals[k].Y);

            if (Math.Abs(_mSinA * _mDelta) < 1.0)
            {
                //dot product ...
                var cosA = (_mNormals[k].X * _mNormals[j].X + _mNormals[j].Y * _mNormals[k].Y);
                if (cosA > 0) // angle ==> 0 degrees
                {
                    _mDestPoly.Add(
                        new IntPoint(
                            Round(_mSrcPoly[j].X + _mNormals[k].X * _mDelta),
                            Round(_mSrcPoly[j].Y + _mNormals[k].Y * _mDelta)));
                    return;
                }
                //else angle ==> 180 degrees   
            }
            else if (_mSinA > 1.0)
            {
                _mSinA = 1.0;
            }
            else if (_mSinA < -1.0)
            {
                _mSinA = -1.0;
            }

            if (_mSinA * _mDelta < 0)
            {
                _mDestPoly.Add(
                    new IntPoint(
                        Round(_mSrcPoly[j].X + _mNormals[k].X * _mDelta),
                        Round(_mSrcPoly[j].Y + _mNormals[k].Y * _mDelta)));
                _mDestPoly.Add(_mSrcPoly[j]);
                _mDestPoly.Add(
                    new IntPoint(
                        Round(_mSrcPoly[j].X + _mNormals[j].X * _mDelta),
                        Round(_mSrcPoly[j].Y + _mNormals[j].Y * _mDelta)));
            }
            else
            {
                switch (jointype)
                {
                    case JoinType.JtMiter:
                    {
                        var r = 1 + (_mNormals[j].X * _mNormals[k].X + _mNormals[j].Y * _mNormals[k].Y);
                        if (r >= _mMiterLim)
                        {
                            DoMiter(j, k, r);
                        }
                        else
                        {
                            DoSquare(j, k);
                        }
                        break;
                    }
                    case JoinType.JtSquare:
                        DoSquare(j, k);
                        break;
                    case JoinType.JtRound:
                        DoRound(j, k);
                        break;
                }
            }
            k = j;
        }

        //------------------------------------------------------------------------------

        internal void DoSquare(int j, int k)
        {
            var dx = Math.Tan(Math.Atan2(_mSinA, _mNormals[k].X * _mNormals[j].X + _mNormals[k].Y * _mNormals[j].Y) / 4);
            _mDestPoly.Add(
                new IntPoint(
                    Round(_mSrcPoly[j].X + _mDelta * (_mNormals[k].X - _mNormals[k].Y * dx)),
                    Round(_mSrcPoly[j].Y + _mDelta * (_mNormals[k].Y + _mNormals[k].X * dx))));
            _mDestPoly.Add(
                new IntPoint(
                    Round(_mSrcPoly[j].X + _mDelta * (_mNormals[j].X + _mNormals[j].Y * dx)),
                    Round(_mSrcPoly[j].Y + _mDelta * (_mNormals[j].Y - _mNormals[j].X * dx))));
        }

        //------------------------------------------------------------------------------

        internal void DoMiter(int j, int k, double r)
        {
            var q = _mDelta / r;
            _mDestPoly.Add(
                new IntPoint(
                    Round(_mSrcPoly[j].X + (_mNormals[k].X + _mNormals[j].X) * q),
                    Round(_mSrcPoly[j].Y + (_mNormals[k].Y + _mNormals[j].Y) * q)));
        }

        //------------------------------------------------------------------------------

        internal void DoRound(int j, int k)
        {
            var a = Math.Atan2(_mSinA, _mNormals[k].X * _mNormals[j].X + _mNormals[k].Y * _mNormals[j].Y);
            var steps = Math.Max(Round(_mStepsPerRad * Math.Abs(a)), 1);

            double x = _mNormals[k].X, y = _mNormals[k].Y;
            for (var i = 0; i < steps; ++i)
            {
                _mDestPoly.Add(new IntPoint(Round(_mSrcPoly[j].X + x * _mDelta), Round(_mSrcPoly[j].Y + y * _mDelta)));
                var x2 = x;
                x = x * _mCos - _mSin * y;
                y = x2 * _mSin + y * _mCos;
            }
            _mDestPoly.Add(
                new IntPoint(
                    Round(_mSrcPoly[j].X + _mNormals[j].X * _mDelta), Round(_mSrcPoly[j].Y + _mNormals[j].Y * _mDelta)));
        }
    }

    /// <summary>
    ///     Clipper Exception.
    /// </summary>
    [Serializable]
    public class ClipperException : Exception
    {
        /// <summary>
        ///     Clipper Exception constructor.
        /// </summary>
        /// <param name="description">Exception description</param>
        public ClipperException(string description) : base(description) {}
    }

    //------------------------------------------------------------------------------
} //end ClipperLib namespace
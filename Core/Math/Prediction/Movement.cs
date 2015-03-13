#region

using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Events;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Math.Prediction
{
    /// <summary>
    ///     Calculates a prediction based off data values given by the source input and converts it into a output prediction
    ///     for movement, containing spell casting position and unit position in 3D-Space.
    /// </summary>
    public class Movement
    {
        #region GetPrediction

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <param name="ft">Add Delay</param>
        /// <param name="checkCollision">Check Collision</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            PredictionOutput result = null;

            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                //Increase the delay due to the latency and server tick:
                input.Delay += Game.Ping / 2000f + 0.05f;

                if (input.Aoe)
                {
                    return Cluster.GetAoEPrediction(input);
                }
            }

            //Target too far away.
            if (System.Math.Abs(input.Range - float.MaxValue) > float.Epsilon &&
                input.Unit.DistanceSquared(input.RangeCheckFrom) > System.Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            //Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                //Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);
                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
            }

            //Normal prediction
            if (result == null)
            {
                result = GetStandardPrediction(input);
            }

            //Check if the unit position is in range
            if (System.Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance == HitChance.High &&
                    input.RangeCheckFrom.DistanceSquared(input.Unit.Position) >
                    System.Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition) >
                    System.Math.Pow(
                        input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > System.Math.Pow(input.Range, 2))
                {
                    if (result.Hitchance != HitChance.OutOfRange)
                    {
                        result.CastPosition = input.RangeCheckFrom +
                                              input.Range *
                                              (result.UnitPosition - input.RangeCheckFrom).Normalized().SetZ();
                    }
                    else
                    {
                        result.Hitchance = HitChance.OutOfRange;
                    }
                }
            }

            //Check for collision
            if (checkCollision && input.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }

            return result;
        }

        #endregion

        #region GetDashingPrediction

        /// <summary>
        ///     Returns Dashing Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <returns>Returns Dashing Prediction</returns>
        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input };
            input.Delay += 0.1f;
            //Normal dashes.
            if (!dashData.IsBlink)
            {
                //Mid air:
                var dashPred = GetPositionOnPath(
                    input, new List<Vector2> { input.Unit.ServerPosition.ToVector2(), dashData.Path.Last() },
                    dashData.Speed);
                if (dashPred.Hitchance == HitChance.High)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                //At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var endP = dashData.Path.Last();
                    var timeToPoint = input.Delay + input.From.ToVector2().Distance(endP) / input.Speed;
                    if (timeToPoint <=
                        input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    {
                        return new PredictionOutput
                        {
                            CastPosition = endP.ToVector3(),
                            UnitPosition = endP.ToVector3(),
                            Hitchance = HitChance.Dashing
                        };
                    }
                }

                result.CastPosition = dashData.Path.Last().ToVector3();
                result.UnitPosition = result.CastPosition;

                //Figure out where the unit is going.
            }

            return result;
        }

        #endregion

        #region GetImmobilePrediction

        /// <summary>
        ///     Returns Immobile Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <param name="remainingImmobileT">Remaining Immobile Time</param>
        /// <returns>Returns Immobile Prediction</returns>
        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var timeToReachTargetPosition = input.Delay + input.Unit.Distance(input.From) / input.Speed;

            if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
            {
                return new PredictionOutput
                {
                    CastPosition = input.Unit.ServerPosition,
                    UnitPosition = input.Unit.Position,
                    Hitchance = HitChance.Immobile
                };
            }

            return new PredictionOutput
            {
                Input = input,
                CastPosition = input.Unit.ServerPosition,
                UnitPosition = input.Unit.ServerPosition,
                Hitchance = HitChance.High
            };
        }

        #endregion

        #region GetStandardPrediction

        /// <summary>
        ///     Returns Standard Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <returns>Returns Standard Prediction</returns>
        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                //input.Delay /= 2;
                speed /= 1.5f;
            }

            var result = GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);

            if (result.Hitchance >= HitChance.High && input.Unit is Obj_AI_Hero) {}

            return result;
        }

        #endregion

        /// <summary>
        ///     Returns if the unit is immobile and immobile time.
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <returns>Immobile Time left</returns>
        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => System.Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        #region GetPositionOnDash

        /// <summary>
        ///     Get Position on Unit's Path.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <param name="path">Path in Vector2 List</param>
        /// <param name="speed">Unit Speed</param>
        /// <returns></returns>
        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            speed = (System.Math.Abs(speed - (-1)) < float.Epsilon) ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1)
            {
                return new PredictionOutput
                {
                    Input = input,
                    UnitPosition = input.Unit.ServerPosition,
                    CastPosition = input.Unit.ServerPosition,
                    Hitchance = HitChance.VeryHigh
                };
            }

            var pLength = path.PathLength();

            //Skillshots with only a delay
            if (pLength >= input.Delay * speed - input.RealRadius &&
                System.Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                var tDistance = input.Delay * speed - input.RealRadius;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();

                        var cp = a + direction * tDistance;
                        var p = a +
                                direction *
                                ((i == path.Count - 2)
                                    ? System.Math.Min(tDistance + input.RealRadius, d)
                                    : (tDistance + input.RealRadius));

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = cp.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance =
                                Path.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                    ? HitChance.VeryHigh
                                    : HitChance.High
                        };
                    }

                    tDistance -= d;
                }
            }

            //Skillshot with a delay and speed.
            if (pLength >= input.Delay * speed - input.RealRadius &&
                System.Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                path = path.CutPath(System.Math.Max(0, input.Delay * speed - input.RealRadius));
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();
                    a = a - speed * tT * direction;
                    var sol = a.VectorMovementCollision(b, speed, input.From.ToVector2(), input.Speed, tT);
                    var t = (float) sol[0];
                    var pos = (Vector2) sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        var p = pos + input.RealRadius * direction;

                        if (input.Type == SkillshotType.SkillshotLine)
                        {
                            var alpha = (input.From.ToVector2() - p).AngleBetween(a - b);
                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float) System.Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(beta);
                                var cp2 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(-beta);

                                pos = cp1.DistanceSquared(pos) < cp2.DistanceSquared(pos) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = pos.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance =
                                Path.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                    ? HitChance.VeryHigh
                                    : HitChance.High
                        };
                    }
                    tT += tB;
                }
            }

            var position = path.Last();
            return new PredictionOutput
            {
                Input = input,
                CastPosition = position.ToVector3(),
                UnitPosition = position.ToVector3(),
                Hitchance = HitChance.Medium
            };
        }

        #endregion

        #region Movement Prediction

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <param name="radius">Prediction Radius</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <param name="radius">Prediction Radius</param>
        /// <param name="speed">Prediction Speed</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius, Speed = speed });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" />
        /// </param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        #endregion
    }

    #region Prediction Input/Output

    /// <summary>
    ///     Prediction Input, collect data values from the input source of the requested prediction to execute a movement
    ///     prediction for both a unit position and a skillshot casting area which is then returned as a
    ///     <see cref="PredictionOutput" />
    /// </summary>
    public class PredictionInput
    {
        /// <summary>
        ///     From source location on a 3D-Space.
        /// </summary>
        private Vector3 _from;

        /// <summary>
        ///     Range check from location on a 3D-Space.
        /// </summary>
        private Vector3 _rangeCheckFrom;

        /// <summary>
        ///     Marks if the prediction should include Area-of-Effect calculations to hit as many as targets possible.
        /// </summary>
        public bool Aoe = false;

        /// <summary>
        ///     Marks if the input source has a collision flag to collide with other objects before reaching the target.
        /// </summary>
        public bool Collision = false;

        /// <summary>
        ///     Flags that contains the unit types that the skillshot can collide with.
        /// </summary>
        public CollisionableObjects CollisionObjects = CollisionableObjects.Minions | CollisionableObjects.YasuoWall;

        /// <summary>
        ///     The skillshot delay in seconds.
        /// </summary>
        public float Delay;

        /// <summary>
        ///     The skillshot width's radius or the angle in case of the cone skillshots.
        /// </summary>
        public float Radius = 1f;

        /// <summary>
        ///     The skillshot range in units.
        /// </summary>
        public float Range = float.MaxValue;

        /// <summary>
        ///     The skillshot speed in units per second.
        /// </summary>
        public float Speed = float.MaxValue;

        /// <summary>
        ///     The skillshot type.
        /// </summary>
        public SkillshotType Type = SkillshotType.SkillshotLine;

        /// <summary>
        ///     The unit that the prediction will made for.
        /// </summary>
        public Obj_AI_Base Unit = ObjectManager.Player;

        /// <summary>
        ///     Set to true to increase the prediction radius by the unit bounding radius.
        /// </summary>
        public bool UseBoundingRadius = true;

        /// <summary>
        ///     The position from where the skillshot missile gets fired.
        /// </summary>
        public Vector3 From
        {
            get { return _from.IsValid() ? _from : ObjectManager.Player.ServerPosition; }
            set { _from = value; }
        }

        /// <summary>
        ///     The position from where the range is checked.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return _rangeCheckFrom.IsValid()
                    ? _rangeCheckFrom
                    : (From.IsValid() ? From : ObjectManager.Player.ServerPosition);
            }
            set { _rangeCheckFrom = value; }
        }

        /// <summary>
        ///     Calculates the real radius of the unit.
        /// </summary>
        internal float RealRadius
        {
            get { return UseBoundingRadius ? Radius + Unit.BoundingRadius : Radius; }
        }
    }

    /// <summary>
    ///     Prediction Output, contains the calculated data from the source prediction input.
    /// </summary>
    public class PredictionOutput
    {
        /// <summary>
        ///     Cast Predicted Position data in a 3D-Space given value.
        /// </summary>
        private Vector3 _castPosition;

        /// <summary>
        ///     Unit Predicted Position data ina a 3D-Space given value.
        /// </summary>
        private Vector3 _unitPosition;

        /// <summary>
        ///     External modifable data value which is declared for output data after calculation of how many Area-of-Effect
        ///     targets will get hit by the prediction input source.
        /// </summary>
        public int AoeHitCount;

        /// <summary>
        ///     The list of the targets that the spell will hit (only if aoe was enabled).
        /// </summary>
        public List<Obj_AI_Hero> AoeTargetsHit = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The list of the units that the skillshot will collide with.
        /// </summary>
        public List<Obj_AI_Base> CollisionObjects = new List<Obj_AI_Base>();

        /// <summary>
        ///     Returns the hitchance.
        /// </summary>
        public HitChance Hitchance = HitChance.Impossible;

        internal PredictionInput Input;

        /// <summary>
        ///     The position where the skillshot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get
            {
                return _castPosition.IsValid() && _castPosition.IsValid()
                    ? _castPosition.SetZ()
                    : Input.Unit.ServerPosition;
            }
            set { _castPosition = value; }
        }

        /// <summary>
        ///     The number of targets the skillshot will hit (only if aoe was enabled).
        /// </summary>
        public int AoeTargetsHitCount
        {
            get { return System.Math.Max(AoeHitCount, AoeTargetsHit.Count); }
        }

        /// <summary>
        ///     The position where the unit is going to be when the skillshot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get { return _unitPosition.IsValid() ? _unitPosition.SetZ() : Input.Unit.ServerPosition; }
            set { _unitPosition = value; }
        }
    }

    #endregion
}
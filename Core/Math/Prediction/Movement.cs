// <copyright file="Movement.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Calculates a prediction based off data values given by the source input and converts it into a output prediction
    ///     for movement, containing spell casting position and unit position in 3D-Space.
    /// </summary>
    public class Movement
    {
        #region Public Methods and Operators

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
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns Dashing Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input };

            // Normal dashes.
            if (!dashData.IsBlink)
            {
                var endP = dashData.EndPos;

                // Mid air:
                var dashPred = GetPositionOnPath(
                    input,
                    new List<Vector2> { input.Unit.ServerPosition.ToVector2(), endP },
                    dashData.Speed);
                if (dashPred.Hitchance >= HitChance.High
                    && dashPred.UnitPosition.ToVector2().Distance(input.Unit.Position.ToVector2(), endP, true) < 200)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                // At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var timeToPoint = input.Delay / 2f + input.From.Distance(endP) / input.Speed - 0.25f;
                    if (timeToPoint
                        <= input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    {
                        return new PredictionOutput
                                   {
                                       Input = input, CastPosition = endP.ToVector3(), UnitPosition = endP.ToVector3(),
                                       Hitchance = HitChance.Dashing
                                   };
                    }
                }

                result.CastPosition = endP.ToVector3();
                result.UnitPosition = result.CastPosition;

                // Figure out where the unit is going.
            }

            return result;
        }

        /// <summary>
        ///     Returns Immobile Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="remainingImmobileT">Remaining Immobile Time</param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var result = new PredictionOutput
                             {
                                 Input = input, CastPosition = input.Unit.ServerPosition,
                                 UnitPosition = input.Unit.ServerPosition, Hitchance = HitChance.High
                             };
            var timeToReachTargetPosition = input.Delay + input.Unit.Distance(input.From) / input.Speed;

            if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
            {
                result.UnitPosition = input.Unit.Position;
                result.Hitchance = HitChance.Immobile;
            }

            return result;
        }

        /// <summary>
        ///     Get Position on Unit's Path.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="path">Path in Vector2 List</param>
        /// <param name="speed">Unit Speed</param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            speed = Math.Abs(speed - -1) < float.Epsilon ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1)
            {
                return new PredictionOutput
                           {
                               Input = input, UnitPosition = input.Unit.ServerPosition,
                               CastPosition = input.Unit.ServerPosition, Hitchance = HitChance.VeryHigh
                           };
            }

            var pLength = path.PathLength();
            var dist = input.Delay * speed - input.RealRadius;

            // Skillshots with only a delay
            if (pLength >= dist && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                var tDistance = dist;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();

                        var cp = a + direction * tDistance;
                        var p = a
                                + direction
                                * (i == path.Count - 2
                                       ? Math.Min(tDistance + input.RealRadius, d)
                                       : tDistance + input.RealRadius);

                        return new PredictionOutput
                                   {
                                       Input = input, CastPosition = cp.ToVector3(), UnitPosition = p.ToVector3(),
                                       Hitchance =
                                           GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                               ? HitChance.VeryHigh
                                               : HitChance.High
                                   };
                    }

                    tDistance -= d;
                }
            }

            // Skillshot with a delay and speed.
            if (pLength >= dist && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var tDistance = dist;
                if ((input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone)
                    && input.Unit.DistanceSquared(input.From) < 200 * 200)
                {
                    tDistance = input.Delay * speed;
                }
                path = path.CutPath(Math.Max(0, tDistance));
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();
                    a = a - speed * tT * direction;
                    var sol = a.VectorMovementCollision(b, speed, input.From.ToVector2(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.DistanceSquared(b) < 20)
                        {
                            break;
                        }

                        var p = pos + input.RealRadius * direction;

                        if (input.Type == SkillshotType.SkillshotLine)
                        {
                            var alpha = (input.From.ToVector2() - p).AngleBetween(a - b);
                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float)Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(beta);
                                var cp2 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(-beta);

                                pos = cp1.DistanceSquared(pos) < cp2.DistanceSquared(pos) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                                   {
                                       Input = input, CastPosition = pos.ToVector3(), UnitPosition = p.ToVector3(),
                                       Hitchance =
                                           GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                               ? HitChance.VeryHigh
                                               : HitChance.High
                                   };
                    }

                    tT += tB;
                }
            }

            var position = path.Last().ToVector3();
            return new PredictionOutput
                       { Input = input, CastPosition = position, UnitPosition = position, Hitchance = HitChance.Medium };
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="ft">Add Delay</param>
        /// <param name="checkCollision">Check Collision</param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                // Increase the delay due to the latency and server tick:
                input.Delay += Game.Ping / 2000f + 0.06f;

                if (input.AoE)
                {
                    return Cluster.GetAoEPrediction(input);
                }
            }

            // Target too far away.
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon
                && input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            PredictionOutput result = null;

            // Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                // Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);
                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
            }

            // Normal prediction
            if (result == null)
            {
                result = GetStandardPrediction(input);
            }

            // Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High
                    && input.RangeCheckFrom.DistanceSquared(input.Unit.Position)
                    > Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition)
                    > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > Math.Pow(input.Range, 2)
                    && result.Hitchance != HitChance.OutOfRange)
                {
                    result.CastPosition = input.RangeCheckFrom
                                          + input.Range
                                          * (result.UnitPosition - input.RangeCheckFrom).ToVector2()
                                                .Normalized()
                                                .ToVector3();
                }
            }

            // Calc hitchance again
            if (result.Hitchance == HitChance.High)
            {
                result.Hitchance = GetHitChance(input);
            }

            // Check for collision
            if (checkCollision && input.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.Compare(originalUnit));
                if (result.CollisionObjects.Count > 0)
                {
                    result.Hitchance = HitChance.Collision;
                }
            }

            return result;
        }

        /// <summary>
        ///     Returns Standard Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                // input.Delay /= 2;
                speed /= 1.5f;
            }

            return GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);
        }

        /// <summary>
        ///     Returns if the unit is immobile and immobile time.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns>Immobile Time left</returns>
        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                    buff.IsValid
                    && (buff.Type == BuffType.Knockup || buff.Type == BuffType.Snare || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Suppression))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return result - Game.Time;
        }

        private static double GetAngle(Vector2 from, Vector2 to, Vector2 wayPoint)
        {
            if (to == wayPoint)
            {
                return 60;
            }
            var a = Math.Pow(wayPoint.X - from.X, 2) + Math.Pow(wayPoint.Y - from.Y, 2);
            var b = Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2);
            var c = Math.Pow(wayPoint.X - to.X, 2) + Math.Pow(wayPoint.Y - to.Y, 2);
            return Math.Cos((a + b - c) / (2 * Math.Sqrt(a) * Math.Sqrt(b))) * 180 / Math.PI;
        }

        private static HitChance GetHitChance(PredictionInput input)
        {
            var hero = input.Unit as Obj_AI_Hero;

            if (hero == null || input.Radius <= 1f)
            {
                return HitChance.VeryHigh;
            }

            if (hero.IsCastingInterruptableSpell(true) || hero.IsRecalling())
            {
                return HitChance.VeryHigh;
            }

            if (hero.Path.Length > 0 != hero.IsMoving)
            {
                return HitChance.Medium;
            }

            var wayPoint = input.Unit.GetWaypoints().Last();
            var distUnitToWay = hero.Distance(wayPoint);
            var distUnitToFrom = hero.Distance(input.From);
            var distFromToWay = input.From.Distance(wayPoint);
            var delay = input.Delay
                        + (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon ? 0 : distUnitToFrom / input.Speed);
            var moveArea = hero.MoveSpeed * delay;
            var fixRange = moveArea * 0.4f;
            var minPath = 900 + moveArea;
            var moveAngle = 31d;

            if (input.Radius > 70)
            {
                moveAngle++;
            }
            else if (input.Radius <= 60)
            {
                moveAngle--;
            }

            if (input.Delay < 0.3)
            {
                moveAngle++;
            }

            if (GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d)
            {
                fixRange = moveArea * 0.3f;
                minPath = 700 + moveArea;
                moveAngle += 1.5;
            }

            if (input.Type == SkillshotType.SkillshotCircle)
            {
                fixRange -= input.Radius / 2;
            }

            if (distFromToWay <= distUnitToFrom)
            {
                if (distUnitToFrom > input.Range - fixRange)
                {
                    return HitChance.Medium;
                }
            }
            else if (distUnitToWay > 350)
            {
                moveAngle += 1.5;
            }

            if (distUnitToFrom < 250 || hero.MoveSpeed < 250 || distFromToWay < 250)
            {
                return HitChance.VeryHigh;
            }

            if (distUnitToWay > minPath)
            {
                return HitChance.VeryHigh;
            }

            if (hero.HealthPercent < 20 || GameObjects.Player.HealthPercent < 20)
            {
                return HitChance.VeryHigh;
            }

            if (GetAngle(input.From.ToVector2(), hero.ServerPosition.ToVector2(), wayPoint) < moveAngle
                && distUnitToWay > 260)
            {
                return HitChance.VeryHigh;
            }

            if (input.Type == SkillshotType.SkillshotCircle
                && GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d && distUnitToWay > fixRange)
            {
                return HitChance.VeryHigh;
            }

            return HitChance.High;
        }

        #endregion
    }

    #region Prediction Input/Output

    /// <summary>
    ///     Prediction Input, collect data values from the input source of the requested prediction to execute a movement
    ///     prediction for both a unit position and a skill-shot casting area which is then returned as a
    ///     <see cref="PredictionOutput" />
    /// </summary>
    public class PredictionInput
    {
        #region Fields

        /// <summary>
        ///     From source location on a 3D-Space.
        /// </summary>
        private Vector3 @from;

        /// <summary>
        ///     Range check from location on a 3D-Space.
        /// </summary>
        private Vector3 rangeCheckFrom;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether prediction should include Area of Effect calculations to hit as many as
        ///     targets possible.
        /// </summary>
        public bool AoE { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether input source has a collision flag to collide with other objects before
        ///     reaching the target.
        /// </summary>
        public bool Collision { get; set; }

        /// <summary>
        ///     Gets or sets the flags that contains the unit types that the skill-shot can collide with.
        /// </summary>
        public CollisionableObjects CollisionObjects { get; set; } = CollisionableObjects.Minions
                                                                     | CollisionableObjects.YasuoWall;

        /// <summary>
        ///     Gets or sets the skill-shot delay in seconds.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the position from where the skill-shot missile gets fired.
        /// </summary>
        public Vector3 From
        {
            get
            {
                return this.@from.IsValid() ? this.@from : ObjectManager.Player.ServerPosition;
            }

            set
            {
                this.@from = value;
            }
        }

        /// <summary>
        ///     Gets or sets the skill-shot width's radius or the angle in case of the cone skill shots.
        /// </summary>
        public float Radius { get; set; } = 1f;

        /// <summary>
        ///     Gets or sets the skill-shot range in units.
        /// </summary>
        public float Range { get; set; } = float.MaxValue;

        /// <summary>
        ///     Gets or sets the position from where the range is checked.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return this.rangeCheckFrom.IsValid() ? this.rangeCheckFrom : this.From;
            }

            set
            {
                this.rangeCheckFrom = value;
            }
        }

        /// <summary>
        ///     Gets or sets the skill-shot speed in units per second.
        /// </summary>
        public float Speed { get; set; } = float.MaxValue;

        /// <summary>
        ///     Gets or sets the skill-shot type.
        /// </summary>
        public SkillshotType Type { get; set; } = SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets or sets the unit that the prediction will made for.
        /// </summary>
        public Obj_AI_Base Unit { get; set; } = ObjectManager.Player;

        /// <summary>
        ///     Gets or sets a value indicating whether use bounding radius.
        /// </summary>
        public bool UseBoundingRadius { get; set; } = true;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the the calculated real radius of the unit.
        /// </summary>
        internal float RealRadius => this.UseBoundingRadius ? this.Radius + this.Unit.BoundingRadius : this.Radius;

        #endregion
    }

    /// <summary>
    ///     Prediction Output, contains the calculated data from the source prediction input.
    /// </summary>
    public class PredictionOutput
    {
        #region Fields

        /// <summary>
        ///     Cast Predicted Position data in a 3D-Space given value.
        /// </summary>
        private Vector3 castPosition;

        /// <summary>
        ///     Unit Predicted Position data ina a 3D-Space given value.
        /// </summary>
        private Vector3 unitPosition;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the data value which is declared for output data after calculation of how many Area-of-Effect
        ///     targets will get hit by the prediction input source.
        /// </summary>
        public int AoeHitCount { get; set; }

        /// <summary>
        ///     Gets or sets the list of the targets that the spell will hit (only if Area of Effect was enabled).
        /// </summary>
        public List<Obj_AI_Hero> AoeTargetsHit { get; set; } = new List<Obj_AI_Hero>();

        /// <summary>
        ///     Gets the number of targets the skill-shot will hit (only if Area of Effect was enabled).
        /// </summary>
        public int AoeTargetsHitCount => Math.Max(this.AoeHitCount, this.AoeTargetsHit.Count);

        /// <summary>
        ///     Gets or sets the position where the skill-shot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get
            {
                return this.castPosition.IsValid() ? this.castPosition.SetZ() : this.Input.Unit.ServerPosition;
            }

            set
            {
                this.castPosition = value;
            }
        }

        /// <summary>
        ///     Gets or sets the collision objects list which the input source would collide with.
        /// </summary>
        public List<Obj_AI_Base> CollisionObjects { get; set; } = new List<Obj_AI_Base>();

        /// <summary>
        ///     Gets or sets the hit chance.
        /// </summary>
        public HitChance Hitchance { get; set; } = HitChance.Impossible;

        /// <summary>
        ///     Gets or sets the input.
        /// </summary>
        public PredictionInput Input { get; set; }

        /// <summary>
        ///     Gets or sets where the unit is going to be when the skill-shot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get
            {
                return this.unitPosition.IsValid() ? this.unitPosition.SetZ() : this.Input.Unit.ServerPosition;
            }

            set
            {
                this.unitPosition = value;
            }
        }

        #endregion
    }

    #endregion
}
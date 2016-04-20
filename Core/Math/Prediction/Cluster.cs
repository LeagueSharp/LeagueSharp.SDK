// <copyright file="Cluster.cs" company="LeagueSharp">
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
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Enumerations;

    using SharpDX;

    /// <summary>
    ///     Cluster (Area of Effect) Prediction class.
    /// </summary>
    public static class Cluster
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns an Area of Effect Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        public static PredictionOutput GetAoEPrediction(PredictionInput input)
        {
            switch (input.Type)
            {
                case SkillshotType.SkillshotCircle:
                    return Circle.GetCirclePrediction(input);
                case SkillshotType.SkillshotCone:
                    return Cone.GetConePrediction(input);
                case SkillshotType.SkillshotLine:
                    return Line.GetLinePrediction(input);
            }

            return new PredictionOutput();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the possible targets of a prediction input source.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns><see cref="PossibleTarget" /> list.</returns>
        internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
        {
            var result = new List<PossibleTarget>();

            foreach (var enemy in
                GameObjects.EnemyHeroes.Where(
                    h =>
                    !h.Compare(input.Unit)
                    && h.IsValidTarget(input.Range + 200 + input.RealRadius, true, input.RangeCheckFrom)))
            {
                var inputs = input.Clone() as PredictionInput;

                if (inputs == null)
                {
                    continue;
                }

                inputs.Unit = enemy;
                var prediction = Movement.GetPrediction(inputs, false, true);

                if (prediction.Hitchance >= HitChance.High)
                {
                    result.Add(new PossibleTarget { Position = prediction.UnitPosition.ToVector2(), Unit = enemy });
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        ///     Circle Cluster (Area of Effect) Prediction sub class.
        /// </summary>
        public static class Circle
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Returns an Area of Effect circler prediction from a prediction input source.
            /// </summary>
            /// <param name="input">
            ///     <see cref="PredictionInput" /> input
            /// </param>
            /// <returns>
            ///     <see cref="PredictionOutput" /> output
            /// </returns>
            public static PredictionOutput GetCirclePrediction(PredictionInput input)
            {
                var mainTargetPrediction = Movement.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    // Add the posible targets in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                while (posibleTargets.Count > 1)
                {
                    var mecCircle = ConvexHull.GetMec(posibleTargets.Select(h => h.Position).ToList());

                    if (mecCircle.Radius <= input.RealRadius - 10
                        && mecCircle.Center.DistanceSquared(input.RangeCheckFrom) < input.Range * input.Range)
                    {
                        return new PredictionOutput
                                   {
                                       AoeTargetsHit = posibleTargets.Select(h => (Obj_AI_Hero)h.Unit).ToList(),
                                       CastPosition = mecCircle.Center.ToVector3(),
                                       UnitPosition = mainTargetPrediction.UnitPosition,
                                       Hitchance = mainTargetPrediction.Hitchance, Input = input,
                                       AoeHitCount = posibleTargets.Count
                                   };
                    }

                    float maxdist = -1;
                    var maxdistindex = 1;
                    for (var i = 1; i < posibleTargets.Count; i++)
                    {
                        var distance = posibleTargets[i].Position.DistanceSquared(posibleTargets[0].Position);

                        if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                        {
                            maxdistindex = i;
                            maxdist = distance;
                        }
                    }

                    posibleTargets.RemoveAt(maxdistindex);
                }

                return mainTargetPrediction;
            }

            #endregion
        }

        /// <summary>
        ///     Cone Cluster (Area of Effect) Prediction sub class.
        /// </summary>
        public static class Cone
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Returns an Area-of-Effect cone prediction from a prediction input source.
            /// </summary>
            /// <param name="input">
            ///     <see cref="PredictionInput" /> input
            /// </param>
            /// <returns>
            ///     <see cref="PredictionOutput" /> output
            /// </returns>
            public static PredictionOutput GetConePrediction(PredictionInput input)
            {
                var mainTargetPrediction = Movement.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    // Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();

                    foreach (var target in posibleTargets)
                    {
                        target.Position = target.Position - input.From.ToVector2();
                    }

                    for (var i = 0; i < posibleTargets.Count; i++)
                    {
                        for (var j = 0; j < posibleTargets.Count; j++)
                        {
                            if (i != j)
                            {
                                var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;

                                if (!candidates.Contains(p))
                                {
                                    candidates.Add(p);
                                }
                            }
                        }
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = default(Vector2);
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        var hits = GetHits(candidate, input.Range, input.Radius, positionsList);

                        if (hits > bestCandidateHits)
                        {
                            bestCandidate = candidate;
                            bestCandidateHits = hits;
                        }
                    }

                    if (bestCandidateHits > 1 && input.From.DistanceSquared(bestCandidate) > 50 * 50)
                    {
                        return new PredictionOutput
                                   {
                                       Hitchance = mainTargetPrediction.Hitchance, AoeHitCount = bestCandidateHits,
                                       UnitPosition = mainTargetPrediction.UnitPosition,
                                       CastPosition = bestCandidate.ToVector3(), Input = input
                                   };
                    }
                }

                return mainTargetPrediction;
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Returns the number of hits within the cone.
            /// </summary>
            /// <param name="end">Vector2 end-point of the cone</param>
            /// <param name="range">Cone Range</param>
            /// <param name="angle">Cone Facing Angle</param>
            /// <param name="points">Vector2 points</param>
            /// <returns>Number of Hits</returns>
            internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
            {
                return (from point in points
                        let edge1 = end.Rotated(-angle / 2)
                        let edge2 = edge1.Rotated(angle)
                        where
                            point.DistanceSquared(default(Vector2)) < range * range && edge1.CrossProduct(point) > 0
                            && point.CrossProduct(edge2) > 0
                        select point).Count();
            }

            #endregion
        }

        /// <summary>
        ///     Line Cluster (Area of Effect) Prediction sub class.
        /// </summary>
        public static class Line
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Returns an Area-of-Effect line prediction from a prediction input source.
            /// </summary>
            /// <param name="input">
            ///     <see cref="PredictionInput" /> input
            /// </param>
            /// <returns>
            ///     <see cref="PredictionOutput" /> output
            /// </returns>
            public static PredictionOutput GetLinePrediction(PredictionInput input)
            {
                var mainTargetPrediction = Movement.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                                         {
                                             new PossibleTarget
                                                 {
                                                     Position = mainTargetPrediction.UnitPosition.ToVector2(),
                                                     Unit = input.Unit
                                                 }
                                         };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    // Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();
                    foreach (var targetCandidates in
                        posibleTargets.Select(
                            target => GetCandidates(input.From.ToVector2(), target.Position, input.Radius, input.Range))
                        )
                    {
                        candidates.AddRange(targetCandidates);
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = default(Vector2);
                    var bestCandidateHitPoints = new List<Vector2>();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        if (
                            GetHits(
                                input.From.ToVector2(),
                                candidate,
                                input.Radius + (input.Unit.BoundingRadius / 3) - 10,
                                new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
                        {
                            var hits = GetHits(input.From.ToVector2(), candidate, input.Radius, positionsList).ToList();
                            var hitsCount = hits.Count;

                            if (hitsCount >= bestCandidateHits)
                            {
                                bestCandidateHits = hitsCount;
                                bestCandidate = candidate;
                                bestCandidateHitPoints = hits.ToList();
                            }
                        }
                    }

                    if (bestCandidateHits > 1)
                    {
                        float maxDistance = -1;
                        Vector2 p1 = default(Vector2), p2 = default(Vector2);

                        // Center the position
                        for (var i = 0; i < bestCandidateHitPoints.Count; i++)
                        {
                            for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                            {
                                var startP = input.From.ToVector2();
                                var endP = bestCandidate;
                                var proj1 = positionsList[i].ProjectOn(startP, endP);
                                var proj2 = positionsList[j].ProjectOn(startP, endP);
                                var dist = bestCandidateHitPoints[i].DistanceSquared(proj1.LinePoint)
                                           + bestCandidateHitPoints[j].DistanceSquared(proj2.LinePoint);

                                if (dist >= maxDistance
                                    && (proj1.LinePoint - positionsList[i]).AngleBetween(
                                        proj2.LinePoint - positionsList[j]) > 90)
                                {
                                    maxDistance = dist;
                                    p1 = positionsList[i];
                                    p2 = positionsList[j];
                                }
                            }
                        }

                        return new PredictionOutput
                                   {
                                       Hitchance = mainTargetPrediction.Hitchance, AoeHitCount = bestCandidateHits,
                                       UnitPosition = mainTargetPrediction.UnitPosition,
                                       CastPosition = ((p1 + p2) * 0.5f).ToVector3(), Input = input
                                   };
                    }
                }

                return mainTargetPrediction;
            }

            #endregion

            #region Methods

            /// <summary>
            ///     Returns a list of Vector2 point candidates for the linear prediction.
            /// </summary>
            /// <param name="from">Vector2 from position</param>
            /// <param name="to">Vector2 to position</param>
            /// <param name="radius">The Radius</param>
            /// <param name="range">The Range</param>
            /// <returns>Vector2 list</returns>
            internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
            {
                var middlePoint = (from + to) / 2;
                var intersections = @from.CircleCircleIntersection(middlePoint, radius, from.Distance(middlePoint));

                if (intersections.Length > 1)
                {
                    var c1 = intersections[0];
                    var c2 = intersections[1];

                    c1 = from + (range * (to - c1).Normalized());
                    c2 = from + (range * (to - c2).Normalized());

                    return new[] { c1, c2 };
                }

                return new Vector2[] { };
            }

            /// <summary>
            ///     Returns the number of hits
            /// </summary>
            /// <param name="start">Vector2 starting point</param>
            /// <param name="end">Vector2 ending point</param>
            /// <param name="radius">Line radius</param>
            /// <param name="points">Vector2 points</param>
            /// <returns>Number of Hits</returns>
            internal static IEnumerable<Vector2> GetHits(
                Vector2 start,
                Vector2 end,
                double radius,
                List<Vector2> points)
            {
                return points.Where(p => p.DistanceSquared(start, end, true) <= radius * radius);
            }

            #endregion
        }

        /// <summary>
        ///     Container for a possible target output.
        /// </summary>
        internal class PossibleTarget
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the unit position.
            /// </summary>
            public Vector2 Position { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            public Obj_AI_Base Unit { get; set; }

            #endregion
        }
    }
}
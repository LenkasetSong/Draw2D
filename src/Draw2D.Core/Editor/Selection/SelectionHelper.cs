﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Selection
{
    public static class SelectionHelper
    {
        public static ShapeObject TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, target, radius) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, target, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, target, radius) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, target, radius) : null;

            if (shapePoint != null || shape != null || guide != null)
            {
                if (shapePoint != null)
                {
                    Debug.WriteLine(string.Format("Hover Shape Point: {0}", shapePoint.GetType()));
                    return shapePoint;
                }
                else if (shape != null)
                {
                    Debug.WriteLine(string.Format("Hover Shape: {0}", shape.GetType()));
                    return shape;
                }
                else if (guidePoint != null)
                {
                    Debug.WriteLine(string.Format("Hover Guide Point: {0}", guidePoint.GetType()));
                    return guidePoint;
                }
                else if (guide != null)
                {
                    Debug.WriteLine(string.Format("Hover Guide: {0}", guide.GetType()));
                    return guide;
                }
            }

            Debug.WriteLine(string.Format("No Hover"));
            return null;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 point, double radius, Modifier modifier)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, point, radius) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, point, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, point, radius) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, point, radius) : null;

            if (shapePoint != null || shape != null || guidePoint != null || guide != null)
            {
                bool haveNewSelection =
                    (shapePoint != null && !context.Selected.Contains(shapePoint))
                    || (shape != null && !context.Selected.Contains(shape))
                    || (guidePoint != null && !context.Selected.Contains(guidePoint))
                    || (guide != null && !context.Selected.Contains(guide));

                if (context.Selected.Count >= 1
                    && !haveNewSelection
                    && !modifier.HasFlag(Modifier.Control))
                {
                    return true;
                }
                else
                {
                    if (shapePoint != null)
                    {
                        if (modifier.HasFlag(Modifier.Control))
                        {
                            if (context.Selected.Contains(shapePoint))
                            {
                                Debug.WriteLine(string.Format("Deselected Shape Point: {0}", shapePoint.GetType()));
                                shapePoint.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Shape Point: {0}", shapePoint.GetType()));
                                shapePoint.Select(context.Selected);
                            }
                            return context.Selected.Count > 0;
                        }
                        else
                        {
                            context.Selected.Clear();
                            Debug.WriteLine(string.Format("Selected Shape Point: {0}", shapePoint.GetType()));
                            shapePoint.Select(context.Selected);
                            return true;
                        }
                    }
                    else if (shape != null)
                    {
                        if (modifier.HasFlag(Modifier.Control))
                        {
                            if (context.Selected.Contains(shape))
                            {
                                Debug.WriteLine(string.Format("Deselected Shape: {0}", shape.GetType()));
                                shape.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Shape: {0}", shape.GetType()));
                                shape.Select(context.Selected);
                            }
                            return context.Selected.Count > 0;
                        }
                        else
                        {
                            context.Selected.Clear();
                            Debug.WriteLine(string.Format("Selected Shape: {0}", shape.GetType()));
                            shape.Select(context.Selected);
                            return true;
                        }
                    }
                    else if (guidePoint != null)
                    {
                        if (modifier.HasFlag(Modifier.Control))
                        {
                            if (context.Selected.Contains(guidePoint))
                            {
                                Debug.WriteLine(string.Format("Deselected Guide Point: {0}", guidePoint.GetType()));
                                guidePoint.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Guide Point: {0}", guidePoint.GetType()));
                                guidePoint.Select(context.Selected);
                            }
                            return context.Selected.Count > 0;
                        }
                        else
                        {
                            context.Selected.Clear();
                            Debug.WriteLine(string.Format("Selected Guide Point: {0}", guidePoint.GetType()));
                            guidePoint.Select(context.Selected);
                            return true;
                        }
                    }
                    else if (guide != null)
                    {
                        if (modifier.HasFlag(Modifier.Control))
                        {
                            if (context.Selected.Contains(guide))
                            {
                                Debug.WriteLine(string.Format("Deselected Guide: {0}", guide.GetType()));
                                guide.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Guide: {0}", guide.GetType()));
                                guide.Select(context.Selected);
                            }
                            return context.Selected.Count > 0;
                        }
                        else
                        {
                            context.Selected.Clear();
                            Debug.WriteLine(string.Format("Selected Guide: {0}", guide.GetType()));
                            guide.Select(context.Selected);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Rect2 rect, double radius, Modifier modifier)
        {
            var shapes =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Shapes, rect, radius) : null;

            var guides =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Guides, rect, radius) : null;

            if (shapes != null || guides != null)
            {
                if (shapes != null)
                {
                    if (modifier.HasFlag(Modifier.Control))
                    {
                        foreach (var shape in shapes)
                        {
                            if (context.Selected.Contains(shape))
                            {
                                Debug.WriteLine(string.Format("Deselected Shape: {0}", shape.GetType()));
                                shape.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Shape: {0}", shape.GetType()));
                                shape.Select(context.Selected);
                            }
                        }
                        return context.Selected.Count > 0;
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Selected Shapes: {0}", shapes != null ? shapes.Count : 0));
                        context.Selected.Clear();
                        foreach (var shape in shapes)
                        {
                            shape.Select(context.Selected);
                        }
                        return true;
                    }
                }
                else if (guides != null)
                {
                    if (modifier.HasFlag(Modifier.Control))
                    {
                        foreach (var guide in guides)
                        {
                            if (context.Selected.Contains(guide))
                            {
                                Debug.WriteLine(string.Format("Deselected Guide: {0}", guide.GetType()));
                                guide.Deselect(context.Selected);
                            }
                            else
                            {
                                Debug.WriteLine(string.Format("Selected Guide: {0}", guide.GetType()));
                                guide.Select(context.Selected);
                            }
                        }
                        return context.Selected.Count > 0;
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Selected Guides: {0}", guides != null ? guides.Count : 0));
                        context.Selected.Clear();
                        foreach (var guide in guides)
                        {
                            guide.Select(context.Selected);
                        }
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

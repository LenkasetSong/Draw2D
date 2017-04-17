﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Editor.Selection.Helpers;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public partial class SelectionTool : IEdit
    {
        private IList<ShapeObject> _shapesToCopy = null;
        private ShapeObject _hover = null;
        private bool _disconnected = false;

        public void Cut(IToolContext context)
        {
            Copy(context);
            Delete(context);
        }

        public void Copy(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                _shapesToCopy = context.Renderer.Selected.ToList();
            }
        }

        public void Paste(IToolContext context)
        {
            if (_shapesToCopy != null)
            {
                lock (context.Renderer.Selected)
                {
                    this.DeHover(context);
                    context.Renderer.Selected.Clear();

                    CopyHelper.Copy(_shapesToCopy, context.CurrentContainer, context.Renderer.Selected);

                    context.Invalidate();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Delete(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                DeleteHelper.Delete(context.CurrentContainer, context.Renderer.Selected);

                this.DeHover(context);
                context.Renderer.Selected.Clear();

                context.Invalidate();

                this.CurrentState = State.None;
            }
        }

        public void Group(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                this.DeHover(context);

                var shapes = context.Renderer.Selected.ToList();

                Delete(context);

                var group = new GroupShape();

                foreach (var shape in shapes)
                {
                    if (!(shape is PointShape))
                    {
                        group.Shapes.Add(shape);
                    }
                    //if (shape is PointShape point)
                    //{
                    //    group.Points.Add(point);
                    //}
                    //else
                    //{
                    //    group.Shapes.Add(shape);
                    //}
                }

                group.Select(context.Renderer.Selected);
                context.CurrentContainer.Shapes.Add(group);

                context.Invalidate();

                this.CurrentState = State.None;
            }
        }

        public void SelectAll(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                this.DeHover(context);
                context.Renderer.Selected.Clear();

                foreach (var shape in context.CurrentContainer.Shapes)
                {
                    shape.Select(context.Renderer.Selected);
                }

                context.Invalidate();

                this.CurrentState = State.None;
            }
        }

        public void Hover(IToolContext context, ShapeObject shape)
        {
            if (shape != null)
            {
                _hover = shape;
                _hover.Select(context.Selected);
            }
        }

        public void DeHover(IToolContext context)
        {
            if (_hover != null)
            {
                _hover.Deselect(context.Selected);
                _hover = null;
            }
        }

        public void Connect(IToolContext context, PointShape point)
        {
            var target = context.HitTest.TryToGetPoint(
                context.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                point);
            if (target != point)
            {
                foreach (var item in context.CurrentContainer.Shapes)
                {
                    if (item is ConnectableShape connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, PointShape point)
        {
            foreach (var shape in context.CurrentContainer.Shapes)
            {
                if (shape is ConnectableShape connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.Selected.Remove(point);
                            context.Selected.Add(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, ShapeObject shape)
        {
            if (shape is ConnectableShape connectable)
            {
                connectable.Deselect(context.Selected);
                _disconnected = connectable.Disconnect();
                connectable.Select(context.Selected);
            }
        }
    }
}
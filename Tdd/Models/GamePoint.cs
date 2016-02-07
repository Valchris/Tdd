﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tdd.Models.Pathing;

namespace Tdd.Models
{
    public class GamePoint : Point, IHasNeighbours<GamePoint>
    {
        private const double MaxDistance = 1;
        private readonly GameRoom GameRoom;

        public GamePoint(GameRoom gameRoom, double x, double y) : base(x, y)
        {
            this.GameRoom = gameRoom;
        }

        public GamePoint(GameRoom gameRoom, Point p) : base(p.X, p.Y)
        {
            this.GameRoom = gameRoom;
        }

        [JsonIgnore]
        public IEnumerable<GamePoint> Neighbours
        {

            get
            {
                var list = new List<GamePoint>();

                var upPoint = new GamePoint(this.GameRoom, X, Y - 1);
                var up = this.GameRoom.Towers.Where(t => Point.isNear(upPoint, t.Location, MaxDistance)).Any();
                if(!up)
                {
                    list.Add(new GamePoint(this.GameRoom, upPoint));
                }

                var leftPoint = new GamePoint(this.GameRoom, X - 1, Y);
                var left = this.GameRoom.Towers.Where(t => Point.isNear(leftPoint, t.Location, MaxDistance)).Any();
                if(!left)
                {
                    list.Add(new GamePoint(this.GameRoom, leftPoint));
                }

                var rightPoint = new GamePoint(this.GameRoom, X + 1, Y);
                var right = this.GameRoom.Towers.Where(t => Point.isNear(rightPoint, t.Location, MaxDistance)).Any();
                if(!right)
                {
                    list.Add(new GamePoint(this.GameRoom, rightPoint));
                }

                var downPoint = new GamePoint(this.GameRoom, X, Y+1);
                var down = this.GameRoom.Towers.Where(t => Point.isNear(downPoint, t.Location, MaxDistance)).Any();
                if(!down)
                {
                    list.Add(new GamePoint(this.GameRoom, downPoint));
                }

                return list;
            }
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (int)(Math.Pow(this.X, 2) + this.Y);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Point p = obj as Point;
            return (int)p.X == (int)this.X && (int)p.Y == (int)this.Y;
        }
    }
}
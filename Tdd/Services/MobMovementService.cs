﻿using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tdd.Models;

namespace Tdd.Services
{
    public class MobMovementService : IMobMovementService
    {
        private readonly IScaleoutService scaleoutService;
        private readonly IPathingService pathingService;

        public MobMovementService(IScaleoutService scaleoutService, IPathingService pathingService)
        {
            this.scaleoutService = scaleoutService;
            this.pathingService = pathingService;
        }

        public void RemoveMobsAtEnding(Mob mob, GameRoom room, GameRound round)
        {
            if (Point.IsNear(mob.CurrentLocation, mob.EndingLocation, 0.5))
            {
                foreach (var player in room.Players)
                {
                    if (mob.EndingLocation.X == player.EndingLocation.X && mob.EndingLocation.Y == player.EndingLocation.Y)
                    {
                        player.CurrentLife -= 1;
                        round.Mobs.Remove(mob);
                        this.scaleoutService.Store(Persist.GameRoom, room.Id, room);
                        break;
                    }
                }
            }
        }

        private GamePoint CalculateMobPath(Mob mob, GameRoom room)
        {
            var path = this.pathingService.FindPath<GamePoint>(new GamePoint(room, mob.CurrentLocation), new GamePoint(room, mob.EndingLocation), (p1, p2) =>
            {
                // Euclidian Squared heuristic
                var dx = p1.X - p2.X;
                var dy = p1.Y - p2.Y;
                if (dx > 0)
                {
                    dx += 1;
                }

                if (dy > 0)
                {
                    dy += 1;
                }
                return Math.Sqrt(dx * dx + dy * dy);
            }, (p) =>
            {
                // Euclidian squared heuristic estimate
                var dx = p.X - mob.EndingLocation.X;
                var dy = p.Y - mob.EndingLocation.Y;
                return dx * dx + dy * dy;
            });
            mob.Path = path?.Reverse().Skip(1);
            return mob.Path?.FirstOrDefault();
        }


        public void UpdateMobLocation(Mob mob, GameRoom room, GameRound round)
        {
            var span = DateTime.UtcNow.Subtract(mob.LastUpdated);

            if (span.Milliseconds > 0)
            {
                var next = mob.Path?.FirstOrDefault();
                if(next != null && Point.IsNear(mob.CurrentLocation, next, 0.1))
                {
                    mob.Path = mob.Path.Skip(1);
                    next = mob.Path?.FirstOrDefault();
                }
                
                if(mob.Path == null || next == null) 
                {
                    next = this.CalculateMobPath(mob, room);
                }


                if (next != null)
                {
                    var moveSpeed = mob.CurrentSpeed;

                    // Adjust mob speed for slow effects
                    if ((mob.Status as IDictionary<string, object>)?.ContainsKey("slow") ?? false)
                    {
                        moveSpeed = (int)(moveSpeed * (double)mob.Status.slow.speed);

                        if (DateTime.UtcNow > (DateTime)mob.Status.slow.remaining)
                        {
                            var status = (IDictionary<string, object>)mob.Status;
                            status.Remove("slow");
                        }

                    }

                    mob.CurrentLocation = Point.TrackTo(mob.CurrentLocation, next, (span.TotalMilliseconds * moveSpeed / Constants.GameSpeed));
                    // mob.Path = path?.ToList(); // Debugging
                }
                else
                {
                    Console.WriteLine("No valid path found for mob");
                    mob.CurrentLocation = new Point(mob.EndingLocation.X, mob.EndingLocation.Y);
                }

                mob.LastUpdated = DateTime.UtcNow;
            }
        
        }
    }
}
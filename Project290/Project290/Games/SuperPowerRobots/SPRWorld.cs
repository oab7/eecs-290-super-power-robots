﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project290.Screens;
using Microsoft.Xna.Framework;
using Project290.Rendering;
using Project290.GameElements;
using Project290.Inputs;
using Microsoft.Xna.Framework.Graphics;
using Project290.Clock;
using Project290.Physics.Dynamics;
using Project290.Physics.Collision.Shapes;
using Project290.Physics.Factories;
using Project290.Games.SuperPowerRobots.Entities;
using Project290.Physics.Common;
using Project290.Physics.Common.ConvexHull;
using Project290.Games.SuperPowerRobots.Controls;
using Project290.Physics.Dynamics.Contacts;
using Project290.Physics.Common.PolygonManipulation;

namespace Project290.Games.SuperPowerRobots
{
    public class SPRWorld
    {
        private World m_World;
        private SortedDictionary<ulong, Entity> m_Entities;
        public static Dictionary<String, Vertices> computedSpritePolygons = new Dictionary<string,Vertices>();
        private Battle battle;
        public bool m_isGameOver { get; private set; }
        public bool m_hasLost { get; private set; }

        // Anything that is a fixture must have an object type for collision logic purposes
        /*public enum ObjectTypes
        {
            Bot = 1,
            Weapon = 2,  // Since weapons will be fixtures, they too need an object type.
            Bullet = 3,
            Wall = 4
        }*/

        public SPRWorld(World world, int currentLevel)
        {
            int botHalfWidth = 31; // Half the bot's width (e.g. the distance from the centroid to the edge)
            m_World = world;
            this.m_Entities = new SortedDictionary<ulong,Entity>();

            // Make polygons out of weapons and anything that needs collision.
            // NOTE: Stores the convex hull for each item, since collision detection
            //          relies upon these verticies being convex polygons.
            String[] toPreload = new String[] { "Gun", "Axe", "Shield" };

            foreach (String texture in toPreload) {
                Texture2D a = TextureStatic.Get(texture);
                uint[] data = new uint[a.Width * a.Height];
                a.GetData<uint>(data);
                Vertices v = Melkman.GetConvexHull(PolygonTools.CreatePolygon(data, a.Width, a.Height));
                Vector2 scale = new Vector2(Settings.MetersPerPixel, Settings.MetersPerPixel);
                v.Scale(ref scale);
                if (!computedSpritePolygons.ContainsKey(texture))
                {
                    computedSpritePolygons.Add(texture, v);
                }
            }

            //walls
            Vector2[] outer = { new Vector2(0, 0) * Settings.MetersPerPixel, new Vector2(0, 1920) * Settings.MetersPerPixel, new Vector2(1080, 1920) * Settings.MetersPerPixel, new Vector2(1080, 0) * Settings.MetersPerPixel };
            Vector2[] inner = { new Vector2(200, 200) * Settings.MetersPerPixel, new Vector2(200, 1720) * Settings.MetersPerPixel, new Vector2(880, 1720) * Settings.MetersPerPixel, new Vector2(880, 200) * Settings.MetersPerPixel };

            FixtureFactory.CreateRectangle(world, 1920 * Settings.MetersPerPixel, 200 * Settings.MetersPerPixel, 1f, new Vector2(960, 100) * Settings.MetersPerPixel, "Wall").Body.BodyType = BodyType.Static;
            FixtureFactory.CreateRectangle(world, 200 * Settings.MetersPerPixel, 1080 * Settings.MetersPerPixel, 1f, new Vector2(100, 540) * Settings.MetersPerPixel, "Wall").Body.BodyType = BodyType.Static;
            FixtureFactory.CreateRectangle(world, 1920 * Settings.MetersPerPixel, 200 * Settings.MetersPerPixel, 1f, new Vector2(960, 980) * Settings.MetersPerPixel, "Wall").Body.BodyType = BodyType.Static;
            FixtureFactory.CreateRectangle(world, 200 * Settings.MetersPerPixel, 1080 * Settings.MetersPerPixel, 1f, new Vector2(1820, 540) * Settings.MetersPerPixel, "Wall").Body.BodyType = BodyType.Static;

            Vector2[] edges = { new Vector2(-botHalfWidth, -botHalfWidth) * Settings.MetersPerPixel, new Vector2(botHalfWidth, -botHalfWidth) * Settings.MetersPerPixel, new Vector2(botHalfWidth, botHalfWidth) * Settings.MetersPerPixel, new Vector2(-botHalfWidth, botHalfWidth) * Settings.MetersPerPixel };
            
			this.battle = new Battle(this, botHalfWidth, world, currentLevel);
            GameWorld.audio.SongPlay("sprbattle");
        }

        public World World { get { return m_World; } }

        public void AddEntity(Entity e)
        {
            if (!this.m_Entities.ContainsKey(e.GetID()))
                this.m_Entities.Add(e.GetID(), e);
        }

        public void Update(float dTime)
        {
            //then call the entity updates, take damage, listen to controls, spawn any projectiles, etc.

            //update all entities
            for (int i = 0; i < m_Entities.Values.Count; i++)
            {
                m_Entities.Values.ElementAt(i).Update(dTime);
            }

            // Because weapons will spawn projectiles in their update cycles, we must process changes to the physics
            // before we remove stuff.
            this.m_World.ProcessChanges();
            
            //check for dead bots
            List<ulong> toRemove = new List<ulong>();

            foreach (KeyValuePair<ulong, Entity> a in m_Entities)
            {
                Entity e = a.Value;
                // If the physics library has gotten rid of that entity's body, we should get rid of it as an entity.
                if (!m_World.BodyList.Contains(e.Body)) 
                {
                    toRemove.Add(a.Key);
                }
            }
            foreach (ulong key in toRemove)
            {
                m_Entities.Remove(key);
            }

            // Detect if the game is over (i.e. all Computer bots are destroyed)
            this.m_isGameOver = true;
            this.m_hasLost = true;
            foreach (Entity e in m_Entities.Values)
            {
                if (e is Bot)
                {
                    if (((Bot)e).m_player == Bot.Player.Computer)
                        this.m_isGameOver = false;
                    if (((Bot)e).m_player == Bot.Player.Human)
                        this.m_hasLost = false;
                }
            }
        }

        public IEnumerable<Entity> GetEntities()
        {
            return m_Entities.Values.AsEnumerable();
        }

        public int WinReward()
        {
            return this.battle.winReward;
        }

        public void Draw()
        {
            Drawer.Draw(
                TextureStatic.Get("background"),
                Drawer.FullScreenRectangle,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f);
            Drawer.DrawString(
                FontStatic.Get("defaultFont"),
                "Score: " + ScoreKeeper.score.ToString(),
                new Vector2(205,205),
                Color.White,
                0f,
                Vector2.Zero,
                0.4f,
                SpriteEffects.None,
                1f
            );
            foreach (Entity e in m_Entities.Values)
            {
                e.Draw();
            }
        }

        public static float SignedAngle(Vector2 v1, Vector2 v2)
        {
            if (v1.LengthSquared() == 0 || v2.LengthSquared() == 0) return 0;

            float angle = (float)Math.Acos(Vector2.Dot(v1, v2) / (v1.Length() * v2.Length()));
            if (float.IsNaN(angle)) angle = 0;

            float cross = (v1.X * v2.Y) - (v1.Y * v2.X);

            return angle * cross / Math.Abs(cross);
        }
    }
}

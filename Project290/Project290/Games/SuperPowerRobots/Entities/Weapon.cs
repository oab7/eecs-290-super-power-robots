﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project290.Physics.Dynamics;
using Project290.Physics.Factories;
using Project290.Rendering;
using Project290.Physics.Collision.Shapes;
using Project290.Physics.Dynamics.Joints;
using Project290.Physics.Dynamics.Contacts;

namespace Project290.Games.SuperPowerRobots.Entities
{
    public enum WeaponType
    {
        gun,
        melee,
        shield
    }
    public class Weapon : Entity
    {
        private Bot m_owner;
        private bool m_firing;
        private float m_reloadTime;
        private float m_reloading;
        private float m_power;
        private WeaponType weaponType;

        public Weapon(SPRWorld sprWorld, Body body, Bot bot, float rotation, Texture2D texture, float width, float height, WeaponType weaponType, float health, float power)
            : base(sprWorld, body, texture, width, height, health)
        {
            this.SetRotation(rotation);
            this.m_owner = bot;
            this.m_firing = false;
            this.m_reloadTime = .2f;
            this.m_reloading = 0;
            this.weaponType = weaponType;
            this.m_power = power;
        }
        //Weapons can spawn Projectiles, attached or unattached.

        public void Fire()
        {
            if (m_reloading <= 0)
            {
                this.m_firing = true;
                m_reloading = m_reloadTime;
            }
        }

        public override void Update(float dTime)
        {
            if (m_reloading > 0)
            {
                m_reloading -= dTime;
            }

            if (this.m_firing && weaponType == WeaponType.gun)
            {
                Body tempBody = BodyFactory.CreateBody(this.SPRWorld.World);
                tempBody.BodyType = BodyType.Dynamic;
                tempBody.IsBullet = true;
                float rotation = this.GetRotation();
                tempBody.Position = this.GetPosition() + 40 * Settings.MetersPerPixel * (new Vector2((float) Math.Cos(rotation), (float)Math.Sin(rotation)));
                tempBody.SetTransform(tempBody.Position, 0);
                Fixture f = FixtureFactory.CreateCircle(4 * Settings.MetersPerPixel, 0.000001f, tempBody);
                f.UserData = SPRWorld.ObjectTypes.Bullet;
                f.OnCollision += OnBulletHit;
                Vector2 initialVelocity = new Vector2((float) Math.Cos(rotation), (float) Math.Sin(rotation)) / 10000000;
                Projectile justFired = new Projectile(this.SPRWorld, tempBody, TextureStatic.Get("Projectile"), initialVelocity, this.GetRotation(), 5, 5 * Settings.MetersPerPixel, 5 * Settings.MetersPerPixel);
                this.SPRWorld.AddEntity(justFired);
                this.m_firing = false;
            }
            
			if (this.m_firing && weaponType == WeaponType.melee)
            {
                Body tempBody = BodyFactory.CreateBody(this.SPRWorld.World);
                tempBody.BodyType = BodyType.Dynamic;
                float rotation = this.GetRotation();
                tempBody.Position = this.GetPosition();
                tempBody.SetTransform(tempBody.Position, 0);
                Projectile justFired = new Projectile(this.SPRWorld, tempBody, TextureStatic.Get("Axe"), new Vector2(0, 0), this.GetRotation(), 5, Settings.MetersPerPixel * 5, 5 * Settings.MetersPerPixel);
                AngleJoint joint = JointFactory.CreateAngleJoint(this.SPRWorld.World, this.m_owner.Body, this.Body);
            }
        }

        public bool OnBulletHit(Fixture a, Fixture b, Contact c)
        {
            // Fixture a is always the bullet, and Fixture b is what it hit.
            if ((SPRWorld.ObjectTypes) b.UserData == SPRWorld.ObjectTypes.Wall)
            {
                a.Body.Dispose(); // Simply delete the bullet.
            }
            if ((SPRWorld.ObjectTypes)b.UserData == SPRWorld.ObjectTypes.Weapon)
            {
                // Sean's TODO: Add damage calculations....
                a.Body.Dispose(); // Simply delete the bullet.
            }
            return true;
        }
    }
}

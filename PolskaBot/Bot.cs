﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using PolskaBot.Core;

namespace PolskaBot
{
    public partial class Bot : Form
    {
        API api;

        Thread renderer;

        int FPS = 60;

        const float k = 0.015f;

        const byte alpha = 216;
        static Color mapBG = Color.FromArgb(20, 102, 102, 102);
        static Color hero = Color.FromArgb(alpha, 102, 102, 102);
        static Color box = Color.FromArgb(alpha, 255, 255, 0);
        static Color boxPirate = Color.Green;
        static Color boxMemorised = Color.FromArgb(150, 255, 255, 0);

        static Color hitpoints = Color.FromArgb(0, 204, 51);
        static Color shield = Color.FromArgb(51, 143, 204);

        static Font font = new Font("Arial", 8, FontStyle.Regular);

        public Bot()
        {
            InitializeComponent();
            Load += (s, e) => Init();
            FormClosed += (s, e) => renderer.Abort();
        }

        private void Init()
        {
            api = new API(API.Mode.BOT);
            api.Login();
            AddContextMenu();
            renderer = new Thread(new ThreadStart(Render));
            renderer.Start();
        }

        private void AddContextMenu()
        {
            var contextMenu = new ContextMenu();

            var drawBackground = new MenuItem();
            drawBackground.Text = "Draw background";
            drawBackground.Checked = true;
            drawBackground.Click += (s, e) =>
            {
                Properties.Settings.Default.DrawMap = !Properties.Settings.Default.DrawMap;
                ((MenuItem)s).Checked = Properties.Settings.Default.DrawMap;
            };

            contextMenu.MenuItems.Add(drawBackground);
            minimap.ContextMenu = contextMenu;
        }

        private void Render()
        {
            while(true)
            {
                var bitmap = new Bitmap(minimap.Width, minimap.Height);
                using (var g = Graphics.FromImage(bitmap))
                {
                    DrawBackground(g);
                    DrawPlayer(g);
                    DrawDetails(g);
                }

                Invoke((MethodInvoker)delegate
                {
                    minimap.Image = bitmap;
                });
                Task.Delay(1000 / FPS);
            }
        }

        private void DrawPlayer(Graphics g)
        {
            if(api.account.ready)
            {
                g.DrawLine(new Pen(hero), new Point(Scale(api.account.X), 0), new Point(Scale(api.account.X), minimap.Height));
                g.DrawLine(new Pen(hero), new Point(0, Scale(api.account.Y)), new Point(minimap.Width, Scale(api.account.Y)));
            }
        }

        private void DrawDetails(Graphics g)
        {
            if (api.account.ready)
            {
                string hpDetails = $"{api.account.hitpoints}/{api.account.maxHitpoints}";
                string shieldDetails = $"{api.account.shield}/{api.account.maxShield}";
                SizeF sizeHP = g.MeasureString(hpDetails, font);
                SizeF sizeShield = g.MeasureString(shieldDetails, font);
                g.DrawString(hpDetails, font, new SolidBrush(hitpoints), minimap.Width - sizeHP.Width - 10, minimap.Height - 20);
                g.DrawString(shieldDetails, font, new SolidBrush(shield), minimap.Width - sizeHP.Width - sizeShield.Width - 10, minimap.Height - 20);
            }
        }

        private void DrawBackground(Graphics g)
        {
            if (!Properties.Settings.Default.DrawMap)
                return;

            g.DrawImage(Properties.Resources._1, 0, 0, 315, 202);
        }

        private int Scale(int value)
        {
            return (int)(value * k);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project290.Rendering;
using Project290.Screens.Title;
using Project290.Menus.MenuDelegates;
using Project290.Games.SuperPowerRobots;

namespace Project290.GameElements
{
    /// <summary>
    /// Use this for loading anything and everything. This is its own class for organization and nothing more.
    /// </summary>
    public static class Loader
    {
        /// <summary>
        /// Loads all the shared and start/title/pause screen stuff.
        /// </summary>
        public static void LoadShared()
        {
            // Box art Textures
            TextureStatic.Load("BoxArtGame1", @"Shared\BoxArt\BoxArtGame1");
            TextureStatic.Load("BoxArtGame2", @"Shared\BoxArt\BoxArtGame2");
            TextureStatic.Load("BoxArtGame3", @"Shared\BoxArt\BoxArtGame3");
            TextureStatic.Load("BoxArtGame4", @"Shared\BoxArt\BoxArtGame4");
            TextureStatic.Load("BoxArtGame5", @"Shared\BoxArt\BoxArtGame5");
            TextureStatic.Load("BoxArtHolder", @"Shared\BoxArt\BoxArtHolder");
            
            // Other Shared Textures
            TextureStatic.Load("TitleNameBorder", @"Shared\Art\titleNameBorder");
            TextureStatic.Load("Blank", @"Shared\Art\Blank");
            TextureStatic.Load("tileSafeCheck", @"Shared\Art\tileSafeCheck");
            TextureStatic.Load("particle1", @"DefaultBackground\particle1");
            TextureStatic.Load("checkers", @"DefaultBackground\checkers");
            TextureStatic.Load("colorSwirl", @"DefaultBackground\colorSwirl");
            TextureStatic.Load("sampleInstructions", @"Shared\Art\sampleInstructions");
            TextureStatic.Load("instructionBorder", @"Shared\Art\instructionBorder");
            TextureStatic.Load("gradient", @"Shared\Art\Gradient");

            // Audio
            GameWorld.audio.LoadSound("boxArtScroll", @"Shared\Sounds\boxArtScroll");
            GameWorld.audio.LoadSound("menuClick", @"Shared\Sounds\menuClick");
            GameWorld.audio.LoadSound("menuGoBack", @"Shared\Sounds\menuGoBack");
            GameWorld.audio.LoadSound("menuScrollDown", @"Shared\Sounds\menuScrollDown");
            GameWorld.audio.LoadSound("menuScrollUp", @"Shared\Sounds\menuScrollUp");
            GameWorld.audio.LoadSound("volumeControlDown", @"Shared\Sounds\volumeControlDown");
            GameWorld.audio.LoadSound("volumeControlUp", @"Shared\Sounds\volumeControlUp");

            // Fonts
            FontStatic.Load("defaultFont", @"Shared\Fonts\defaultFont");
            FontStatic.Load("controllerFont", @"Shared\Fonts\controllerFont");
        }

        /// <summary>
        /// Loads the content of a game. There will be one of these methods per mini game
        /// </summary>
        public static void LoadSPRGameContent()
        {
            // TODO: load all Textures.
            TextureStatic.Load("4SideFriendlyRobot", @"SPRGame\4sidedrobotalt");
            TextureStatic.Load("BlankSide", @"SPRGame\blankside");
            TextureStatic.Load("Axe", @"SPRGame\axe");
            TextureStatic.Load("Gun", @"SPRGame\gun");
            TextureStatic.Load("Shield", @"SPRGame\shield");
            TextureStatic.Load("Projectile", @"SPRGame\whitestuff");
            // TODO: load all Audio.

            // TODO: load all Fonts, and anything else.
        }

        /// <summary>
        /// Loads the game info.
        /// </summary>
        public static void LoadGameInfo()
        {
            int scoreBoardIndex = 0;

            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Super Power Robots",
                "BoxArtGame5",
                "These robots seem a little phallic",
                "Owen, Tom, Ian, Sean (OTIS)",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchSPRGameDelegate(scoreBoardIndex++)));

            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Snake Death",
                "BoxArtGame1",
                "Embark on an epic journey across distant\ngalaxies to defeat the evil space overlord.\nFifteen increasingly difficult levels await you,\nand your dexterity, intellect, and bladder will\nbe vigorously challenged.",
                "By Ty Taylor",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchStupidGameDelegate(scoreBoardIndex++)));

            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Hypercube Arcade",
                "BoxArtGame2",
                "You will not win!",
                "By Ty Taylor and Marc Buchner",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchStupidGameDelegate(scoreBoardIndex++)));
            
            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Game 3",
                "BoxArtGame3",
                "Ahhh! The fractals are everywhere!",
                "By Ty Taylor and Samuel L. Jackson",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchStupidGameDelegate(scoreBoardIndex++)));

            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Game 4",
                "BoxArtGame4",
                "The cake is a fib.",
                "By Ty Taylor and Leeroy Jenkins",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchStupidGameDelegate(scoreBoardIndex++)));

            GameInfoCollection.GameInfos.Add(new GameInfo(
                "Game 5",
                "BoxArtGame5",
                "The worst game ever made.",
                "By Ty Taylor, M. C. Escher, and some other guy",
                "sampleInstructions",
                scoreBoardIndex,
                new LaunchStupidGameDelegate(scoreBoardIndex++)));

            // Must be last!
            GameInfoCollection.GameInfos.Add(new GameInfo(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                scoreBoardIndex,
                null));
        }
    }
}

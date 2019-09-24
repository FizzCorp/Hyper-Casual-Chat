using System;
using System.Collections.Generic;
using Fizz.UI.Core;
using UnityEngine;

namespace Fizz.UI
{
    public static class Utils
    {
        private static Dictionary<string, Color> _userColor = new Dictionary<string, Color>();

        public static DateTime GetDateTimeToUnixTime(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static string GetCurrentUnixTimeStamp()
        {
            DateTime dateTime = new DateTime(2017, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return Math.Truncate((DateTime.Now - dateTime).TotalMilliseconds).ToString();
        }

        public static string GetRemainingTimeToString(long unixTimeStamp)
        {
            TimeSpan span = new TimeSpan(unixTimeStamp);
            return string.Format("{0:hh\\:mm\\:ss}", span);
        }

        public static Color GetUserNickColor(string userId)
        {
            if (_userColor.ContainsKey(userId))
            {
                return _userColor[userId];
            }
            else
            {
                Color _newColor = GetRandomColor();
                _userColor.Add(userId, _newColor);
                return _newColor;
            }
        }

        public static string GetDownloadDirectoryPath()
        {
            return System.IO.Path.Combine(Application.persistentDataPath, "fizzAssets");
        }

        public static string GetDictionaryToString (Dictionary<string, string> data, string skipKey = "")
        {
            string str = string.Empty;
            if (data == null) return str;
            foreach (KeyValuePair<string, string> pair in data)
            {
                if (pair.Key.Equals (skipKey)) continue;
                str += pair.Key + ": " + pair.Value + ", ";
            }
            return str;
        }

        public static string GetFormattedTimeForUnixTimeStamp(long unixTimeStamp, bool todaysTime = true, bool fullDate = false)
        {
            DateTime dateTime = GetDateTimeToUnixTime(unixTimeStamp);
            string timeFormat = string.Empty;
            if (DateTime.Now.Subtract(dateTime).Days > 6)
            {
                if (fullDate)
                {
                    timeFormat = string.Format("{0:d/MM/yyyy}", dateTime);
                }
                else
                {
                    timeFormat = dateTime.Year != DateTime.Now.Year ? string.Format("{0:dd} {1:MMM} {2:yyyy}", dateTime, dateTime, dateTime) : string.Format("{0:ddd}, {1:dd} {2:MMM}", dateTime, dateTime, dateTime);
                }
            }
            else if (dateTime.Day != DateTime.Now.Day)
            {
                if (dateTime.Day == DateTime.Now.Day - 1)
                {
                    timeFormat = Registry.Localization.GetText("DateFormat_Yesterday");
                }
                else if (dateTime.Day >= DateTime.Now.Day - 6)
                {
                    timeFormat = string.Format("{0}", dateTime.DayOfWeek);
                }
                else
                {
                    timeFormat = string.Format("{0:ddd}, {1:dd} {2:MMM}", dateTime, dateTime, dateTime);
                }
            }
            else
            {
                timeFormat = (todaysTime) ? string.Format("{0:h:mm tt}", dateTime) : Registry.Localization.GetText ("DateFormat_Today");
            }
            return timeFormat;
        }

        private static Color GetRandomColor()
        {
            int colorIndex = UnityEngine.Random.Range(1, 11);
            Color color;
            switch (colorIndex)
            {
                case 1:
                    color = new Color(92 / 255.0f, 83 / 255.0f, 214 / 255.0f, 1.0f);
                    break;
                case 2:
                    color = new Color(170 / 255.0f, 91 / 255.0f, 196 / 255.0f, 1.0f);
                    break;
                case 3:
                    color = new Color(37 / 255.0f, 177 / 255.0f, 41 / 255.0f, 1.0f);
                    break;
                case 4:
                    color = new Color(254 / 255.0f, 150 / 255.0f, 1 / 255.0f, 1.0f);
                    break;
                case 5:
                    color = new Color(241 / 255.0f, 90 / 255.0f, 43 / 255.0f, 1.0f);
                    break;
                case 6:
                    color = new Color(55 / 255.0f, 176 / 255.0f, 216 / 255.0f, 1.0f);
                    break;
                case 7:
                    color = new Color(00 / 255.0f, 178 / 255.0f, 130 / 255.0f, 1.0f);
                    break;
                case 8:
                    color = new Color(216 / 255.0f, 69 / 255.0f, 162 / 255.0f, 1.0f);
                    break;
                case 9:
                    color = new Color(189 / 255.0f, 200 / 255.0f, 18 / 255.0f, 1.0f);
                    break;
                case 10:
                    color = new Color(12 / 255.0f, 135 / 255.0f, 113 / 255.0f, 1.0f);
                    break;
                default:
                    color = new Color(12 / 255.0f, 135 / 255.0f, 113 / 255.0f, 1.0f);
                    break;
            }
            return color;
        }

        public static T LoadPrefabs<T>(string prefabName) where T : FizzBaseComponent
        {
            T prefab = Resources.Load<T>("FizzCustomPrefabs/" + prefabName);
            if (prefab == null)
            {
                prefab = Resources.Load<T>("FizzPrefabs/" + prefabName);
            }
            return prefab;
        }

        public static Sprite LoadSprite(string spriteName)
        {
            Sprite sprite = Resources.Load<Sprite>("FizzCustomSprites/" + spriteName);
            if (sprite == null)
            {
                sprite = Resources.Load<Sprite>("FizzSprites/" + spriteName);
            }
            return sprite;
        }

        public static Font LoadFont(string fontName)
        {
            Font font = Resources.Load<Font>("FizzCustomFonts/" + fontName);
            if (font == null)
            {
                font = Resources.Load<Font>("FizzFonts/" + fontName);
            }
            return font;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Object = UnityEngine.Object;

namespace HoloPlaySDK
{

    public class utils
    {

        public static string formatPathToOS(string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);
            return path;
        }
        public static bool getDoesConfigFileExist(string relativePathToConfig)
        {
            string temp;
            return getConfigPathToFile(relativePathToConfig, out temp);
        }
        
        public static bool getConfigPathToFile(string relativePathToConfig, out string fullPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return getConfigPathToFileWin(relativePathToConfig, out fullPath);
            }
            return getConfigPathToFileMac(relativePathToConfig, out fullPath);
        }

        public static bool getConfigPathToFileWin(string relativePathToConfig, out string fullPath)
        {
            relativePathToConfig = formatPathToOS(relativePathToConfig);

            string[] drives = System.Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                if (File.Exists(drive + relativePathToConfig))
                {
                    fullPath = drive + relativePathToConfig;
                    return true;
                }
            }
            fullPath = Path.GetFileName(relativePathToConfig); //return the base name of the file only.
            return false;
        }

        public static bool getConfigPathToFileMac(string relativePathToConfig, out string fullPath)
        {
            string[] directories = Directory.GetDirectories("/Volumes/");
            foreach (string d in directories)
            {
                string fixedPath = d + "/" + relativePathToConfig;
                fixedPath = formatPathToOS(fixedPath);

                FileInfo f = new FileInfo(fixedPath);
                if (f.Exists)
                {
                    fullPath = f.FullName;
                    return true;
                }
            }
            fullPath = Path.GetFileName(relativePathToConfig); //return the base name of the file only.        
            return false;
        }

        public static float angleBetweenPoints(Vector2 v1, Vector2 v2)
        {
            return Mathf.Atan2(v1.x - v2.x, v1.y - v2.y) * Mathf.Rad2Deg;
        }


        /// <summary>
        /// Gets a depth value from the raw depth texture
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static System.UInt16 colorToDepth16(Color32 c)
        {
            return (System.UInt16)((c.g << 8) | c.r);
        }


        /// <summary>
        /// Converts a color value coming in from the depth plugin, and compresses it into an 8 bit depth value
        /// </summary>
        /// <param name="c">the corresponding color from the incoming depth map</param>
        /// <param name="thresholdDistance">the farthest distance that you care to represent in the 8 bits, smaller will give you more precision.  A too low value will cause the value to loop over, causing banding.</param>
        /// <returns></returns>
        public static byte colorToDepth8(Color32 c, float thresholdDistance)
        {

            System.UInt16 v = colorToDepth16(c);

            if (v == 0)
                return 0;

            int p = (int)(((float)v / thresholdDistance) * 127f);

            //  if (p > 35) //this is a noise threshold. Sometimes the low values are not accurate.
            return (byte)(255 - p); //the first int here is a noise threshold.
                                    //p = 255 - p;
                                    //  return 0;
        }

        public static string colorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
        public static Color hexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
    }

}

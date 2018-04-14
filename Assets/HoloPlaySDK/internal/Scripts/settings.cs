using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace HoloPlaySDK
{
    public class settings
    {

        public static readonly string usbConfigPath = "holoPlaySDK_calibration/holoPlayConfig.json";
        public static readonly string touchCalibrationPath = "holoPlaySDK_calibration/holoPlayTouch.txt";


        /// <summary>
        /// returns if the path exists or not.
        /// </summary>
        /// <param name="relativePathToConfig"></param>
        /// <returns></returns>
        public static bool getConfigFile(string relativePathToConfig)
        {
            string temp;
            return getConfigFile(relativePathToConfig, out temp);
        }

        /// <summary>
        /// this method is used to figure out which drive is the usb flash drive is related to HoloPlayer, and then returns that path so that our settings can load normally from there.
        /// </summary>
        /// <param name="relativePathToConfig"></param>
        /// <param name="fullPath">if true is returned, will contain the absolute path to the requested config file</param>
        /// <returns>True if found, false if not</returns>
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        public static bool getConfigFile(string relativePathToConfig, out string fullPath)
        {
            relativePathToConfig = utils.formatPathToOS(relativePathToConfig);

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
#else  //osx,  TODO: linux untested in standalone
        public static bool getConfigFile(string relativePathToConfig, out string fullPath)
        {
            string[] directories = Directory.GetDirectories("/Volumes/");
            foreach (string d in directories)
            {
                string fixedPath = d + "/" + relativePathToConfig;
                fixedPath = utils.formatPathToOS(fixedPath);

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
#endif


    }
}

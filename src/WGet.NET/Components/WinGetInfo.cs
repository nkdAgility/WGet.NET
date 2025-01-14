﻿//--------------------------------------------------//
// Created by basicx-StrgV                          //
// https://github.com/basicx-StrgV/                 //
//--------------------------------------------------//
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WGetNET.HelperClasses;

namespace WGetNET
{
    /// <summary>
    /// The <see cref="WGetNET.WinGetInfo"/> class offers informations about the installed winget version.
    /// </summary>
    public class WinGetInfo
    {
        private const string _versionCmd = "--version";
        private const string _exportSettingsCmd = "settings export";

        internal readonly ProcessManager _processManager;

        /// <summary>
        /// Gets if winget is installed on the system.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if winget is installed or <see langword="false"/> if not.
        /// </returns>
        public bool WinGetInstalled
        {
            get
            {
                if (CheckWinGetVersion() != string.Empty)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the version number of the winget installation.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> with the version number.
        /// </returns>
        public string WinGetVersion
        {
            get
            {
                return CheckWinGetVersion();
            }
        }

        /// <summary>
        /// Gets the version number of the winget installation.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Version"/> object.
        /// </returns>
        public Version WinGetVersionObject
        {
            get
            {
                return GetVersionObject();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WGetNET.WinGetInfo"/> class.
        /// </summary>
        public WinGetInfo()
        {
            _processManager = new ProcessManager("winget");
        }

        /// <summary>
        /// Exports the WinGet settings to a json string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> containing the settings json.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public string ExportSettings()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_exportSettingsCmd);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronous exports the WinGet settings to a json string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.String"/> containing the settings json.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<string> ExportSettingsAsync()
        {
            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_exportSettingsCmd);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the WinGet settings to a json and writes them to the given file.
        /// </summary>
        /// <param name="file">
        /// The file for the export.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull, and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public bool ExportSettingsToFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_exportSettingsCmd);

                return FileHandler.ExportOutputToFile(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronous exports the WinGet settings to a json and writes them to the given file.
        /// </summary>
        /// <param name="file">
        /// The file for the export.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was succesfull, and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<bool> ExportSettingsToFileAsync(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_exportSettingsCmd);

                return await FileHandler.ExportOutputToFileAsync(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Checks if the installed WinGet version is the same or higher as the given version.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <returns>
        /// <see langword="true"/> if the installed WinGet version is the same or higher as the given version, or <see langword="false"/> if not.
        /// </returns>
        protected bool WinGetVersionIsMatchOrAbove(int major, int minor = 0)
        {
            Version winGetVersion = WinGetVersionObject;
            if (winGetVersion.Major >= major && winGetVersion.Minor >= minor)
            {
                return true;
            }
            return false;
        }

        private string CheckWinGetVersion()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_versionCmd);

                for (int i = 0; i < result.Output.Length; i++)
                {
#if NETCOREAPP3_1_OR_GREATER
                    if (result.Output[i].StartsWith('v'))
                    {
                        return result.Output[i].Trim();
                    }
#elif NETSTANDARD2_0
                    if (result.Output[i].StartsWith("v"))
                    {
                        return result.Output[i].Trim();
                    }
#endif
                }
            }
            catch
            {
                //No handling needed, winget can not be accessed
            }

            return string.Empty;
        }

        private Version GetVersionObject()
        {
            string versionString = CheckWinGetVersion();

#if NETCOREAPP3_1_OR_GREATER
            //Remove the first letter from the version string.
            if (versionString.StartsWith('v'))
            {
                versionString = versionString[1..].Trim();

            }

            //Remove text from the end of the version string.
            for (int i = 0; i < versionString.Length; i++)
            {
                if (versionString[i] == '-')
                {
                    versionString = versionString[0..i];
                    break;
                }
            }
#elif NETSTANDARD2_0
            //Remove the first letter from the version string.
            if (versionString.StartsWith("v"))
            {
                versionString = versionString.Substring(1).Trim();

            }

            //Remove text from the end of the version string.
            for (int i = 0; i < versionString.Length; i++)
            {
                if (versionString[i] == '-')
                {
                    versionString = versionString.Substring(0, i);
                    break;
                }
            }
#endif

            if (!Version.TryParse(versionString, out Version? versionObject))
            {
                versionObject = Version.Parse("0.0.0");
            }

            return versionObject;
        }
    }
}
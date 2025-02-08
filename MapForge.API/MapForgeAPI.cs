using MapForge.API.Enums;
using MapForge.API.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MapForge.API
{
    /// <summary>
    /// Map Forge API.
    /// </summary>
    public class MapForgeAPI
    {
        static FileSystemWatcher _fileWatcher;
        static ConcurrentQueue<FileAction> _fileActions = new ConcurrentQueue<FileAction>();

        /// <summary>
        /// Returns if API is already initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Returns path of plugin directory.
        /// </summary>
        public static string PluginDirectory { get; private set; }

        /// <summary>
        /// Returns path of bundles directory.
        /// </summary>
        public static string BundlesDirectory { get; private set; }

        /// <summary>
        /// A static dictionary that stores loaded bundles, mapping a bundle's name (as a string) 
        /// to its corresponding <see cref="BundleInfo"/> object.
        /// </summary>
        public static Dictionary<string, BundleInfo> LoadedBundles = new Dictionary<string, BundleInfo>();

        /// <summary>
        /// Returns registered objects.
        /// </summary>
        public static ObjectsInfo Objects { get; private set; } = new ObjectsInfo();

        /// <summary>
        /// Initializes API.
        /// </summary>
        /// <param name="pluginDirectory">The plugin directory.</param>
        public static void Initialize(string pluginDirectory, ObjectsInfo objects)
        {
            if (IsInitialized)
                return;

            PluginDirectory = pluginDirectory;

            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);

            BundlesDirectory = Path.Combine(pluginDirectory, "Bundles");

            if (!Directory.Exists(BundlesDirectory))
                Directory.CreateDirectory(BundlesDirectory);

            _fileWatcher = new FileSystemWatcher(BundlesDirectory);
            _fileWatcher.EnableRaisingEvents = true;

            _fileWatcher.Deleted += (s, ev) =>
            {
                _fileActions.Enqueue(new FileAction()
                {
                    FullPath = ev.FullPath,
                    FileName = ev.Name,
                    Type = FileActionType.Deleted,
                });
            };

            _fileWatcher.Changed += (s, ev) =>
            {
                _fileActions.Enqueue(new FileAction()
                {
                    FullPath = ev.FullPath,
                    FileName = ev.Name,
                    Type = FileActionType.Changed,
                });
            };

            Objects = objects;
            IsInitialized = true;
        }

        public static void CheckForFileChanges()
        {
            if (_fileActions.Count == 0)
                return;

            while (_fileActions.TryDequeue(out FileAction action))
            {
                if (!TryGetBundle(action.FileName, out BundleInfo info))
                    continue;

                switch (action.Type)
                {
                    case FileActionType.Deleted:
                        info.Unload();
                        break;
                    case FileActionType.Changed:
                        if (!info.Reload())
                            _fileActions.Enqueue(action);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets all available bundles in directory.
        /// </summary>
        /// <returns>Bundle names</returns>
        public static string[] GetAvailableBundles()
        {
            string[] files = Directory.GetFiles(BundlesDirectory, "*");

            return files.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        /// <summary>
        /// Tries to load bundle.
        /// </summary>
        /// <param name="bundleName">The name of bundle.</param>
        /// <param name="info">Bundle information.</param>
        /// <returns>Bundle if loaded successfully.</returns>
        public static bool TryLoadBundle(string bundleName, out BundleInfo info)
        {
            info = LoadBundle(bundleName);
            return info != null;
        }

        /// <summary>
        /// Tries to get bundle.
        /// </summary>
        /// <param name="bundleName">The name of bundle.</param>
        /// <param name="info">Bundle information</param>
        /// <returns>Bundle if its loaded.</returns>
        public static bool TryGetBundle(string bundleName, out BundleInfo info)
        {
            if (!IsBundleLoaded(bundleName))
            {
                info = null;
                return false;
            }

            info = LoadedBundles[bundleName];
            return true;
        }

        /// <summary>
        /// Loads bundle.
        /// </summary>
        /// <param name="bundleName">The name of bundle.</param>
        /// <returns>Bundle if loaded successfully.</returns>
        public static BundleInfo LoadBundle(string bundleName)
        {
            if (IsBundleLoaded(bundleName))
            {
                // Bundle is already loaded.
                return LoadedBundles[bundleName];
            }

            string targetPath = Path.Combine(BundlesDirectory, bundleName);

            if (!BundleExists(bundleName))
            {
                // Bundle not exists...
                return null;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(targetPath);

            if (bundle == null)
            {
                // File is not a asset bundle
                return null;
            }

            BundleInfo info = new BundleInfo(bundle, targetPath, bundleName);
            LoadedBundles.Add(bundleName, info);

            return info;
        }

        /// <summary>
        /// Checks if bundle is already loaded.
        /// </summary>
        /// <param name="bundleName">The name of bundle.</param>
        /// <returns>Return true if bundle is loaded.</returns>
        public static bool IsBundleLoaded(string bundleName) => LoadedBundles.ContainsKey(bundleName);

        /// <summary>
        /// Checks if bundle exists in bundles directory.
        /// </summary>
        /// <param name="bundleName">The name of bundle.</param>
        /// <returns>Return true if bundle exists.</returns>
        public static bool BundleExists(string bundleName)
        {
            string targetPath = Path.Combine(BundlesDirectory, bundleName);
            return File.Exists(targetPath);
        }
    }
}

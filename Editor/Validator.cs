using System.IO;
using UnityEditor.Experimental;
using UnityEngine;

namespace Kataru
{
    public class Validator : AssetsModifiedProcessor
    {
        static string storyPath;
        static string bookmarkPath;
        static string targetPath;

        /// <summary>
        /// The OnAssetsModified method is called whenever an Asset has been changed in the project.
        /// This methods determines if any Preset has been added, removed, or moved
        /// and updates the CustomDependency related to the changed folder.
        /// </summary>
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            var settings = KataruSettings.Get();
            if (settings == null) return;

            storyPath = "Assets/" + settings.storyPath;
            bookmarkPath = "Assets/StreamingAssets/" + settings.bookmarkPath;
            targetPath = "Assets/StreamingAssets/" + settings.targetPath;

            if (!KataruWasModified(changedAssets, addedAssets, deletedAssets, movedAssets))
            {
                return;
            }

            // Run validation
            Runner.Compile(storyPath, bookmarkPath, targetPath);
        }

        /// <summary>
        /// Checks if any of the file delta was a Kataru file.
        /// </summary>
        /// <param name="changedAssets"></param>
        /// <param name="addedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <returns></returns>
        bool KataruWasModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            foreach (string asset in changedAssets)
            {
                if (IsKataruAsset(asset)) return true;
            }
            foreach (string asset in addedAssets)
            {
                if (IsKataruAsset(asset)) return true;
            }
            foreach (string asset in deletedAssets)
            {
                if (IsKataruAsset(asset)) return true;
            }
            foreach (AssetMoveInfo assetMove in movedAssets)
            {
                if (IsKataruAsset(assetMove.destinationAssetPath)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a single asset was a Kataru file.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool IsKataruAsset(string asset)
        {
            Debug.Log($"Asset '{asset}'");
            return asset.StartsWith(storyPath) || asset == bookmarkPath;
        }
    }
}
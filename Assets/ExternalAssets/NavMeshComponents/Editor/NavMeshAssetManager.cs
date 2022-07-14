using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace UnityEditor.AI
{
    public class NavMeshAssetManager : ScriptableSingleton<NavMeshAssetManager>
    {
        private readonly List<AsyncBakeOperation> _mBakeOperations = new();

        private readonly List<SavedPrefabNavMeshData> _mPrefabNavMeshDataAssets = new();

        internal List<AsyncBakeOperation> GetBakeOperations()
        {
            return _mBakeOperations;
        }

        private static string GetAndEnsureTargetPath(NavMeshSurface surface)
        {
            // Create directory for the asset if it does not exist yet.
            var activeScenePath = surface.gameObject.scene.path;

            var targetPath = "Assets";
            if (!string.IsNullOrEmpty(activeScenePath))
            {
                targetPath = Path.Combine(Path.GetDirectoryName(activeScenePath) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(activeScenePath));
            }
            else
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(surface.gameObject);
                var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(surface.gameObject);
                if (isPartOfPrefab && !string.IsNullOrEmpty(prefabStage.prefabAssetPath))
                {
                    var prefabDirectoryName = Path.GetDirectoryName(prefabStage.prefabAssetPath);
                    if (!string.IsNullOrEmpty(prefabDirectoryName))
                        targetPath = prefabDirectoryName;
                }
            }

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            return targetPath;
        }

        private static void CreateNavMeshAsset(NavMeshSurface surface)
        {
            var targetPath = GetAndEnsureTargetPath(surface);

            var combinedAssetPath = Path.Combine(targetPath, "NavMesh-" + surface.name + ".asset");
            combinedAssetPath = AssetDatabase.GenerateUniqueAssetPath(combinedAssetPath);
            AssetDatabase.CreateAsset(surface.navMeshData, combinedAssetPath);
        }

        private NavMeshData GetNavMeshAssetToDelete(NavMeshSurface navSurface)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(navSurface) && !PrefabUtility.IsPartOfModelPrefab(navSurface))
            {
                // Don't allow deleting the asset belonging to the prefab parent
                var parentSurface = PrefabUtility.GetCorrespondingObjectFromSource(navSurface);
                if (parentSurface && navSurface.navMeshData == parentSurface.navMeshData)
                    return null;
            }

            // Do not delete the NavMeshData asset referenced from a prefab until the prefab is saved
            var prefabStage = PrefabStageUtility.GetPrefabStage(navSurface.gameObject);
            var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(navSurface.gameObject);
            if (isPartOfPrefab && IsCurrentPrefabNavMeshDataStored(navSurface))
                return null;

            return navSurface.navMeshData;
        }

        private void ClearSurface(NavMeshSurface navSurface)
        {
            var hasNavMeshData = navSurface.navMeshData != null;
            StoreNavMeshDataIfInPrefab(navSurface);

            var assetToDelete = GetNavMeshAssetToDelete(navSurface);
            navSurface.RemoveData();

            if (hasNavMeshData)
            {
                SetNavMeshData(navSurface, null);
                EditorSceneManager.MarkSceneDirty(navSurface.gameObject.scene);
            }

            if (assetToDelete)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetToDelete));
        }

        public void StartBakingSurfaces(Object[] surfaces)
        {
            // Remove first to avoid double registration of the callback
            EditorApplication.update -= UpdateAsyncBuildOperations;
            EditorApplication.update += UpdateAsyncBuildOperations;

            foreach (var o in surfaces)
            {
                var surf = (NavMeshSurface)o;
                StoreNavMeshDataIfInPrefab(surf);

                var oper = new AsyncBakeOperation
                {
                    bakeData = InitializeBakeData(surf)
                };

                oper.bakeOperation = surf.UpdateNavMesh(oper.bakeData);
                oper.surface = surf;

                _mBakeOperations.Add(oper);
            }
        }

        private static NavMeshData InitializeBakeData(NavMeshSurface surface)
        {
            var emptySources = new List<NavMeshBuildSource>();
            var emptyBounds = new Bounds();
            Transform transform;
            return UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), emptySources, emptyBounds
                , (transform = surface.transform).position, transform.rotation);
        }

        private void UpdateAsyncBuildOperations()
        {
            foreach (var oper in _mBakeOperations)
            {
                if (oper.surface == null || oper.bakeOperation == null)
                    continue;

                if (!oper.bakeOperation.isDone) continue;
                var surface = oper.surface;
                var delete = GetNavMeshAssetToDelete(surface);
                if (delete != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete));

                surface.RemoveData();
                SetNavMeshData(surface, oper.bakeData);

                if (surface.isActiveAndEnabled)
                    surface.AddData();
                CreateNavMeshAsset(surface);
                EditorSceneManager.MarkSceneDirty(surface.gameObject.scene);
            }

            _mBakeOperations.RemoveAll(o => o.bakeOperation == null || o.bakeOperation.isDone);
            if (_mBakeOperations.Count == 0)
                EditorApplication.update -= UpdateAsyncBuildOperations;
        }

        public bool IsSurfaceBaking(NavMeshSurface surface)
        {
            return surface != null && _mBakeOperations.Where(oper => oper.surface != null && oper.bakeOperation != null).Any(oper => oper.surface == surface);
        }

        public void ClearSurfaces(Object[] surfaces)
        {
            foreach (var o in surfaces)
            {
                var s = (NavMeshSurface)o;
                ClearSurface(s);
            }
        }

        private static void SetNavMeshData(NavMeshSurface navSurface, NavMeshData navMeshData)
        {
            var so = new SerializedObject(navSurface);
            var navMeshDataProperty = so.FindProperty("m_NavMeshData");
            navMeshDataProperty.objectReferenceValue = navMeshData;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private void StoreNavMeshDataIfInPrefab(NavMeshSurface surfaceToStore)
        {
            var prefabStage = PrefabStageUtility.GetPrefabStage(surfaceToStore.gameObject);
            var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(surfaceToStore.gameObject);
            if (!isPartOfPrefab)
                return;

            // check if data has already been stored for this surface
            if (_mPrefabNavMeshDataAssets.Any(storedAssetInfo => storedAssetInfo.surface == surfaceToStore))
            {
                return;
            }

            if (_mPrefabNavMeshDataAssets.Count == 0)
            {
                PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
                PrefabStage.prefabSaving += DeleteStoredNavMeshDataAssetsForOwnedSurfaces;

                PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
                PrefabStage.prefabStageClosing += ForgetUnsavedNavMeshDataChanges;
            }

            var isDataOwner = true;
            if (PrefabUtility.IsPartOfPrefabInstance(surfaceToStore) &&
                !PrefabUtility.IsPartOfModelPrefab(surfaceToStore))
            {
                var basePrefabSurface = PrefabUtility.GetCorrespondingObjectFromSource(surfaceToStore);
                isDataOwner = basePrefabSurface == null || surfaceToStore.navMeshData != basePrefabSurface.navMeshData;
            }

            _mPrefabNavMeshDataAssets.Add(new SavedPrefabNavMeshData
                { surface = surfaceToStore, navMeshData = isDataOwner ? surfaceToStore.navMeshData : null });
        }

        private bool IsCurrentPrefabNavMeshDataStored(NavMeshSurface surface)
        {
            return surface != null &&
                   (from storedAssetInfo in _mPrefabNavMeshDataAssets
                       where storedAssetInfo.surface == surface
                       select storedAssetInfo.navMeshData == surface.navMeshData).FirstOrDefault();
        }

        private void DeleteStoredNavMeshDataAssetsForOwnedSurfaces(GameObject gameObjectInPrefab)
        {
            // Debug.LogFormat("DeleteStoredNavMeshDataAsset() when saving prefab {0}", gameObjectInPrefab.name);

            var surfaces = gameObjectInPrefab.GetComponentsInChildren<NavMeshSurface>(true);
            foreach (var surface in surfaces)
                DeleteStoredPrefabNavMeshDataAsset(surface);
        }

        private void DeleteStoredPrefabNavMeshDataAsset(NavMeshSurface surface)
        {
            for (var i = _mPrefabNavMeshDataAssets.Count - 1; i >= 0; i--)
            {
                var storedAssetInfo = _mPrefabNavMeshDataAssets[i];
                if (storedAssetInfo.surface != surface) continue;
                var storedNavMeshData = storedAssetInfo.navMeshData;
                if (storedNavMeshData != null && storedNavMeshData != surface.navMeshData)
                {
                    var assetPath = AssetDatabase.GetAssetPath(storedNavMeshData);
                    AssetDatabase.DeleteAsset(assetPath);
                }

                _mPrefabNavMeshDataAssets.RemoveAt(i);
                break;
            }

            if (_mPrefabNavMeshDataAssets.Count != 0) return;
            PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
            PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
        }

        private void ForgetUnsavedNavMeshDataChanges(PrefabStage prefabStage)
        {
            // Debug.Log("On prefab closing - forget about this object's surfaces and stop caring about prefab saving");

            if (prefabStage == null)
                return;

            var allSurfacesInPrefab = prefabStage.prefabContentsRoot.GetComponentsInChildren<NavMeshSurface>(true);
            NavMeshSurface surfaceInPrefab = null;
            var index = 0;
            do
            {
                if (allSurfacesInPrefab.Length > 0)
                    surfaceInPrefab = allSurfacesInPrefab[index];

                for (var i = _mPrefabNavMeshDataAssets.Count - 1; i >= 0; i--)
                {
                    var storedPrefabInfo = _mPrefabNavMeshDataAssets[i];
                    if (storedPrefabInfo.surface == null)
                    {
                        // Debug.LogFormat("A surface from the prefab got deleted after it has baked a new NavMesh but it hasn't saved it. Now the unsaved asset gets deleted. ({0})", storedPrefabInfo.navMeshData);

                        // surface got deleted, thus delete its initial NavMeshData asset
                        if (storedPrefabInfo.navMeshData != null)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(storedPrefabInfo.navMeshData);
                            AssetDatabase.DeleteAsset(assetPath);
                        }

                        _mPrefabNavMeshDataAssets.RemoveAt(i);
                    }
                    else if (surfaceInPrefab != null && storedPrefabInfo.surface == surfaceInPrefab)
                    {
                        //Debug.LogFormat("The surface {0} from the prefab was storing the original navmesh data and now will be forgotten", surfaceInPrefab);

                        var baseSurface = PrefabUtility.GetCorrespondingObjectFromSource(surfaceInPrefab);
                        if (baseSurface == null || surfaceInPrefab.navMeshData != baseSurface.navMeshData)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(surfaceInPrefab.navMeshData);
                            AssetDatabase.DeleteAsset(assetPath);

                            //Debug.LogFormat("The surface {0} from the prefab has baked new NavMeshData but did not save this change so the asset has been now deleted. ({1})",
                            //    surfaceInPrefab, assetPath);
                        }

                        _mPrefabNavMeshDataAssets.RemoveAt(i);
                    }
                }
            } while (++index < allSurfacesInPrefab.Length);

            if (_mPrefabNavMeshDataAssets.Count != 0) return;
            PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
            PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
        }

        internal struct AsyncBakeOperation
        {
            public NavMeshSurface surface;
            public NavMeshData bakeData;
            public AsyncOperation bakeOperation;
        }

        private struct SavedPrefabNavMeshData
        {
            public NavMeshSurface surface;
            public NavMeshData navMeshData;
        }
    }
}